module Minotaur.GUI.Rect
open Minotaur.Utilities.Vector
open Minotaur.Utilities.Misc

type Anchor = TopLeft | TopCenter | Center

type Rect = {
    absolutePosition: Vector
    localPosition: Vector
    dimensions: Vector
    pivot: Anchor
    anchor: Anchor
    parent: Rect option
}

// Removed RectParent type, is replace by Option.

let private rectPivotShift pivot dimensions =
    match pivot with
        | TopLeft -> vector 0 0
        | TopCenter -> vector (center dimensions.x |> (~-)) 0
        | Center -> vector (center dimensions.x |> (~-)) (center dimensions.y |> (~-))

let private rectAnchorShift parentRect anchor =
    let anchorShift = 
        match anchor with
        | TopLeft -> vector 0 0
        | TopCenter -> vector (center parentRect.dimensions.x) 0
        | Center -> vector (center parentRect.dimensions.x) (center parentRect.dimensions.y)
    anchorShift

// Replaced pattern matching
let private absolutePosition parent pivot anchor localPosition dimensions =
    let onPresentParent p =
        let pivotShift = rectPivotShift pivot dimensions
        let anchorShift = rectAnchorShift p anchor
        localPosition + pivotShift + anchorShift + p.absolutePosition
    Option.map onPresentParent parent |> Option.defaultValue localPosition
    
let rect pivot anchor parent x y dimensions =
    let localPosition = vector x y
    let absolutePosition = absolutePosition parent pivot anchor localPosition dimensions
    { absolutePosition = absolutePosition; localPosition = localPosition; dimensions = dimensions; pivot = pivot; anchor = anchor; parent = parent }

let rectWithNewPosition pivot anchor x y rect =
    let localPosition = vector x y
    let absolutePosition = absolutePosition rect.parent pivot anchor localPosition rect.dimensions
    { rect with absolutePosition = absolutePosition; pivot = pivot; anchor = anchor }

let rectWithNewDimensions dimensions rect =
    let absolutePosition = absolutePosition rect.parent rect.pivot rect.anchor rect.localPosition dimensions
    { rect with absolutePosition = absolutePosition; dimensions = dimensions }

let rectWithNewParent parent rect =
    let absolutePosition = absolutePosition parent rect.pivot rect.anchor rect.localPosition rect.dimensions
    { rect with absolutePosition = absolutePosition; parent = parent; }