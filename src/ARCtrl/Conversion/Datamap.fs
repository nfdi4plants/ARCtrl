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

    static member composeFragmentDescriptors (datamap : Datamap) : ResizeArray<LDNode> =
        datamap.DataContexts
        |> ResizeArray.map BaseTypes.composeFragmentDescriptor

    static member decomposeFragmentDescriptors (fragmentDescriptors : ResizeArray<LDNode>, ?graph : LDGraph, ?context : LDContext) : Datamap =
        fragmentDescriptors
        |> ResizeArray.map (fun fd -> BaseTypes.decomposeFragmentDescriptor(fd, ?graph = graph, ?context = context))
        |> Datamap

    // Forward compatibility: the members below read the datamap as the new ARC Datamap RO-Crate profile defines it,
    // see https://w3id.org/arc/ro-crate/datamap/0.1. There a dataset names its entries in mentions, each as a
    // descriptor that is about the fragment and lists the explication. ARCtrl does not write this model yet.

    /// True, if the node is a descriptor: an entity of both the types Statement and ItemList.
    static member validateDescriptor (node : LDNode, ?context : LDContext) =
        node.HasType("http://schema.org/Statement", ?context = context)
        && node.HasType("http://schema.org/ItemList", ?context = context)

    /// Returns the file or the fragment of a file that a descriptor describes. The descriptor names it in about. If
    /// it does not, the file of the graph is returned that names the descriptor in subjectOf.
    static member private tryGetDescribedFile (descriptor : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let isFile (node : LDNode) = LDFile.validate(node, ?context = context)
        match descriptor.GetPropertyNodes(LDFile.about, ?graph = graph, ?context = context) |> Seq.tryFind isFile with
        | Some file -> Some file
        | None ->
            let namesDescriptor (value : obj) =
                match value with
                | :? LDRef as r -> r.Id = descriptor.Id
                | :? LDNode as n -> n.Id = descriptor.Id
                | _ -> false
            graph
            |> Option.bind (fun graph ->
                graph.Nodes
                |> Seq.tryFind (fun node ->
                    isFile node
                    && node.GetPropertyValues(LDPropertyValue.subjectOf, ?context = context) |> Seq.exists namesDescriptor
                )
            )

    /// Returns the PropertyValue among the items of a descriptor that states the explication of the described data.
    /// The ARC Datamap profile names it Explication and gives it the propertyID of the explication.
    static member private tryGetExplicationItem (descriptor : LDNode, ?graph : LDGraph, ?context : LDContext) =
        descriptor.GetPropertyNodes("http://schema.org/itemListElement", ?graph = graph, ?context = context)
        |> Seq.tryFind (fun item ->
            item.HasType(LDPropertyValue.schemaType, ?context = context)
            && (
                LDPropertyValue.tryGetNameAsString(item, ?context = context) = Some "Explication"
                || match LDPropertyValue.tryGetPropertyIDAsString(item, ?context = context) with
                   | Some propertyID -> propertyID.Replace("http://", "https://") = "https://purl.org/nfdi4plants/ontology/dpbo/DPBO_0000111"
                   | None -> false
            )
        )

    /// Reads the entries of a datamap from the descriptors among the given nodes. A descriptor is about a file or a
    /// fragment of a file, and lists the PropertyValue that states the explication. Any other node is left out.
    static member decomposeDescriptors (nodes : seq<LDNode>, ?graph : LDGraph, ?context : LDContext) : ResizeArray<DataContext> =
        nodes
        |> Seq.choose (fun node ->
            if DatamapConversion.validateDescriptor(node, ?context = context) then
                DatamapConversion.tryGetDescribedFile(node, ?graph = graph, ?context = context)
                |> Option.map (fun file ->
                    let statement = DatamapConversion.tryGetExplicationItem(node, ?graph = graph, ?context = context)
                    BaseTypes.decomposeDataContext(file, ?statement = statement, ?graph = graph, ?context = context)
                )
            else
                None
        )
        |> ResizeArray

    /// Reads the datamap of a dataset. Its entries are the fragment descriptors that the dataset names in
    /// variableMeasured, as ARCtrl writes them, followed by the descriptors that it names in mentions, as the ARC
    /// Datamap profile defines them.
    static member decomposeDatamap (dataset : LDNode, ?graph : LDGraph, ?context : LDContext) : Datamap option =
        let datamap =
            LDDataset.getVariableMeasuredAsFragmentDescriptors(dataset, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
        let mentions = dataset.GetPropertyNodes(LDDataset.mentions, ?graph = graph, ?context = context)
        datamap.DataContexts.AddRange(DatamapConversion.decomposeDescriptors(mentions, ?graph = graph, ?context = context))
        datamap |> Option.fromValueWithDefault (Datamap.init())