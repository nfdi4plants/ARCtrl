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

let nameIRIAlternative = "http://fantasy-site.org/name"


let tests_resolveTerm = testList "resolveTerm" [
    ftestCase "null" <| fun _ -> 
        let context = new LDContext()
        let resolved = context.TryResolveTerm(nameTerm)
        Expect.isNone resolved "missing term was resolved"
    ftestCase "fullIRI" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let resolved = context.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    ftestCase "compactIRI" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameCompactIRI)
        context.AddMapping(schemaTerm, schemaIRI)
        let resolved = context.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
    ftestCase "Nested_Shadowed" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        outerContext.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = outerContext.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRIAlternative "term was not resolved correctly"
    ftestCase "Nested" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let resolved = outerContext.TryResolveTerm(nameTerm)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameIRI "term was not resolved correctly"
]

let tests_getTerm = testList "getTerm" [
    ftestCase "null" <| fun _ -> 
        let context = new LDContext()
        let resolved = context.TryGetTerm(nameIRI)
        Expect.isNone resolved "missing term was resolved"
    ftestCase "fullIRI" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameIRI)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    ftestCase "compactIRI" <| fun _ ->
        let context = new LDContext()
        context.AddMapping(nameTerm, nameCompactIRI)
        context.AddMapping(schemaTerm, schemaIRI)
        let resolved = context.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    ftestCase "Nested_Shadowed" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        outerContext.AddMapping(nameTerm, nameIRIAlternative)
        let resolved = outerContext.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
    ftestCase "Nested" <| fun _ ->
        let innerContext = new LDContext()
        innerContext.AddMapping(nameTerm, nameIRI)
        let outerContext = new LDContext(baseContexts = ResizeArray [innerContext])
        let resolved = outerContext.TryGetTerm(nameIRI)
        let resolved = Expect.wantSome resolved "term was not resolved"
        Expect.equal resolved nameTerm "term was not resolved correctly"
] 

let main = testList "LDContext" [
    tests_resolveTerm
    tests_getTerm
]