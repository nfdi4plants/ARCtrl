#r "nuget: DynamicObj, 3.0.0"

open DynamicObj

type IROCrateObject =
    abstract member SchemaType : string
    abstract member Id: string
    abstract member AdditionalType: string option

type ROCrateObject(id:string, schemaType: string, ?additionalType) =
    inherit DynamicObj()

    let mutable _schemaType = "schema.org/Dataset"
    let mutable _additionalType = additionalType

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = _schemaType
        and set(value) = _schemaType <- value

    member this.AdditionalType
        with get() = _additionalType
        and set(value) = _additionalType <- value

    interface IROCrateObject with
        member this.SchemaType = schemaType
        member this.Id = id
        member this.AdditionalType = additionalType

type Dataset (id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/Dataset", ?additionalType = additionalType)

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

///
type Study(id: string) =
    // inheritance
    inherit Dataset(id, "Study")
    static member create(
        // mandatory
        id,
        // Properties from Thing
        identifier,
        // optional
        // Properties from CreativeWork
        ?about,
        ?citation,
        ?comment,
        ?creator,
        ?dateCreated,
        ?dateModified,
        ?datePublished,
        ?hasPart,
        ?headline,
        ?url,
        // Properties from Thing
        ?description
    ) =
        let ds = Study(id = id)

        // Properties from CreativeWork
        DynObj.setValueOpt ds (nameof about) about
        DynObj.setValueOpt ds (nameof citation) citation
        DynObj.setValueOpt ds (nameof comment) comment
        DynObj.setValueOpt ds (nameof creator) creator
        DynObj.setValueOpt ds (nameof dateCreated) dateCreated
        DynObj.setValueOpt ds (nameof dateModified) dateModified
        DynObj.setValueOpt ds (nameof datePublished) datePublished
        DynObj.setValueOpt ds (nameof hasPart) hasPart
        DynObj.setValueOpt ds (nameof headline) headline
        DynObj.setValueOpt ds (nameof url) url

        // Properties from Thing
        DynObj.setValueOpt ds (nameof description) description
        DynObj.setValue ds (nameof identifier) identifier

        ds

module I =
    type Investigation(
        id,
        // Properties from Thing
        identifier: obj,
        // optional
        // Properties from CreativeWork
        ?citation,
        ?comment,
        ?creator,
        ?dateCreated,
        ?dateModified,
        ?datePublished,
        ?hasPart,
        ?headline,
        ?mentions,
        ?url,
        // Properties from Thing
        ?description
    ) as i =

        inherit Dataset(id, "Investigation")

        do 

            // Properties from CreativeWork
            DynObj.setValueOpt i (nameof citation) citation
            DynObj.setValueOpt i (nameof comment) comment
            DynObj.setValueOpt i (nameof creator) creator
            DynObj.setValueOpt i (nameof dateCreated) dateCreated
            DynObj.setValueOpt i (nameof dateModified) dateModified
            DynObj.setValueOpt i (nameof datePublished) datePublished
            DynObj.setValueOpt i (nameof hasPart) hasPart
            DynObj.setValueOpt i (nameof headline) headline
            DynObj.setValueOpt i (nameof mentions) mentions
            DynObj.setValueOpt i (nameof url) url

            // Properties from Thing
            DynObj.setValueOpt i (nameof description) description
            DynObj.setValue i (nameof identifier) identifier

I.Investigation("lol", [1.])
I.Investigation("lol", "")
I.Investigation("lol", [2])

Study.create("lol", [2])
Study.create("lol", [2.])
Study.create("lol", "")