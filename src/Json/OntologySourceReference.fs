namespace ARCtrl.JSON

open Thoth.Json.Core
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Json

module OntologySourceReference =

    let encoder (osr : OntologySourceReference) = 
        [
            Encode.tryInclude "description" Encode.string (osr.Description)
            Encode.tryInclude "file" Encode.string (osr.File)
            Encode.tryInclude "name" Encode.string (osr.Name)
            Encode.tryInclude "version" Encode.string (osr.Version)
            Encode.tryIncludeSeq "comments" Comment.encoder (osr.Comments)
            "@context", ROCrateContext.OntologySourceReference.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<OntologySourceReference> =
        Decode.object (fun get ->
            OntologySourceReference(
                ?description = get.Optional.Field "description" Decode.uri,
                ?file = get.Optional.Field "file" Decode.string,
                ?name = get.Optional.Field "name" Decode.string,
                ?version = get.Optional.Field "version" Decode.string,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)
            )
        )

    module ROCrate =
        
        let genID (o:OntologySourceReference) = 
            match o.File with
            | Some f -> f
            | None -> 
                match o.Name with
                | Some n -> "#OntologySourceRef_" + n.Replace(" ","_")
                | None -> "#DummyOntologySourceRef"


        let encoder (osr : OntologySourceReference) = 
            [
                "@id", Encode.string (osr |> genID)
                "@type", Encode.string "OntologySourceReference"
                Encode.tryInclude "description" Encode.string (osr.Description)
                Encode.tryInclude "file" Encode.string (osr.File)
                Encode.tryInclude "name" Encode.string (osr.Name)
                Encode.tryInclude "version" Encode.string (osr.Version)
                Encode.tryIncludeSeq "comments" Comment.encoder (osr.Comments)
                "@context", ROCrateContext.OntologySourceReference.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder (options : ConverterOptions) : Decoder<OntologySourceReference> =
            Decode.object (fun get ->
                OntologySourceReference(
                    ?description = get.Optional.Field "description" Decode.uri,
                    ?file = get.Optional.Field "file" Decode.string,
                    ?name = get.Optional.Field "name" Decode.string,
                    ?version = get.Optional.Field "version" Decode.string,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)               
                )
            )

module OntologySourceReference =
    
  
    let fromJsonString (s:string) = 
        Decode.fromJsonString decoder s  

    let fromJsonldString (s:string) = 
        Decode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s      

    let toJsonString (oa:OntologySourceReference) = 
        encoder (ConverterOptions()) oa
        |> Encode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (oa:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) oa
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> Encode.toJsonString 2

        // let fromFile (path : string) = 
        //     File.ReadAllText path 
        //     |> fromString

        //let toFile (path : string) (osr:OntologySourceReference) = 
        //    File.WriteAllText(path,toString osr)