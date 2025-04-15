module Minotaur.Window.Window
open System
open System.Collections.Generic
open System.Threading
open Minotaur
open Colors
open GUI.Rect
open Utilities.Misc
open GUI.Fragment
open Window.Bindings
open Utilities.Vector

// Changes:
// Replaced NullableFragment with Option.
// Replaced list buffers with 2d arrays.

exception NullValue of string

type Window = {
    rect: Rect
    sleepTime: int
    keyReader: unit -> ConsoleKey
    charBuffer: char array2d
    foregroundColorBuffer: Color array2d
    backgroundColorBuffer: Color array2d
    fragments: List<Fragment option>
    bindings: List<Binding>
}
let defaultForeground = white
let defaultBackground = black

let createEmptyBuffers dimensions =
    let x, y = dimensions.x, dimensions.y
    let chars = Array2D.create x y ' '
    let foregrounds = Array2D.create x y defaultForeground
    let backgrounds = Array2D.create x y defaultBackground
    chars, foregrounds, backgrounds

let window fps =
    Console.prepareConsole ()
    let consoleDimensions = Console.consoleSize ()
    let chars, foregrounds, backgrounds = createEmptyBuffers consoleDimensions
    {
        rect = rect TopLeft TopLeft (None ()) 0 0 consoleDimensions
        sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
        keyReader = Console.keyReader ()
        charBuffer = chars
        foregroundColorBuffer = foregrounds
        backgroundColorBuffer = backgrounds
        fragments = List<Fragment option> ()
        bindings = List<Binding> ()
    }

let resize window = 
    let consoleDimensions = Console.consoleSize ()
    let rect = rect TopLeft TopLeft (None ()) 0 0 consoleDimensions
    let chars, foregrounds, backgrounds = createEmptyBuffers consoleDimensions
    let newWindow = { window with rect = rect; charBuffer = chars; foregroundColorBuffer = foregrounds; backgroundColorBuffer = backgrounds }
    for i = 0 to newWindow.fragments.Count - 1 do
        newWindow.fragments.[i] <- Option.map (fun f -> { f with rect = rectWithNewParent (Parent newWindow.rect) f.rect }) newWindow.fragments.[i]
    newWindow

let scaleToConsole window =
    if window.rect.dimensions = Console.consoleSize () |> not then
        true, resize window
    else
        false, window

let clearBuffers window =
    for y = 0 to window.rect.dimensions.y - 1 do
        for x = 0 to window.rect.dimensions.x - 1 do
            window.charBuffer[x, y] <- ' '
            window.foregroundColorBuffer[x, y] <- defaultForeground
            window.backgroundColorBuffer[x, y] <- defaultBackground

let addFragment window fragment =
    let rec findEmptySlot i fragmentToAdd =
        if i <> window.fragments.Count then
            match window.fragments.[i] with
            | Some _ -> findEmptySlot (i + 1) fragmentToAdd
            | Option.None -> window.fragments.[i] <- fragmentToAdd; i
        else
            window.fragments.Add fragmentToAdd
            i
    findEmptySlot 0 (Some fragment)

let getFragment window index =
    match window.fragments.[index] with
        | Some f -> f
        | Option.None -> raise (NullValue "Trying read null value")
let setFragment window index fragment = window.fragments.[index] <- Some fragment

let addBinding window binding =
    window.bindings.Add binding
     
let writeBuffer window =
    let writeFragmentToBuffer (fragment: Fragment) =
        let fDim = fragment.rect.dimensions
        let fPos = fragment.rect.absolutePosition
        for y = 0 to fDim.y - 1 do
            for x = 0 to fDim.x - 1 do
                let X = fPos.x + x
                let Y = fPos.y + y
                if X > -1 && X < window.rect.dimensions.x && Y > -1 && Y < window.rect.dimensions.y then
                    window.charBuffer[X, Y] <- fragment.chars.[y].[x]
                    window.foregroundColorBuffer[X, Y] <- fragment.foregroundColor
                    window.backgroundColorBuffer[X, Y] <- fragment.backgroundColor
    for fragment in window.fragments do
        match fragment with
            | Some f -> writeFragmentToBuffer f
            | Option.None -> ()     

let drawBuffer window =
    for y = 0 to window.rect.dimensions.y - 1 do
        for x = 0 to window.rect.dimensions.x - 1 do
            Console.setColor <| unpackColor window.backgroundColorBuffer[x, y] <| unpackColor window.foregroundColorBuffer[x, y]
            Console.writeChar x y window.charBuffer[x, y]

let rec mainLoop window : unit =
    Console.clearConsole ()

    let windowScaled, newWindow = scaleToConsole window
    if not <| windowScaled then clearBuffers window

    let key = newWindow.keyReader ()
    for bind in newWindow.bindings do
        if bind.key = key then
            bind.func ()

    writeBuffer newWindow
    drawBuffer newWindow

    Thread.Sleep newWindow.sleepTime
    mainLoop newWindow