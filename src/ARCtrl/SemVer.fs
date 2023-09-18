module ARCtrl.SemVer

module SemVerAux =

    let private Pattern =
            @"^(?<major>\d+)" +
            @"(\.(?<minor>\d+))?" +
            @"(\.(?<patch>\d+))?" +
            @"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
            @"(\+(?<build>[0-9A-Za-z\-\.]+))?$"

    let SemVerRegex = System.Text.RegularExpressions.Regex(Pattern)

type SemVer = {
    Major: int
    Minor: int
    Patch: int
    PreRelease: string option
    Metadata: string option
} with
    static member make major minor patch pre meta = {
        Major = major
        Minor = minor
        Patch = patch
        PreRelease = pre
        Metadata = meta
    }

    static member create(major: int, minor: int, patch: int, ?pre: string, ?meta: string) =
        {
            Major = major
            Minor = minor
            Patch = patch
            PreRelease = pre
            Metadata = meta
        }

    static member tryOfString (str: string) =
        let m = SemVerAux.SemVerRegex.Match(str)
        match m.Success with
        | false -> None
        | true ->
            let g = m.Groups
            let major = int g.["major"].Value
            let minor = int g.["minor"].Value
            let patch = int g.["patch"].Value
            let pre = 
                match g.["pre"].Success with
                | true -> Some g.["pre"].Value
                | false -> None
            let meta =
                match g.["build"].Success with
                | true -> Some g.["build"].Value
                | false -> None
            Some <| SemVer.create(major, minor, patch, ?pre=pre, ?meta=meta)

    member this.AsString() : string =
        let sb = System.Text.StringBuilder()
        sb.AppendFormat("{0}.{1}.{2}", this.Major, this.Minor, this.Patch) |> ignore
        if this.PreRelease.IsSome then
            sb.AppendFormat("-{0}", this.PreRelease.Value) |> ignore
        if this.Metadata.IsSome then
            sb.AppendFormat("+{0}", this.Metadata.Value) |> ignore
        sb.ToString()