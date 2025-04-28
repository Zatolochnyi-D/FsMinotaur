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

type Window(fps: int) =
    let console = ConsoleFacade ()
    let rect = rect TopLeft TopLeft None 0 0 (vectorFromStructTuple console.ConsoleSize)
    let sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
    let buffer = createEmptyBuffers rect.dimensions
    let fragments = storage<Fragment> ()
    let bindings = storage<Binding> ()

    member val Fps = fps
    member val SleepTime = sleepTime
    member val Size = vectorFromStructTuple console.ConsoleSize
    member val Rect = rect
    member val Buffer = buffer
    member val Fragments = fragments
    member val Bindings = bindings

    new () = Window 60
    new(oldWindow: Window) = Window oldWindow.Fps

    member _.SetColor (background: Color) (foreground: Color) = console.SetColor (background.get, foreground.get)
    member _.WriteChar x y char = console.WriteChar (x, y, char)
    member _.Clear () = console.Clear ()
    member _.ReadKey () = console.ReadKey ()

let private resize (window: Window) = 
    let consoleDimensions = window.Size
    let rect = rect TopLeft TopLeft None 0 0 consoleDimensions
    let newWindow = Window window
    for i = 0 to storageSize window.Fragments - 1 do
        newWindow.Fragments.list[i] <- Option.map (fun f -> fragmentWithNewParent (Some newWindow.Rect) f) newWindow.Fragments.list[i]
    newWindow

let private tryScaleToConsole (window: Window) =
    if window.Rect.dimensions <> window.Size then
        true, resize window
    else
        false, window

let private clearBuffers (window: Window) =
    Array2D.iteri (fun x y _ -> window.Buffer[x, y] <- (' ', defaultBackground, defaultForeground)) window.Buffer

let addFragment (window: Window) fragment = addElement window.Fragments fragment

let getFragment (window: Window) index = getElement window.Fragments index

let setFragment (window: Window) index fragment = setElement window.Fragments index fragment

let addBinding (window: Window) binding = addElement window.Bindings binding
     
let private writeBuffer (window: Window) =
    let writeFragmentToBuffer (fragment: Fragment) =
        let fDim = fragment.rect.dimensions
        let fPos = fragment.rect.absolutePosition
        for y = 0 to fDim.y - 1 do
            for x = 0 to fDim.x - 1 do
                let X = fPos.x + x
                let Y = fPos.y + y
                if X > -1 && X < window.Rect.dimensions.x && Y > -1 && Y < window.Rect.dimensions.y then
                    window.Buffer[X, Y] <- fragment.chars[x, y], fragment.backgroundColor, fragment.foregroundColor
    Storage.iter (fun f -> writeFragmentToBuffer f) window.Fragments

let private drawBuffer (window: Window) =
    Array2D.iteri (fun x y (ch, bg: Color, fg: Color) -> window.SetColor <| bg <| fg; window.WriteChar x y ch) window.Buffer

let rec mainLoop (window: Window) : unit =
    window.Clear ()

    let windowScaled, newWindow = tryScaleToConsole window
    if not <| windowScaled then clearBuffers window

    let key = newWindow.ReadKey ()
    for bind in newWindow.Bindings.list do
        Option.iter (fun x -> if x.key = key then x.func ()) bind

    writeBuffer newWindow
    drawBuffer newWindow

    Thread.Sleep newWindow.SleepTime
    mainLoop newWindow