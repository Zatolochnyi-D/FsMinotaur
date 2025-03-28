module Minotaur.Window
open System.Collections.Generic
open System.Threading
open Minotaur.Colors
open Minotaur.Console
open Minotaur.GUI.Rect
open Minotaur.Utilities.Misc

type Window = {
    rect: Rect
    sleepTime: int
    charBuffer: List<List<char>>
    foregroundColorBuffer: List<List<Color>>
    backgroundColorBuffer: List<List<Color>>
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
        rect = rect TopLeft TopLeft 0 0 width height
        sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
        charBuffer = chars
        foregroundColorBuffer = foregrounds
        backgroundColorBuffer = backgrounds
    }

let resize window = 
    let width, height = consoleSize ()
    let rect = rect TopLeft TopLeft 0 0 width height
    let chars, foregrounds, backgrounds = createEmptyBuffers width height
    { window with rect = rect; charBuffer = chars; foregroundColorBuffer = foregrounds; backgroundColorBuffer = backgrounds }

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

let rec mainLoop window : unit =
    clearConsole ()

    let windowScaled, newWindow = scaleToConsole window
    if not <| windowScaled then clearBuffers window

    

    Thread.Sleep newWindow.sleepTime
    mainLoop newWindow    