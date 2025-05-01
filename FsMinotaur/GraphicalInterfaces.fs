module Minotaur.GUI.Interfaces
open Fragment
open Rect

type IGraphicalElement =
    abstract member GetFragment: unit -> Fragment
    abstract member SetRect: Rect -> IGraphicalElement

type IButton =
    inherit IGraphicalElement
    abstract member Select: unit -> unit
    abstract member Deselect: unit -> unit
    abstract member Execute: unit -> unit