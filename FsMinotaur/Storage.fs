module Minotaur.Utilities.Storage

exception NullValue of string

type Storage<'T> = { list: 'T option ResizeArray }

let storage<'T> () = { list = ResizeArray<'T option>() }

module Storage =
    let addElement storage element =
        let rec findEmptySlot i elementToAdd =
            if i <> storage.list.Count then
                match storage.list[i] with
                | Some _ -> findEmptySlot (i + 1) elementToAdd
                | None -> storage.list[i] <- elementToAdd; i
            else
                storage.list.Add elementToAdd
                i
        findEmptySlot 0 (Some element)

    let getElement storage index =
        if index >= 0 && index < storage.list.Count then
            match storage.list[index] with
            | Some v -> v
            | None -> raise (NullValue "Trying read null value")
        else
            raise (NullValue "Trying read null value")

    let setElement storage index element = storage.list[index] <- Some element

    let removeElement storage index = storage.list[index] <- None

    let storageSize storage = storage.list.Count

    let count storage =
        let mutable result = 0
        for el in storage.list do
            result <- result + Option.count el
        result

    let iter action storage =
        for el in storage.list do
            Option.iter action el

    let map action storage =
        for i = 0 to storageSize storage - 1 do
            storage.list[i] <- storage.list[i] |> Option.map action
