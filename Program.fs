module Minotaur.Main
open System
open Minotaur.GUI.Fragment
open Colors
open Minotaur.GUI.Rect
open Minotaur.GUI.TextBox
open Minotaur.Window.Window
open Minotaur.Window.Bindings
open Minotaur.Utilities.Vector

let defaultColor = white
let selectionColor = green

let mainWindow = window 30

let menu1TextBoxFunc = textBox Center TopCenter Middle (Some mainWindow.rect) defaultColor black
let menu1Item1 =
    let item = menu1TextBoxFunc 0 5 "┌────────────┐\n\
                                     │  Settings  │\n\
                                     └────────────┘"
    setTextForeground item selectionColor
let menu1Item1Id = addFragment mainWindow menu1Item1.fragment
let menu1Item2 = menu1TextBoxFunc 0 10 "┌────────────┐\n\
                                        │    Exit    │\n\
                                        └────────────┘"
let menu1Item2Id = addFragment mainWindow menu1Item2.fragment

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
    setFragment mainWindow prevIndex (setFragmentForeground (getFragment mainWindow prevIndex) defaultColor)
    setFragment mainWindow nextIndex (setFragmentForeground (getFragment mainWindow nextIndex) selectionColor)

addBinding mainWindow (binding ConsoleKey.S (fun () -> switch 1)) |> ignore
addBinding mainWindow (binding ConsoleKey.W (fun () -> switch -1)) |> ignore
addBinding mainWindow (binding ConsoleKey.Escape (fun () -> Environment.Exit 0)) |> ignore

[<EntryPoint>]
let main args =
    Console.prepareConsole ()
    mainLoop mainWindow
    0