namespace ARCtrl.ISA.Json

type ConverterOptions() = 

    let mutable setID = false
    let mutable isJsonLD = false

    member this.SetID 
        with get() = setID
        and set(setId) = setID <- setId
    /// Currently this is only used for ROCrate support.
    member this.IsJsonLD 
        with get() = isJsonLD
        and set(iroc) = isJsonLD <- iroc
