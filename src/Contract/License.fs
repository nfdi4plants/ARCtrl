namespace ARCtrl.Contract

[<AutoOpen>]
module LicenseContractExtensions =

    let (|LicensePath|_|) (input) =
        match input with
        | [|ARCtrl.ArcPathHelper.LICENSEFileName|] -> 
            Some ARCtrl.ArcPathHelper.LICENSEFileName
        | [|alternativeName|] when ARCtrl.ArcPathHelper.alternativeLICENSEFileNames |> List.contains alternativeName ->
            Some alternativeName
        | _ -> None

module License =

    let defaultLicenseContract =
        Contract.createCreate(
            ARCtrl.ArcPathHelper.LICENSEFileName,
            DTOType.PlainText,
            DTO.Text ARCtrl.FileSystem.DefaultLicense.dl
        )