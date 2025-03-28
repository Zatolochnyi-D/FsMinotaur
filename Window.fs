module Minotaur.Window
open System.Collections.Generic
open System.Threading
open Minotaur.Colors
open Minotaur.Console
open Minotaur.GUI.Rect
open Minotaur.Utilities.Misc
open Minotaur.GUI.Fragment

type Window = {
    rect: Rect
    sleepTime: int
    charBuffer: List<List<char>>
    foregroundColorBuffer: List<List<Color>>
    backgroundColorBuffer: List<List<Color>>
    fragments: List<Fragment>
}
let defaultForeground = white
let defaultBackground = black

let createEmptyBuffers width height = 
    let chars = List<List<char>> ()
    for y = 0 to height - 1 do
        List<char> () |> chars.Add
        for _ = 0 to width - 1 do
            chars.[y].Add ' '
    let foregrounds = List<List<Color>> ()
    for y = 0 to height - 1 do
        List<Color> () |> foregrounds.Add
        for _ = 0 to width - 1 do
            foregrounds.[y].Add defaultForeground
    let backgrounds = List<List<Color>> ()
    for y = 0 to height - 1 do
        List<Color> () |> backgrounds.Add
        for _ = 0 to width - 1 do
            backgrounds.[y].Add defaultBackground
    chars, foregrounds, backgrounds

let window fps =
    prepareConsole ()
    let width, height = consoleSize ()
    let chars, foregrounds, backgrounds = createEmptyBuffers width height
    {
        rect = rect TopLeft TopLeft (None ()) 0 0 width height
        sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
        charBuffer = chars
        foregroundColorBuffer = foregrounds
        backgroundColorBuffer = backgrounds
        fragments = List<Fragment> ()
    }

let resize window = 
    let width, height = consoleSize ()
    let rect = rect TopLeft TopLeft (None ()) 0 0 width height
    let chars, foregrounds, backgrounds = createEmptyBuffers width height
    let newWindow = { window with rect = rect; charBuffer = chars; foregroundColorBuffer = foregrounds; backgroundColorBuffer = backgrounds }
    for i = 0 to newWindow.fragments.Count - 1 do
        let f = newWindow.fragments.[i]
        newWindow.fragments.[i] <- { f with rect = rectWithNewParent (Parent newWindow.rect) f.rect }
    newWindow

let scaleToConsole window =
    if vectorTupleEqual window.rect.dimensions (consoleSize ()) |> not then
        true, resize window
    else
        false, window

let clearBuffers window =
    for y = 0 to window.rect.dimensions.y - 1 do
        for x = 0 to window.rect.dimensions.x - 1 do
            window.charBuffer.[y].[x] <- ' '
            window.foregroundColorBuffer.[y].[x] <- defaultForeground
            window.backgroundColorBuffer.[y].[x] <- defaultBackground

let addFragment window fragment =
    window.fragments.Add fragment

let writeBuffer window =
    for fragment in window.fragments do
        let fDim = fragment.rect.dimensions
        let fPos = fragment.rect.absolutePosition
        for y = 0 to fDim.y - 1 do
            for x = 0 to fDim.x - 1 do
                let X = fPos.x + x
                let Y = fPos.y + y
                if X > -1 && X < window.rect.dimensions.x && Y > -1 && Y < window.rect.dimensions.y then
                    window.charBuffer.[Y].[X] <- fragment.chars.[y].[x]
                    window.foregroundColorBuffer.[Y].[X] <- fragment.foregroundColor
                    window.backgroundColorBuffer.[Y].[X] <- fragment.backgroundColor

let drawBuffer window =
    for y = 0 to window.rect.dimensions.y - 1 do
        for x = 0 to window.rect.dimensions.x - 1 do
            setColor <| unpackColor window.backgroundColorBuffer.[y].[x] <| unpackColor window.foregroundColorBuffer.[y].[x]
            writeChar x y window.charBuffer.[y].[x]

let rec mainLoop window : unit =
    clearConsole ()

    let windowScaled, newWindow = scaleToConsole window
    if not <| windowScaled then clearBuffers window

    writeBuffer newWindow
    drawBuffer newWindow

    Thread.Sleep newWindow.sleepTime
    mainLoop newWindow    