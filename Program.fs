module Minotaur.Main
open System.Collections.Generic
open Window
open Minotaur.GUI.Fragment
open Colors
open Minotaur.Utilities.Misc
open Minotaur.GUI.Rect
open Minotaur.GUI.TextBox

let mainWindow = window 30

let selectionColor = green

let menu1TextBoxFunc = textBox Center TopCenter Middle mainWindow.rect white black
let menu1Item1 =
    let item = menu1TextBoxFunc 0 5 "┌────────────┐\n\
                                     │  Settings  │\n\
                                     └────────────┘"
    setFragmentForeground item selectionColor

let menu1Item2 = menu1TextBoxFunc 0 10 "┌────────────┐\n\
                                        │    Exit    │\n\
                                        └────────────┘"

[<EntryPoint>]
let main args =
    addFragment mainWindow menu1Item1
    addFragment mainWindow menu1Item2

    mainLoop mainWindow
    0