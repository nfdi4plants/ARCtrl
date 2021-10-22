namespace ISADotNet.XLSX.AssayFile

open ISADotNet
open ISADotNet.XLSX
open AnnotationColumn


module ValueOrder = 

    let private tryInt (str:string) =
        match System.Int32.TryParse str with
        | true,int -> Some int
        | _ -> None

    let orderName = "ValueIndex"

    let createOrderComment (index : int) =
        Comment.fromString orderName (string index)

    let tryGetIndex (comments : Comment list) =
        comments 
        |> API.CommentList.tryItem orderName 
        |> Option.bind tryInt

    let tryGetParameterIndex (param : ProtocolParameter) =
        param.ParameterName 
        |> Option.bind (fun oa -> 
            oa.Comments |> Option.bind tryGetIndex
        )

    let tryGetParameterValueIndex (paramValue : ProcessParameterValue) =
        paramValue.Category 
        |> Option.bind tryGetParameterIndex

    let tryGetFactorIndex (factor : Factor) =
        factor.FactorType 
        |> Option.bind (fun oa -> 
            oa.Comments |> Option.bind tryGetIndex
        )
      
    let tryGetFactorValueIndex (factorValue : FactorValue) =
        factorValue.Category 
        |> Option.bind tryGetFactorIndex

    let tryGetCharacteristicIndex (characteristic : MaterialAttribute) =
        characteristic.CharacteristicType 
        |> Option.bind (fun oa -> 
            oa.Comments |> Option.bind tryGetIndex
        )
      
    let tryGetCharacteristicValueIndex (characteristicValue : MaterialAttributeValue) =
        characteristicValue.Category 
        |> Option.bind tryGetCharacteristicIndex

[<AutoOpen>]
module ValueOrderExtensions = 
    
    type Factor with
        
        /// Create a ISAJson Factor from ISATab string entries
        static member fromStringWithValueOrder (name:string) (term:string) (source:string) (accession:string) valueIndex =
            Factor.fromStringWithComments name term source accession [ValueOrder.createOrderComment valueIndex]

        /// Create a ISAJson Factor from ISATab string entries
        static member fromStringWithNumberValueOrder (name:string) (term:string) (source:string) (accession:string) valueIndex =
            Factor.fromStringWithNumberAndComments name term source accession [ValueOrder.createOrderComment valueIndex]

        member this.GetValueIndex() = ValueOrder.tryGetFactorIndex this |> Option.get

    type FactorValue with

        member this.GetValueIndex() = ValueOrder.tryGetFactorValueIndex this |> Option.get

    type MaterialAttribute with
    
        /// Create a ISAJson characteristic from ISATab string entries
        static member fromStringWithValueOrder (term:string) (source:string) (accession:string) valueIndex =
            MaterialAttribute.fromStringWithComments term source accession [ValueOrder.createOrderComment valueIndex]

        /// Create a ISAJson characteristic from ISATab string entries
        static member fromStringWithNumberValueOrder (term:string) (source:string) (accession:string) valueIndex =
            MaterialAttribute.fromStringWithNumberAndComments term source accession [ValueOrder.createOrderComment valueIndex]

        member this.GetValueIndex() = ValueOrder.tryGetCharacteristicIndex this |> Option.get

    type MaterialAttributeValue with
    
            member this.GetValueIndex() = ValueOrder.tryGetCharacteristicValueIndex this |> Option.get

    type ProtocolParameter with
    
        /// Create a ISAJson parameter from ISATab string entries
        static member fromStringWithValueOrder (term:string) (source:string) (accession:string) valueIndex =
            ProtocolParameter.fromStringWithComments term source accession [ValueOrder.createOrderComment valueIndex]

        /// Create a ISAJson parameter from ISATab string entries
        static member fromStringWithNumberValueOrder (term:string) (source:string) (accession:string) valueIndex =
            ProtocolParameter.fromStringWithNumberAndComments term source accession [ValueOrder.createOrderComment valueIndex]

        member this.GetValueIndex() = ValueOrder.tryGetParameterIndex this |> Option.get

    type ProcessParameterValue with
    
        member this.GetValueIndex() = ValueOrder.tryGetParameterValueIndex this |> Option.get

/// Functions for parsing nodes and node values of an annotation table
///
/// The distinction between columns and nodes is made, as some columns are just used to give additional information for other columns. These columns are grouped together as one node
/// e.g a "Term Source REF" column after a "Parameter" Column adds info to the Parameter Column
///
/// On the other hand, some colums are stand alone nodes, e.g. "Sample Name"
module AnnotationNode = 
    
    type NodeHeader = ColumnHeader seq

    /// Splits the headers of an annotation table into nodes
    ///
    /// The distinction between columns and nodes is made, as some columns are just used to give additional information for other columns. These columns are grouped together as one node
    /// e.g a "Term Source REF" column after a "Parameter" Column adds info to the Parameter Column
    ///
    /// On the other hand, some colums are stand alone nodes, e.g. "Sample Name"
    let splitIntoNodes (headers : seq<string>) =
        headers
        |> Seq.groupWhen false (fun header -> 
            match (AnnotationColumn.ColumnHeader.fromStringHeader header).Kind with
            | "Unit"                    -> false
            | "Term Source REF"         -> false
            | "Term Accession Number"   -> false
            | _ -> true
        )

    /// If the headers of a node depict a unit, returns a function for parsing the values of the matrix to this unit
    let tryGetUnitGetterFunction (headers:string seq) =

        Seq.tryPick tryParseUnitHeader headers
        |> Option.map (fun h -> 
            let unitNameGetter matrix i = 
                Dictionary.tryGetValue (i,h.HeaderString) matrix       
            let termAccessionGetter =
                match Seq.tryPick (tryParseTermAccessionNumberHeader h) headers with
                | Some h ->
                    fun matrix i -> 
                        match Dictionary.tryGetValue (i,h.HeaderString) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> fun _ _ -> None
            let termSourceGetter =
                match Seq.tryPick (tryParseTermSourceReferenceHeader h) headers with
                | Some h ->
                    fun matrix i -> 
                        match Dictionary.tryGetValue (i,h.HeaderString) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> fun _ _ -> None
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                OntologyAnnotation.make 
                    None
                    (unitNameGetter matrix i |> Option.map AnnotationValue.fromString)
                    (termSourceGetter matrix i)
                    (termAccessionGetter matrix i |> Option.map URI.fromString)  
                    None
        )
    
    /// If the headers of a node depict a value header (parameter,factor,characteristic), returns the category and a function for parsing the values of the matrix to the values
    let tryGetValueGetter (columnOrder : int) hasUnit (valueHeader : ColumnHeader) (headers:string seq) =
        let category1, termAccessionGetter =
            match Seq.tryPick (tryParseTermAccessionNumberHeader valueHeader) headers with
            | Some h ->
                h.Term,
                fun (matrix:System.Collections.Generic.Dictionary<int*string,string>) (i:int) -> 
                    match Dictionary.tryGetValue (i,h.HeaderString) matrix with
                    | Some "user-specific" -> None
                    | Some v -> Some v
                    | _ -> None 
            | None -> None, fun _ _ -> None
        let category2, termSourceGetter =
            match Seq.tryPick (tryParseTermSourceReferenceHeader valueHeader) headers with
            | Some h ->
                h.Term,
                fun matrix i -> 
                    match Dictionary.tryGetValue (i,h.HeaderString) matrix with
                    | Some "user-specific" -> None
                    | Some v -> Some v
                    | _ -> None 
            | None -> None, fun _ _ -> None
    
        let category =           
            // Merge "Term Source REF" (TSR) and "Term Accession Number" (TAN) from different OntologyAnnotations
            mergeOntology valueHeader.Term category1 |> mergeOntology category2
            |> Option.map (fun oa -> 
                oa.Comments |> Option.defaultValue []
                |> API.CommentList.add (ValueOrder.createOrderComment columnOrder)
                |> API.OntologyAnnotation.setComments oa
            )


        let valueGetter = 
            fun matrix i ->
                let value = 
                    match Dictionary.tryGetValue (i,valueHeader.HeaderString) matrix with
                    | Some "user-specific" -> None
                    // Trim() should remove any accidental whitespaces at the beginning or end of a term
                    | Some v -> Some v
                    | _ -> None 

                // Set termAcession and termSource of the value to None if they are the same as the header. 
                // This is done as Swate fills empty with the header but these values should not be transferred to the isa model
                let termAccession,termSource = 
                    if hasUnit then 
                        None, None
                    else
                        match termAccessionGetter matrix i,termSourceGetter matrix i,category with
                        | Some a, Some s,Some c ->
                            match c.TermAccessionNumber,c.TermSourceREF with
                            | Some ca, Some cs when a.Contains ca && s.Contains cs ->
                                None,None
                            | _ -> Some a, Some s
                        | (a,s,c) -> a,s
                Value.fromOptions 
                    value
                    termAccession
                    termSource
        category,valueGetter

    /// If the headers of a node depict a parameter, returns the parameter and a function for parsing the values of the matrix to the values of this parameter
    let tryGetParameterGetter (columnOrder : int) (headers:string seq) =
        Seq.tryPick tryParseParameterHeader headers
        |> Option.map (fun h -> 
            let unitGetter = tryGetUnitGetterFunction headers
                  
            let category,valueGetter = tryGetValueGetter columnOrder unitGetter.IsSome h headers                              
                
            let parameter = category |> Option.map (Some >> ProtocolParameter.make None)

            parameter,
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                ProcessParameterValue.make 
                    parameter
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )
    
    /// If the headers of a node depict a factor, returns the factor and a function for parsing the values of the matrix to the values of this factor
    let tryGetFactorGetter (columnOrder : int) (headers:string seq) =
        Seq.tryPick tryParseFactorHeader headers
        |> Option.map (fun h -> 
            let unitGetter = tryGetUnitGetterFunction headers
            
            let category,valueGetter = tryGetValueGetter columnOrder unitGetter.IsSome h headers    
                    
            let factor = 
                category
                |> Option.map (fun oa ->  
                    Factor.make None (oa.Name |> Option.map AnnotationValue.toString) (Some oa) None
                )
            
            factor,
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                FactorValue.make 
                    None
                    factor
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )

    /// If the headers of a node depict a characteristic, returns the characteristic and a function for parsing the values of the matrix to the values of this characteristic
    let tryGetCharacteristicGetter (columnOrder : int) (headers:string seq) =
        Seq.tryPick tryParseCharacteristicsHeader headers
        |> Option.map (fun h -> 
            let unitGetter = tryGetUnitGetterFunction headers
                  
            let category,valueGetter = tryGetValueGetter columnOrder unitGetter.IsSome h headers    
                    
            let characteristic = category |> Option.map (Some >> MaterialAttribute.make None)            
            
            characteristic,
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                MaterialAttributeValue.make 
                    None
                    characteristic
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )

    /// If the headers of a node depict a sample name, returns a function for parsing the values of the matrix to the sample names
    let tryGetDataFileGetter (headers:string seq) =
        Seq.tryPick tryParseDataFileName headers
        |> Option.map (fun h -> 

            let dataType = 
                if h.Kind = "Image File" then Some DataFile.ImageFile
                elif h.Kind = "Raw Data File" then Some DataFile.RawDataFile
                elif h.Kind = "Derived Data File" then Some DataFile.DerivedDataFile 
                else None
            let numberComment = h.Number |> Option.map (string >> (Comment.fromString "Number") >> List.singleton)
            
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                
                Data.make
                    None
                    (Dictionary.tryGetValue (i,h.HeaderString) matrix)
                    dataType
                    numberComment
        )

    /// If the headers of a node depict a sample name, returns a function for parsing the values of the matrix to the sample names
    let tryGetSampleNameGetter (headers:string seq) =
        Seq.tryPick tryParseSampleName headers
        |> Option.map (fun h -> 
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                Dictionary.tryGetValue (i,h.HeaderString) matrix
        )

    /// If the headers of a node depict a source name, returns a function for parsing the values of the matrix to the source names
    let tryGetSourceNameGetter (headers:string seq) =
        Seq.tryPick tryParseSourceName headers
        |> Option.map (fun h -> 
            fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
                Dictionary.tryGetValue (i,h.HeaderString) matrix
        )
    
    /// Returns true, if the headers contain a value node: characteristic, parameter or factor
    let isValueNode (headers:string seq) =
        (Seq.exists (tryParseFactorHeader >> Option.isSome) headers)
        ||
        (Seq.exists (tryParseCharacteristicsHeader >> Option.isSome) headers)
        ||
        (Seq.exists (tryParseParameterHeader >> Option.isSome) headers)