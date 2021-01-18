module Update

open System
open Microsoft.FSharp.Reflection

let private (|SomeObj|_|) =
    let ty = typedefof<option<_>>
    fun (a:obj) ->
        /// Check for nulls otherwise 'a.GetType()' would fail
        if isNull a 
        then 
            None 
        else
            let aty = a.GetType()
            let v = aty.GetProperty("Value")
            if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
                if a = null then None
                else Some(v.GetValue(a, [| |]))
            else None

let private getFieldNameValues (objValue:'a) =
    objValue.GetType().GetProperties() |> Array.map (fun n -> n.Name, n.GetValue(objValue) )

let private updateAppend (oldRT:'a) (newRT:'a) = 
    let newMap  = getFieldNameValues newRT |> Map.ofArray
    let oldType = getFieldNameValues oldRT
    let fields =
        oldType |> Array.map (fun (fieldName, fieldValue) ->
            match fieldValue with
            /// Strings are IEnumerable Chars
            | :? String ->
                newMap.[fieldName]
            | :? System.Collections.IEnumerable -> 
                let oldSeq = fieldValue
                let newSeq = newMap.[fieldName]
                let fieldT = fieldValue.GetType()
                /// Maps are IEnumerables but are not easily to append. TODO(?)
                let isMap =
                    let genericMap = typeof<Map<_,_>>
                    fieldT.Name = genericMap.Name
                let t =
                    if fieldT.IsArray then fieldT.GetElementType() else
                        fieldValue.GetType().GetGenericArguments() |> Array.head
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
                    newMap.[fieldName]
                else
                    let r = 
                        appendGenericListsByType newSeq oldSeq t
                    r |> box
            | others -> 
                newMap.[fieldName]
        )
    FSharpValue.MakeRecord(typeof<'a>, fields) :?> 'a
    

let private updateOnlyByExisting (oldRT:'a) (newRT:'a) =
    let newMap  = getFieldNameValues newRT |> Map.ofArray
    let oldType = getFieldNameValues oldRT
    let fields =
        oldType |> Array.map (fun (fieldName, fieldValue) ->
            match fieldValue with 
            | x when newMap.[fieldName] = null->
                fieldValue
            /// Handle OptionTypes
            // https://stackoverflow.com/questions/6289761/how-to-downcast-from-obj-to-optionobj
            | SomeObj(opt) ->
                let newOpt = newMap.[fieldName]
                match newOpt with
                | SomeObj n ->
                    newMap.[fieldName]
                | _ -> 
                    fieldValue
            | :? String ->
                let newStr = newMap.[fieldName]
                if string newStr = "" then fieldValue else newStr
            /// https://stackoverflow.com/questions/47280544/determine-if-any-kind-of-list-sequence-array-or-ienumerable-is-empty
            | :? System.Collections.IEnumerable -> 
                let newSeq = newMap.[fieldName]
                if newSeq :?> System.Collections.IEnumerable |> Seq.cast |> Seq.isEmpty
                then fieldValue 
                else newSeq
            | _ ->
                newMap.[fieldName]
        )
    FSharpValue.MakeRecord(typeof<'a>, fields) :?> 'a

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
            updateAppend recordType_1 recordType_2
        | UpdateByExisting ->
            updateOnlyByExisting recordType_1 recordType_2