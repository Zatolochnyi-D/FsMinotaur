module StorageTests

open Xunit
open FsUnit.Xunit
open Minotaur.Utilities.Storage

[<Fact>]
let ``When 2 elements added, when new element is added, it should be third in storage.`` () =
    let s = storage<int> ()
    for i = 0 to 1 do
        Storage.addElement s i |> ignore
    Storage.addElement s 2 |> ignore

    Storage.getElement s 2 |> should equal 2

[<Fact>]
let ``When 3 elements added and 2 removed, when new element is added, it should be second in storage.`` () =
    let s = storage<int> ()
    for i = 0 to 2 do
        Storage.addElement s i |> ignore
    for i = 1 to 2 do
        Storage.removeElement s i
    Storage.addElement s 3 |> ignore

    Storage.getElement s 1 |> should equal 3

[<Fact>]
let ``Values in storage should not shift (on remove idices still point on the same values).`` () =
    let s = storage<int> ()
    for i = 0 to 1 do
        Storage.addElement s i |> ignore
    Storage.removeElement s 0

    Storage.getElement s 1 |> should equal 1

[<Theory>]
[<InlineData(0, false)>]
[<InlineData(3, false)>]
[<InlineData(1, true)>]
[<InlineData(6, true)>]
let ``On getting non-existent value a throw should happen.`` index createAndDelete =
    let s = storage<int> ()
    if createAndDelete then
        for i = 0 to index + 1 do
            Storage.addElement s i |> ignore
        Storage.removeElement s index
    
    (fun () -> Storage.getElement s index |> ignore) |> should throw typeof<NullValue>

[<Theory>]
[<InlineData(5, 3)>]
[<InlineData(9, 5)>]
[<InlineData(8, 7)>]
[<InlineData(5, 5)>]
let ``When elements added and removed, storage should have size of amount of elements currently in.`` amountToAdd amountToRemove =
    let s = storage<int> ()
    for i = 0 to amountToAdd - 1 do
        Storage.addElement s i |> ignore
    for i = 0 to amountToRemove - 1 do
        Storage.removeElement s i |> ignore

    amountToAdd - amountToRemove |> should equal (Storage.count s)