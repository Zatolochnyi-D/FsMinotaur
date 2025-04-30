module Minotaur.GUI.Fragment
open Minotaur.Colors
open Minotaur.GUI.Rect
open Minotaur.Utilities.Vector

type Fragment = {
    rect: Rect
    chars: char array2d
    foregroundColor: Color
    backgroundColor: Color
}

let fragment pivot anchor parent foregroundColor backgroundColor x y (content: char array2d) =
    let dimensions = vector (Array2D.length1 content) (Array2D.length2 content)
    let rect = rect pivot anchor parent x y dimensions
    { rect = rect; chars = content; foregroundColor = foregroundColor; backgroundColor = backgroundColor }

let setFragmentForeground fragment color = { fragment with foregroundColor = color }
let setFragmentBackground fragment color = { fragment with backgroundColor = color }

let fragmentWithNewParent parent fragment = { fragment with rect = rectWithNewParent parent fragment.rect }