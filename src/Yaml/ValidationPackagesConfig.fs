namespace ARCtrl.Yaml

open ARCtrl.ValidationPackages
open YAMLicious

module ValidationPackagesConfig = 

    let [<Literal>] ARC_SPECIFICATION_KEY = "arc_specification"
    let [<Literal>] VALIDATION_PACKAGES_KEY = "validation_packages"

    let encoder (validationpackage : ValidationPackagesConfig) = 
        [
            Encode.optionalField ARC_SPECIFICATION_KEY Encode.string validationpackage.ARCSpecification
            Some (VALIDATION_PACKAGES_KEY, Encode.resizearray ValidationPackage.encoder validationpackage.ValidationPackages)
        ]
        |> List.choose id
        |> Encode.object

    let decoder : (YamlNode -> ValidationPackagesConfig) = 
        Decode.object (fun get ->
            ValidationPackagesConfig(
                validation_packages = get.Required.Field VALIDATION_PACKAGES_KEY (Decode.resizearray ValidationPackage.decoder),
                ?arc_specification = get.Optional.Field ARC_SPECIFICATION_KEY Decode.string
            )
        )

[<AutoOpen>]
module ValidationPackageConfigExtensions =

    open ARCtrl.Yaml
    type ValidationPackagesConfig with

        static member fromYamlString (s:string)  = 
            Decode.fromYamlString ValidationPackagesConfig.decoder s

        static member toYamlString(?whitespace) = 
            fun (vp:ValidationPackagesConfig) ->
                ValidationPackagesConfig.encoder vp
                |> Encode.toYamlString (Encode.defaultWhitespace whitespace)                  

        member this.toYamlString(?whitespace) = 
            ValidationPackagesConfig.toYamlString(?whitespace=whitespace) this

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)