namespace ISADotNet.XLSX.AssayFile

open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.XLSX
open System.Text.RegularExpressions

/// Functions for 
module AnnotationColumn =

    type ColumnHeader =
        {
            HeaderString : string
            Kind : string
            Term : OntologyAnnotation option
            Number : int Option
        }

        static member create headerString kind term number =
            {
                HeaderString = headerString
                Kind = kind
                Term = term 
                Number = number       
            }

        static member fromStringHeader header =
                  
            let namePattern = @"(?<= \[).*(?=\])"
            let ontologySourcePattern = @"(?<=#t)\S+:[^;)]*"
            let numberPattern = @"(?<= \(#)\d+(?=[;)])"

            let nameRegex = Regex.Match(header,namePattern)
            let kindRegex = Regex.Match(header,@".*(?= \(#.*\))")
            if nameRegex.Success then
                let kind = Regex.Match(header,@".*(?= \[)")
                let ontologySourceRegex = Regex.Match(header,ontologySourcePattern)
                let numberRegex = Regex.Match(header,numberPattern)
                let number = if numberRegex.Success then Some (int numberRegex.Value) else None
                let termSource,termAccession = 
                    if ontologySourceRegex.Success then 
                        ontologySourceRegex.Value.Split ':'
                        |> fun o -> o.[0], o.[1]
                    else "", ""
                ColumnHeader.create header kind.Value (Some (OntologyAnnotation.fromString nameRegex.Value termAccession termSource)) number
            elif kindRegex.Success then
                let kind = kindRegex.Value
                let numberRegex = Regex.Match(header,numberPattern)
                let number = if numberRegex.Success then Some (int numberRegex.Value) else None
                ColumnHeader.create header kind None number
            else
                ColumnHeader.create header header None None
        

    let mergeOntology (termSourceHeaderOntology : OntologyAnnotation Option) (termAccessionHeaderOntology : OntologyAnnotation Option) =
        match termSourceHeaderOntology, termAccessionHeaderOntology with
        | Some oa1, Some oa2 -> API.Update.UpdateAll.updateRecordType oa1 oa2 |> Some
        | Some oa, None -> Some oa
        | None, Some oa -> Some oa
        | None, None -> None

    
    let tryParseTermSourceReferenceHeader (termHeader:ColumnHeader) (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Term Source REF" && h.Number = termHeader.Number -> 
            match h.Term,termHeader.Term with
            | None, None -> Some h
            | Some t1, Some t2 when t1.Name = t2.Name -> Some h
            | _ -> None
        | _ -> None
    
    let tryParseTermAccessionNumberHeader (termHeader:ColumnHeader) (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Term Accession Number"  && h.Number = termHeader.Number -> 
            match h.Term,termHeader.Term with
            | None, None -> Some h
            | Some t1, Some t2 when t1.Name = t2.Name -> Some h
            | _ -> None
        | _ -> None
    
    let tryParseParameterHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Parameter" || h.Kind = "Parameter Value" ->
            Some h
        | _ -> None
    
    let tryParseFactorHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Factor" || h.Kind = "Factor Value" ->
            Some h
        | _ -> None

    let tryParseCharacteristicsHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Characteristics" || h.Kind = "Characteristics Value" ->
            Some h
        | _ -> None

    let tryParseUnitHeader (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Unit" ->
            Some h
        | _ -> None   
    
    let tryParseSampleName (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Sample Name" ->
            Some h
        | _ -> None

    let tryParseSourceName (header:string) =
        match ColumnHeader.fromStringHeader header with
        | h when h.Kind = "Source Name" ->
            Some h
        | _ -> None

    let isSample header = tryParseSampleName header |> Option.isSome 
    let isSource header = tryParseSourceName header |> Option.isSome 