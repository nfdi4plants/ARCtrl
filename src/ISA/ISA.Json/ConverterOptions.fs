namespace ARCtrl.ISA.Json

type ConverterOptions() = 

    let mutable setID = false
    let mutable includeType = false
    let mutable includeContext = false
    let mutable isRoCrate = false

    member this.SetID with
        get() = setID
        and set(setId) = setID <- setId
    member this.IncludeType with
        get() = includeType
        and set(iT) = includeType <- iT
    member this.IncludeContext with
        get() = includeContext
        and set(iC) = includeContext <- iC
    member this.IsRoCrate with
        get() = isRoCrate
        and set(iR) = isRoCrate <- iR
