module ARCtrl.Template.Web

open ARCtrl.Template
open ARCtrl.ISA

let getTemplates(url: string option) =
    let defaultURL = @"https://github.com/nfdi4plants/Swate-templates/releases/download/latest/templates.json"
    let url = defaultArg url defaultURL
    async {
        let! jsonString = ARCtrl.WebRequest.downloadFile url
        let mapResult = Json.Templates.decodeFromString jsonString
        return mapResult
    }