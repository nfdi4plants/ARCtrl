module Tests.Common

open ARCtrl.ROCrate

open TestingUtils

module Expect =

    let inline ROCrateObjectHasId (expectedId:string) (roc:#ROCrateObject) =
        Expect.equal roc.Id expectedId "object did not contain correct @id"

    let inline ROCrateObjectHasType (expectedType:string) (roc:#ROCrateObject) =
        Expect.equal roc.SchemaType expectedType "object did not contain correct @type"

    let inline ROCrateObjectHasAdditionalType (expectedAdditionalType:string) (roc:#ROCrateObject) =
        Expect.isSome roc.AdditionalType "additionalType was None"
        Expect.equal roc.AdditionalType (Some expectedAdditionalType) "object did not contain correct additionalType"

    let inline ROCrateObjectHasProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#ROCrateObject) =
        #if !FABLE_COMPILER
        Expect.isTrue (roc.Properties.ContainsKey expectedPropertyName) $"object did not contain the property 'expectedPropertyName'"
        Expect.equal (roc.TryGetTypedValue<'P>(expectedPropertyName)) (Some expectedPropertyValue) "property value of 'expectedPropertyName' was not correct"
        #endif
        #if FABLE_COMPILER
        Expect.equal (roc.TryGetValue(expectedPropertyName)) (Some expectedPropertyValue) "property value of 'expectedPropertyName' was not correct"
        #endif
    let inline ROCrateObjectHasExpectedInterfaceMembers (expectedType:string) (expectedId:string) (expectedAdditionalType:string option) (roc:#ROCrateObject) =
        let interfacerino = roc :> IROCrateObject
        Expect.equal interfacerino.SchemaType expectedType "object did not contain correct @type via interface access"
        Expect.equal interfacerino.Id expectedId "object did not contain correct @id via interface access"
        Expect.equal interfacerino.AdditionalType expectedAdditionalType "object did not contain correct additionalType via interface access"
