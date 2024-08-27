module TestObjects.ROCrate.Person

open ARCtrl.ROCrate
open DynamicObj

let mandatory_properties() =

    let s = Person("person_id_1")

    s.SetValue("givenName","givenName")

    s

let all_properties() =

    let s = Person("person_id_2", "additionalType")

    s.SetValue("givenName","givenName")

    s.SetValue("familyName","familyName")
    s.SetValue("email","email")
    s.SetValue("identifier","identifier")
    s.SetValue("affiliation", ROCrateObject("organization_id","schema.org/Organization"))
    s.SetValue("jobTitle", ROCrateObject("defined_term_id","schema.org/DefinedTerm"))
    s.SetValue("additionalName","additionalName")
    s.SetValue("address","address")
    s.SetValue("telephone","telephone")
    s.SetValue("faxNumber","faxNumber")
    s.SetValue("disambiguatingDescription","disambiguatingDescription")

    s