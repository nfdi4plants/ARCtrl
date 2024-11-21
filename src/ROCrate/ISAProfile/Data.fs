namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Data(
    id,
    name,
    ?additionalType,
    ?comment,
    ?encodingFormat,
    ?disambiguatingDescription
) as this =
    inherit LDObject(
        id = id,
        schemaType = ResizeArray[|"schema.org/MediaObject"|],
        additionalType = defaultArg additionalType (ResizeArray[||])
    )
    do
        DynObj.setProperty (nameof name) name this

        DynObj.setOptionalProperty (nameof comment) comment this
        DynObj.setOptionalProperty (nameof encodingFormat) encodingFormat this
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "Data" (nameof name) this
    static member getName = fun (d: Data) -> d.GetName()