module Tests.Assay

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_empty() = ArcAssay.create("My Assay")
    let create_filled() = 
        ArcAssay.create(
            "My Cool Assay",
            OntologyAnnotation("MT", "MS", "MS:424242", ResizeArray [Comment.create("ByeBye","Space")]), 
            OntologyAnnotation("TT", "MS", "MS:696969"), 
            OntologyAnnotation("TP", "MS", "MS:123456", ResizeArray [Comment.create("Hello","Space")]), 
            ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            ResizeArray [|Comment.create("Hello", "World")|]
        )

open Helper

let private test_coreEmpty =
    let json = """{"Identifier":"My Assay"}"""
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcAssay.toJsonString
        ArcAssay.fromJsonString

let private test_compressedEmpty =
    let json = """{"cellTable":[],"oaTable":[],"stringTable":[],"object":{"Identifier":"My Assay"}}"""
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcAssay.toCompressedJsonString
        ArcAssay.fromCompressedJsonString

let private test_isaEmpty =
    let json = """{"filename":"assays/My Assay/isa.assay.xlsx"}"""
    createBaseJsonTests
        "isa-empty"
        create_empty
        ArcAssay.toISAJsonString
        ArcAssay.fromISAJsonString

let private test_roCrateEmpty =
    let json = """{"@id":"#assay/My_Assay","@type":["Assay"],"additionalType":"Assay","identifier":"My Assay","filename":"assays/My Assay/isa.assay.xlsx","@context":{"sdo":"http://schema.org/","Assay":"sdo:Dataset","identifier":"sdo:identifier","additionalType":"sdo:additionalType","measurementType":"sdo:variableMeasured","technologyType":"sdo:measurementTechnique","technologyPlatform":"sdo:measurementMethod","dataFiles":"sdo:hasPart","performers":"sdo:creator","processSequences":"sdo:about","comments":"sdo:comment","filename":"sdo:url"}}"""
    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        (fun () -> ArcAssay.toROCrateJsonString None)
        ArcAssay.fromROCrateJsonString

let private test_core =
    let json = """{"Identifier":"My Cool Assay","MeasurementType":{"annotationValue":"MT","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ByeBye","value":"Space"}]},"TechnologyType":{"annotationValue":"TT","termSource":"MS","termAccession":"MS:696969"},"TechnologyPlatform":{"annotationValue":"TP","termSource":"MS","termAccession":"MS:123456","comments":[{"name":"Hello","value":"Space"}]},"Tables":[{"name":"New Table","header":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"values":[[[0,0],{"celltype":"FreeText","values":["Input 1"]}],[[0,1],{"celltype":"FreeText","values":["Input 2"]}],[[1,0],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[1,1],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[2,0],{"celltype":"FreeText","values":["Output 1"]}],[[2,1],{"celltype":"FreeText","values":["Output 2"]}]]},{"name":"My Second Table"}],"Performers":[{"firstName":"Kevin","lastName":"Frey"}],"Comments":[{"name":"Hello","value":"World"}]}"""
    createBaseJsonTests
        "core"
        create_filled
        ArcAssay.toJsonString
        ArcAssay.fromJsonString

let private test_compressed =
    let json = """{"cellTable":[{"t":3,"v":[2]},{"t":3,"v":[4]},{"t":5,"v":[0]},{"t":3,"v":[6]},{"t":3,"v":[7]}],"oaTable":[{"a":8}],"stringTable":["New Table","My Second Table","Input 1","FreeText","Input 2","Term","Output 1","Output 2","SCIEX instrument model"],"object":{"Identifier":"My Cool Assay","MeasurementType":{"annotationValue":"MT","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ByeBye","value":"Space"}]},"TechnologyType":{"annotationValue":"TT","termSource":"MS","termAccession":"MS:696969"},"TechnologyPlatform":{"annotationValue":"TP","termSource":"MS","termAccession":"MS:123456","comments":[{"name":"Hello","value":"Space"}]},"Tables":[{"n":0,"h":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"c":[[0,1],[2,2],[3,4]]},{"n":1}],"Performers":[{"firstName":"Kevin","lastName":"Frey"}],"Comments":[{"name":"Hello","value":"World"}]}}"""
    createBaseJsonTests
        "compressed"
        create_filled
        ArcAssay.toCompressedJsonString
        ArcAssay.fromCompressedJsonString

let private test_isa =
    let json = """{"filename":"assays/My Cool Assay/isa.assay.xlsx","measurementType":{"annotationValue":"MT","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ByeBye","value":"Space"}]},"technologyType":{"annotationValue":"TT","termSource":"MS","termAccession":"MS:696969"},"technologyPlatform":"TP (MS:123456)","materials":{"samples":[{"name":"Output 1"},{"name":"Output 2"}]},"processSequence":[{"name":"New Table","executesProtocol":{"components":[{"componentName":"SCIEX instrument model ()","componentType":{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242","comments":[{"name":"ColumnIndex","value":"0"}]}}]},"inputs":[{"name":"Input 1"},{"name":"Input 2"}],"outputs":[{"name":"Output 1"},{"name":"Output 2"}]},{"name":"My Second Table"}],"comments":[{"name":"Hello","value":"World"}]}"""
    createBaseJsonTests
        "isa"
        (fun () -> 
            let a = create_filled() 
            a.Performers <- ResizeArray() // ISA-JSON Assay does not contain persons
            a)
        ArcAssay.toISAJsonString
        ArcAssay.fromISAJsonString

let private test_roCrate =
    let json = """{"@id":"#assay/My_Cool_Assay","@type":["Assay"],"additionalType":"Assay","identifier":"My Cool Assay","filename":"assays/My Cool Assay/isa.assay.xlsx","measurementType":{"@id":"MS:424242","@type":"PropertyValue","category":"MT","categoryCode":"MS:424242","comments":["{\"@id\":\"#Comment_ByeBye_Space\",\"@type\":\"Comment\",\"name\":\"ByeBye\",\"value\":\"Space\",\"@context\":{\"sdo\":\"http://schema.org/\",\"Comment\":\"sdo:Comment\",\"name\":\"sdo:name\",\"value\":\"sdo:text\"}}"],"@context":{"sdo":"http://schema.org/","additionalType":"sdo:additionalType","category":"sdo:name","categoryCode":"sdo:propertyID","value":"sdo:value","valueCode":"sdo:valueReference","unit":"sdo:unitText","unitCode":"sdo:unitCode","comments":"sdo:disambiguatingDescription"}},"technologyType":{"@id":"MS:696969","@type":"OntologyAnnotation","annotationValue":"TT","termSource":"MS","termAccession":"MS:696969","@context":{"sdo":"http://schema.org/","OntologyAnnotation":"sdo:DefinedTerm","annotationValue":"sdo:name","termSource":"sdo:inDefinedTermSet","termAccession":"sdo:termCode","comments":"sdo:disambiguatingDescription"}},"technologyPlatform":{"@id":"MS:123456","@type":"OntologyAnnotation","annotationValue":"TP","termSource":"MS","termAccession":"MS:123456","comments":["{\"@id\":\"#Comment_Hello_Space\",\"@type\":\"Comment\",\"name\":\"Hello\",\"value\":\"Space\",\"@context\":{\"sdo\":\"http://schema.org/\",\"Comment\":\"sdo:Comment\",\"name\":\"sdo:name\",\"value\":\"sdo:text\"}}"],"@context":{"sdo":"http://schema.org/","OntologyAnnotation":"sdo:DefinedTerm","annotationValue":"sdo:name","termSource":"sdo:inDefinedTermSet","termAccession":"sdo:termCode","comments":"sdo:disambiguatingDescription"}},"performers":[{"@id":"#Kevin_Frey","@type":"Person","firstName":"Kevin","lastName":"Frey","@context":{"sdo":"http://schema.org/","Person":"sdo:Person","orcid":"sdo:identifier","firstName":"sdo:givenName","lastName":"sdo:familyName","midInitials":"sdo:additionalName","email":"sdo:email","address":"sdo:address","phone":"sdo:telephone","fax":"sdo:faxNumber","comments":"sdo:disambiguatingDescription","roles":"sdo:jobTitle","affiliation":"sdo:affiliation"}}],"processSequence":[{"@id":"#Process_New_Table","@type":["Process"],"name":"New Table","executesProtocol":{"@id":"#EmptyProtocol","@type":["Protocol"],"components":[{"@id":"ARCtrl.Json.PropertyValue+ROCrate+genID@16[ARCtrl.Process.Component]/{Name = instrument model; TSR = MS; TAN = MS:424242; Comments = seq [ARCtrl.Comment]}Some(Ontology {Name = SCIEX instrument model})","@type":"PropertyValue","additionalType":"Component","category":"instrument model","categoryCode":"MS:424242","value":"SCIEX instrument model","@context":{"sdo":"http://schema.org/","additionalType":"sdo:additionalType","category":"sdo:name","categoryCode":"sdo:propertyID","value":"sdo:value","valueCode":"sdo:valueReference","unit":"sdo:unitText","unitCode":"sdo:unitCode","comments":"sdo:disambiguatingDescription"}}],"@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Protocol":"bio:LabProtocol","name":"sdo:name","protocolType":"bio:intendedUse","description":"sdo:description","version":"sdo:version","components":"bio:labEquipment","reagents":"bio:reagent","computationalTools":"bio:computationalTool","uri":"sdo:url","comments":"sdo:comment"}},"inputs":[{"@id":"#Source_Input_1","@type":["Source"],"name":"Input 1","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Source":"bio:Sample","name":"sdo:name","characteristics":"bio:additionalProperty"}},{"@id":"#Source_Input_2","@type":["Source"],"name":"Input 2","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Source":"bio:Sample","name":"sdo:name","characteristics":"bio:additionalProperty"}}],"outputs":[{"@id":"#Sample_Output_1","@type":["Sample"],"name":"Output 1","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Sample":"bio:Sample","name":"sdo:name","additionalProperties":"bio:additionalProperty"}},{"@id":"#Sample_Output_2","@type":["Sample"],"name":"Output 2","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Sample":"bio:Sample","name":"sdo:name","additionalProperties":"bio:additionalProperty"}}],"@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Process":"bio:LabProcess","name":"sdo:name","executesProtocol":"bio:executesLabProtocol","parameterValues":"bio:parameterValue","performer":"sdo:agent","date":"sdo:endTime","inputs":"sdo:object","outputs":"sdo:result","comments":"sdo:disambiguatingDescription"}},{"@id":"#Process_My_Second_Table","@type":["Process"],"name":"My Second Table","@context":{"sdo":"http://schema.org/","bio":"https://bioschemas.org/","Process":"bio:LabProcess","name":"sdo:name","executesProtocol":"bio:executesLabProtocol","parameterValues":"bio:parameterValue","performer":"sdo:agent","date":"sdo:endTime","inputs":"sdo:object","outputs":"sdo:result","comments":"sdo:disambiguatingDescription"}}],"comments":[{"@id":"#Comment_Hello_World","@type":"Comment","name":"Hello","value":"World","@context":{"sdo":"http://schema.org/","Comment":"sdo:Comment","name":"sdo:name","value":"sdo:text"}}],"@context":{"sdo":"http://schema.org/","Assay":"sdo:Dataset","identifier":"sdo:identifier","additionalType":"sdo:additionalType","measurementType":"sdo:variableMeasured","technologyType":"sdo:measurementTechnique","technologyPlatform":"sdo:measurementMethod","dataFiles":"sdo:hasPart","performers":"sdo:creator","processSequences":"sdo:about","comments":"sdo:comment","filename":"sdo:url"}}"""
    createBaseJsonTests
        "ROCrate"
        create_filled
        (fun () -> ArcAssay.toROCrateJsonString None)
        ArcAssay.fromROCrateJsonString


let main = testList "Assay" [
    test_coreEmpty
    test_compressedEmpty
    test_isaEmpty
    test_roCrateEmpty
    test_core
    test_compressed
    test_isa
    test_roCrate
]