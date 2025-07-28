module ARCtrl.WebRequestHelpers.NodeJs

open Fable.Core
open Fable.SimpleHttp

open Fable.Core.JsInterop

open Fable.SimpleHttp

open Fable.Core
open Fable.Core.JsInterop
open Fetch

let isNode() =
    emitJsExpr
        ()
        """typeof process !== "undefined" &&
process.versions != null &&
process.versions.node != null;"""

//// From here: https://github.com/fable-compiler/fable3-samples/blob/25ea2404b28c897988b144f0141bc116da292679/nodejs/src/App.fs#L7

let downloadFile url =
    fetch url []
    |> Promise.bind (fun res -> res.text()) // get the result
    |> Promise.map (fun txt -> // bind the result to make further operation
        txt
    )
    |> Async.AwaitPromise
