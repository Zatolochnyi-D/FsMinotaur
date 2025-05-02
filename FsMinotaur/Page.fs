module Minotaur.Window.Page
open Minotaur.GUI.Interfaces
open Minotaur.Utilities.Storage
open Minotaur.Window.Bindings
open System

type Page() as this = 
    let elements = storage<IGraphicalElement> ()
    let selectableIndices = ResizeArray<int> ()
    let mutable currentSelected = None 
    let bindings = storage<Binding> ()
    do Storage.addElement bindings (binding ConsoleKey.W this.CycleUp) |> ignore
    do Storage.addElement bindings (binding ConsoleKey.S this.CycleDown) |> ignore
    do Storage.addElement bindings (binding ConsoleKey.Spacebar this.Execute) |> ignore

    member val Elements = elements

    member private this.CycleDown () = 
        this.Cycle (fun x -> (x + 1) % selectableIndices.Count)

    member private this.CycleUp () = 
        this.Cycle (fun x -> (x - 1 + selectableIndices.Count) % selectableIndices.Count)

    member private this.Cycle cycleFunction =
        this.RunOnButton (fun b -> b.Deselect())
        currentSelected <- Option.map cycleFunction currentSelected
        this.RunOnButton (fun b -> b.Select())

    member private this.Execute () = 
        this.RunOnButton (fun b -> b.Execute())

    member private _.RunOnButton (funcToRun: IButton -> unit) =
        currentSelected
        |> Option.map (fun i -> Storage.getElement elements selectableIndices[i])
        |> Option.map (fun el -> el :?> IButton)
        |> Option.iter funcToRun

    member _.AddStaticElement element = Storage.addElement elements element

    member this.AddSelectableElement element =
        let index = Storage.addElement elements element
        selectableIndices.Add index
        currentSelected <- Some 0
        this.RunOnButton (fun b -> b.Select ())
        index

    member _.UpdateFragments rect = Storage.map (fun (el: IGraphicalElement) -> el.SetRect rect) elements

    member _.TriggerBinding key = Storage.iter (fun x -> if x.key = key then x.func ()) bindings
