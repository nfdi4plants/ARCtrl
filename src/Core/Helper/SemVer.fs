module ARCtrl.Helper.SemVer

open Regex.ActivePatterns

module SemVerAux =

    let Pattern =
            @"^(?<major>\d+)" +
            @"(\.(?<minor>\d+))?" +
            @"(\.(?<patch>\d+))?" +
            @"(-(?<pre>[0-9A-Za-z-\.]+))?" +
            @"(\+(?<build>[0-9A-Za-z-\.]+))?$"

    let private isNumericIdentifier (identifier: string) =
        identifier.Length > 0 && identifier |> Seq.forall System.Char.IsDigit

    /// Compares two pre-release identifiers, see https://semver.org/#spec-item-11
    let compareIdentifier (a: string) (b: string) =
        match isNumericIdentifier a, isNumericIdentifier b with
        | true, true ->
            // Compared by value without parsing, so large numbers can't overflow. The original identifiers break the tie
            // for leading zeros (e.g. "01" and "1"), so that only equal identifiers compare as equal
            let a', b' = a.TrimStart('0'), b.TrimStart('0')
            if a'.Length <> b'.Length then compare a'.Length b'.Length
            elif a' <> b' then compare a' b'
            else compare a b
        // Numeric identifiers have lower precedence than alphanumeric identifiers
        | true, false -> -1
        | false, true -> 1
        | false, false -> compare a b

    /// Compares the pre-release versions of two otherwise equal versions, see https://semver.org/#spec-item-11
    let comparePreRelease (a: string option) (b: string option) =
        match a, b with
        | None, None -> 0
        // A pre-release version has lower precedence than the associated normal version
        | Some _, None -> -1
        | None, Some _ -> 1
        | Some a, Some b ->
            let a, b = a.Split('.'), b.Split('.')
            let rec loop i =
                // If all preceding identifiers are equal, the larger set of identifiers has higher precedence
                if i = a.Length || i = b.Length then
                    compare a.Length b.Length
                else
                    match compareIdentifier a.[i] b.[i] with
                    | 0 -> loop (i + 1)
                    | c -> c
            loop 0


// Metadata is ignored for equality and comparison, see https://semver.org/#spec-item-10
[<CustomEquality; CustomComparison>]
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
        match str with
        | Regex (SemVerAux.Pattern) m ->
            let g = m.Groups
            let major = int g.["major"].Value
            // Minor and patch are optional in the pattern and default to 0, e.g. "1.2" is read as 1.2.0
            let minor =
                match g.["minor"].Success with
                | true -> int g.["minor"].Value
                | false -> 0
            let patch =
                match g.["patch"].Success with
                | true -> int g.["patch"].Value
                | false -> 0
            let pre = 
                match g.["pre"].Success with
                | true -> Some g.["pre"].Value
                | false -> None
            let meta =
                match g.["build"].Success with
                | true -> Some g.["build"].Value
                | false -> None
            Some <| SemVer.create(major, minor, patch, ?pre=pre, ?meta=meta)
        | _ -> None

    member this.AsString() : string =
        let sb = System.Text.StringBuilder()
        sb.Append(sprintf "%i.%i.%i" this.Major this.Minor this.Patch) |> ignore
        if this.PreRelease.IsSome then
            sb.Append(sprintf "-%s" this.PreRelease.Value) |> ignore
        if this.Metadata.IsSome then
            sb.Append(sprintf "+%s" this.Metadata.Value) |> ignore
        sb.ToString()

    override this.Equals(other) =
        match other with
        | :? SemVer as other ->
            this.Major = other.Major
            && this.Minor = other.Minor
            && this.Patch = other.Patch
            && this.PreRelease = other.PreRelease
        | _ -> false

    override this.GetHashCode() =
        hash (this.Major, this.Minor, this.Patch, this.PreRelease)

    interface System.IComparable with
        member this.CompareTo(other) =
            match other with
            | :? SemVer as other ->
                if this.Major <> other.Major then compare this.Major other.Major
                elif this.Minor <> other.Minor then compare this.Minor other.Minor
                elif this.Patch <> other.Patch then compare this.Patch other.Patch
                else SemVerAux.comparePreRelease this.PreRelease other.PreRelease
            | _ -> invalidArg "other" "Cannot compare SemVer with a value of a different type"