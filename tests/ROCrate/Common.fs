module Tests.Common

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils

module Expect =

    let inline ROCrateObjectHasId (expectedId:string) (roc:#ROCrateObject) =
        Expect.equal roc.Id expectedId "object did not contain correct @id"

    let inline ROCrateObjectHasType (expectedType:string) (roc:#ROCrateObject) =
        Expect.equal roc.SchemaType expectedType "object did not contain correct @type"

    let inline ROCrateObjectHasAdditionalType (expectedAdditionalType:string) (roc:#ROCrateObject) =
        Expect.isSome roc.AdditionalType "additionalType was None"
        Expect.equal roc.AdditionalType (Some expectedAdditionalType) "object did not contain correct additionalType"

    let inline ROCrateObjectHasDynamicProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#ROCrateObject) =
        Expect.isSome (roc.TryGetDynamicPropertyHelper(expectedPropertyName)) $"object did not contain the dynamic property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline ROCrateObjectHasStaticProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#ROCrateObject) =
        Expect.isSome (roc.TryGetDynamicPropertyHelper(expectedPropertyName)) $"object did not contain the dynamic property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline ROCrateObjectHasExpectedInterfaceMembers (expectedType:string) (expectedId:string) (expectedAdditionalType:string option) (roc:#ROCrateObject) =
        let interfacerino = roc :> IROCrateObject
        Expect.equal interfacerino.SchemaType expectedType "object did not contain correct @type via interface access"
        Expect.equal interfacerino.Id expectedId "object did not contain correct @id via interface access"
        Expect.equal interfacerino.AdditionalType expectedAdditionalType "object did not contain correct additionalType via interface access"
