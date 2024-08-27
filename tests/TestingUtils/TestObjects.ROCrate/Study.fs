module TestObjects.ROCrate.Study

open ARCtrl.ROCrate

let mandatory_properties() =

    let s = Study("study_id_1")

    s.SetValue("identifier", "identifier")

    s

let all_properties() =

    let s = Study("study_id_2")

    s.SetValue("identifier", "identifier")

    s.SetValue("creator", Person.all_properties())
    s.SetValue("headline", "headline")
    s.SetValue("hasPart", ROCrateObject("file_id_1","schema.org/MediaObject"))
    s.SetValue("about", LabProcess.all_properties())
    s.SetValue("description", "description")
    s.SetValue("dateCreated", Common.testDateTime)
    s.SetValue("datePublished", Common.testDate)
    s.SetValue("dateModified", Common.testDate)
    s.SetValue("citation", ScholarlyArticle.all_properties())
    s.SetValue("comment", "")
    s.SetValue("url", "url")

    s