module Minotaur.Utilities.Vector

type Vector =
    {
        x: int
        y: int
    }
    static member (+) (a, b) = { x = a.x + b.x; y = a.y + b.y }

let vector x y = { x = x; y = y; }