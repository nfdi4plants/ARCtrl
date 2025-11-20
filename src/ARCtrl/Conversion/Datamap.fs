namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns


type DatamapConversion =

    static member composeFragmentDescriptors (datamap : DataMap) : ResizeArray<LDNode> =
        datamap.DataContexts
        |> ResizeArray.map BaseTypes.composeFragmentDescriptor

    static member decomposeFragmentDescriptors (fragmentDescriptors : ResizeArray<LDNode>, ?graph : LDGraph, ?context : LDContext) : DataMap =
        fragmentDescriptors
        |> ResizeArray.map (fun fd -> BaseTypes.decomposeFragmentDescriptor(fd, ?graph = graph, ?context = context))
        |> DataMap