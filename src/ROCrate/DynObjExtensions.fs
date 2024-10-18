namespace ARCtrl.ROCrate

open DynamicObj

module DynObj =

    let inline hasProperty (propertyName: string) (obj: #DynamicObj) = DynObj.tryGetPropertyValue propertyName obj |> Option.isSome

    let inline getMandatoryDynamicPropertyOrThrow<'TPropertyValue> (className:string) (propertyName: string) (obj: #DynamicObj) =
        if hasProperty propertyName obj then
            match DynObj.tryGetTypedPropertyValue<'TPropertyValue> propertyName obj with
            | Some value -> value
            | None -> raise (System.InvalidCastException($"Property '{propertyName}' is set on this '{className}' object but cannot be cast to '{(typeof<'TPropertyValue>).Name}'"))
        else
            raise (System.MissingMemberException($"No property '{propertyName}' set on this '{className}' object although it is mandatory. Was it created correctly?"))