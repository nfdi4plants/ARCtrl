module Tests.ValidationPackagesConfig

open TestingUtils

open ARCtrl
open ARCtrl.Yaml
open ARCtrl.Contract
open ARCtrl.ValidationPackages
open ARCtrl.Contract.ValidationPackagesConfigExtensions

let vpc = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([ValidationPackage("name", "version")]), "arc_specification")

let extension_methods_tests = testList "Extension methods" [
    testCase "ToCreateContract" <| fun _ ->
        let contract = vpc.ToCreateContract()
        Expect.equal contract.Operation Operation.CREATE ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType (Some DTOType.YAML) ""
        Expect.equal contract.DTO (Some (DTO.Text (vpc |> ValidationPackagesConfig.toYamlString()))) ""

    testCase "ToDeleteContract" <| fun _ ->
        let contract = vpc.ToDeleteContract()
        Expect.equal contract.Operation Operation.DELETE ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType None ""
        Expect.equal contract.DTO None ""

    testCase "toCreateContract" <| fun _ ->
        let contract = ValidationPackagesConfig.toCreateContract vpc
        Expect.equal contract.Operation Operation.CREATE ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType (Some DTOType.YAML) ""
        Expect.equal contract.DTO (Some (DTO.Text (vpc |> ValidationPackagesConfig.toYamlString()))) ""

    testCase "toDeleteContract" <| fun _ ->
        let contract = ValidationPackagesConfig.toDeleteContract vpc
        Expect.equal contract.Operation Operation.DELETE ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType None ""
        Expect.equal contract.DTO None ""

    testCase "tryFromReadContract" <| fun _ ->
        let contract = {
            Operation = READ
            DTOType = Some DTOType.YAML
            Path = ValidationPackagesConfigHelper.ConfigFilePath
            DTO = Some (DTO.Text (ValidationPackagesConfig.toYamlString() vpc))
        }
        let result = ValidationPackagesConfig.tryFromReadContract contract
        Expect.equal result (Some vpc) ""
]

let main = testList "ValidationPackageConfig" [
    extension_methods_tests
]