namespace ARCtrl.Yaml

open ARCtrl.ValidationPackages
open YAMLicious

module ValidationPackage = 

    let [<Literal>] NAME_KEY = "name"
    let [<Literal>] VERSION_KEY = "version"

    let encoder (validationpackage : ValidationPackage) = 
        [
            Some (NAME_KEY, Encode.string validationpackage.Name)
            Encode.optionalField VERSION_KEY Encode.string validationpackage.Version
        ]
        |> List.choose id
        |> Encode.object

    let decoder : (YamlNode -> ValidationPackage) = 
        Decode.object (fun get ->
            ValidationPackage(
                name = get.Required.Field NAME_KEY (Decode.nonNull Decode.string),
                ?version = get.Optional.Field VERSION_KEY Decode.string
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