module Minotaur.Main
open System
open Colors
open Minotaur.GUI.Rect
open Minotaur.GUI.TextBox
open Minotaur.Window.Window
open Minotaur.Window.Bindings
open Minotaur.GUI.Page
open Minotaur.GUI.Button

let mainWindow = Window 30
let mainPage = Page ()

let menu1TextBoxFunc = textBox Center TopCenter Middle (Some mainWindow.Rect) white black

let text = menu1TextBoxFunc 0 4 "+==Text==+";

let menu1Item1 = menu1TextBoxFunc 0 10 "┌────────────┐\n\
                                        │  Settings  │\n\
                                        └────────────┘"

let menu1Item2 = menu1TextBoxFunc 0 14 "┌────────────┐\n\
                                        │    Exit    │\n\
                                        └────────────┘"
mainPage.AddStaticElement text |> ignore
Button (menu1Item1, (fun () -> Console.Beep())) |> mainPage.AddSelectableElement |> ignore
Button (menu1Item2, (fun () -> exit 0)) |> mainPage.AddSelectableElement |> ignore
mainWindow.AddPage mainPage |> ignore
mainWindow.SetPageIndex 0

binding ConsoleKey.Escape (fun () -> exit 0) |> mainWindow.AddBinding |> ignore

[<EntryPoint>]
let main _ =
    mainWindow.MainLoop ()
    0