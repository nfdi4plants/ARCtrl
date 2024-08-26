#r "nuget: DynamicObj, 3.0.0"

open DynamicObj

type IROCrateObject =
    abstract member SchemaType : string
    abstract member Id: string
    abstract member AdditionalType: string option

type Dataset (id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "schema.org/Dataset"

    member this.AdditionalType = additionalType

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

type InvestigationDataset(id: string) =
    // inheritance
    inherit Dataset(id, "Investigation")
    static member create(
        // mandatory
        id,
        // Properties from Thing
        identifier,
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
    ) =
        let ds = InvestigationDataset(id = id)

        // Properties from CreativeWork
        DynObj.setValueOpt ds (nameof citation) citation
        DynObj.setValueOpt ds (nameof comment) comment
        DynObj.setValueOpt ds (nameof creator) creator
        DynObj.setValueOpt ds (nameof dateCreated) dateCreated
        DynObj.setValueOpt ds (nameof dateModified) dateModified
        DynObj.setValueOpt ds (nameof datePublished) datePublished
        DynObj.setValueOpt ds (nameof hasPart) hasPart
        DynObj.setValueOpt ds (nameof headline) headline
        DynObj.setValueOpt ds (nameof mentions) mentions
        DynObj.setValueOpt ds (nameof url) url

        // Properties from Thing
        DynObj.setValueOpt ds (nameof description) description
        DynObj.setValue ds (nameof identifier) identifier

        ds

    static member tryGetCitation (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "citation" with
        | Some value -> Some value
        | None -> None

    static member tryGetComment (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "comment" with
        | Some value -> Some value
        | None -> None

    static member tryGetCreator (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "creator" with
        | Some value -> Some value
        | None -> None

    static member tryGetDateCreated (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "dateCreated" with
        | Some value -> Some value
        | None -> None

    static member tryGetDateModified (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "dateModified" with
        | Some value -> Some value
        | None -> None

    static member tryGetDatePublished (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "datePublished" with
        | Some value -> Some value
        | None -> None

    static member tryGetHasPart (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "hasPart" with
        | Some value -> Some value
        | None -> None

    static member tryGetHeadline (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "headline" with
        | Some value -> Some value
        | None -> None

    static member tryGetMentions (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "mentions" with
        | Some value -> Some value
        | None -> None

    static member tryGetUrl (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "url" with
        | Some value -> Some value
        | None -> None

    static member tryGetDescription (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "description" with
        | Some value -> Some value
        | None -> None

    static member tryGetIdentifier (ds: InvestigationDataset) =
        match DynObj.tryGetValue ds "identifier" with
        | Some value -> Some value
        | None -> None
        

type StudyDataset(id: string) =
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
        let ds = StudyDataset(id = id)

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

    static member tryGetAbout (ds: StudyDataset) =
        match DynObj.tryGetValue ds "about" with
        | Some value -> Some value
        | None -> None

    static member tryGetCitation (ds: StudyDataset) =
        match DynObj.tryGetValue ds "citation" with
        | Some value -> Some value
        | None -> None

    static member tryGetComment (ds: StudyDataset) =
        match DynObj.tryGetValue ds "comment" with
        | Some value -> Some value
        | None -> None

    static member tryGetCreator (ds: StudyDataset) =
        match DynObj.tryGetValue ds "creator" with
        | Some value -> Some value
        | None -> None

    static member tryGetDateCreated (ds: StudyDataset) =
        match DynObj.tryGetValue ds "dateCreated" with
        | Some value -> Some value
        | None -> None

    static member tryGetDateModified (ds: StudyDataset) =
        match DynObj.tryGetValue ds "dateModified" with
        | Some value -> Some value
        | None -> None

    static member tryGetDatePublished (ds: StudyDataset) =
        match DynObj.tryGetValue ds "datePublished" with
        | Some value -> Some value
        | None -> None

    static member tryGetHasPart (ds: StudyDataset) =
        match DynObj.tryGetValue ds "hasPart" with
        | Some value -> Some value
        | None -> None

    static member tryGetHeadline (ds: StudyDataset) =
        match DynObj.tryGetValue ds "headline" with
        | Some value -> Some value
        | None -> None

    static member tryGetMentions (ds: StudyDataset) =
        match DynObj.tryGetValue ds "mentions" with
        | Some value -> Some value
        | None -> None

    static member tryGetUrl (ds: StudyDataset) =
        match DynObj.tryGetValue ds "url" with
        | Some value -> Some value
        | None -> None

    static member tryGetIdentifier (ds: StudyDataset) =
        match DynObj.tryGetValue ds "identifier" with
        | Some value -> Some value
        | None -> None
        
    static member tryGetDescription (ds: StudyDataset) =
        match DynObj.tryGetValue ds "description" with
        | Some value -> Some value
        | None -> None
        

type AssayDataset(id: string) =
    // inheritance
    inherit Dataset(id, "Assay")
    static member create(
        // mandatory
        id,
        // Properties from Thing
        identifier,
        // optional
        // Properties from CreativeWork
        // Properties from Dataset
        ?measurementMethod,
        ?measurementTechnique,
        ?variableMeasured,
        // Properties from CreativeWork
        ?about,
        ?comment,
        ?creator,
        ?hasPart,
        ?url
    ) : AssayDataset =
        let ds = AssayDataset(id = id)

        // Properties from Dataset
        DynObj.setValueOpt ds (nameof measurementMethod) measurementMethod
        DynObj.setValueOpt ds (nameof measurementTechnique) measurementTechnique
        DynObj.setValueOpt ds (nameof variableMeasured) variableMeasured

        // Properties from CreativeWork
        DynObj.setValueOpt ds (nameof about) about
        DynObj.setValueOpt ds (nameof comment) comment
        DynObj.setValueOpt ds (nameof creator) creator
        DynObj.setValueOpt ds (nameof hasPart) hasPart
        DynObj.setValueOpt ds (nameof url) url

        // Properties from Thing
        DynObj.setValue ds (nameof identifier) identifier

        ds

    static member tryGetMeasurementMethod (ds: AssayDataset) =
        match DynObj.tryGetValue ds "measurementMethod" with
        | Some value -> Some value
        | None -> None

    static member tryGetMeasurementTechnique (ds: AssayDataset) =
        match DynObj.tryGetValue ds "measurementTechnique" with
        | Some value -> Some value
        | None -> None

    static member tryGetVariableMeasured (ds: AssayDataset) =
        match DynObj.tryGetValue ds "variableMeasured" with
        | Some value -> Some value
        | None -> None

    static member tryGetAbout (ds: AssayDataset) =
        match DynObj.tryGetValue ds "about" with
        | Some value -> Some value
        | None -> None

    static member tryGetComment (ds: AssayDataset) =
        match DynObj.tryGetValue ds "comment" with
        | Some value -> Some value
        | None -> None

    static member tryGetCreator (ds: AssayDataset) =
        match DynObj.tryGetValue ds "creator" with
        | Some value -> Some value
        | None -> None

    static member tryGetHasPart (ds: AssayDataset) =
        match DynObj.tryGetValue ds "hasPart" with
        | Some value -> Some value
        | None -> None

    static member tryGetUrl (ds: AssayDataset) =
        match DynObj.tryGetValue ds "url" with
        | Some value -> Some value
        | None -> None

    static member tryGetIdentifier (ds: AssayDataset) =
        match DynObj.tryGetValue ds "identifier" with
        | Some value -> Some value
        | None -> None

let interfaceOnly: IROCrateObject list =
    [
        InvestigationDataset.create(id = "./", identifier = "inv")
        StudyDataset.create(id ="./studies/study1", identifier = "study1")
        AssayDataset.create(id ="./assays/assay1", identifier = "assay1")
    ]

module ROCrateObject =

    let tryAsDataset (roco: IROCrateObject) =
        match roco with 
        | :? Dataset as ds when ds.SchemaType = "schema.org/Dataset" -> Some ds
        | _ -> None

    let tryAsInvestigationDataset (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Investigation", "schema.org/Dataset" -> 
            roco :?> InvestigationDataset |> Some // both type annotations are present
        | _ -> 
            None
    let tryAsStudyDataset (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Study", "schema.org/Dataset" -> 
            roco :?> StudyDataset |> Some // both type annotations are present
        | _ -> 
            None

    let tryAsAssayDataset (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Assay", "schema.org/Dataset" -> 
            roco :?> AssayDataset |> Some // both type annotations are present
        | _ -> 
            None

type LabProcess(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "bioschemas.org/LabProcess"

    member this.AdditionalType = additionalType

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

    static member create(
        // mandatory
        id,
        name,
        agent,
        object,
        result,
        // optional
        ?additionalType,
        ?executesLabProtocol,
        ?parameterValue,
        ?endTime,
        ?disambiguatingDescription
    ) =
        let lp = LabProcess(id)

        DynObj.setValue lp (nameof id) id
        DynObj.setValue lp (nameof name) name
        DynObj.setValue lp (nameof agent) agent
        DynObj.setValue lp (nameof object) object
        DynObj.setValue lp (nameof result) result

        DynObj.setValueOpt lp (nameof additionalType) additionalType
        DynObj.setValueOpt lp (nameof executesLabProtocol) executesLabProtocol
        DynObj.setValueOpt lp (nameof parameterValue) parameterValue
        DynObj.setValueOpt lp (nameof endTime) endTime
        DynObj.setValueOpt lp (nameof disambiguatingDescription) disambiguatingDescription

        lp

