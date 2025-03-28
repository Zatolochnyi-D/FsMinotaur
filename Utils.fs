module Minotaur.Utilities.Misc
open System
open System.Linq
open Minotaur.Utilities.Vector

let floorToInt (x: double) =
    x |> Math.Floor |> int

let vectorTupleEqual (vector: Vector) (x: int, y: int) =
    vector.x = x && vector.y = y

let center sideSize =
    let shift = -1 + sideSize % 2
    sideSize / 2 + shift

let charList (string: string) =
    string.ToCharArray().ToList()