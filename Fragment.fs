module Minotaur.GUI.Fragment
open System.Collections.Generic
open Minotaur.Colors
open Minotaur.GUI.Rect

type Fragment = {
    rect: Rect
    chars: List<List<char>>
    foregroundColor: Color
    backgroundColor: Color
}

let fragment pivot anchor parent foregroundColor backgroundColor x y (content: List<List<char>>) =
    let rect = rect pivot anchor (Parent parent) x y content.[0].Count content.Count
    { rect = rect; chars = content; foregroundColor = foregroundColor; backgroundColor = backgroundColor }

let setFragmentForeground fragment color = { fragment with foregroundColor = color }