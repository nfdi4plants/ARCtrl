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


        static member isChildTerm(parent : OntologyAnnotation,child : OntologyAnnotation,ont : Obo.OboOntology) =
            Term.SearchByParent(child.NameText, 1, parent |> OntologyAnnotation.toTerm)
            |> Array.isEmpty
            |> not       

        member this.IsChildTermOf(parent : OntologyAnnotation) =
            OntologyAnnotation.isChildTerm(parent,this)


        static member isChildTerm (parent : OntologyAnnotation,child : OntologyAnnotation) =
            Term.SearchByParent(child.NameText, 1, parent |> OntologyAnnotation.toTerm)
            |> Array.isEmpty
            |> not       

        //member this.IsChildTermOf(parent : OntologyAnnotation) =
        //    OntologyAnnotation.isChildTerm parent this


