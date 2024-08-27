module TestObjects.ROCrate.ScholarlyArticle

open ARCtrl.ROCrate
open DynamicObj

let mandatory_properties() =

    let s = ScholarlyArticle("scholarly_article_id_1")

    s.SetValue("headline","headline")
    s.SetValue("identifier","identifier")

    s

let all_properties() =

    let s = ScholarlyArticle("scholarly_article_id_2", "additionalType")

    s.SetValue("headline","headline")
    s.SetValue("identifier","identifier")

    s.SetValue("author",(TestObjects.ROCrate.Person.all_properties()))
    s.SetValue("creativeWorkStatus", ROCrateObject("defined_term_id","schema.org/DefinedTerm"))
    s.SetValue("disambiguatingDescription", "disambiguatingDescription")
    
    s