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
    charBuffer: char array2d
    foregroundColorBuffer: Color array2d
    backgroundColorBuffer: Color array2d
    fragments: Fragment option ResizeArray
    bindings: Binding ResizeArray
}
let defaultForeground = white
let defaultBackground = black

let private createEmptyBuffers dimensions =
    let x, y = dimensions.x, dimensions.y
    let chars = Array2D.create x y ' '
    let foregrounds = Array2D.create x y defaultForeground
    let backgrounds = Array2D.create x y defaultBackground
    chars, foregrounds, backgrounds

let window fps =
    let consoleDimensions = Console.consoleSize ()
    let chars, foregrounds, backgrounds = createEmptyBuffers consoleDimensions
    {
        rect = rect TopLeft TopLeft None 0 0 consoleDimensions
        sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
        keyReader = Console.keyReader ()
        charBuffer = chars
        foregroundColorBuffer = foregrounds
        backgroundColorBuffer = backgrounds
        fragments = ResizeArray<Fragment option> ()
        bindings = ResizeArray<Binding> ()
    }

let private resize window = 
    let consoleDimensions = Console.consoleSize ()
    let rect = rect TopLeft TopLeft None 0 0 consoleDimensions
    let chars, foregrounds, backgrounds = createEmptyBuffers consoleDimensions
    let newWindow = { window with rect = rect; charBuffer = chars; foregroundColorBuffer = foregrounds; backgroundColorBuffer = backgrounds }
    for i = 0 to newWindow.fragments.Count - 1 do
        newWindow.fragments.[i] <- Option.map (fun f -> fragmentWithNewParent (Some newWindow.rect) f) newWindow.fragments.[i]
    newWindow

let tryScaleToConsole window =
    if window.rect.dimensions <> Console.consoleSize () then
        true, resize window
    else
        false, window

// Idk where to add generic functions so let's hope it works.
// Replaced for-loop with Array2D functions.
let clearBuffers window =
    let clearFunction (array: 'T array2d) (deafultValue: 'T) = Array2D.iteri (fun x y _ -> array[x, y] <- deafultValue) array
    clearFunction window.charBuffer ' '
    clearFunction window.foregroundColorBuffer defaultForeground
    clearFunction window.backgroundColorBuffer defaultBackground

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
                    window.charBuffer[X, Y] <- fragment.chars[x, y]
                    window.foregroundColorBuffer[X, Y] <- fragment.foregroundColor
                    window.backgroundColorBuffer[X, Y] <- fragment.backgroundColor
    for fragment in window.fragments do
        Option.iter (fun f -> writeFragmentToBuffer f) fragment 

let drawBuffer window =
    for y = 0 to window.rect.dimensions.y - 1 do
        for x = 0 to window.rect.dimensions.x - 1 do
            Console.setColor <| unpackColor window.backgroundColorBuffer[x, y] <| unpackColor window.foregroundColorBuffer[x, y]
            Console.writeChar x y window.charBuffer[x, y]

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