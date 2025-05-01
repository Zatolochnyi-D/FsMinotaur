module Minotaur.GUI.Button
open Interfaces
open Minotaur.Colors
open TextBox

type Button(textBox, action) =
    let mutable textBox = textBox

    interface IButton with        
        member _.Deselect() = textBox <- setTextForeground textBox white

        member _.Select(): unit = textBox <- setTextForeground textBox green

        member _.Execute(): unit = action ()

        member _.GetFragment () = textBox.fragment
        
        member this.SetRect rect = 
            textBox <- (textBox :> IGraphicalElement).SetRect rect :?> TextBox
            this :> IGraphicalElement