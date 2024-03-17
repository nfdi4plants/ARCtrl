module Tests.ROCrate


open ARCtrl.ISA
open ARCtrl.ISA.Json

module private JsonExtensions =

    let private f2 i = 
        if i < 10 then sprintf "0%i" i
        else sprintf "%i" i 

    type System.DateTime with
        member this.ToJsonTimeString() = 
            $"{f2 this.Hour}:{f2 this.Minute}:{f2 this.Second}.{this.Millisecond}"

        member this.ToJsonDateString() = 
            $"{this.Year}-{f2 this.Month}-{f2 this.Day}"
        
        member this.ToJsonDateTimeString() = 
            $"{this.ToJsonDateString()}T{this.ToJsonTimeString()}Z"

    module Time =
    
        let fromInts hour minute = 
            let d = System.DateTime(1,1,1,hour,minute,0)
            d.ToJsonTimeString()

    module Date =
    
        let fromInts year month day = 
            let d = System.DateTime(year,month,day)
            d.ToJsonDateString()
      
    module DateTime =
    
        let fromInts year month day hour minute = 
            let d = System.DateTime(year,month,day,hour,minute,0)
            d.ToJsonDateTimeString()

module private Objects =
    

    let comment = 
        Comment.make (Some "MyComment") (Some "Key") (Some "Value")

    let publicationStatus = 
        OntologyAnnotation.make 
            None
            (Some "published")
            (Some "pso")
            (Some "http://purl.org/spar/pso/published")
            (Some [|comment|])

    let publication =
        Publication.make
            (Some "12345678")
            (Some "11.1111/abcdef123456789")
            (Some "Lukas Weil, Other Gzuy") // (Some "Lukas Weil, Other Gzúy")
            (Some "Fair is great")
            (Some publicationStatus)
            (Some [|comment|])

    let role = 
        OntologyAnnotation.make 
            None
            (Some "software developer role")
            (Some "swo")
            (Some "http://www.ebi.ac.uk/swo/SWO_0000392")
            (Some [|comment|])

    let person =
        Person.make
            None
            None
            (Some "Weil")
            (Some "Lukas")
            (Some "H")
            (Some "weil@email.com")
            (Some "0123 456789")
            (Some "9876 543210")
            (Some "fantasyStreet 23, 123 Town")
            (Some "Universiteee")
            (Some [|role|])
            (Some [|comment|])

    let characteristic = 
        MaterialAttribute.make 
            (Some "Characteristic/Organism")
            (Some (
                OntologyAnnotation.make
                    None
                    (Some "organism")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                    (Some [|comment|])
            ))

    let characteristicValue = 
        MaterialAttributeValue.make 
            (Some "CharacteristicValue/Arabidopsis")
            (Some characteristic)
            (Some (
                OntologyAnnotation.make
                    None
                    (Some "Arabidopsis thaliana")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                    (Some [|comment|])
                |> Value.Ontology
            ))
            None

    let studyDesignDescriptor = 
        OntologyAnnotation.make
            None
            (Some "Time Series Analysis")
            (Some "ncit")
            (Some "http://purl.obolibrary.org/obo/NCIT_C18235")               
            (Some [|comment|])

    let protocolType = 
        OntologyAnnotation.make
            None
            (Some "growth protocol")
            (Some "dfbo")
            (Some "http://purl.obolibrary.org/obo/DFBO_1000162")
            (Some [|comment|])

    let parameter = 
        ProtocolParameter.make
            None
            (Some (
                OntologyAnnotation.make
                    None
                    (Some "temperature unit")
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000005")
                    (Some [|comment|])
            ))

    let parameterUnit =              
        OntologyAnnotation.make
            None
            (Some "degree celsius")
            (Some "uo")
            (Some "http://purl.obolibrary.org/obo/UO_0000027")
            (Some [|comment|])

    let parameterValue = 
        ProcessParameterValue.make
            (Some parameter)
            (Some (Value.Int 20))
            (Some parameterUnit)

    let protocolComponent =
        Component.make
            (Some "PCR instrument")
            (Some (
                OntologyAnnotation.make
                    None
                    (Some "real-time PCR machine")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0001110")
                    (Some [|comment|])
                |> Value.Ontology
            ))
            None
            (Some (
                OntologyAnnotation.make
                    None
                    (Some "PCR instrument")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0000989")
                    (Some [|comment|])
            ))
    
    let protocol = 
        Protocol.make 
            None
            (Some "MyProtocol")
            (Some protocolType)
            (Some "bla bla bla\nblabbbbblaaa")
            (Some "http://nfdi4plants.org/protocols/MyProtocol")
            (Some "1.2.3")
            (Some [parameter])
            (Some [protocolComponent])                   
            (Some [comment])

    let factor = 
        Factor.make 
                None
                (Some "Time")
                (Some (
                    OntologyAnnotation.make
                        (Some "OntologyTerm/Time")
                        (Some "time")
                        (Some "pato")
                        (Some "http://purl.obolibrary.org/obo/PATO_0000165")
                        (Some [|comment|])
                ))
                (Some [|comment|])

    let factorUnit = 
        OntologyAnnotation.make
            None
            (Some "hour")
            (Some "uo")
            (Some "http://purl.obolibrary.org/obo/UO_0000032")
            (Some [|comment|])
        

    let factorValue = 
        FactorValue.make
            None
            (Some factor)
            (Some (Value.Float 4.5))
            (Some factorUnit)

    let source =
        Source.make
            None
            (Some "MySource")
            (Some [characteristicValue])

    let sample = 
        Sample.make
            None
            (Some "MySample")
            (Some [characteristicValue])
            (Some [factorValue])
            (Some [source])

    let data = 
        Data.make
            None
            (Some "MyData")
            (Some DataFile.DerivedDataFile)
            (Some [comment])

    let material = 
        Material.make
            None
            (Some "MyMaterial")
            (Some MaterialType.ExtractName)
            (Some [characteristicValue])
            None

    let derivedMaterial = 
        Material.make
            None
            (Some "MyDerivedMaterial")
            (Some MaterialType.LabeledExtractName)
            (Some [characteristicValue])
            (Some [material])

    let studyMaterials = 
        StudyMaterials.make
            (Some [source])
            (Some [sample])
            (Some [material;derivedMaterial])

    let studyProcess = 
        Process.make
            None
            (Some "MyProcess1")
            (Some protocol)
            (Some [parameterValue])
            (Some "Lukas While")
            (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
            None
            (Some (Process.create (Id = "Process/MyProcess2")))
            (Some [ProcessInput.Source source])
            (Some [ProcessOutput.Sample sample])
            (Some [comment])

    let assayProcess =
        Process.make
            None
            (Some "MyProcess2")
            (Some protocol)
            (Some [parameterValue])
            (Some "Lukas While")
            (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
            (Some (Process.create (Id = "Process/MyProcess1")))
            None
            (Some [ProcessInput.Sample sample])
            (Some [ProcessOutput.Data data])
            (Some [comment])

    let ontologySourceReference =
      OntologySourceReference.make
        (Some "This is the most important OSR.")
        (Some "https://raw.githubusercontent.com/nfdi4plants/nfdi4plants_ontology/main/dpbo.obo")
        (Some "DPBO")
        (Some "2024-02-20")
        (Some [|comment;|])

    let measurementType = 
        OntologyAnnotation.make
            None
            (Some "LC/MS Label-Free Quantification")
            (Some "ncit")
            (Some "http://purl.obolibrary.org/obo/NCIT_C161813")
            (Some [|comment|])

    let technologyType = 
        OntologyAnnotation.make
            None
            (Some "Time-of-Flight")
            (Some "ncit")
            (Some "http://purl.obolibrary.org/obo/NCIT_C70698")
            (Some [|comment|])

    let assayMaterials =
        AssayMaterials.make
            (Some [sample])
            (Some [material;derivedMaterial])

    let assay = 
        Assay.make
            None
            (Some "MyAssay/isa.assay.xlsx")
            (Some measurementType)
            (Some technologyType)
            (Some "Mass spectrometry platform")
            (Some [data])
            (Some assayMaterials)                   
            (Some [characteristic])
            (Some [parameterUnit;factorUnit])
            (Some [assayProcess])
            (Some [comment])

    let study = 
        Study.make 
            None
            (Some "MyStudy/isa.study.xlsx")
            (Some "MyStudy")
            (Some "bla bla bla")
            (Some "bla bla bla\nblabbbbblaaa")
            (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
            (Some (JsonExtensions.Date.fromInts 2020 10 20))                   
            (Some [publication])
            (Some [person])
            (Some [studyDesignDescriptor])
            (Some [protocol])
            (Some studyMaterials)
            (Some [studyProcess])
            (Some [assay])
            (Some [factor])
            (Some [characteristic])
            (Some [parameterUnit;factorUnit])
            (Some [comment])

    let investigation = 
        Investigation.make 
            None
            (Some "isa.investigation.xlsx")
            (Some "MyInvestigation")
            (Some "bla bla bla")
            (Some "bla bla bla\nblabbbbblaaa")
            (Some (JsonExtensions.DateTime.fromInts 2020 3 15 18 23))
            (Some (JsonExtensions.Date.fromInts 2020 4 3))                   
            (Some [ontologySourceReference])
            (Some [publication])
            (Some [person])
            (Some [study])
            (Some [comment])
            ([Remark.make 0 "hallo"])

open Objects
open Fable.Pyxpecto

[<RequireQualifiedAccess>]
module private JsonResults =

    let x = 0

open TestingUtils

let private tests_DisambiguatingDescription = testList "DisambiguatingDescription" [
    ftestCase "Full" <| fun _ ->
        let comment = Comment.create(Name="My, cool  comment wiht = lots; of special <> chars", Value="STARTING VALUE")
        let textString = Comment.encoderDisambiguatingDescription comment |> GEncode.toJsonString 0
        let actual = GDecode.fromJsonString Comment.decoderDisambiguatingDescription textString
        Expect.equal actual comment ""
    ftestCase "None" <| fun _ ->
        let comment = Comment.create()
        let textString = Comment.encoderDisambiguatingDescription comment |> GEncode.toJsonString 0
        let actual = GDecode.fromJsonString Comment.decoderDisambiguatingDescription textString
        Expect.equal actual comment ""
]

let private tests_investigation = testList "Investigation" [
    ftestCase "Investigation" <| fun _ ->
        let s = Investigation.toRoCrateString investigation
        let p = @"C:\Users\Kevin\source\repos\ARCtrl\ro-crate-test.json"
        System.IO.File.WriteAllText(p, s)
        Expect.pass()
]

let Main = testList "ROCrate" [
    tests_DisambiguatingDescription
    tests_investigation
]