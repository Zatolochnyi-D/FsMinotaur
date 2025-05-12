module Minotaur.Examples
open System
open Colors
open Minotaur.GUI.Rect
open Minotaur.GUI.TextBox
open Minotaur.Window.Window
open Minotaur.Window.Page
open Minotaur.GUI.Button
open Minotaur.GUI.Interfaces

let getExamplePage (window: Window) =
    let menuTextBoxFunc = textBox Center TopCenter Middle (Some window.Rect) white black
    let titleText = menuTextBoxFunc 0 4 "+== T E X T ==+"
    let mainPage = Page ()
    let title = { new IGraphicalElement with                      
                    member _.GetFragment() = 
                        titleText.fragment
                    member _.SetRect parent = 
                        textBoxWithParent parent titleText
                }
    mainPage.AddStaticElement title |> ignore
    let menu1Item1 = menuTextBoxFunc 0 10 "┌────────────┐\n\
                                           │  Settings  │\n\
                                           └────────────┘"

    let menu1Item2 = menuTextBoxFunc 0 14 "┌────────────┐\n\
                                           │    Back    │\n\
                                           └────────────┘"
    Button (menu1Item1, (fun () -> Console.Beep())) |> mainPage.AddSelectableElement |> ignore
    Button (menu1Item2, (fun () -> window.SetPageIndex 0)) |> mainPage.AddSelectableElement |> ignore
    mainPage