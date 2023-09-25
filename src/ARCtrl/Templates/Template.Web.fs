module ARCtrl.Template.Web

open ARCtrl.Template
open ARCtrl.ISA
open Fable.Core


let getTemplates(url: string option) =
    let defaultURL = @"https://github.com/nfdi4plants/Swate-templates/releases/download/latest/templates.json"
    let url = defaultArg url defaultURL
    async {
        let! jsonString = ARCtrl.WebRequest.downloadFile url
        let mapResult = Json.Templates.decodeFromString jsonString
        return mapResult
    }

open Fable.Core.JsInterop

/// <summary>
/// This class is used to make async functions more accessible from JavaScript.
/// </summary>
[<AttachMembers>]
type JsWeb =
    static member getTemplates(url: string option) =
        async {
            let! map = getTemplates(url)
            return System.Collections.Generic.Dictionary(map)
        }
        |> Async.StartAsPromise