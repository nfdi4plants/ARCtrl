module ARCtrl.CrossAsync

open Fable.Core


// We need the `<'T>` here as it allows to mark the function/variable inline for better output.
let inline crossAsync<'T> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    promise
#else
    async
#endif

type CrossAsync<'T> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    JS.Promise<'T>
#else
    Async<'T>
#endif

let sequential =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Promise.all
    #else
    Async.Sequential   
    #endif

let map f v =
    crossAsync {
        let! v = v
        return f v
    }

let asAsync (v:CrossAsync<'T>) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Async.AwaitPromise v
    #else
    v
    #endif

let catchWith (f : exn -> 'T) (p : CrossAsync<'T>) : CrossAsync<'T> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Promise.catch f p
    #else
    async {
        let! r = Async.Catch p
        match r with
        | Choice1Of2 x -> return x
        | Choice2Of2 e -> return f e
    }
    #endif
    
let catchAsResult (p : CrossAsync<'T>) : CrossAsync<Result<'T,string>>=
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    p
    |> Promise.map (fun x -> Ok x)
    |> Promise.catch (fun e -> Error (e.ToString()))
    #else
    async {
        let! r = Async.Catch p
        match r with
        | Choice1Of2 x -> return Ok x
        | Choice2Of2 e -> return Error (e.ToString())
    }
    #endif
