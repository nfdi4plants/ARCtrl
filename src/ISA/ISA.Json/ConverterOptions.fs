namespace ARCtrl.ISA.Json

type ConverterOptions() = 

    let mutable setID = false
    let mutable isJsonLD = false

    member this.SetID with get() = setID
                      and set(setId) = setID <- setId
    member this.IsJsonLD with get() = isJsonLD
                            and set(iJ) = isJsonLD <- iJ
