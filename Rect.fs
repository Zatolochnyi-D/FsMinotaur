module Minotaur.GUI.Rect
open Minotaur.Utilities.Vector

type Anchor = TopLeft | TopCenter | Center

type Rect = {
    position: Vector
    dimensions: Vector
    pivot: Anchor
    anchor: Anchor
}

let rect pivot anchor x y width height =
    let position = { x = x; y = y }
    let dimensions = { x = width; y = height }
    { pivot = pivot; anchor = anchor; position = position; dimensions = dimensions }