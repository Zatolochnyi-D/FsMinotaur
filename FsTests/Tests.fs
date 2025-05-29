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

[<Theory>]
[<InlineData(4, 2)>]
[<InlineData(19, 8)>]
[<InlineData(86, 19)>]
[<InlineData(123, 54)>]
let ``On get the element under given index should be given`` count index =
    let s = storage<int> ()
    for i = 0 to count - 1 do
        Storage.addElement s i |> ignore

    Storage.getElement s index |> should equal index

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

[<Fact>]
let ``Values in storage should not shift (on remove idices still point on the same values).`` () =
    let s = storage<int> ()
    for i = 0 to 1 do
        Storage.addElement s i |> ignore
    Storage.removeElement s 0

    Storage.getElement s 1 |> should equal 1

[<Theory>]
[<InlineData(1, 0, 8)>]
[<InlineData(1, 0, 13)>]
[<InlineData(2, 0, 21)>]
[<InlineData(3, 2, 34)>]
[<InlineData(5, 3, 55)>]
let ``On setting to existing index, set value should be returned on get.`` sizeToCreate indexToSet valueToSet =
    let s = storage<int> ()
    for i = 0 to sizeToCreate - 1 do
        Storage.addElement s i |> ignore
    
    Storage.setElement s indexToSet valueToSet
    Storage.getElement s indexToSet |> should equal valueToSet

[<Fact>]
let ``On setting non-existent index System.ArgumentOutOfRangeException should be thrown.`` () =
    let s = storage<int> ()
    
    (fun () -> Storage.setElement s 3 1) |> should throw typeof<System.ArgumentOutOfRangeException>

[<Fact>]
let ``On remove, removed index should become empty (and throw on trying to get it).`` () =
    let s = storage<int> ()
    for i = 0 to 2 do
        Storage.addElement s i |> ignore
    Storage.removeElement s 1

    (fun () -> Storage.getElement s 1 |> ignore) |> should throw typeof<NullValue>

let storages () = seq {
    let s = storage<int> ()
    for j = 0 to 5 do
        Storage.addElement s j |> ignore
    yield s, 6

    let s = storage<int> ()
    for j = 0 to 8 do
        Storage.addElement s j |> ignore
    yield s, 9

    let s = storage<int> ()
    for j = 0 to 14 do
        Storage.addElement s j |> ignore
    yield s, 15
}

[<Theory>]
[<MemberData("storages")>]
let ``When elements added and removed, storage should have count of amount of elements currently in.`` (storage, count) =
    for i = 0 to 3 do
        Storage.removeElement storage i |> ignore

    count - 4 |> should equal (Storage.count storage)

[<Theory>]
[<MemberData("storages")>]
let ``When elements are added and removed, storage size should equal the highest amount of elements inside`` (storage, count) =
    for i = 0 to 3 do
        Storage.removeElement storage i |> ignore
    count |> should equal (Storage.storageSize storage)