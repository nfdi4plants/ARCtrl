module Tests.Common

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils

module Expect =

    let inline LDObjectHasId (expectedId:string) (roc:#LDObject) =
        Expect.equal roc.Id expectedId "object did not contain correct @id"

    let inline LDObjectHasType (expectedType:string) (roc:#LDObject) =
        Expect.equal roc.SchemaType expectedType "object did not contain correct @type"

    let inline LDObjectHasAdditionalType (expectedAdditionalType:string) (roc:#LDObject) =
        Expect.isSome roc.AdditionalType "additionalType was None"
        Expect.equal roc.AdditionalType (Some expectedAdditionalType) "object did not contain correct additionalType"

    let inline LDObjectHasDynamicProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#LDObject) =
        Expect.isSome (roc.TryGetDynamicPropertyHelper(expectedPropertyName)) $"object did not contain the dynamic property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline LDObjectHasStaticProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#LDObject) =
        Expect.isSome (roc.TryGetDynamicPropertyHelper(expectedPropertyName)) $"object did not contain the dynamic property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline LDObjectHasExpectedInterfaceMembers (expectedType:string) (expectedId:string) (expectedAdditionalType:string option) (roc:#LDObject) =
        let interfacerino = roc :> ILDObject
        Expect.equal interfacerino.SchemaType expectedType "object did not contain correct @type via interface access"
        Expect.equal interfacerino.Id expectedId "object did not contain correct @id via interface access"
        Expect.equal interfacerino.AdditionalType expectedAdditionalType "object did not contain correct additionalType via interface access"
