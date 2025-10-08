namespace ARCtrl.Contract

[<AutoOpen>]
module LicenseContractExtensions =

    let (|LicensePath|_|) (input) =
        match input with
        | [|ARCtrl.ArcPathHelper.LICENSEFileName|] -> 
            Some ARCtrl.ArcPathHelper.LICENSEFileName
        | _ -> None

module License =

    let deaultLicenseContract =
        Contract.createCreate(
            ARCtrl.ArcPathHelper.LICENSEFileName,
            DTOType.PlainText,
            DTO.Text ARCtrl.FileSystem.DefaultLicense.dl
        )