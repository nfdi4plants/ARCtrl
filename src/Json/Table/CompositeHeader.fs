namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module CompositeHeader =

  let [<Literal>] HeaderType = "headertype"
  let [<Literal>] HeaderValues = "values"

  let encoder (ch: CompositeHeader) = 
    let oaToJsonString (oa:OntologyAnnotation) = OntologyAnnotation.encoder (ConverterOptions()) oa
    let t, v = 
      match ch with
      | CompositeHeader.FreeText s -> s, []
      | CompositeHeader.Parameter oa -> "Parameter", [oaToJsonString oa]
      | CompositeHeader.Factor oa -> "Factor", [oaToJsonString oa]
      | CompositeHeader.Characteristic oa -> "Characteristic", [oaToJsonString oa]
      | CompositeHeader.Component oa -> "Component", [oaToJsonString oa]
      | CompositeHeader.ProtocolType -> "ProtocolType", []
      | CompositeHeader.ProtocolREF -> "ProtocolREF", []
      | CompositeHeader.ProtocolDescription -> "ProtocolDescription", []
      | CompositeHeader.ProtocolUri -> "ProtocolUri", []
      | CompositeHeader.ProtocolVersion -> "ProtocolVersion", []
      | CompositeHeader.Performer -> "Performer", []
      | CompositeHeader.Date -> "Date", []
      | CompositeHeader.Input io -> "Input", [IOType.encoder io]
      | CompositeHeader.Output io -> "Output", [IOType.encoder io]
    Encode.object [
      HeaderType, Encode.string t
      HeaderValues, v |> Encode.list
    ]

  let decoder : Decoder<CompositeHeader> = 
    Decode.object (fun get ->
      let headerType = get.Required.Field (HeaderType) Decode.string
      let oa() = get.Required.Field (HeaderValues) (Decode.index 0 <| OntologyAnnotation.decoder (ConverterOptions()))
      let io() = get.Required.Field (HeaderValues) (Decode.index 0 <| IOType.decoder)
      match headerType with
      | "Characteristic" -> oa() |> CompositeHeader.Characteristic
      | "Parameter" -> oa() |> CompositeHeader.Parameter
      | "Component" -> oa() |> CompositeHeader.Component
      | "Factor" -> oa() |> CompositeHeader.Factor
      | "Input" -> io() |> CompositeHeader.Input
      | "Output" -> io() |> CompositeHeader.Output
      | "ProtocolType" -> CompositeHeader.ProtocolType
      | "ProtocolREF" -> CompositeHeader.ProtocolREF
      | "ProtocolDescription" -> CompositeHeader.ProtocolDescription
      | "ProtocolUri" -> CompositeHeader.ProtocolUri
      | "ProtocolVersion" -> CompositeHeader.ProtocolVersion
      | "Performer" -> CompositeHeader.Performer
      | "Date" -> CompositeHeader.Date
      | anyelse -> CompositeHeader.FreeText anyelse
    ) 

[<AutoOpen>]
module CompositeHeaderExtensions =

    type CompositeHeader with
        static member fromJsonString (jsonString: string) : CompositeHeader = 
            GDecode.fromJsonString CompositeHeader.decoder jsonString

        member this.ToJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            Encode.toJsonString spaces (CompositeHeader.encoder this)

        static member toJsonString(a:CompositeHeader) = a.ToJsonString()