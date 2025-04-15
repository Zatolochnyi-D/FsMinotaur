module Minotaur.Console
open System
open Minotaur.Utilities.Vector

let defaultBackgroundColor = ConsoleColor.Black
let defaultForegroundColor = ConsoleColor.White

let prepareConsole () =
    Console.CursorVisible <- false
    Console.BackgroundColor <- defaultBackgroundColor
    Console.ForegroundColor <- defaultForegroundColor

let consoleSize () =
    vector Console.WindowWidth Console.WindowHeight

let clearConsole () =
    Console.Clear ()

let writeChar x y (string: char) =
    Console.SetCursorPosition(x, y)
    Console.Write string

let setColor backgroundColor foregroundColor =
    if Console.BackgroundColor <> backgroundColor then Console.BackgroundColor <- backgroundColor
    if Console.ForegroundColor <> foregroundColor then Console.ForegroundColor <- foregroundColor

let keyReader () = 
    let mutable key = ConsoleKey.None
    async {
        while true do 
            key <- (Console.ReadKey true).Key
    } |> Async.Start
    fun () -> 
        let result = key
        key <- ConsoleKey.None
        result