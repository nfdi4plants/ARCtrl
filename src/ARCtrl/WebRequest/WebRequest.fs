module ARCtrl.WebRequest

open Fable.Core
open Fable.SimpleHttp

let downloadFile url =
    let browserAndDotnet() =  
        async {
            let! (statusCode, responseText) = Http.get url

            return 
                match statusCode with
                | 200 -> responseText
                | _ -> failwithf "Status %d => %s" statusCode responseText
        }
    #if FABLE_COMPILER_JAVASCRIPT
    
    if ARCtrl.WebRequestHelpers.NodeJs.isNode() then
        // From here: https://github.com/fable-compiler/fable3-samples/blob/25ea2404b28c897988b144f0141bc116da292679/nodejs/src/App.fs#L7
        ARCtrl.WebRequestHelpers.NodeJs.importSideEffects()
        ARCtrl.WebRequestHelpers.NodeJs.downloadFile url
    else
        browserAndDotnet()
    #endif

    #if FABLE_COMPILER_PYTHON
    ARCtrl.WebRequestHelpers.Py.downloadFile url
    #endif

    #if !FABLE_COMPILER
    browserAndDotnet()
    #endif