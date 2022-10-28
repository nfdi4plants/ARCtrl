module JsonSchemaValidation

open Expecto
open FSharp.Data
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Newtonsoft.Json.Schema

module JSchema = 

    let validate (schemaBaseURL : string) (schemaURL : string) (objectString : string) : (bool * #seq<string>) = 
        let resolver = JSchemaUrlResolver()
        let settings = JSchemaReaderSettings(Resolver = resolver,BaseUri = System.Uri(schemaBaseURL))

        let schemaString = Http.RequestString schemaURL
        let schemaReader = new JsonTextReader(new System.IO.StringReader(schemaString))

        let schema = JSchema.Load(schemaReader,settings)
        let objectJson = JObject.Parse(objectString)

        objectJson.IsValid(schema)

module Expect =

    let mutable schemaBaseURL = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/"
    
    let matchingSchema (schemaURL : string) (objectString : string)=
        let isValid,msg = JSchema.validate schemaBaseURL schemaURL objectString
        Expect.isTrue isValid (sprintf "Json Object did not match Json Schema: %A" msg)

    let matchingAssay (assayString : string) =
        let assayUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/assay_schema.json"
        matchingSchema assayUrl assayString

    let matchingComment (commentString : string) =
        let commentUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/comment_schema.json"
        matchingSchema commentUrl commentString

    let matchingData (dataString : string) =
        let dataUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/data_schema.json"
        matchingSchema dataUrl dataString
    
    let matchingFactor (factorString : string) =
        let factorUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/factor_schema.json"
        matchingSchema factorUrl factorString

    let matchingFactorValue (factorValueString : string) =
        let factorValueUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/factor_value_schema.json"
        matchingSchema factorValueUrl factorValueString

    let matchingInvestigation (investigationString : string) =
        let investigationUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/investigation_schema.json"
        matchingSchema investigationUrl investigationString

    let matchingMaterialAttribute (materialAttributeString : string) =
        let materialAttributeUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/material_attribute_schema.json"
        matchingSchema materialAttributeUrl materialAttributeString

    let matchingMaterialAttributeValue (materialAttributeValueString : string) =
        let materialAttributeValueUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/material_attribute_value_schema.json"
        matchingSchema materialAttributeValueUrl materialAttributeValueString

    let matchingMaterial (materialString : string) =
        let materialUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/material_schema.json"
        matchingSchema materialUrl materialString

    let matchingOntologyAnnotation (ontologyAnnotationString : string) =
        let ontologyAnnotationUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/ontology_annotation_schema.json"
        matchingSchema ontologyAnnotationUrl ontologyAnnotationString

    let matchingOntologySourceReference (ontologySourceReferenceString : string) =
        let ontologySourceReferenceUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/ontology_source_reference_schema.json"
        matchingSchema ontologySourceReferenceUrl ontologySourceReferenceString
    
    let matchingPerson (personString : string) =
        let personUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/person_schema.json"
        matchingSchema personUrl personString

    let matchingProcessParameterValue (processParameterValueString : string) =
        let processParameterValueUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/process_parameter_value_schema.json"
        matchingSchema processParameterValueUrl processParameterValueString

    let matchingProcess (processString : string) =
        let processUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/process_schema.json"
        matchingSchema processUrl processString

    let matchingProtocolParameter (protocolParameterString : string) =
        let protocolParameterUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/protocol_parameter_schema.json"
        matchingSchema protocolParameterUrl protocolParameterString

    let matchingProtocol (protocolString : string) =
        let protocolUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/protocol_schema.json"
        matchingSchema protocolUrl protocolString

    let matchingPublication (publicationString : string) =
        let publicationUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/publication_schema.json"
        matchingSchema publicationUrl publicationString

    let matchingSample (sampleString : string) =
        let sampleUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/sample_schema.json"
        matchingSchema sampleUrl sampleString

    let matchingSource (sourceString : string) =
        let sourceUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/source_schema.json"
        matchingSchema sourceUrl sourceString

    let matchingStudy (studyString : string) =
        let studyUrl = "https://raw.githubusercontent.com/ISA-tools/isa-specs/master/source/_static/isajson/study_schema.json"
        matchingSchema studyUrl studyString
