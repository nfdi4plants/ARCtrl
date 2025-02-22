namespace ARCtrl.Helper

module Seq = 
    let inline compare (a: seq<'a>) (b: seq<'a>) =
        if Seq.length a = Seq.length b then
            [ for i in 0 .. (Seq.length a) - 1 do yield (Seq.item i a = Seq.item i b) ]
            |> Seq.fold (fun acc e -> acc && e) true
        else 
            false

module Option =
 
    /// If the value matches the default, a None is returned, else a Some is returned
    let fromValueWithDefault d v =
        if d = v then None
        else Some v

    /// Applies the function f on the value of the option if it exists, else applies it on the default value. If the result value matches the default, a None is returned
    let mapDefault (d : 'T) (f: 'T -> 'T) (o : 'T option) =
        match o with
        | Some v -> f v
        | None   -> f d
        |> fromValueWithDefault d

    /// Applies the function f on the value of the option if it exists, else returns the default value. 
    let mapOrDefault (d : 'T Option) (f: 'U -> 'T) (o : 'U option) =
        match o with
        | Some v -> Some (f v)
        | None   -> d

module internal List = 
    
    let tryPickAndRemove (f : 'T -> 'U option) (lst : 'T list) =
        let rec loop newList remainingList = 
            match remainingList with
            | h::t -> 
                match f h with
                | Some v -> Some v, newList @ t
                | None -> loop (newList @ [h]) t
            | _ -> None, newList
        loop [] lst

module Dictionary = 

    open System.Collections.Generic

    let addOrUpdate (key : 'Key) (value : 'T) (dict : Dictionary<'Key,'T>) =
        if dict.ContainsKey key then
            dict.[key] <- value
        else
            dict.Add(key,value)

    let ofSeq (s : seq<'Key*'T>) = 
        let dict = Dictionary()
        s
        |> Seq.iter dict.Add
        dict

    ///// <summary>Lookup an element in the dictionary, returning a <c>Some</c> value if the element is in the domain 
    ///// of the dictionary and <c>None</c> if not.</summary>
    ///// <param name="key">The input key.</param>
    ///// <returns>The mapped value, or None if the key is not in the dictionary.</returns>
    //let tryFind key (table:IDictionary<_,_>) =
    //    match table.ContainsKey(key) with
    //    | true -> Some table.[key]
    //    | false -> None

    let tryFind (key : 'Key) (dict : Dictionary<'Key,'T>) =
        let b,v = dict.TryGetValue key
        if b then Some v 
        else None

    let ofSeqWithMerge (merge : 'T -> 'T -> 'T) (s : seq<'Key*'T>) = 
        let dict = Dictionary()
        s
        |> Seq.iter (fun (k,v) -> 
            match tryFind k dict with
            | Some v' ->                
                dict.Remove(k) |> ignore
                dict.Add(k,merge v' v)
            | None ->
                dict.Add(k,v)
            )
        dict

module ResizeArray =  

    open System.Collections.Generic

    let map  f (a : ResizeArray<_>) =
        let b = ResizeArray<_>()
        for i in a do
            b.Add(f i)
        b       

    let choose f (a : ResizeArray<_>) =
        let b = ResizeArray<_>()
        for i in a do
            match f i with
            | Some x -> b.Add(x)
            | None -> ()
        b

    let filter f (a : ResizeArray<_>) =
        let b = ResizeArray<_>()
        for i in a do
            if f i then b.Add(i)
        b

    let fold f s (a : ResizeArray<_>) =
        let mutable state = s
        for i in a do
            state <- f state i
        state

    let foldBack f (a : ResizeArray<_>) s =
        let mutable state = s
        for i in a do
            state <- f i state
        state

    let iter f (a : ResizeArray<_>) =
        for i in a do
            f i

    let reduce f (a : ResizeArray<_>) =
        match a with
        | a when a.Count = 0 -> failwith "ResizeArray.reduce: empty array"
        | a when a.Count = 1 -> a.[0]
        | a -> 
            let mutable state = a.[0]
            for i in 1 .. a.Count - 1 do
                state <- f state a.[i]
            state

    let collect f (a : ResizeArray<_>) =
        let b = ResizeArray<_>()
        for i in a do
            let c = f i
            for j in c do
                b.Add(j)
        b

    let distinct (a : ResizeArray<_>) =
        let b = ResizeArray<_>()
        for i in a do
            if not (b.Contains(i)) then
                b.Add(i)
        b

    let isEmpty (a : ResizeArray<_>) =
        a.Count = 0

    /// Immutable append
    let append (a : ResizeArray<_>) (b : ResizeArray<_>) =
        let c = ResizeArray<_>()
        for i in a do
            c.Add(i)
        for i in b do
            c.Add(i)
        c

    /// append a single element
    let appendSingleton (b : 'T) (a : ResizeArray<_>) =
        let c = ResizeArray<_>()
        for i in a do
            c.Add(i)
        c.Add(b)
        c


    // Make sure that output type matches
    let groupBy (f : 'T -> 'a) (a : ResizeArray<'T>) : ResizeArray<'a*ResizeArray<'T>> =
        Seq.groupBy f a
        |> Seq.map (fun (k,v) -> k, ResizeArray v)
        |> ResizeArray

    let tryPick f (a : ResizeArray<'T>) =
        let rec loop i =
            if i < a.Count then
                match f a.[i] with
                | Some v -> Some v
                | None -> loop (i + 1)
            else None
        loop 0

    let zip (a : ResizeArray<'T>) (b : ResizeArray<'U>) =
        let c = ResizeArray<_>()
        let n = min a.Count b.Count
        for i in 0 .. n - 1 do
            c.Add(a.[i], b.[i])
        c