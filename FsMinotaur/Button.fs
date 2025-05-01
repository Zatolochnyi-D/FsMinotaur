module Minotaur.GUI.Button
open Interfaces
open Minotaur.Colors
open TextBox

type Button(textBox, action) =
    let mutable textBox = textBox
    let selectionColor = green
    let defaultColor = white

    interface IButton with        
        member this.Deselect() = 
            textBox <- setTextForeground textBox defaultColor
        member this.Select(): unit = 
            textBox <- setTextForeground textBox selectionColor
        member this.Execute(): unit = 
            action ()
        member val Fragment = textBox.fragment
        member this.SetRect(arg1: Rect.Rect): IGraphicalElement = 
            textBox