namespace ISADotNet.QueryModel

open ISADotNet
open Swate.Api

[<AutoOpen>]
module OntologyAnnotation =

    type OntologyAnnotation with
       

        /// Translates a SwateAPI `term` into an ISADotNet `OntologyAnnotation`
        static member ofTerm (term : Term) =
            let ref,num = OntologyAnnotation.splitAnnotation term.Accession
            let description = Comment.fromString "Description" term.Definition
            OntologyAnnotation.fromStringWithComments term.Name ref num [description]

        /// Translates an ISADotNet `OntologyAnnotation` into a SwateAPI `term`
        static member toTerm (term : OntologyAnnotation) =
            TermMinimal.create term.NameText term.AnnotationID

        /// Translates a OBO `term` into an ISADotNet `OntologyAnnotation`
        static member ofOboTerm (term : Obo.OboTerm) =
            let ref,num = OntologyAnnotation.splitAnnotation term.Id
            OntologyAnnotation.fromString term.Name ref num

        /// Translates an ISADotNet `OntologyAnnotation` into a OBO `term`
        static member toOboTerm (term : OntologyAnnotation) =
            Obo.OboTerm.Create(term.AnnotationID,term.NameText)

        member this.ToOboTerm() =
            OntologyAnnotation.toOboTerm this

        member this.ToTerm() =
            OntologyAnnotation.toTerm(this)

        static member findTerm(nameOrId : string) =
            Term.Search(nameOrId, 1).[0]

        static member findTerm(nameOrId : string,ont : Obo.OboOntology) =
            match ont.TryGetOntologyAnnotation nameOrId with
            | Some oa ->
                oa
            | None ->
                match ont.TryGetOntologyAnnotationByName nameOrId with
                | Some oa -> oa
                | None -> failwithf "could not find Ontology term %s in given ontology" nameOrId

        static member isChildTerm(parent : OntologyAnnotation,child : OntologyAnnotation,ont : Obo.OboOntology) =
            ont.GetParentOntologyAnnotations(child)
            |> List.contains parent

        member this.IsChildTermOf(parent : OntologyAnnotation) =
            OntologyAnnotation.isChildTerm(parent,this)

        static member isChildTerm (parent : OntologyAnnotation,child : OntologyAnnotation) =
            Term.SearchByParent(child.NameText, 1, parent |> OntologyAnnotation.toTerm)
            |> Array.isEmpty
            |> not       

        member this.IsChildTermOf(parent : OntologyAnnotation, ont : Obo.OboOntology) =
            OntologyAnnotation.isChildTerm(parent,this,ont)

        static member isEquivalentTo(term : OntologyAnnotation,targetTerm : OntologyAnnotation,ont : Obo.OboOntology) =
            ont.GetEquivalentOntologyAnnotations(term)
            |> List.contains targetTerm

        member this.IsEquivalentTo(targetTerm : OntologyAnnotation, ont : Obo.OboOntology) =
            OntologyAnnotation.isEquivalentTo(targetTerm,this,ont)

        static member getAs (term : OntologyAnnotation, targetOntology : string, ont : Obo.OboOntology) =
            ont.GetEquivalentOntologyAnnotations(term)
            |> List.find (fun t -> t.TermSourceREFString = targetOntology)

        member this.GetAs(targetOntology : string, ont : Obo.OboOntology) =
            OntologyAnnotation.getAs(this,targetOntology,ont)
