namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProcessInputExtensions =
    
    type ProcessInput with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessInput.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None           
            fun (f:ProcessInput) ->
                ProcessInput.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            ProcessInput.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this