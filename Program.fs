module Minotaur.Main
open Window

[<EntryPoint>]
let main args =
    let w = window 30
    mainLoop w
    0