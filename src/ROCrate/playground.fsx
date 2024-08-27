#r "nuget: DynamicObj, 3.0.0"

open DynamicObj

type IROCrateObject =
    abstract member SchemaType : string
    abstract member Id: string
    abstract member AdditionalType: string option

type Dataset (id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "schema.org/Dataset"

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = _schemaType
        and set(value) = _schemaType <- value

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
            match roco with
            | :? InvestigationDataset as ids -> Some ids
            | _ -> None
        | _ -> 
            None

    let tryAsStudyDataset (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Study", "schema.org/Dataset" -> 
            match roco with
            | :? StudyDataset as sds -> Some sds
            | _ -> None
        | _ -> 
            None

    let tryAsAssayDataset (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Assay", "schema.org/Dataset" -> 
            match roco with
            | :? AssayDataset as ads -> Some ads
            | _ -> None
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

type LabProtocol(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "bioschemas.org/LabProtocol"

    member this.AdditionalType = additionalType

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

    static member create(
        // mandatory
        id,
        ?name,
        ?intendedUse,
        ?description,
        ?url,
        ?comment,
        ?version,
        ?labEquipment,
        ?reagent,
        ?computationalTool
    ) =
        let lp = LabProcess(id)

        DynObj.setValueOpt lp (nameof name) name
        DynObj.setValueOpt lp (nameof intendedUse) intendedUse
        DynObj.setValueOpt lp (nameof description) description
        DynObj.setValueOpt lp (nameof url) url
        DynObj.setValueOpt lp (nameof comment) comment
        DynObj.setValueOpt lp (nameof version) version
        DynObj.setValueOpt lp (nameof labEquipment) labEquipment
        DynObj.setValueOpt lp (nameof reagent) reagent
        DynObj.setValueOpt lp (nameof computationalTool) computationalTool

        lp

type Sample(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "bioschemas.org/Sample"

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
        ?additionalProperty,
        ?derivesFrom
    ) =
        let s = Sample(id)

        DynObj.setValue s (nameof name) name

        DynObj.setValueOpt s (nameof additionalProperty) additionalProperty
        DynObj.setValueOpt s (nameof derivesFrom) derivesFrom

        s

type Data(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "schema.org/MediaObject"

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
        ?comment,
        ?encodingFormat,
        ?disambiguatingDescription
    ) =
        let d = Data(id)

        DynObj.setValue d (nameof name) name

        DynObj.setValueOpt d (nameof comment) comment
        DynObj.setValueOpt d (nameof encodingFormat) encodingFormat
        DynObj.setValueOpt d (nameof disambiguatingDescription) disambiguatingDescription

        d

type PropertyValue(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "schema.org/PropertyValue"

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
        value,
        ?propertyID,
        ?unitCode,
        ?unitText,
        ?valueReference,
        ?additionalType
    ) =
        let pv = PropertyValue(id, ?additionalType = additionalType)

        DynObj.setValue pv (nameof name) name
        DynObj.setValue pv (nameof value) value

        DynObj.setValueOpt pv (nameof propertyID) propertyID
        DynObj.setValueOpt pv (nameof unitCode) unitCode
        DynObj.setValueOpt pv (nameof unitText) unitText
        DynObj.setValueOpt pv (nameof valueReference) valueReference
        
        pv

type Person(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "schema.org/Person"

    member this.AdditionalType = additionalType

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

    static member create(
        // mandatory
        id,
        givenName,
        ?familyName,
        ?email,
        ?identifier,
        ?affiliation,
        ?jobTitle,
        ?additionalName,
        ?address,
        ?telephone,
        ?faxNumber,
        ?disambiguatingDescription
    ) =
        let p = Person(id)

        DynObj.setValue p (nameof givenName) givenName

        DynObj.setValueOpt p (nameof familyName) familyName
        DynObj.setValueOpt p (nameof email) email
        DynObj.setValueOpt p (nameof identifier) identifier
        DynObj.setValueOpt p (nameof affiliation) affiliation
        DynObj.setValueOpt p (nameof jobTitle) jobTitle
        DynObj.setValueOpt p (nameof additionalName) additionalName
        DynObj.setValueOpt p (nameof address) address
        DynObj.setValueOpt p (nameof telephone) telephone
        DynObj.setValueOpt p (nameof faxNumber) faxNumber
        DynObj.setValueOpt p (nameof disambiguatingDescription) disambiguatingDescription

        p


type ScholarlyArticle(id: string, ?additionalType: string) =
    inherit DynamicObj()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = "schema.org/ScholarlyArticle"

    member this.AdditionalType = additionalType

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

    static member create(
        // mandatory
        id,
        headline,
        identifier,
        ?author,
        ?url,
        ?creativeWorkStatus,
        ?disambiguatingDescription
    ) =
        let sa = ScholarlyArticle(id)

        DynObj.setValue sa (nameof headline) headline
        DynObj.setValue sa (nameof identifier) identifier

        DynObj.setValueOpt sa (nameof author) author
        DynObj.setValueOpt sa (nameof url) url
        DynObj.setValueOpt sa (nameof creativeWorkStatus) creativeWorkStatus
        DynObj.setValueOpt sa (nameof disambiguatingDescription) disambiguatingDescription

        sa