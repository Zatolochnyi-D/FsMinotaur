module Minotaur.GUI.TextBox
open System.Collections.Generic
open System.Linq
open Minotaur.GUI.Rect
open Minotaur.GUI.Fragment
open Minotaur.Utilities.Misc

type TextAlignment = Left | Middle

let alignLeft targetLength (string: string) =
    let emptyLength = targetLength - string.Length
    let empty = String.replicate emptyLength " "
    string + empty

let alignCenter targetLength (string: string) =
    let emptyLength = targetLength - string.Length
    let leftEmpty = String.replicate (emptyLength / 2 + emptyLength % 2) " "
    let rightEmpty = String.replicate (emptyLength / 2) " "
    leftEmpty + string + rightEmpty

let textBox pivot anchor alignment parent x y (text: string) foregroundColor backgroundColor =
    let lines = text.Split '\n'
    let biggestLength = lines.Max (fun x -> x.Length)
    let fragmentContent = List<List<char>> ()
    let alignFunc = 
        match alignment with
        | Left -> alignLeft
        | Middle -> alignCenter
    for line in lines do
        alignFunc biggestLength <| line |> charList |> fragmentContent.Add
    fragment pivot anchor parent foregroundColor backgroundColor x y fragmentContent

let horizontallyCenteredTextBox = textBox TopCenter TopCenter Middle
let centeredTextBox = textBox Center Center Middle