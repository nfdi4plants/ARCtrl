namespace ISADotNet.QueryModel

open ISADotNet
open Swate.Api

[<AutoOpen>]
module OntologyAnnotation =

    module Annotation = 

        open System.Text.RegularExpressions

        let fullTermAccessionRegex = ".*/(?<ref>\\w*)_(?<num>\\w*)"

        /// Creates an annotation of format `TermSourceRef:TermAccessionNumber` (e.g: `MS:1000690`)
        /// 
        /// If termAccessionNumber is given in full URI form `http://purl.obolibrary.org/obo/MS_1000121`, takes last part of it. 
        let createShortAnnotation (termSourceRef : string) (termAccessionNumber : string) =
            let r = Regex.Match(termAccessionNumber,fullTermAccessionRegex)
    
            if r.Success then
                let termSourceRef = r.Groups.Item("ref").Value
                let termAccessionNumber = r.Groups.Item("num").Value
                $"{termSourceRef}:{termAccessionNumber}"
            else
                $"{termSourceRef}:{termAccessionNumber}"
    
        /// Splits the Annotation of format `TermSourceRef:TermAccessionNumber` (e.g: `MS:1000690`) into a tuple of TermSourceRef*TermAccessionNumber 
        let splitAnnotation (a : string) = 
            a.Split ';'
            |> fun a -> a.[0],a.[1]

    open Annotation

    type OntologyAnnotation with
    
        /// Translates a SwateAPI `term` into an ISADotNet `OntologyAnnotation`
        static member ofTerm (term : Term) =
            let ref,num = splitAnnotation term.Accession
            let description = Comment.fromString "Description" term.Definition
            OntologyAnnotation.fromStringWithComments term.Name ref num [description]

        /// Translates an ISADotNet `OntologyAnnotation` into a SwateAPI `term`
        static member toTerm (term : OntologyAnnotation) =
            let annotation = createShortAnnotation term.TermAccessionString term.TermAccessionString
            TermMinimal.create term.NameText annotation

        static member isChildTerm (parent : OntologyAnnotation) (child : OntologyAnnotation) =
            Term.SearchByParent(child.NameText, 1, parent |> OntologyAnnotation.toTerm)
            |> Array.isEmpty
            |> not

        member this.ToTerm() =
            OntologyAnnotation.toTerm(this)

        member this.IsChildTermOf(parent : OntologyAnnotation) =
            OntologyAnnotation.isChildTerm parent this