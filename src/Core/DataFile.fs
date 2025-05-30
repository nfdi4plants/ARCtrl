namespace ARCtrl

open ARCtrl

[<RequireQualifiedAccess>]
type DataFile =

    | RawDataFile // "Raw Data File"
    | DerivedDataFile // "Derived Data File"
    | ImageFile // "Image File"

    static member RawDataFileJson       = "Raw Data File"
    static member DerivedDataFileJson   = "Derived Data File"
    static member ImageFileJson         = "Image File"

    member this.AsString =
        match this with
        | RawDataFile       -> "Raw Data File"
        | DerivedDataFile   -> "Derived Data File"
        | ImageFile         -> "Image File"

    static member fromString (dt : string) =
        match dt with
        | "RawDataFileJson" | "Raw Data File"           -> RawDataFile
        | "DerivedDataFileJson" | "Derived Data File"   -> DerivedDataFile
        | "ImageFileJson" | "Image File"                -> ImageFile
        | _ -> failwith $"Invalid DataFile type: {dt}"

    member this.IsDerivedData =
        match this with
        | DerivedDataFile -> true
        | _ -> false

    member this.IsRawData =
        match this with
        | RawDataFile -> true
        | _ -> false

    member this.IsImage =
        match this with
        | ImageFile -> true
        | _ -> false