module Minotaur.GUI.Rect
open Minotaur.Utilities.Vector
open Minotaur.Utilities.Misc

type Anchor = TopLeft | TopCenter | Center
type Rect = {
    absloutePosition: Vector
    localPosition: Vector
    dimensions: Vector
    pivot: Anchor
    anchor: Anchor
}
type Parent = Parent of Rect | None of unit

let rectPivotShift pivot dimensions =
    match pivot with
        | TopLeft -> vector 0 0
        | TopCenter -> vector (center dimensions.x |> (~-)) 0
        | Center -> vector (center dimensions.x |> (~-)) (center dimensions.y |> (~-))

let rectAnchorShift parentRect anchor =
    let anchorShift = 
        match anchor with
        | TopLeft -> vector 0 0
        | TopCenter -> vector (center parentRect.dimensions.x) 0
        | Center -> vector (center parentRect.dimensions.x) (center parentRect.dimensions.y)
    anchorShift

let absolutePosition parent pivot anchor localPosition dimensions =
    match parent with 
        | Parent p -> let pivotShift = rectPivotShift pivot dimensions
                      let anchorShift = rectAnchorShift p anchor
                      localPosition + pivotShift + anchorShift + p.absloutePosition
        | None _   -> localPosition
    
let rect pivot anchor parent x y width height =
    let dimensions = vector width height
    let localPosition = vector x y
    let absolutePosition = absolutePosition parent pivot anchor localPosition dimensions
    { absloutePosition = absolutePosition; localPosition = localPosition; dimensions = dimensions; pivot = pivot; anchor = anchor }

let rectWithNewParent parent rect =
    let absolutePosition = absolutePosition parent rect.pivot rect.anchor rect.localPosition rect.dimensions
    { rect with absloutePosition = absolutePosition }