module Minotaur.Window.Window
open System.Threading
open ConsoleFacade
open Minotaur
open Colors
open GUI.Rect
open Utilities.Misc
open GUI.Fragment
open Window.Bindings
open Utilities.Vector
open Minotaur.Utilities.Storage

let defaultForeground = white
let defaultBackground = black

let private createEmptyBuffers dimensions =
    let x, y = dimensions.x, dimensions.y
    Array2D.create x y (' ', defaultBackground, defaultForeground)

type Window = {
    console: ConsoleFacade
    rect: Rect
    sleepTime: int
    buffer: (char * Color * Color) array2d
    fragments: Fragment Storage
    bindings: Binding Storage
}

let window fps =
    let cons = ConsoleFacade ()
    let consoleDimensions = vectorFromStructTuple cons.ConsoleSize
    {
        console = cons
        rect = rect TopLeft TopLeft None 0 0 consoleDimensions
        sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
        buffer = createEmptyBuffers consoleDimensions
        fragments = storage<Fragment> ()
        bindings = storage<Binding> ()
    }

let private resize window = 
    let consoleDimensions = vectorFromStructTuple window.console.ConsoleSize
    let rect = rect TopLeft TopLeft None 0 0 consoleDimensions
    let newWindow = { window with rect = rect; buffer = createEmptyBuffers consoleDimensions }
    for i = 0 to storageSize window.fragments - 1 do
        newWindow.fragments.list[i] <- Option.map (fun f -> fragmentWithNewParent (Some newWindow.rect) f) newWindow.fragments.list[i]
    newWindow

let private tryScaleToConsole window =
    if window.rect.dimensions <> vectorFromStructTuple window.console.ConsoleSize then
        true, resize window
    else
        false, window

let private clearBuffers window =
    Array2D.iteri (fun x y _ -> window.buffer[x, y] <- (' ', defaultBackground, defaultForeground)) window.buffer

let addFragment window fragment = addElement window.fragments fragment

let getFragment window index = getElement window.fragments index

let setFragment window index fragment = setElement window.fragments index fragment

let addBinding window binding = addElement window.bindings binding
     
let private writeBuffer window =
    let writeFragmentToBuffer (fragment: Fragment) =
        let fDim = fragment.rect.dimensions
        let fPos = fragment.rect.absolutePosition
        for y = 0 to fDim.y - 1 do
            for x = 0 to fDim.x - 1 do
                let X = fPos.x + x
                let Y = fPos.y + y
                if X > -1 && X < window.rect.dimensions.x && Y > -1 && Y < window.rect.dimensions.y then
                    window.buffer[X, Y] <- fragment.chars[x, y], fragment.backgroundColor, fragment.foregroundColor
    Storage.iter (fun f -> writeFragmentToBuffer f) window.fragments

let private drawBuffer window =
    Array2D.iteri (fun x y (ch, bg: Color, fg: Color) -> window.console.SetColor (bg.get, fg.get); window.console.WriteChar (x, y, ch)) window.buffer

let rec mainLoop window : unit =
    window.console.Clear ()

    let windowScaled, newWindow = tryScaleToConsole window
    if not <| windowScaled then clearBuffers window

    let key = newWindow.console.ReadKey ()
    for bind in newWindow.bindings.list do
        Option.iter (fun x -> if x.key = key then x.func ()) bind

    writeBuffer newWindow
    drawBuffer newWindow

    Thread.Sleep newWindow.sleepTime
    mainLoop newWindow