namespace ARCtrl.ROCrate

open DynamicObj

type ArcROCrateMetadata(?about : LDObject) as this =

    inherit LDObject(id = "ro-crate-metadata",schemaType = ResizeArray([|"CreativeWork"|]))

    do DynObj.setOptionalProperty (nameof about) about this

    do
        let conformsTo = DynamicObj()
        conformsTo.SetProperty("@id", "https://w3id.org/ro/crate/1.1")
        this.SetProperty("conformsTo", conformsTo)

    do
        let context = LDContext()
        context.SetProperty("sdo", "http://schema.org/")
        context.SetProperty("arc", "http://purl.org/nfdi4plants/ontology/")
        context.SetProperty("CreativeWork", "sdo:CreativeWork")
        context.SetProperty("about", "sdo:about")
        context.SetProperty("conformsTo", "sdo:conformsTo")
        this.SetProperty("@context", context)

