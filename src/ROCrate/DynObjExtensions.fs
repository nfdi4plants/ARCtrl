namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open Fable.Core.PyInterop

module DynObj =

    let inline hasProperty (propertyName: string) (obj: #DynamicObj) = DynObj.tryGetPropertyValue propertyName obj |> Option.isSome

    let inline getMandatoryDynamicPropertyOrThrow<'TPropertyValue> (className:string) (propertyName: string) (obj: #DynamicObj) =
        if hasProperty propertyName obj then
            match DynObj.tryGetTypedPropertyValue<'TPropertyValue> propertyName obj with
            | Some value -> value
            | None -> raise (System.InvalidCastException($"Property '{propertyName}' is set on this '{className}' object but cannot be cast to '{(typeof<'TPropertyValue>).Name}'"))
        else
            raise (System.MissingMemberException($"No property '{propertyName}' set on this '{className}' object although it is mandatory. Was it created correctly?"))

    let inline tryGetTypedPropertyValueAsResizeArray<'T> (name : string) (obj : DynamicObj) =
        match obj.TryGetPropertyValue(name) with
#if FABLE_COMPILER_PYTHON
        // Fable's Python backend compiles every array type test to `isinstance(x, Array)` against its
        // native Array class, but a ResizeArray is represented by a plain Python list, so
        // `:? ResizeArray<'T>` is always false and the singleton branch below would swallow arrays.
        | Some v when emitPyExpr v "isinstance($0, list)" -> Some (v :?> ResizeArray<'T>)
#else
        | Some (:? ResizeArray<'T> as ra) -> Some ra
#endif
        | Some (:? 'T as singleton) -> Some (ResizeArray [singleton])
        | _ -> None