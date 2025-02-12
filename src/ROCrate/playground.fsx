#r "nuget: DynamicObj, 3.0.0"

open DynamicObj

type ILDNode =
    abstract member SchemaType : string
    abstract member Id: string
    abstract member AdditionalType: string option

type LDNode(id:string, schemaType: string, ?additionalType) =
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

    interface ILDNode with
        member this.SchemaType = schemaType
        member this.Id = id
        member this.AdditionalType = additionalType

type Dataset (id: string, ?additionalType: string) =
    inherit LDNode(id = id, schemaType = "schema.org/Dataset", ?additionalType = additionalType)

     //interface implementations
    interface ILDNode with 
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
        DynObj.setOptionalProperty ds (nameof about) about
        DynObj.setOptionalProperty ds (nameof citation) citation
        DynObj.setOptionalProperty ds (nameof comment) comment
        DynObj.setOptionalProperty ds (nameof creator) creator
        DynObj.setOptionalProperty ds (nameof dateCreated) dateCreated
        DynObj.setOptionalProperty ds (nameof dateModified) dateModified
        DynObj.setOptionalProperty ds (nameof datePublished) datePublished
        DynObj.setOptionalProperty ds (nameof hasPart) hasPart
        DynObj.setOptionalProperty ds (nameof headline) headline
        DynObj.setOptionalProperty ds (nameof url) url

        // Properties from Thing
        DynObj.setOptionalProperty ds (nameof description) description
        DynObj.setProperty ds (nameof identifier) identifier

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
            DynObj.setOptionalProperty i (nameof citation) citation
            DynObj.setOptionalProperty i (nameof comment) comment
            DynObj.setOptionalProperty i (nameof creator) creator
            DynObj.setOptionalProperty i (nameof dateCreated) dateCreated
            DynObj.setOptionalProperty i (nameof dateModified) dateModified
            DynObj.setOptionalProperty i (nameof datePublished) datePublished
            DynObj.setOptionalProperty i (nameof hasPart) hasPart
            DynObj.setOptionalProperty i (nameof headline) headline
            DynObj.setOptionalProperty i (nameof mentions) mentions
            DynObj.setOptionalProperty i (nameof url) url

            // Properties from Thing
            DynObj.setOptionalProperty i (nameof description) description
            DynObj.setProperty i (nameof identifier) identifier

I.Investigation("lol", [1.])
I.Investigation("lol", "")
I.Investigation("lol", [2])

Study.create("lol", [2])
Study.create("lol", [2.])
Study.create("lol", "")