namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Dataset (id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/Dataset", ?additionalType = additionalType)

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType