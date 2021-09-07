namespace ISADotNet

open System.Text.Json.Serialization

type URI = string

module URI =

    /// Create a ISAJson URI from a ISATab string entry
    let fromString (s : string) : URI=
        s

    /// Create a ISATab string URI from a ISAJson object
    let toString (s : URI) : string=
        s