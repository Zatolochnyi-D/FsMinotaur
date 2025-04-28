module Minotaur.Utilities.Misc
open System
open System.Linq

let floorToInt (x: double) =
    x |> Math.Floor |> int

let center sideSize =
    let shift = -1 + sideSize % 2
    sideSize / 2 + shift

let charList (string: string) =
    string.ToCharArray().ToList()