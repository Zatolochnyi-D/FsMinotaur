module Minotaur.Main
open System
open Minotaur.GUI.Fragment
open Colors
open Minotaur.GUI.Rect
open Minotaur.GUI.TextBox
open Minotaur.Window.Window
open Minotaur.Window.Bindings

let defaultColor = white
let selectionColor = green

let mainWindow = Window 30

let menu1TextBoxFunc = textBox Center TopCenter Middle (Some mainWindow.Rect) defaultColor black
let menu1Item1 =
    let item = menu1TextBoxFunc 0 5 "┌────────────┐\n\
                                     │  Settings  │\n\
                                     └────────────┘"
    setTextForeground item selectionColor
let menu1Item1Id = mainWindow.AddFragment menu1Item1.fragment
let menu1Item2 = menu1TextBoxFunc 0 10 "┌────────────┐\n\
                                        │    Exit    │\n\
                                        └────────────┘"
let menu1Item2Id = mainWindow.AddFragment menu1Item2.fragment

let itemCounter firstSelection =
    let minSelection, maxSelection = 0, 1
    let mutable currentSelection = firstSelection
    fun shift -> 
        currentSelection <- currentSelection + shift
        currentSelection <- min currentSelection maxSelection
        currentSelection <- max currentSelection minSelection
        currentSelection

let selector = itemCounter 0

let switch direction =
    let prevIndex = selector 0
    let nextIndex = selector direction
    mainWindow.SetFragment prevIndex (setFragmentForeground (mainWindow.GetFragment prevIndex) defaultColor)
    mainWindow.SetFragment nextIndex (setFragmentForeground (mainWindow.GetFragment nextIndex) selectionColor)

mainWindow.AddBinding (binding ConsoleKey.S (fun () -> switch 1)) |> ignore
mainWindow.AddBinding (binding ConsoleKey.W (fun () -> switch -1)) |> ignore
mainWindow.AddBinding (binding ConsoleKey.Escape (fun () -> Environment.Exit 0)) |> ignore

[<EntryPoint>]
let main _ =
    mainWindow.MainLoop ()
    0