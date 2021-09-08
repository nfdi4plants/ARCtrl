namespace ISADotNet.XLSX.AssayFile

open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.XLSX
open System.Text.RegularExpressions

/// Functions for parsing single column headers of an annotation table
module AnnotationColumn =

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
        static member fromStringHeader header =
                  
            let kindPattern = @".*(?= [\[\(])"
            let namePattern = @"(?<= \[)[^#\]]*(?=[\]#])"
            let ontologySourcePattern = @"(?<=\()\S+:[^;)#]*(?=[\)\#])"
            let numberPattern = @"(?<=#)\d+(?=[\)\]])"

            let nameRegex = Regex.Match(header,namePattern)
            let kindRegex = Regex.Match(header,kindPattern)
            let ontologySourceRegex = Regex.Match(header,ontologySourcePattern)
            let numberRegex = Regex.Match(header,numberPattern)

            let number = if numberRegex.Success then Some (int numberRegex.Value) else None
            let numberComment = number |> Option.map (string >> (Comment.fromString "Number") >> List.singleton)

            // Parsing a header of shape: Kind [TermName] (#Number, #tOntology)
            if nameRegex.Success then
                                                                             
                let ontology = OntologyAnnotation.fromString nameRegex.Value "" ""

                ColumnHeader.create header kindRegex.Value (Some {ontology with Comments = numberComment}) number

            // Parsing a header of shape: Kind (#Number)
            elif kindRegex.Success then
                let kind = kindRegex.Value

                let ontology = 
                    if ontologySourceRegex.Success then 
                        ontologySourceRegex.Value.Split ':'
                        |> fun o -> OntologyAnnotation.fromString ""  o.[0] o.[1] 
                        |> fun ontology -> {ontology with Comments = numberComment}
                        |> Some
                    else None

                ColumnHeader.create header kind ontology number

            // Parsing a header of shape: Kind
            else
                ColumnHeader.create header header None None
        
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
        | h when h.Kind = "Characteristics" || h.Kind = "Characteristics Value" ->
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