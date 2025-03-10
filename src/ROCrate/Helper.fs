module ARCtrl.ROCrate.Helper

module ID =

    let clean (id : string) =
        id.Replace(" ", "_")