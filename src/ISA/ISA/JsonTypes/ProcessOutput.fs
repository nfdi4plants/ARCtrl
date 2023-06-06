namespace ISA

[<RequireQualifiedAccess>]
type ProcessOutput =
    | Sample of Sample
    | Data of Data
    | Material of Material 

    member this.TryGetName =
        match this with
        | ProcessOutput.Sample s     -> s.Name
        | ProcessOutput.Material m   -> m.Name
        | ProcessOutput.Data d       -> d.Name

    member this.GetName =
        this.TryGetName
        |> Option.defaultValue ""

    static member Default = Sample (Sample.empty)

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            match this with 
            | ProcessOutput.Sample s     -> sprintf "Sample {%s}" ((s :> IISAPrintable).PrintCompact())
            | ProcessOutput.Material m   -> sprintf "Material {%s}" ((m :> IISAPrintable).PrintCompact())
            | ProcessOutput.Data d       -> sprintf "Data {%s}" ((d :> IISAPrintable).PrintCompact())
