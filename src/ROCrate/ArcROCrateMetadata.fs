namespace ARCtrl.ROCrate

open DynamicObj

type ArcROCrateMetadata(?about : LDNode) as this =

    inherit LDNode(id = "ro-crate-metadata",schemaType = ResizeArray([|"CreativeWork"|]))

    do DynObj.setOptionalProperty (nameof about) about this

    do
        let conformsTo = DynamicObj()
        conformsTo.SetProperty("@id", "https://w3id.org/ro/crate/1.1")
        this.SetProperty("conformsTo", conformsTo)

    do
        let context = LDContext()
        context.AddMapping("sdo", "http://schema.org/")
        context.AddMapping("arc", "http://purl.org/nfdi4plants/ontology/")
        context.AddMapping("CreativeWork", "sdo:CreativeWork")
        context.AddMapping("about", "sdo:about")
        context.AddMapping("conformsTo", "sdo:conformsTo")
        this.SetProperty("@context", context)

