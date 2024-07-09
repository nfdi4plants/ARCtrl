namespace ARCtrl.Yaml

open ARCtrl.ValidationPackages
open YAMLicious
open YAMLicious.YAMLiciousTypes

module ValidationPackage = 

    let encoder (validationpackage : ValidationPackage) = 
        [
            "name", Encode.string validationpackage.Name
            Encode.tryInclude "version" Encode.string (validationpackage.Version)
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : (YAMLElement -> ValidationPackage) = 
        Decode.object (fun get ->
            ValidationPackage(
                name = get.Required.Field "name" Decode.string,
                ?version = get.Optional.Field "version" Decode.string
            )
        )

[<AutoOpen>]
module ValidationPackageExtensions =

    open ARCtrl.Yaml
    type ValidationPackage with

        static member fromYamlString (s:string)  = 
            Decode.fromYamlString ValidationPackage.decoder s

        static member toYamlString(?whitespace) = 
            fun (vp:ValidationPackage) ->
                ValidationPackage.encoder vp
                |> Encode.toYamlString (Encode.defaultWhitespace whitespace)                  

        member this.toYamlString(?whitespace) = 
            ValidationPackage.toYamlString(?whitespace=whitespace) this

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)