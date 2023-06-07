namespace ISA.Spreadsheet.AssayFile

open ISA
open ISA.Spreadsheet
open System.Text.RegularExpressions

/// Functions for parsing single column headers of an annotation table
module AnnotationColumn =

    [<Literal>]
    let OboPurlURL = @"http://purl.obolibrary.org/obo/"

    module RegexPattern =

        // Pattern matches start of line and then any number of characters except newline, open bracket and open curly bracket.
        let columnTypePattern = @"^[^[(\n]*" //@"[^\[(]*(?= [\[\(])"
        let namePattern = @"(?<= \[).*(?=[\]])"
        let nameNumberPattern = @"(?<= \[)[^#\]]*(?=[\]#])"
        // [\w-]+ = Pattern matches any letter, number, underscore and minus, but minimum 1
        // (:|_) = ':' or '_', latter is important for url
        let ontologySourcePattern = "[\w-]+(:|_)[\w-]+"//@"(?<=\()\S+:[^;)#]*(?=[\)\#])"
        let numberPattern = @"(?<=#)\d+(?=[\)\]])"

    /// Typed depiction of a Swate Header: Kind [TermName] (#Number, #tOntology)
    type ColumnHeader =
        {
            HeaderString : string
            Kind : string
            Term : OntologyAnnotation option
            Number : int Option
        }

        /// Creater helper function 
        static member create headerString kind term number =
            {
                HeaderString = headerString
                Kind = kind
                Term = term 
                Number = number       
            }

        /// Parses a string to a column header
        static member fromStringHeader (header : string) =
                  
            let originalHeader = header
            let header = header.Trim()

            let numberRegex = Regex.Match(header,RegexPattern.numberPattern)
            let nameRegex = 
                let namePattern = if numberRegex.Success then RegexPattern.nameNumberPattern else RegexPattern.namePattern
                Regex.Match(header,namePattern)
            let columnTypePatternRegex = Regex.Match(header,RegexPattern.columnTypePattern)
            let ontologySourceRegex = Regex.Match(header,RegexPattern.ontologySourcePattern)

            let number = if numberRegex.Success then Some (int numberRegex.Value) else None
            let numberComment = number |> Option.map (string >> (Comment.fromString "Number") >> List.singleton)

            // Parsing a header of shape: Kind [TermName] (#Number, #tOntology)
            if nameRegex.Success then
                                                                             
                let ontology = OntologyAnnotation.fromString nameRegex.Value "" ""

                ColumnHeader.create originalHeader (columnTypePatternRegex.Value.Trim()) (Some {ontology with Comments = numberComment}) number

            // Parsing a header of shape: Kind (#Number)
            elif columnTypePatternRegex.Success then
                let kind = (columnTypePatternRegex.Value.Trim())

                let ontology = 
                    if ontologySourceRegex.Success then 
                        ontologySourceRegex.Value.Split ':'
                        |> fun o -> 
                            let tsr = o.[0]
                            let tanNumberOnly = o.[1] 
                            let tan = sprintf "%s%s_%s" OboPurlURL tsr tanNumberOnly//$"{OboPurlURL}{tsr}_{tanNumberOnly}"
                            OntologyAnnotation.fromString "" tsr tan
                        |> fun ontology -> {ontology with Comments = numberComment}
                        |> Some
                    else None

                ColumnHeader.create originalHeader kind ontology number

            // Parsing a header of shape: Kind
            else
                ColumnHeader.create originalHeader header None None
        
    /// If both options have a value, updates the fields of the first ontology with the fields of the second ontology
    let mergeOntology (termSourceHeaderOntology : OntologyAnnotation Option) (termAccessionHeaderOntology : OntologyAnnotation Option) =
        match termSourceHeaderOntology, termAccessionHeaderOntology with
        | Some oa1, Some oa2 -> API.Update.UpdateByExisting.updateRecordType oa1 oa2 |> Some
        | Some oa, None -> Some oa
        | None, Some oa -> Some oa
        | None, None -> None
        
    /// Parses to ColumnHeader, if the given header describes a Term Source Reference
    let tryParseTermSourceReferenceHeader (termHeader:ColumnHeader) (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Term Source REF" (*&& h.Number = termHeader.Number*) -> 
            //match h.Term,termHeader.Term with
            //| None, None -> Some h
            //| Some t1, Some t2 when t1.Name = t2.Name -> Some h
            //| _ -> None
            Some h
        | _ -> None
    
    /// Parses to ColumnHeader, if the given header describes a Term Accession Number
    let tryParseTermAccessionNumberHeader (termHeader:ColumnHeader) (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Term Accession Number"  (*&& h.Number = termHeader.Number*) -> 
            //match h.Term,termHeader.Term with
            //| None, None -> Some h
            //| Some t1, Some t2 when t1.Name = t2.Name -> Some h
            //| _ -> None
            Some h
        | _ -> None
    
    /// Parses to ColumnHeader, if the given header describes a Parameter Value
    let tryParseParameterHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Parameter" || h.Kind = "Parameter Value" ->
            Some h
        | _ -> None
    
    /// Parses to ColumnHeader, if the given header describes a Factor Value
    let tryParseFactorHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Factor" || h.Kind = "Factor Value" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes a Characteristics Value
    let tryParseCharacteristicsHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Characteristics" || h.Kind = "Characteristic" ||h.Kind = "Characteristics Value" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes a Component Value
    let tryParseComponentHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Component" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes a Characteristic Value
    let tryParseProtocolREFHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Protocol REF" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes a Characteristics Value
    let tryParseProtocolTypeHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Protocol Type" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes a Unit of measurement
    let tryParseUnitHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Unit" ->
            Some h
        | _ -> None   
    
    /// Parses to ColumnHeader, if the given header describes a sample name
    let tryParseSampleName (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Sample Name" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes source name
    let tryParseSourceName (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Source Name" ->
            Some h
        | _ -> None

    /// Parses to ColumnHeader, if the given header describes a data file
    let tryParseDataFileName (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Data File Name" -> Some h
        | h when h.Kind = "Raw Data File" -> Some h   
        | h when h.Kind = "Derived Data File" -> Some h  
        | h when h.Kind = "Image File" -> Some h  
        | _ -> None

    /// Returns true, if the given header describes a data column
    let isData header = tryParseDataFileName header |> Option.isSome 

    /// Returns true, if the given header describes a sample name
    let isSample header = tryParseSampleName header |> Option.isSome 

    /// Returns true, if the given header describes a source name
    let isSource header = tryParseSourceName header |> Option.isSome 