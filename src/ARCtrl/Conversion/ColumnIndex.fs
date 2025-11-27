namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex


module ColumnIndex = 

    open ARCtrl

    let private tryInt (str:string) =
        match System.Int32.TryParse str with
        | true,int -> Some int
        | _ -> None

    let orderName = "columnIndex"

    let columndIndexProperty = "https://w3id.org/ro/terms/arc#columnIndex"

    let tryGetIndex (node : LDNode) =
        match node.TryGetPropertyAsSingleton(columndIndexProperty) with
        | Some (:? string as ci) -> tryInt ci
        | _ ->
            match node.TryGetPropertyAsSingleton(orderName) with
            | Some (:? string as ci) -> tryInt ci
            | _ -> None

    let setIndex (node : LDNode) (index : int) =
        node.SetProperty(columndIndexProperty,(string index))

    [<AutoOpen>]
    module ColumnIndexExtensions = 
    
        type LDNode with

            member this.GetColumnIndex() = tryGetIndex this |> Option.get

            member this.TryGetColumnIndex() = tryGetIndex this

            member this.SetColumnIndex (index : int) = setIndex this index