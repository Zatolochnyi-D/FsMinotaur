module Minotaur.Colors
open System

type Color = Color of ConsoleColor
let black = Color ConsoleColor.Black
let darkBlue = Color ConsoleColor.DarkBlue
let darkGreen = Color ConsoleColor.DarkGreen
let darkCyan = Color ConsoleColor.DarkCyan
let darkRed = Color ConsoleColor.DarkRed
let darkMagenta = Color ConsoleColor.DarkMagenta
let darkYellow = Color ConsoleColor.DarkYellow
let gray = Color ConsoleColor.Gray
let darkGray = Color ConsoleColor.DarkGray
let blue = Color ConsoleColor.Blue
let green = Color ConsoleColor.Green
let cyan = Color ConsoleColor.Cyan
let red = Color ConsoleColor.Red
let magenta = Color ConsoleColor.Magenta
let yellow = Color ConsoleColor.Yellow
let white = Color ConsoleColor.White

let unpackColor color =
    match color with
        | Color x -> x

type Color with 
    member this.get = unpackColor this