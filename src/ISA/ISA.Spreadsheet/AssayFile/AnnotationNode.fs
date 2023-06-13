namespace ISA.Spreadsheet

open ISA
open ISA.Spreadsheet
open FsSpreadsheet

module ActivePattern = 

    open ISA.Regex.ActivePatterns

    let mergeTerms tsr1 tan1 tsr2 tan2 =
        if tsr1 <> tsr2 then failwithf "TermSourceRef %s and %s do not match" tsr1 tsr2
        if tan1 <> tan2 then failwithf "TermAccessionNumber %s and %s do not match" tan1 tan2
        {|TermSourceRef = tsr1; TermAccessionNumber = tan1|}

    let (|Term|_|) (ac : string -> string option) (f : OntologyAnnotation -> CompositeHeader) (cells : FsCell list) =
        let (|AC|_|) s =
            ac s
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [AC name] -> 
            let ont = OntologyAnnotation.fromString(name)
            f ont
            |> Some
        | [AC name; TermSourceRef term1; TermAccessionNumber term2] 
        | [AC name; Unit; TermSourceRef term1; TermAccessionNumber term2] -> 
            let term = mergeTerms term1.IdSpace term1.IdSpace term2.LocalId term2.LocalId
            let ont = OntologyAnnotation.fromString(name, term.TermSourceRef, term.TermAccessionNumber)
            f ont
            |> Some
        | _ -> None

    let (|Parameter|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [Parameter name] -> 
            let ont = OntologyAnnotation.fromString(name)
            CompositeHeader.Parameter ont
            |> Some
        | [Parameter name; TermSourceRef term1; TermAccessionNumber term2] 
        | [Parameter name; Unit; TermSourceRef term1; TermAccessionNumber term2] -> 
            let term = mergeTerms term1.IdSpace term1.IdSpace term2.LocalId term2.LocalId
            let ont = OntologyAnnotation.fromString(name, term.TermSourceRef, term.TermAccessionNumber)
            CompositeHeader.Parameter ont
            |> Some
        | _ -> None

    let (|Factor|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [Factor name] -> 
            let ont = OntologyAnnotation.fromString(name)
            CompositeHeader.Factor ont
            |> Some
        | [Factor name; TermSourceRef term1; TermAccessionNumber term2] 
        | [Factor name; Unit; TermSourceRef term1; TermAccessionNumber term2] -> 
            let term = mergeTerms term1.IdSpace term1.IdSpace term2.LocalId term2.LocalId
            let ont = OntologyAnnotation.fromString(name, term.TermSourceRef, term.TermAccessionNumber)
            CompositeHeader.Factor ont
            |> Some
        | _ -> None

    let (|Characteristic|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [Characteristic name] -> 
            let ont = OntologyAnnotation.fromString(name)
            CompositeHeader.Characteristic ont
            |> Some
        | [Characteristic name; TermSourceRef term1; TermAccessionNumber term2] 
        | [Characteristic name; Unit; TermSourceRef term1; TermAccessionNumber term2] -> 
            let term = mergeTerms term1.IdSpace term1.IdSpace term2.LocalId term2.LocalId
            let ont = OntologyAnnotation.fromString(name, term.TermSourceRef, term.TermAccessionNumber)
            CompositeHeader.Characteristic ont
            |> Some
        | _ -> None
    
    let (|Input|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [Input ioType] -> 
            IOType.ofString ioType
            |> CompositeHeader.Input
            |> Some
        | _ -> None

    let (|Output|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [Output ioType] -> 
            IOType.ofString ioType
            |> CompositeHeader.Output
            |> Some
        | _ -> None

    let (|FreeText|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [text] -> 
            CompositeHeader.FreeText text
            |> Some
        | _ -> None

module ISAValue =

    open ISA.QueryModel

    let toHeaders (v : QueryModel.ISAValue) =
        try 
            let ont = v.Category.ShortAnnotationString
            if v.HasUnit then
                [v.HeaderText;"Unit";$"Term Source REF ({ont})";$"Term Accession Number ({ont})"]
            else
                [v.HeaderText;$"Term Source REF ({ont})";$"Term Accession Number ({ont})"]
        with
        | err -> failwithf "Could not parse headers of value with name %s: \n%s" v.HeaderText err.Message

    let toValues (v : QueryModel.ISAValue) =    
        try
            if v.HasUnit then
                if v.HasValue then
                    [v.ValueText;v.Unit.NameText;v.Unit.TermSourceREFString;v.Unit.TermAccessionString]
                else 
                    ["";v.Unit.NameText;v.Unit.TermSourceREFString;v.Unit.TermAccessionString]
            else
                match v.TryValue with
                | Some (Ontology oa) ->
                    [oa.NameText;oa.TermSourceREFString;oa.TermAccessionString]
                | Some _ ->
                    [v.ValueText;"";""]
                | None ->
                    ["";"";""]
        with
        | err -> failwithf "Could not parse headers of value with name %s: \n%s" v.HeaderText err.Message

module ProtocolType =

    open ISA.QueryModel

    let headers =
        [
            "Protocol Type"
            "Term Source REF (MS:1000031)"
            "Term Accession Number (MS:1000031)"
        ]

    let toValues (v : OntologyAnnotation) =    
        [
            v.NameText
            v.TermSourceREF |> Option.defaultValue "user-specific"
            v.TermAccessionNumber |> Option.defaultValue "user-specific"
        ]

module IOType =

    let toHeader (io : QueryModel.IOType) =
        match io with
        | QueryModel.IOType.Source ->      
            "Source Name"
        | QueryModel.IOType.Sample ->      
            "Sample Name"
        | QueryModel.IOType.RawData ->      
            "Raw Data File"
        | QueryModel.IOType.ProcessedData ->      
            "Derived Data File"
        | QueryModel.IOType.Material ->      
            "Material Name"
        | QueryModel.IOType.Data ->      
            "Data File Name"

    let defaultInHeader = toHeader QueryModel.IOType.Source

    let defaultOutHeader = toHeader QueryModel.IOType.Sample