module Tests.Common

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils

module Expect =

    let inline LDNodeHasId (expectedId:string) (roc:#LDNode) =
        Expect.equal roc.Id expectedId "object did not contain correct @id"

    let inline LDNodeHasType (expectedType:string) (roc:#LDNode) =
        Expect.containsAll
            roc.SchemaType
            [expectedType]
            "object did not contain correct @type"

    let inline LDNodeHasTypes (expectedTypes:seq<string>) (roc:#LDNode) =
        Expect.containsAll
            roc.SchemaType
            expectedTypes
            "object did not contain correct @types"

    let inline LDNodeHasAdditionalType (expectedAdditionalType:string) (roc:#LDNode) =
        Expect.containsAll
            roc.AdditionalType
            [expectedAdditionalType]
            "object did not contain correct additionalType"

    let inline LDNodeHasAdditionalTypes (expectedAdditionalTypes:seq<string>) (roc:#LDNode) =
        Expect.containsAll
            roc.AdditionalType
            expectedAdditionalTypes
            "object did not contain correct additionalTypes"

    let inline LDNodeHasDynamicProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#LDNode) =
        Expect.isSome (roc.TryGetDynamicPropertyHelper(expectedPropertyName)) $"object did not contain the dynamic property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    let inline LDNodeHasStaticProperty (expectedPropertyName:string) (expectedPropertyValue:'P) (roc:#LDNode) =
        Expect.isSome (roc.TryGetStaticPropertyHelper(expectedPropertyName)) $"object did not contain the static property '{expectedPropertyName}'"
        Expect.equal
            (DynObj.tryGetTypedPropertyValue<'P> expectedPropertyName roc)
            (Some expectedPropertyValue)
            $"property value of '{expectedPropertyName}' was not correct"

    //let inline LDNodeHasExpectedInterfaceMembers (expectedTypes: seq<string>) (expectedId:string) (expectedAdditionalTypes: seq<string>) (roc:#LDNode) =
    //    let interfacerino = roc :> ILDNode
    //    Expect.sequenceEqual interfacerino.SchemaType expectedTypes "object did not contain correct @types via interface access"
    //    Expect.equal interfacerino.Id expectedId "object did not contain correct @id via interface access"
    //    Expect.sequenceEqual interfacerino.AdditionalType expectedAdditionalTypes "object did not contain correct additionalTypes via interface access"
