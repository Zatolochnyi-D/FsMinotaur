module Minotaur.GUI.Page
open Minotaur
open Interfaces
open Minotaur.Utilities.Storage
open Minotaur.Window.Bindings
open System

type Page() as this = 
    let staticElements = storage<IGraphicalElement> ()
    let selectableElements = storage<IButton> ()
    let selectableIndices = ResizeArray<int> ()
    let mutable currentSelected = None 
    let bindings = storage<Binding> ()
    do Storage.addElement bindings (binding ConsoleKey.W this.CycleUp) |> ignore
    do Storage.addElement bindings (binding ConsoleKey.S this.CycleDown) |> ignore
    do Storage.addElement bindings (binding ConsoleKey.Spacebar this.Execute) |> ignore

    member val StaticElements = staticElements
    member val SelectableElements = selectableElements

    member private this.CycleDown () = 
        this.Cycle (fun x -> (x + 1) % selectableIndices.Count)

    member private this.CycleUp () = 
        this.Cycle (fun x -> (x - 1 + selectableIndices.Count) % selectableIndices.Count)

    member private _.Cycle cycleFunction =
        Option.iter (fun i -> Storage.getElement selectableElements selectableIndices[i] |> fun b -> b.Deselect ()) currentSelected
        currentSelected <- Option.map cycleFunction currentSelected
        Option.iter (fun i -> Storage.getElement selectableElements selectableIndices[i] |> fun b -> b.Select ()) currentSelected

    member private _.Execute () = 
        Option.iter (fun i -> Storage.getElement selectableElements selectableIndices[i] |> fun b -> b.Execute ()) currentSelected

    member _.AddStaticElement element = Storage.addElement staticElements element

    member _.AddSelectableElement element =
        let index = Storage.addElement selectableElements element
        selectableIndices.Add index
        currentSelected <- Some 0
        index

    member _.UpdateFragments rect = Storage.map (fun (el: IGraphicalElement) -> el.SetRect rect) staticElements

    member _.TriggerBinding key = Storage.iter (fun x -> if x.key = key then x.func ()) bindings
