namespace ARCtrl.Yaml

open ARCtrl.ValidationPackages
open YAMLicious
open YAMLicious.YAMLiciousTypes

module ValidationPackagesConfig = 

    let encoder (validationpackage : ValidationPackagesConfig) = 
        [
            Encode.tryInclude "arc_specification" Encode.string  (validationpackage.ARCSpecification)
            "validation_packages", Encode.resizearray ValidationPackage.encoder validationpackage.ValidationPackages
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : (YAMLElement -> ValidationPackagesConfig) = 
        Decode.object (fun get ->
            ValidationPackagesConfig(
                validation_packages = get.Required.Field "validation_packages" (Decode.resizearray ValidationPackage.decoder),
                ?arc_specification = get.Optional.Field "arc_specification" Decode.string
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