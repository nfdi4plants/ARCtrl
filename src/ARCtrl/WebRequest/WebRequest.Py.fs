module ARCtrl.WebRequestHelpers.Py

open Fable.Core

// Taken from https://github.com/Zaid-Ajaj/Fable.Requests/blob/master/src/Library.fs
// Can be replaced either with Fable.SimpleHttp when it supports Python
// or Fable.Requests when it supports netstandard2.0


#if FABLE_COMPILER_PYTHON
type private InteropResponseType =
    abstract status_code : int
    abstract text : string
    abstract encoding : string
    abstract headers : obj

[<RequireQualifiedAccess>]
module Interop = 
    [<Emit("$1[$0]")>]
    let get<'T> (key: string) (dict: obj) : 'T = nativeOnly
    [<Emit("list($0.keys())")>]
    let keys (dict: obj) : string array = nativeOnly

type Response = {
    statusCode: int
    text: string
    headers: Map<string, string>
    encoding: string
}

[<Erase>]
type private RequestsApi = 
    [<Emit "$0.get($1, headers=$2)">]
    abstract get: url:string * ?headers:obj -> InteropResponseType
    [<Emit "$0.post($1, data=$2, headers=$3)">]
    abstract post: url: string * ?data: string * ?headers:obj -> InteropResponseType
    [<Emit "$0.put($1, data=$2, headers=$3)">]
    abstract put: url: string * ?data: string * ?headers:obj -> InteropResponseType
    [<Emit "$0.delete($1, data=$2, headers=$3)">]
    abstract delete: url: string * ?data:string * ?headers:obj -> InteropResponseType
    [<Emit "$0.head($1, data=$2, headers=$3)">]
    abstract head: url:string * ?data: string * ?headers:obj -> InteropResponseType
    [<Emit "$0.options($1, data=$2, headers=$3)">]
    abstract options: url:string * ?data: string * ?headers:obj -> InteropResponseType

type Requests() = 
    static member private createHeadersDict(headers:Map<string, string> option)  = 
        headers
        |> Option.map (fun values -> PyInterop.createObj [ for pair in values -> pair.Key, box pair.Value ])
    
    static member private mapResponseType(response: InteropResponseType) : Response = {
        statusCode = response.status_code
        text = response.text
        encoding = response.text
        headers = Map.ofList [
            for headerName in Interop.keys response.headers do
                let headerValue =  Interop.get<string> headerName response.headers
                headerName, headerValue
        ]
    }

    [<ImportAll("requests")>]
    static member private requestsApi: RequestsApi = nativeOnly

    /// <summary>
    /// Sends a GET request to the specified URL and returns a response.
    /// </summary>
    static member get(url: string, ?headers:Map<string, string>) : Response =
        let headersDict = Requests.createHeadersDict headers
        let response = Requests.requestsApi.get(url, ?headers=headersDict)
        Requests.mapResponseType response

    /// <summary>
    /// Sends a POST request to the specified URL and returns a response.
    /// </summary>
    static member post(url: string, ?data: string, ?headers:Map<string, string>) : Response =
        let headersDict = Requests.createHeadersDict headers
        let response: InteropResponseType = Requests.requestsApi.post(url, ?data=data, ?headers=headersDict)
        Requests.mapResponseType response

    /// <summary>
    /// Sends a PUT request to the specified URL and returns a response.
    /// </summary>
    static member put(url: string, ?data: string, ?headers:Map<string, string>) : Response =
        let headersDict = Requests.createHeadersDict headers
        let response: InteropResponseType = Requests.requestsApi.put(url, ?data=data, ?headers=headersDict)
        Requests.mapResponseType response

    /// <summary>
    /// Sends a DELETE request to the specified URL and returns a response.
    /// </summary>
    static member delete(url: string, ?data: string, ?headers:Map<string, string>) : Response =
        let headersDict = Requests.createHeadersDict headers
        let response: InteropResponseType = Requests.requestsApi.delete(url, ?data=data, ?headers=headersDict)
        Requests.mapResponseType response

    /// <summary>
    /// Sends a HEAD request to the specified URL and returns a response.
    /// </summary>
    static member head(url: string, ?data: string, ?headers:Map<string, string>) : Response =
        let headersDict = Requests.createHeadersDict headers
        let response: InteropResponseType = Requests.requestsApi.head(url, ?data=data, ?headers=headersDict)
        Requests.mapResponseType response

    /// <summary>
    /// Sends an OPTIONS request to the specified URL and returns a response.
    /// </summary>
    static member options(url: string, ?data: string, ?headers:Map<string, string>) : Response =
        let headersDict = Requests.createHeadersDict headers
        let response: InteropResponseType = Requests.requestsApi.head(url, ?data=data, ?headers=headersDict)
        Requests.mapResponseType response

let downloadFile url =
    async {
        let response = Requests.get url
        return response.text
    }
#endif