namespace ARCtrl.Json

open ARCtrl.Process


[<AutoOpen>]
module MaterialAttributeExtensions =
    
        type MaterialAttribute with
    
            static member fromISAJsonString (s:string) = 
                Decode.fromJsonString MaterialAttribute.ISAJson.decoder s   
    
            static member toISAJsonString(?spaces, ?useIDReferencing) =
                let useIDReferencing = Option.defaultValue false useIDReferencing
                let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
               
                fun (v:MaterialAttribute) ->
                    MaterialAttribute.ISAJson.encoder idMap v
                    |> Encode.toJsonString (Encode.defaultSpaces spaces)

            member this.ToJsonString(?spaces, ?useIDReferencing) =
                MaterialAttribute.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing  ) this