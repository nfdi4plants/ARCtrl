module Tests.Common

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils

module Expect =

    let inline LDObjectHasId (expectedId:string) (roc:#LDObject) =
        Expect.equal roc.Id expectedId "object did not contain correct @id"

    let inline LDObjectHasType (expectedType:string) (roc:#LDObject) =
        Expect.containsAll
            roc.SchemaType
            [expectedType]
            "object did not contain correct @type"

    let inline LDObjectHasTypes (expectedTypes:seq<string>) (roc:#LDObject) =
        Expect.containsAll
            roc.SchemaType
            expectedTypes
            "object did not contain correct @types"

    let inline LDObjectHasAdditionalType (expectedAdditionalType:string) (roc:#LDObject) =
        Expect.containsAll
            roc.AdditionalType
            [expectedAdditionalType]
            "object did not contain correct additionalType"

    let inline LDObjectHasAdditionalTypes (expectedAdditionalTypes:seq<string>) (roc:#LDObject) =
        Expect.containsAll
            roc.AdditionalType
            expectedAdditionalTypes
            "object did not contain correct additionalTypes"

    let inline LDObjectHasDynamicProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#LDObject) =
        Expect.isSome (roc.TryGetDynamicPropertyHelper(expectedPropertyName)) $"object did not contain the dynamic property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline LDObjectHasStaticProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#LDObject) =
        Expect.isSome (roc.TryGetStaticPropertyHelper(expectedPropertyName)) $"object did not contain the static property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline LDObjectHasExpectedInterfaceMembers (expectedTypes: seq<string>) (expectedId:string) (expectedAdditionalTypes: seq<string>) (roc:#LDObject) =
        let interfacerino = roc :> ILDObject
        Expect.sequenceEqual interfacerino.SchemaType expectedTypes "object did not contain correct @types via interface access"
        Expect.equal interfacerino.Id expectedId "object did not contain correct @id via interface access"
        Expect.sequenceEqual interfacerino.AdditionalType expectedAdditionalTypes "object did not contain correct additionalTypes via interface access"
