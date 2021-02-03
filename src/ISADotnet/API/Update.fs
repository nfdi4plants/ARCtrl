namespace ISADotNet.API

open ISADotNet

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

module Update =

    open System
    open Microsoft.FSharp.Reflection

    /// matches if the matched object can be parsed to Some 'a and returns it.
    let private (|SomeObj|_|) =
        /// create generalized option type
        let ty = typedefof<option<_>>
        fun (a:obj) ->
            /// Check for nulls otherwise 'a.GetType()' would fail
            if isNull a 
            then 
                None 
            else
                let aty = a.GetType()
                /// Get option'.Value
                let v = aty.GetProperty("Value")
                if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
                    /// return value if existing
                    Some(v.GetValue(a, [| |]))
                else 
                    None
    
    let private makeOptionValue typey v isSome =
        let optionType = typeof<unit option>.GetGenericTypeDefinition().MakeGenericType([|typey|])
        let cases = FSharp.Reflection.FSharpType.GetUnionCases(optionType)
        let cases = cases |> Array.partition (fun x -> x.Name = "Some")
        let someCase = fst cases |> Array.exactlyOne
        let noneCase = snd cases |> Array.exactlyOne
        let relevantCase, args =
            match isSome with
            | true -> someCase, [| v |]
            | false -> noneCase, [| |]
        FSharp.Reflection.FSharpValue.MakeUnion(relevantCase, args)

    /// updates oldRT with newRT by replacing all values, but appending all lists.
    ///
    /// newRTList@oldRTList
    let rec private updateAppend (oldVal: obj) (newVal:obj) = 
        
        /// match all field Values and try to cast them to types.
        match oldVal with
        /// Strings are IEnumerable Chars but should not be appenden. So these have to be handled first.
        | :? String ->
            newVal
        | SomeObj(oldInternal) ->
            match oldInternal with
            | :? String -> 
                newVal
            | :? System.Collections.IEnumerable ->
                let newOpt = newVal
                match newOpt with
                | SomeObj newInternal ->
                    updateAppend oldInternal newInternal
                    |> fun v -> makeOptionValue (oldInternal.GetType()) v true
                | _ -> oldVal
            | _ -> 
                newVal
        /// Match all IEnumarables, like list, array, seq. These should be appended.
        | :? System.Collections.IEnumerable -> 
            let oldSeq = oldVal
            let newSeq = newVal
            let fieldT = oldVal.GetType()
            /// Maps are IEnumerables but are not easily to append. TODO(?)
            let isMap =
                let genericMap = typeof<Map<_,_>>
                fieldT.Name = genericMap.Name
            /// t is the type of the IEnumerable elements.
            let t =
                /// element types are accessed differently for (list, seq) and Array. 
                if fieldT.IsArray then fieldT.GetElementType() else
                    oldVal.GetType().GetGenericArguments() |> Array.head
            /// This function accesses the append method of the list/array module and applies it accordingly to the element type.
            let appendGenericListsByType l1 l2 (t:Type) =
                // https://stackoverflow.com/questions/41253131/how-to-create-an-empty-list-of-a-specific-runtime-type
                System.Reflection.Assembly
                    .GetAssembly(typeof<_ list>)
                    .GetType(if fieldT.IsArray then "Microsoft.FSharp.Collections.ArrayModule" else "Microsoft.FSharp.Collections.ListModule")
                    .GetMethod("Append")
                    .MakeGenericMethod(t)
                    .Invoke(null, [|l1;l2|])
            /// If the IEnumerable is a map then we just replace with the new entry.
            if isMap then 
                newVal
            else
                let r = 
                    appendGenericListsByType oldSeq newSeq t
                r |> box
        /// All others do not need to be appended and can be replaced.
        | others -> 
            newVal


    /// updates oldRT with newRT by replacing all values, but only if the new value is not empty.
    let rec private updateOnlyByExisting (oldVal: obj) (newVal:obj) =      
        
        /// try to cast values to types to check for isEmpty according to type.
        match oldVal with 
        /// Check if newValue isNull = isEmpty
        | _ when newVal = null ->
            oldVal
        /// Handle OptionTypes
        // https://stackoverflow.com/questions/6289761/how-to-downcast-from-obj-to-optionobj
        /// Check of value is option, then check if new value isNone = isEmpty
        | SomeObj(oldInternal) ->
            let newOpt = newVal
            match newOpt with
            | SomeObj newInternal ->
                updateOnlyByExisting oldInternal newInternal
                |> fun v -> makeOptionValue (oldInternal.GetType()) v true
            | _ -> 
                oldVal
        /// Check if value is string, then check if new value is "" = isEmpty
        | :? String ->
            let newStr = newVal
            if string newStr = "" then oldVal else newStr
        /// https://stackoverflow.com/questions/47280544/determine-if-any-kind-of-list-sequence-array-or-ienumerable-is-empty
        /// Check if value is IEnumarable, then cast newValue to Seq and check if isEmpty
        | :? System.Collections.IEnumerable -> 
            let newSeq = newVal
            if newSeq :?> System.Collections.IEnumerable |> Seq.cast |> Seq.isEmpty
            then oldVal 
            else newSeq
        /// Others don't need to be checked as they have no clearly enough defined "empty" state
        | _ ->
            newVal
        
        

    /// This type specifies the exact manner on how complex types will be updated
    type UpdateOptions = 
        /// Updates all existing fields by replacing them with the corresponding new fields
        | UpdateAll
        /// Updates all existing fields by replacing them with the corresponding new fields if the new field is not empty.
        ///
        /// Empty string = ""; Empty IEnumerable (Set, List, Array, Seq, Map); Empty Option = None; null
        | UpdateByExisting
        /// Updates all existing fields by replacing them with the corresponding new fields, except any lists which will be appended with the new list shown first
        ///
        /// Maps are currently not appended
        | UpdateAllAppendLists

        /// This function will update recordType_1 with the values given in recordType_2 as specified by UpdateOption.
        member this.updateRecordType (recordType_1:'a) (recordType_2:'a) =    
            match this with
            | UpdateAll ->
                recordType_2
            | UpdateAllAppendLists ->
                (FSharp.Reflection.FSharpValue.GetRecordFields recordType_1,FSharp.Reflection.FSharpValue.GetRecordFields recordType_2)
                ||> Array.map2 updateAppend
                |> fun fields -> FSharpValue.MakeRecord(typeof<'a>, fields) :?> 'a
            | UpdateByExisting ->
                (FSharp.Reflection.FSharpValue.GetRecordFields recordType_1,FSharp.Reflection.FSharpValue.GetRecordFields recordType_2)
                ||> Array.map2 updateOnlyByExisting
                |> fun fields -> FSharpValue.MakeRecord(typeof<'a>, fields) :?> 'a
                