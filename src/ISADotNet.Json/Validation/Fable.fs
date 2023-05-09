namespace ISADotNet.Validation

module Fable =

    open System.Threading.Tasks
    open Fable.Core
    open Fable.Core.JS
    open Fable.Core.JsInterop

    type ValidatorResult = {
        instance: obj
        schema: obj
        options: obj
        path: obj []
        propertyPath: string
        errors: obj []
        throwError: obj option
        throFirst: obj option
        throwAll: obj option
        disableFormat: bool
    }

    type IValidate =
        abstract validateAgainstSchema: jsonString:string * schemaUrl:string -> Promise<ValidatorResult>
        abstract helloWorld: unit -> string

    [<ImportAll("./JsonValidation.js")>]
    let JsonValidation: IValidate = jsNative

    let validate (schemaURL : string) (objectString : string) = 
        let p = JsonValidation.validateAgainstSchema(objectString, schemaURL)
        let mutable result = None
        async {
            do! p.``then``(fun o ->
                    result <- Some o
                )
                |> Async.AwaitPromise
            return result.Value
        }