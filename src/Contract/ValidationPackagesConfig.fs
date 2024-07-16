namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl
open ARCtrl.Yaml
open ARCtrl.Helper
open ARCtrl.ValidationPackages

module ValidationPackagesConfigHelper = 

    let ConfigFilePath = [|ArcPathHelper.ARCConfigFolderName; ArcPathHelper.ValidationPackagesYamlFileName|] |> ArcPathHelper.combineMany

    let ReadContract : Contract = {Operation = READ; DTOType = Some DTOType.YAML; Path = ConfigFilePath; DTO = None}


[<AutoOpen>]
module ValidationPackagesConfigExtensions = 

    let (|ValidationPackagesYamlPath|_|) (input) =
        match input with
        | [|ArcPathHelper.ARCConfigFolderName; ArcPathHelper.ValidationPackagesYamlFileName|] -> 
            let path = ArcPathHelper.combineMany input
            Some path
        | _ -> None

    type ValidationPackagesConfig with
    
        member this.ToCreateContract () =
            Contract.createCreate(ValidationPackagesConfigHelper.ConfigFilePath, DTOType.YAML, DTO.Text (this |> ValidationPackagesConfig.toYamlString()))

        member this.ToDeleteContract () =
            Contract.createDelete(ValidationPackagesConfigHelper.ConfigFilePath)

        static member toDeleteContract (config: ValidationPackagesConfig) : Contract =
            config.ToDeleteContract()

        static member toCreateContract (config: ValidationPackagesConfig) : Contract =
            config.ToCreateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.YAML; Path = p; DTO = Some (DTO.Text yaml)} when p = ValidationPackagesConfigHelper.ConfigFilePath ->
                yaml
                |> ValidationPackagesConfig.fromYamlString
                |> Some 
            | _ -> None