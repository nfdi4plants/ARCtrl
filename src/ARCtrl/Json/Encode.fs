namespace ARCtrl.Json

open Thoth.Json.Core

open Fable.Core

//#if FABLE_COMPILER_PYTHON
//open Fable.Core.PyInterop
//#endif
//#if FABLE_COMPILER_JAVASCRIPT
//open Fable.Core.JsInterop
//#endif

[<RequireQualifiedAccess>]
module Encode = 

    let inline toJsonString spaces (value : IEncodable) = 
        #if FABLE_COMPILER_PYTHON
        Thoth.Json.Python.Encode.toString spaces value
        #endif
        #if FABLE_COMPILER_JAVASCRIPT
        Thoth.Json.JavaScript.Encode.toString spaces value
        #endif
        #if !FABLE_COMPILER
        Thoth.Json.Newtonsoft.Encode.toString spaces value
        #endif
