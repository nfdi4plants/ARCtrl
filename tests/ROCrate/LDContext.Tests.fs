module Tests.LDContext

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let schemaTerm = "schema"
let schemaIRI = "http://schema.org/"

let nameTerm = "name"
let nameIRI = "http://schema.org/name"
let nameCompactIRI = "schema:name"

let nameIRIHttps = "https://schema.org/name"
let schemaIRIHttps = "https://schema.org/"

let nameIRIAlternative = "http://fantasy-site.org/name"


let tests_equal = testList "Equality" [
    testCase "empty" <| fun _ -> 
        let context1 = new LDContext()
        let context2 = new LDContext()
        Expect.equal context1 context2 "empty contexts were not equal"
    testCase "hasKeys_vs_empty" <| fun _ -> 
        let context1 = new LDContext()
        context1.AddMapping(nameTerm, nameIRI)
        let context2 = new LDContext()
        Expect.notEqual context1 context2 "contexts with different keys were equal"
    testCase "differentKeys" <| fun _ -> 
        let context1 = new LDContext()
        context1.AddMapping(nameTerm, nameIRI)
        let context2 = new LDContext()
        context2.AddMapping(nameTerm, nameIRIAlternative)
        Expect.notEqual context1 context2 "contexts with different keys were equal"
    testCase "sameKeys" <| fun _ -> 
        let context1 = new LDContext()
        context1.AddMapping(nameTerm, nameIRI)
        let context2 = new LDContext()
        context2.AddMapping(nameTerm, nameIRI)
        Expect.equal context1 context2 "contexts with same keys were not equal"
]

let tests_resolveTerm = testList "resolveTerm" [
    testCase "null" <| fun _ -> 
        let context = new LDContext()
        let resolved = context.TryResolveTerm(nameTerm)
        Expect.isNone resolved "missing term was resolved"
    testCase "fullIRI" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let resolved = context.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "compactIRI" <| fun _ -> 
        let context = new LDContext()
        context.AddMapping(nameTerm, nameCompactIRI)
        context.AddMapping(schemaTerm, schemaIRI)
        let resolved = context.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "Nested_Shadowed" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        outerContext.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = outerContext.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRIAlternative "term was not resolved correctly"
    testCase "Nested" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let resolved = outerContext.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
]

let tests_getTerm = testList "getTerm" [
    testCase "null" <| fun _ -> 
        let context = new LDContext()
        let resolved = context.TryGetTerm(nameIRI)
        Expect.isNone resolved "missing term was resolved"
    testCase "fullIRI" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "fullIRI_ignoreHTTPs" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRIHttps)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "fullIRI_ignoreHTTP" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let resolved = context.TryGetTerm(nameIRIHttps)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "compactIRI" <| fun _ -> 
        let context = new LDContext()
        context.AddMapping(nameTerm, nameCompactIRI)
        context.AddMapping(schemaTerm, schemaIRI)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "compactIRI_reverseOrder" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(schemaTerm, schemaIRI)
        context.AddMapping(nameTerm, nameCompactIRI)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "compactIRI_ignoreHTTPs" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameCompactIRI)
        context.AddMapping(schemaTerm, schemaIRIHttps)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "compactIRI_ignoreHTTP" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameCompactIRI)
        context.AddMapping(schemaTerm, schemaIRI)
        let resolved = context.TryGetTerm(nameIRIHttps)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "compactIRI_reverseOrder_ignoreHTTPs" <| fun _ -> 
        let context = new LDContext()
        context.AddMapping(schemaTerm, schemaIRIHttps)
        context.AddMapping(nameTerm, nameCompactIRI)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "compactIRI_reverseOrder_ignoreHTTP" <| fun _ -> 
        let context = new LDContext()
        context.AddMapping(schemaTerm, schemaIRI)
        context.AddMapping(nameTerm, nameCompactIRI)
        let resolved = context.TryGetTerm(nameIRIHttps)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "Nested_Shadowed" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        outerContext.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = outerContext.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    testCase "Nested" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let resolved = outerContext.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
] 

let tests_shallowCopy = testList "shallowCopy" [
    testCase "empty" <| fun _ -> 
        let context = new LDContext()
        let copy = context.ShallowCopy()
        Expect.isEmpty copy.Mappings "shallow copy was not empty"
    testCase "empty_immutable" <| fun _ -> 
        let context = new LDContext()
        let copy = context.ShallowCopy()
        context.AddMapping(nameTerm, nameIRI)
        Expect.isEmpty copy.Mappings "shallow copy was not empty"
    testCase "withMapping" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let copy = context.ShallowCopy()
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "withMapping_immutable" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let copy = context.ShallowCopy()
        context.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "withBaseContext" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let copy = outerContext.ShallowCopy()
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "withBaseContext_mutable" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let copy = outerContext.ShallowCopy()
        innerContext.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRIAlternative "term was not resolved correctly"
]

let tests_deepCopy = testList "deepCopy" [
    testCase "empty" <| fun _ -> 
        let context = new LDContext()
        let copy = context.DeepCopy()
        Expect.isEmpty copy.Mappings "deep copy was not empty"
    testCase "empty_immutable" <| fun _ -> 
        let context = new LDContext()
        let copy = context.DeepCopy()
        context.AddMapping(nameTerm, nameIRI)
        Expect.isEmpty copy.Mappings "deep copy was not empty"
    testCase "withMapping" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let copy = context.DeepCopy()
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "withMapping_immutable" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let copy = context.DeepCopy()
        context.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "withBaseContext" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let copy = outerContext.DeepCopy()
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    testCase "withBaseContext_immutable" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let copy = outerContext.DeepCopy()
        innerContext.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = copy.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
]

let main = testList "LDContext" [
    tests_equal
    tests_resolveTerm
    tests_getTerm
    tests_shallowCopy
    tests_deepCopy
]