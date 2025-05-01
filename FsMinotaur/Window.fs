module Minotaur.Window.Window
open System.Threading
open ConsoleFacade
open Minotaur
open Colors
open Window.Bindings
open GUI.Rect
open GUI.Fragment
open GUI.Interfaces
open Utilities.Vector
open Utilities.Storage
open Minotaur.GUI.Page

let defaultForeground = white
let defaultBackground = black

let private createEmptyBuffer dimensions =
    let x, y = dimensions.x, dimensions.y
    Array2D.create x y (' ', defaultBackground, defaultForeground)

type Window(fps: int) =
    let console = ConsoleFacade ()
    let mutable windowRect = rect TopLeft TopLeft None 0 0 (vectorFromStructTuple console.ConsoleSize)
    let sleepTime = fps |> double |> (/) 1000.0 |> System.Math.Floor |> int
    let mutable buffer = createEmptyBuffer windowRect.dimensions
    let mutable currentPage = None
    let pages = storage<Page> ()
    let bindings = storage<Binding> ()

    member val Rect = windowRect

    new () = Window 60
    
    member private _.SyncWithConsoleSizeOrClearBuffers () =
        let consoleDimensions = vectorFromStructTuple console.ConsoleSize
        if windowRect.dimensions <> consoleDimensions then
            windowRect <- rect TopLeft TopLeft None 0 0 consoleDimensions
            buffer <- createEmptyBuffer windowRect.dimensions
            Storage.iter (fun (p: Page) -> p.UpdateFragments windowRect) pages
        else
            Array2D.iteri (fun x y _ -> buffer[x, y] <- ' ', defaultBackground, defaultForeground) buffer

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
        currentPage
        |> Option.map (fun i -> Storage.getElement pages i)
        |> Option.map (fun p -> p.Elements)
        |> Option.iter (Storage.iter <| fun (el: IGraphicalElement) -> writeFragmentToBuffer <| el.GetFragment() )

    member private _.DrawBuffer () =
        Array2D.iteri (fun x y (ch, bg: Color, fg: Color) -> console.SetColor (bg.get, fg.get); console.WriteChar (x, y, ch)) buffer

    member this.MainLoop () : unit =
        console.Clear ()
        this.SyncWithConsoleSizeOrClearBuffers ()

        let key = console.ReadKey ()
        Storage.iter (fun x -> if x.key = key then x.func ()) bindings
        currentPage
        |> Option.map (fun i -> Storage.getElement pages i)
        |> Option.iter (fun p -> p.TriggerBinding key)

        this.WriteBuffer ()
        this.DrawBuffer ()

        Thread.Sleep sleepTime
        this.MainLoop ()

    member _.AddPage page = 
        let index = Storage.addElement pages page
        currentPage <- Some 0
        index

    member _.SetPageIndex index = currentPage <- Some index

    member _.AddBinding binding = Storage.addElement bindings binding