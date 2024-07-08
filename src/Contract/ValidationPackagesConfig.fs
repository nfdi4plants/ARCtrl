namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl.Path
open ARCtrl
open ARCtrl.Yaml
open ARCtrl.Helper
open ARCtrl.ValidationPackages

[<AutoOpen>]
module ValidationPackagesConfigExtensions = 

    let (|ValidationPackagesYamlPath|_|) (input) =
        match input with
        | [|ARCConfigFolderName; ValidationPackagesYamlFileName|] -> 
            let path = ARCtrl.Path.combineMany input
            Some path
        | _ -> None

    let internal config_file_path = [|ARCConfigFolderName; ValidationPackagesYamlFileName|] |> ARCtrl.Path.combineMany

    type ValidationPackagesConfig with
    
        member this.ToCreateContract () =
            Contract.createCreate(config_file_path, DTOType.YAML, DTO.Text (this |> ValidationPackagesConfig.toYamlString()))

        member this.ToUpdateContract () =
            Contract.createUpdate(config_file_path, DTOType.YAML, DTO.Text (this |> ValidationPackagesConfig.toYamlString()))

        member this.ToDeleteContract () =
            Contract.createDelete(config_file_path)

        static member toDeleteContract (config: ValidationPackagesConfig) : Contract =
            config.ToDeleteContract()

        static member toCreateContract (config: ValidationPackagesConfig) : Contract =
            config.ToCreateContract()

        static member toUpdateContract (config: ValidationPackagesConfig) : Contract =
            config.ToUpdateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.YAML; DTO = Some (DTO.Text yaml)} ->
                yaml
                |> ValidationPackagesConfig.fromYamlString
                |> Some 
            | _ -> None