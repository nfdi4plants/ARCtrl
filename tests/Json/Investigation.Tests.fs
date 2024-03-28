module Tests.Investigation

open ARCtrl
open ARCtrl.Json
open TestingUtils


module Helper =
    let create_empty() = ArcInvestigation.create("My Astonishing Investigation")
    let create_filled() = 
        let assay1 = Assay.Helper.create_filled();
        let assay2 = Assay.Helper.create_empty();
        let study1 = Study.Helper.create_filled();
        let study2 = Study.Helper.create_empty()
        study1.RegisteredAssayIdentifiers <- ResizeArray()
        study2.RegisteredAssayIdentifiers <- ResizeArray()
        let inv =
            ArcInvestigation.make
                "My Inv"
                (Some "My Title")
                (Some "My Description")
                (Some "My Submission Date")
                (Some "My Release Date")
                (ResizeArray [OntologySourceReference.create("Description", "path/to/file", "OSR Name")])
                (ResizeArray [Publication.create(doi="any-nice-doi-42")])
                (ResizeArray [Person.create(firstName="Kevin", lastName="Frey")])
                (ResizeArray [])
                (ResizeArray [study1; study2])
                (ResizeArray [study1.Identifier; study2.Identifier])
                (ResizeArray [Comment.create("Hello", "World")])
                (ResizeArray())
        inv.AddAssay(assay1, inv.Studies)
        inv.AddAssay(assay2, inv.Studies)
        inv

    let compareFields (expected: ArcInvestigation) (actual: ArcInvestigation) =
        Expect.equal actual.Identifier expected.Identifier "Identifier"
        Expect.equal actual.Title expected.Title "Title"
        Expect.equal actual.Description expected.Description "Description"
        Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
        Expect.sequenceEqual actual.OntologySourceReferences expected.OntologySourceReferences "OntologySourceReferences"
        Expect.sequenceEqual actual.Publications expected.Publications "Publications"
        Expect.sequenceEqual actual.Contacts expected.Contacts "Contacts"
        Expect.sequenceEqual actual.Assays expected.Assays "Assays"
        Expect.sequenceEqual actual.Studies expected.Studies "Studies"
        Expect.sequenceEqual actual.RegisteredStudyIdentifiers expected.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
        Expect.sequenceEqual actual.Comments expected.Comments "Comments"
        Expect.sequenceEqual actual.Remarks expected.Remarks "Remarks"
    
open Helper

let private test_coreEmpty =
    let json = """{"Identifier":"My Astonishing Investigation"}"""
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcInvestigation.toJsonString
        ArcInvestigation.fromJsonString

let private test_compressedEmpty =
    let json = """{"cellTable":[],"oaTable":[],"stringTable":[],"object":{"Identifier":"My Astonishing Investigation"}}"""
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcInvestigation.toCompressedJsonString
        ArcInvestigation.fromCompressedJsonString

let private test_isaEmpty =
    let json = """{"filename":"isa.investigation.xlsx","identifier":"My Astonishing Investigation"}"""
    createBaseJsonTests
        "isa-empty"
        create_empty
        ArcInvestigation.toISAJsonString
        ArcInvestigation.fromISAJsonString

let private test_roCrateEmpty =
    let json = """{"@id":"./","@type":"Investigation","additionalType":"Investigation","identifier":"My Astonishing Investigation","filename":"isa.investigation.xlsx","@context":{"sdo":"http://schema.org/","Investigation":"sdo:Dataset","identifier":"sdo:identifier","title":"sdo:headline","additionalType":"sdo:additionalType","description":"sdo:description","submissionDate":"sdo:dateCreated","publicReleaseDate":"sdo:datePublished","publications":"sdo:citation","people":"sdo:creator","studies":"sdo:hasPart","ontologySourceReferences":"sdo:mentions","comments":"sdo:comment","filename":"sdo:alternateName"}}"""
    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        ArcInvestigation.toROCrateJsonString
        ArcInvestigation.fromROCrateJsonString


let private test_core =
    let json = """{"Identifier":"My Inv","Title":"My Title","Description":"My Description","SubmissionDate":"My Submission Date","PublicReleaseDate":"My Release Date","OntologySourceReferences":[{"description":"Description","file":"path/to/file","name":"OSR Name"}],"Publications":[{"doi":"any-nice-doi-42"}],"Contacts":[{"firstName":"Kevin","lastName":"Frey"}],"Assays":[{"Identifier":"My Cool Assay","MeasurementType":{"annotationValue":"MT","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ByeBye","value":"Space"}]},"TechnologyType":{"annotationValue":"TT","termSource":"MS","termAccession":"MS:696969"},"TechnologyPlatform":{"annotationValue":"TP","termSource":"MS","termAccession":"MS:123456","comments":[{"name":"Hello","value":"Space"}]},"Tables":[{"name":"New Table","header":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"values":[[[0,0],{"celltype":"FreeText","values":["Input 1"]}],[[0,1],{"celltype":"FreeText","values":["Input 2"]}],[[1,0],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[1,1],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[2,0],{"celltype":"FreeText","values":["Output 1"]}],[[2,1],{"celltype":"FreeText","values":["Output 2"]}]]},{"name":"My Second Table"}],"Performers":[{"firstName":"Kevin","lastName":"Frey"}],"Comments":[{"name":"Hello","value":"World"}]},{"Identifier":"My Assay"}],"Studies":[{"Identifier":"My Study","Title":"My Title","Description":"My Description","SubmissionDate":"My Submission Date","PublicReleaseDate":"My Release Date","Publications":[{"doi":"any-nice-doi-42"}],"Contacts":[{"firstName":"Kevin","lastName":"Frey","phone":"023382093810"}],"StudyDesignDescriptors":[{},{}],"Tables":[{"name":"New Table","header":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"values":[[[0,0],{"celltype":"FreeText","values":["Input 1"]}],[[0,1],{"celltype":"FreeText","values":["Input 2"]}],[[1,0],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[1,1],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[2,0],{"celltype":"FreeText","values":["Output 1"]}],[[2,1],{"celltype":"FreeText","values":["Output 2"]}]]},{"name":"Table 2"}],"RegisteredAssayIdentifiers":["Assay 1","Assay 2"],"Comments":[{"name":"Hello","value":"World"}]},{"Identifier":"My Fancy Stundy"}],"RegisteredStudyIdentifiers":["Study"],"Comments":[{"name":"Hello","value":"World"}]}"""
    createBaseJsonTests
        "core"
        create_filled
        ArcInvestigation.toJsonString
        ArcInvestigation.fromJsonString

let private test_compressed =
    let json = """{"filename":"isa.investigation.xlsx","identifier":"My Inv","title":"My Title","description":"My Description","submissionDate":"My Submission Date","publicReleaseDate":"My Release Date","ontologySourceReferences":[{"description":"Description","file":"path/to/file","name":"OSR Name"}],"publications":[{"doi":"any-nice-doi-42"}],"people":[{"firstName":"Kevin","lastName":"Frey"}],"studies":[{"filename":"assays/My Study/isa.assay.xlsx","identifier":"My Study","title":"My Title","description":"My Description","submissionDate":"My Submission Date","publicReleaseDate":"My Release Date","publications":[{"doi":"any-nice-doi-42"}],"people":[{"firstName":"Kevin","lastName":"Frey","phone":"023382093810"}],"studyDesignDescriptors":[{},{}],"protocols":[{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]}],"materials":{"sources":[{"name":"Input 1"},{"name":"Input 2"}],"samples":[{"name":"Output 1"},{"name":"Output 2"}]},"processSequence":[{"name":"New Table","executesProtocol":{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]},"inputs":[{"name":"Input 1"},{"name":"Input 2"}],"outputs":[{"name":"Output 1"},{"name":"Output 2"}]},{"name":"Table 2"}],"assays":[{"filename":"assays/Assay 1/isa.assay.xlsx"},{"filename":"assays/Assay 2/isa.assay.xlsx"}],"comments":[{"name":"Hello","value":"World"}]},{"filename":"assays/My Fancy Stundy/isa.assay.xlsx","identifier":"My Fancy Stundy"}],"comments":[{"name":"Hello","value":"World"}]}"""
    createBaseJsonTests
        "compressed"
        create_filled
        ArcInvestigation.toCompressedJsonString
        ArcInvestigation.fromCompressedJsonString

let private test_isa =
    let json = """{"filename":"isa.investigation.xlsx","identifier":"My Inv","title":"My Title","description":"My Description","submissionDate":"My Submission Date","publicReleaseDate":"My Release Date","ontologySourceReferences":[{"description":"Description","file":"path/to/file","name":"OSR Name"}],"publications":[{"doi":"any-nice-doi-42"}],"people":[{"firstName":"Kevin","lastName":"Frey"}],"studies":[{"filename":"assays/My Study/isa.assay.xlsx","identifier":"My Study","title":"My Title","description":"My Description","submissionDate":"My Submission Date","publicReleaseDate":"My Release Date","publications":[{"doi":"any-nice-doi-42"}],"people":[{"firstName":"Kevin","lastName":"Frey","phone":"023382093810"}],"studyDesignDescriptors":[{},{}],"protocols":[{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]}],"materials":{"sources":[{"name":"Input 1"},{"name":"Input 2"}],"samples":[{"name":"Output 1"},{"name":"Output 2"}]},"processSequence":[{"name":"New Table","executesProtocol":{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]},"inputs":[{"name":"Input 1"},{"name":"Input 2"}],"outputs":[{"name":"Output 1"},{"name":"Output 2"}]},{"name":"Table 2"}],"assays":[{"filename":"assays/My Cool Assay/isa.assay.xlsx","measurementType":{"annotationValue":"MT","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ByeBye","value":"Space"}]},"technologyType":{"annotationValue":"TT","termSource":"MS","termAccession":"MS:696969"},"technologyPlatform":"TP (MS:123456)","materials":{"samples":[{"name":"Output 1"},{"name":"Output 2"}]},"processSequence":[{"name":"New Table","executesProtocol":{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]},"inputs":[{"name":"Input 1"},{"name":"Input 2"}],"outputs":[{"name":"Output 1"},{"name":"Output 2"}]},{"name":"My Second Table"}],"comments":[{"name":"Hello","value":"World"}]},{"filename":"assays/My Assay/isa.assay.xlsx"}],"comments":[{"name":"Hello","value":"World"}]},{"filename":"assays/My Fancy Stundy/isa.assay.xlsx","identifier":"My Fancy Stundy","assays":[{"filename":"assays/My Cool Assay/isa.assay.xlsx","measurementType":{"annotationValue":"MT","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ByeBye","value":"Space"}]},"technologyType":{"annotationValue":"TT","termSource":"MS","termAccession":"MS:696969"},"technologyPlatform":"TP (MS:123456)","materials":{"samples":[{"name":"Output 1"},{"name":"Output 2"}]},"processSequence":[{"name":"New Table","executesProtocol":{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]},"inputs":[{"name":"Input 1"},{"name":"Input 2"}],"outputs":[{"name":"Output 1"},{"name":"Output 2"}]},{"name":"My Second Table"}],"comments":[{"name":"Hello","value":"World"}]},{"filename":"assays/My Assay/isa.assay.xlsx"}]}],"comments":[{"name":"Hello","value":"World"}]}"""
    createBaseJsonTests
        "isa"
        create_filled
        ArcInvestigation.toISAJsonString
        ArcInvestigation.fromISAJsonString

let private test_roCrate =
    let json = """{"@id":"#study/My_Study","@type":["Study"],"additionalType":"Study","identifier":"My Study","filename":"studies/My Study/isa.study.xlsx","title":"My Title","description":"My Description","studyDesignDescriptors":[{"@id":"#DummyOntologyAnnotation","@type":"OntologyAnnotation","@context":{"sdo":"http://schema.org/","OntologyAnnotation":"sdo:DefinedTerm","annotationValue":"sdo:name","termSource":"sdo:inDefinedTermSet","termAccession":"sdo:termCode","comments":"sdo:disambiguatingDescription"}},{"@id":"#DummyOntologyAnnotation","@type":"OntologyAnnotation","@context":{"sdo":"http://schema.org/","OntologyAnnotation":"sdo:DefinedTerm","annotationValue":"sdo:name","termSource":"sdo:inDefinedTermSet","termAccession":"sdo:termCode","comments":"sdo:disambiguatingDescription"}}],"submissionDate":"My Submission Date","publicReleaseDate":"My Release Date","publications":[{"@id":"any-nice-doi-42","@type":"Publication","doi":"any-nice-doi-42","@context":{"sdo":"http://schema.org/","Publication":"sdo:ScholarlyArticle","pubMedID":"sdo:url","doi":"sdo:sameAs","title":"sdo:headline","status":"sdo:creativeWorkStatus","authorList":"sdo:author","comments":"sdo:disambiguatingDescription"}}],"people":[{"@id":"#Kevin_Frey","@type":"Person","firstName":"Kevin","lastName":"Frey","phone":"023382093810","@context":{"sdo":"http://schema.org/","Person":"sdo:Person","orcid":"sdo:identifier","firstName":"sdo:givenName","lastName":"sdo:familyName","midInitials":"sdo:additionalName","email":"sdo:email","address":"sdo:address","phone":"sdo:telephone","fax":"sdo:faxNumber","comments":"sdo:disambiguatingDescription","roles":"sdo:jobTitle","affiliation":"sdo:affiliation"}}],"processSequence":[{"@id":"#Process_New_Table","@type":["Process"],"name":"New Table","executesProtocol":{"@id":"#Protocol_My_Study_New_Table","@type":["Protocol"],"components":[{"@id":"ARCtrl.Json.PropertyValue+ROCrate+genID@16[ARCtrl.Process.Component]/{Name = instrument model; TSR = MS; TAN = MS:424242; Comments = seq [ARCtrl.Comment]}Some(Ontology {Name = SCIEX instrument model})","@type":"PropertyValue","additionalType":"Component","category":"instrument model","categoryCode":"MS:424242","value":"SCIEX instrument model","@context":{"sdo":"http://schema.org/","additionalType":"sdo:additionalType","category":"sdo:name","categoryCode":"sdo:propertyID","value":"sdo:value","valueCode":"sdo:valueReference","unit":"sdo:unitText","unitCode":"sdo:unitCode","comments":"sdo:disambiguatingDescription"}}],"@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Protocol":"bio:LabProtocol","name":"sdo:name","protocolType":"bio:intendedUse","description":"sdo:description","version":"sdo:version","components":"bio:labEquipment","reagents":"bio:reagent","computationalTools":"bio:computationalTool","uri":"sdo:url","comments":"sdo:comment"}},"inputs":[{"@id":"#Source_Input_1","@type":["Source"],"name":"Input 1","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Source":"bio:Sample","name":"sdo:name","characteristics":"bio:additionalProperty"}},{"@id":"#Source_Input_2","@type":["Source"],"name":"Input 2","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Source":"bio:Sample","name":"sdo:name","characteristics":"bio:additionalProperty"}}],"outputs":[{"@id":"#Sample_Output_1","@type":["Sample"],"name":"Output 1","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Sample":"bio:Sample","name":"sdo:name","additionalProperties":"bio:additionalProperty"}},{"@id":"#Sample_Output_2","@type":["Sample"],"name":"Output 2","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Sample":"bio:Sample","name":"sdo:name","additionalProperties":"bio:additionalProperty"}}],"@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Process":"bio:LabProcess","name":"sdo:name","executesProtocol":"bio:executesLabProtocol","parameterValues":"bio:parameterValue","performer":"sdo:agent","date":"sdo:endTime","inputs":"sdo:object","outputs":"sdo:result","comments":"sdo:disambiguatingDescription"}},{"@id":"#Process_Table_2","@type":["Process"],"name":"Table 2","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Process":"bio:LabProcess","name":"sdo:name","executesProtocol":"bio:executesLabProtocol","parameterValues":"bio:parameterValue","performer":"sdo:agent","date":"sdo:endTime","inputs":"sdo:object","outputs":"sdo:result","comments":"sdo:disambiguatingDescription"}}],"assays":[{"@id":"#assay/Assay_1","@type":["Assay"],"additionalType":"Assay","identifier":"Assay 1","filename":"assays/Assay 1/isa.assay.xlsx","@context":{"sdo":"http://schema.org/","Assay":"sdo:Dataset","identifier":"sdo:identifier","additionalType":"sdo:additionalType","measurementType":"sdo:variableMeasured","technologyType":"sdo:measurementTechnique","technologyPlatform":"sdo:measurementMethod","dataFiles":"sdo:hasPart","performers":"sdo:creator","processSequences":"sdo:about","comments":"sdo:comment","filename":"sdo:url"}},{"@id":"#assay/Assay_2","@type":["Assay"],"additionalType":"Assay","identifier":"Assay 2","filename":"assays/Assay 2/isa.assay.xlsx","@context":{"sdo":"http://schema.org/","Assay":"sdo:Dataset","identifier":"sdo:identifier","additionalType":"sdo:additionalType","measurementType":"sdo:variableMeasured","technologyType":"sdo:measurementTechnique","technologyPlatform":"sdo:measurementMethod","dataFiles":"sdo:hasPart","performers":"sdo:creator","processSequences":"sdo:about","comments":"sdo:comment","filename":"sdo:url"}}],"comments":[{"@id":"#Comment_Hello_World","@type":"Comment","name":"Hello","value":"World","@context":{"sdo":"http://schema.org/","Comment":"sdo:Comment","name":"sdo:name","value":"sdo:text"}}],"@context":{"sdo":"http://schema.org/","Study":"sdo:Dataset","identifier":"sdo:identifier","title":"sdo:headline","additionalType":"sdo:additionalType","description":"sdo:description","submissionDate":"sdo:dateCreated","publicReleaseDate":"sdo:datePublished","publications":"sdo:citation","people":"sdo:creator","assays":"sdo:hasPart","filename":"sdo:alternateName","comments":"sdo:comment","processSequence":"sdo:about","studyDesignDescriptors":"arc:ARC#ARC_00000037"}}"""
    createBaseJsonTests
        "ROCrate"
        create_filled
        ArcInvestigation.toROCrateJsonString
        ArcInvestigation.fromROCrateJsonString

let main = testList "Investigation" [
    test_coreEmpty
    test_core
    test_compressedEmpty
    test_compressed
    test_isaEmpty
    test_isa
    test_roCrate
]