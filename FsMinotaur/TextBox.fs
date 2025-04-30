module Minotaur.GUI.TextBox
open System.Linq
open Minotaur.GUI.Fragment

type TextAlignment = Left | Middle
type TextBox = {
    text: string
    alignment: TextAlignment
    fragment: Fragment
}

let private alignLeft targetLength (string: string) =
    let emptyLength = targetLength - string.Length
    let empty = String.replicate emptyLength " "
    string + empty

let private alignCenter targetLength (string: string) =
    let emptyLength = targetLength - string.Length
    let leftEmpty = String.replicate (emptyLength / 2 + emptyLength % 2) " "
    let rightEmpty = String.replicate (emptyLength / 2) " "
    leftEmpty + string + rightEmpty

let textBox pivot anchor alignment parent foregroundColor backgroundColor x y (text: string) =
    let lines = text.Split '\n'
    let biggestLength = lines.Max (fun x -> x.Length)
    let alignFunction = 
        match alignment with
        | Left -> alignLeft biggestLength
        | Middle -> alignCenter biggestLength
    let alignedLines = Array.map alignFunction lines
    let fragmentContent = Array2D.init biggestLength lines.Length (fun x y -> alignedLines[y].[x])
    let fragment = fragment pivot anchor parent foregroundColor backgroundColor x y fragmentContent
    { text = text; alignment = alignment; fragment = fragment }

let setTextForeground textBox color = { textBox with fragment = setFragmentForeground textBox.fragment color }