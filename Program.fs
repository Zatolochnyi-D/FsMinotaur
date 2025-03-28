module Minotaur.Main
open System.Collections.Generic
open Window
open Minotaur.GUI.Fragment
open Colors
open Minotaur.Utilities.Misc

[<EntryPoint>]
let main args =
    let w = window 30

    let content = List<List<char>> ()
    for i = 0 to 10 do
        List<char> () |> content.Add 
        String.replicate 30 "X" |> charList  |> content.[i].AddRange 
    let f = centeredFragment w.rect white black 0 0 content
    addFragment w f

    mainLoop w
    0