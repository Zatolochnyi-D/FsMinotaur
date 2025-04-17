module Minotaur.Window.Window
open System
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
    buffer: (char * Color * Color) array2d
    fragments: Fragment option ResizeArray
    bindings: Binding ResizeArray
}
let defaultForeground = white
let defaultBackground = black

let private createEmptyBuffers dimensions =
    let x, y = dimensions.x, dimensions.y
    Array2D.create x y (' ', defaultBackground, defaultForeground)

let window fps =
    let consoleDimensions = Console.consoleSize ()
    {
        rect = rect TopLeft TopLeft None 0 0 consoleDimensions
        sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
        keyReader = Console.keyReader ()
        buffer = createEmptyBuffers consoleDimensions
        fragments = ResizeArray<Fragment option> ()
        bindings = ResizeArray<Binding> ()
    }

let private resize window = 
    let consoleDimensions = Console.consoleSize ()
    let rect = rect TopLeft TopLeft None 0 0 consoleDimensions
    let newWindow = { window with rect = rect; buffer = createEmptyBuffers consoleDimensions }
    for i = 0 to newWindow.fragments.Count - 1 do
        newWindow.fragments.[i] <- Option.map (fun f -> fragmentWithNewParent (Some newWindow.rect) f) newWindow.fragments.[i]
    newWindow

let tryScaleToConsole window =
    if window.rect.dimensions <> Console.consoleSize () then
        true, resize window
    else
        false, window

let clearBuffers window =
    Array2D.iteri (fun x y _ -> window.buffer[x, y] <- (' ', defaultBackground, defaultForeground)) window.buffer

let addFragment window fragment =
    let rec findEmptySlot i fragmentToAdd =
        if i <> window.fragments.Count then
            match window.fragments.[i] with
            | Some _ -> findEmptySlot (i + 1) fragmentToAdd
            | None -> window.fragments.[i] <- fragmentToAdd; i
        else
            window.fragments.Add fragmentToAdd
            i
    findEmptySlot 0 (Some fragment)

let getFragment window index =
    match window.fragments.[index] with
        | Some f -> f
        | None -> raise (NullValue "Trying read null value")

let setFragment window index fragment = window.fragments.[index] <- Some fragment

let addBinding window binding = window.bindings.Add binding
     
let writeBuffer window =
    let writeFragmentToBuffer (fragment: Fragment) =
        let fDim = fragment.rect.dimensions
        let fPos = fragment.rect.absolutePosition
        for y = 0 to fDim.y - 1 do
            for x = 0 to fDim.x - 1 do
                let X = fPos.x + x
                let Y = fPos.y + y
                if X > -1 && X < window.rect.dimensions.x && Y > -1 && Y < window.rect.dimensions.y then
                    window.buffer[X, Y] <- fragment.chars[x, y], fragment.backgroundColor, fragment.foregroundColor
    for fragment in window.fragments do
        Option.iter (fun f -> writeFragmentToBuffer f) fragment 

let drawBuffer window =
    Array2D.iteri (fun x y (ch, bg: Color, fg: Color) -> Console.setColor <| bg.get <| fg.get; Console.writeChar x y ch) window.buffer

let rec mainLoop window : unit =
    Console.clearConsole ()

    let windowScaled, newWindow = tryScaleToConsole window
    if not <| windowScaled then clearBuffers window

    let key = newWindow.keyReader ()
    for bind in newWindow.bindings do
        if bind.key = key then
            bind.func ()

    writeBuffer newWindow
    drawBuffer newWindow

    Thread.Sleep newWindow.sleepTime
    mainLoop newWindow