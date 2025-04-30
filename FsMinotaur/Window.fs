module Minotaur.Window.Window
open System.Threading
open ConsoleFacade
open Minotaur
open Colors
open Window.Bindings
open GUI.Rect
open GUI.Fragment
open Utilities.Misc
open Utilities.Vector
open Utilities.Storage

let defaultForeground = white
let defaultBackground = black

let private createEmptyBuffers dimensions =
    let x, y = dimensions.x, dimensions.y
    Array2D.create x y (' ', defaultBackground, defaultForeground)

type Window(fps: int) =
    let console = ConsoleFacade ()
    let mutable windowRect = rect TopLeft TopLeft None 0 0 (vectorFromStructTuple console.ConsoleSize)
    let sleepTime = fps |> double |> (/) 1000.0 |> floorToInt
    let buffer = createEmptyBuffers windowRect.dimensions
    let fragments = storage<Fragment> ()
    let bindings = storage<Binding> ()

    member val Rect = windowRect

    new () = Window 60

    member private _.ClearBuffers () =
        Array2D.iteri (fun x y _ -> buffer[x, y] <- ' ', defaultBackground, defaultForeground) buffer
    
    member private _.TrySyncWithConsoleSize () =
        let consoleDimensions = vectorFromStructTuple console.ConsoleSize
        if windowRect.dimensions <> consoleDimensions then
            windowRect <- rect TopLeft TopLeft None 0 0 consoleDimensions
            for i = 0 to storageSize fragments - 1 do
                fragments.list[i] <- Option.map (fun f -> fragmentWithNewParent (Some windowRect) f) fragments.list[i]
            true
        else
            false

    member private _.WriteBuffer () =
        let writeFragmentToBuffer (fragment: Fragment) =
            let fDim = fragment.rect.dimensions
            let fPos = fragment.rect.absolutePosition
            for y = 0 to fDim.y - 1 do
                for x = 0 to fDim.x - 1 do
                    let X = fPos.x + x
                    let Y = fPos.y + y
                    if X > -1 && X < windowRect.dimensions.x && Y > -1 && Y < windowRect.dimensions.y then
                        buffer[X, Y] <- fragment.chars[x, y], fragment.backgroundColor, fragment.foregroundColor
        Storage.iter (fun f -> writeFragmentToBuffer f) fragments

    member private _.DrawBuffer () =
        Array2D.iteri (fun x y (ch, bg: Color, fg: Color) -> console.SetColor (bg.get, fg.get); console.WriteChar (x, y, ch)) buffer

    member this.MainLoop () : unit =
        console.Clear ()
        if not <| this.TrySyncWithConsoleSize () then this.ClearBuffers()

        let key = console.ReadKey ()
        for bind in bindings.list do
            Option.iter (fun x -> if x.key = key then x.func ()) bind

        this.WriteBuffer ()
        this.DrawBuffer ()

        Thread.Sleep sleepTime
        this.MainLoop ()

    member _.AddFragment fragment = addElement fragments fragment

    member _.GetFragment index = getElement fragments index

    member _.SetFragment index fragment = setElement fragments index fragment

    member _.AddBinding binding = addElement bindings binding