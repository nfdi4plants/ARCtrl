module ARCtrl.ValidationPackagesConfig.Tests

open TestingUtils
open ARCtrl
open ARCtrl.ValidationPackages
open ARCtrl.Yaml
open ARCtrl.Contract

let vpc_toplevel_api_tests = testList "ARC() ValidationPackages top level API" [
    let vpc = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([ValidationPackage("name", "version")]), "arc_specification")
    let arc = ARC()

    testCase "GetValidationPackagesConfigWriteContract" <| fun _ ->
        let contract = arc.GetValidationPackagesConfigWriteContract(vpc)
        Expect.equal contract.Operation Operation.CREATE ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType (Some DTOType.YAML) ""
        Expect.equal contract.DTO (Some (DTO.Text (vpc |> ValidationPackagesConfig.toYamlString()))) ""

    testCase "GetValidationPackagesConfigDeleteContract" <| fun _ ->
        let contract = arc.GetValidationPackagesConfigDeleteContract(vpc)
        Expect.equal contract.Operation Operation.DELETE ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType None ""
        Expect.equal contract.DTO None ""

    testCase "GetValidationPackagesConfigReadContract" <| fun _ ->
        let contract = arc.GetValidationPackagesConfigReadContract()
        Expect.equal contract.Operation Operation.READ ""
        Expect.equal contract.Path ValidationPackagesConfigHelper.ConfigFilePath ""
        Expect.equal contract.DTOType (Some DTOType.YAML) ""
        Expect.equal contract.DTO None ""

    testCase "GetValidationPackagesConfigFromReadContract" <| fun _ ->
        let contract = {
            Operation = READ
            DTOType = Some DTOType.YAML
            Path = ValidationPackagesConfigHelper.ConfigFilePath
            DTO = Some (DTO.Text (ValidationPackagesConfig.toYamlString() vpc))
        }
        let result = arc.GetValidationPackagesConfigFromReadContract contract
        Expect.equal result (Some vpc) ""
]

let main = testList "ValidationPackagesConfig" [
    vpc_toplevel_api_tests
]