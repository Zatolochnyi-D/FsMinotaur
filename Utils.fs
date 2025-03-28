module Minotaur.Utilities.Misc
open System
open System.Linq
open Minotaur.Utilities.Vector

let floorToInt (x: double) =
    x |> Math.Floor |> int

let vectorTupleEqual (vector: Vector) (x: int, y: int) =
    vector.x = x && vector.y = y


let roundToInt (x: double) =
    x |> Math.Round |> int



let center sideSize =
    let shift = -1 + sideSize % 2
    sideSize / 2 + shift

let counter start = 
    let mutable count = start - 1
    fun () ->
        count <- count + 1
        count

// use 0 to get current sum
let sum () =
    let mutable previousSum = 0
    let mutable sum = 0
    fun addition ->
        previousSum <- sum
        sum <- sum + addition
        previousSum

let charList (string: string) =
    string.ToCharArray().ToList()