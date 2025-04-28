module Minotaur.Window.Bindings
open System

type Binding = {
    key: ConsoleKey
    func: unit -> unit
}

let binding key func = { key = key; func = func }