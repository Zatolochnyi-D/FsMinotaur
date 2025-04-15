module Minotaur.GUI.Fragment
open System.Collections.Generic
open Minotaur.Colors
open Minotaur.GUI.Rect
open Minotaur.Utilities.Vector

type Fragment = {
    rect: Rect
    chars: List<List<char>>
    foregroundColor: Color
    backgroundColor: Color
}

let fragment pivot anchor parent foregroundColor backgroundColor x y (content: List<List<char>>) =
    let dimensions = vector content.[0].Count content.Count
    let rect = rect pivot anchor (Parent parent) x y dimensions
    { rect = rect; chars = content; foregroundColor = foregroundColor; backgroundColor = backgroundColor }

let setFragmentForeground fragment color = { fragment with foregroundColor = color }