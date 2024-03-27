module Tests.ROCrate


open ARCtrl
open ARCtrl.Json
open ARCtrl.Process

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
        Comment.make (Some "Key") (Some "Value")

    let publicationStatus = 
        OntologyAnnotation.make 
            (Some "published")
            (Some "pso")
            (Some "http://purl.org/spar/pso/published")
            (ResizeArray [|comment|])

    let publication =
        Publication.make
            (Some "12345678")
            (Some "11.1111/abcdef123456789")
            (Some "Lukas Weil, Other Gzuy") // (Some "Lukas Weil, Other Gzúy")
            (Some "Fair is great")
            (Some publicationStatus)
            (ResizeArray [|comment|])

    let role = 
        OntologyAnnotation.make 
            (Some "software developer role")
            (Some "swo")
            (Some "http://www.ebi.ac.uk/swo/SWO_0000392")
            (ResizeArray [|comment|])

    let person =
        Person.make
            None
            (Some "Weil")
            (Some "Lukas")
            (Some "H")
            (Some "weil@email.com")
            (Some "0123 456789")
            (Some "9876 543210")
            (Some "fantasyStreet 23, 123 Town")
            (Some "Universiteee")
            (ResizeArray [|role|])
            (ResizeArray [|comment|])

    let characteristic = 
        MaterialAttribute.make 
            (Some "Characteristic/Organism")
            (Some (
                OntologyAnnotation.make
                    (Some "organism")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                    (ResizeArray [|comment|])
            ))

    let characteristicValue = 
        MaterialAttributeValue.make 
            (Some "CharacteristicValue/Arabidopsis")
            (Some characteristic)
            (Some (
                OntologyAnnotation.make
                    (Some "Arabidopsis thaliana")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                    (ResizeArray [|comment|])
                |> Value.Ontology
            ))
            None

    let studyDesignDescriptor = 
        OntologyAnnotation.make
            (Some "Time Series Analysis")
            (Some "ncit")
            (Some "http://purl.obolibrary.org/obo/NCIT_C18235")               
            (ResizeArray [|comment|])

    let protocolType = 
        OntologyAnnotation.make
            (Some "growth protocol")
            (Some "dfbo")
            (Some "http://purl.obolibrary.org/obo/DFBO_1000162")
            (ResizeArray [|comment|])

    let parameter = 
        ProtocolParameter.make
            None
            (Some (
                OntologyAnnotation.make
                    (Some "temperature unit")
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000005")
                    (ResizeArray [|comment|])
            ))

    let parameterUnit =              
        OntologyAnnotation.make
            (Some "degree celsius")
            (Some "uo")
            (Some "http://purl.obolibrary.org/obo/UO_0000027")
            (ResizeArray [|comment|])

    let parameterValue = 
        ProcessParameterValue.make
            (Some parameter)
            (Some (Value.Int 20))
            (Some parameterUnit)

    let protocolComponent =
        Component.make
            (Some (
                OntologyAnnotation.make
                    (Some "real-time PCR machine")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0001110")
                    (ResizeArray [|comment|])
                |> Value.Ontology
            ))
            None
            (Some (
                OntologyAnnotation.make
                    (Some "PCR instrument")
                    (Some "obi")
                    (Some "http://purl.obolibrary.org/obo/OBI_0000989")
                    (ResizeArray [|comment|])
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
            (Some "Time")
            (Some (
                OntologyAnnotation.make
                    (Some "time")
                    (Some "pato")
                    (Some "http://purl.obolibrary.org/obo/PATO_0000165")
                    (ResizeArray [|comment|])
            ))
            (ResizeArray [|comment|] |> Some)

    let factorUnit = 
        OntologyAnnotation.make
            (Some "hour")
            (Some "uo")
            (Some "http://purl.obolibrary.org/obo/UO_0000032")
            (ResizeArray [|comment|])
        

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

    //let studyMaterials = 
    //    StudyMaterials
    //        (Some [source])
    //        (Some [sample])
    //        (Some [material;derivedMaterial])

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
        (ResizeArray [|comment;|])

    let measurementType = 
        OntologyAnnotation.make
            (Some "LC/MS Label-Free Quantification")
            (Some "ncit")
            (Some "http://purl.obolibrary.org/obo/NCIT_C161813")
            (ResizeArray [|comment|])

    let technologyType = 
        OntologyAnnotation.make
            (Some "Time-of-Flight")
            (Some "ncit")
            (Some "http://purl.obolibrary.org/obo/NCIT_C70698")
            (ResizeArray [|comment|])

    let technologyPlatform = 
        OntologyAnnotation.make
            (Some "SCIEX instrument model")
            (Some "MS")
            (Some "http://purl.obolibrary.org/obo/MS_1000121")
            (ResizeArray [|comment|])

    //let assayMaterials =
    //    AssayMaterials.make
    //        (Some [sample])
    //        (Some [material;derivedMaterial])

    open ARCtrl.Process.Conversion

    let remark = Remark.make 0 "hallo"

    let assayIdentifier = "My Cool Assay"
    let studyIdentifier = "My Awesome Study"

    let assay = 
        ArcAssay.make
            assayIdentifier
            (Some measurementType)
            (Some technologyType)
            (Some technologyPlatform)
            (ArcTables.fromProcesses >> (fun a -> a.Tables) <| [assayProcess])
            (ResizeArray [person])
            (ResizeArray [comment])

    let study = 
        ArcStudy.make 
            studyIdentifier
            (Some "My Awesome Study Title")
            (Some "Study descriptem ipsum dolor set met tantium.")
            (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
            (Some (JsonExtensions.Date.fromInts 2020 10 20))
            (ResizeArray [publication])
            (ResizeArray [person])
            (ResizeArray [studyDesignDescriptor])
            (ArcTables.fromProcesses >> (fun a -> a.Tables) <| [studyProcess])
            (ResizeArray [assayIdentifier])
            (ResizeArray [comment])

    let investigation = 
        ArcInvestigation.make
            "My Fancy Investigation"
            //by chatgpt
            (Some "Unveiling the Enigmatic: A Deep Dive into the Intricacies of the ISA Paradigm")
            //by chatgpt
            (Some "Cryptic networks of influence, clandestine operations, and the delicate balance between security and privacy form the tapestry of the modern Intelligence, Surveillance, and Analysis (ISA) landscape. This investigation delves into the multifaceted world of ISA, navigating through its intricate web of technologies, methodologies, and ethical dilemmas. From the shadows of espionage to the forefront of data analytics, this exploration unveils the hidden mechanisms driving contemporary intelligence operations. Through a comprehensive analysis, we seek to illuminate the complexities, challenges, and implications inherent in the ISA paradigm, shedding light on its pivotal role in shaping global security and governance")
            (Some (JsonExtensions.DateTime.fromInts 2020 3 15 18 23))
            (Some (JsonExtensions.Date.fromInts 2020 4 3))                   
            (ResizeArray [ontologySourceReference])
            (ResizeArray [publication])
            (ResizeArray [person])
            (ResizeArray [assay])
            (ResizeArray [study])
            (ResizeArray [studyIdentifier])
            (ResizeArray [comment])
            (ResizeArray [remark])


open Objects
open Fable.Pyxpecto

[<RequireQualifiedAccess>]
module private JsonResults =

    let x = 0

open TestingUtils

let private tests_DisambiguatingDescription = testList "DisambiguatingDescription" [
    ftestCase "Full" <| fun _ ->
        let comment = Comment.create(name="My, cool  comment wiht = lots; of special <> chars", value="STARTING VALUE")
        let textString = Comment.ROCrate.encoderDisambiguatingDescription comment |> Encode.toJsonString 0
        let actual = Decode.fromJsonString Comment.ROCrate.decoderDisambiguatingDescription textString
        Expect.equal actual comment ""
    ftestCase "None" <| fun _ ->
        let comment = Comment.create()
        let textString = Comment.ROCrate.encoderDisambiguatingDescription comment |> Encode.toJsonString 0
        let actual = Decode.fromJsonString Comment.ROCrate.decoderDisambiguatingDescription textString
        Expect.equal actual comment ""
]

open ARCtrl.Json

let private tests_investigation = testList "Investigation" [
    ftestCase "Investigation" <| fun _ ->
        let s = ArcInvestigation.toROCrateJsonString () investigation
        let p = @"C:\Users\Kevin\source\repos\ARCtrl\ro-crate-test.json"
        System.IO.File.WriteAllText(p, s)
        Expect.pass()
]

let Main = testList "ROCrate" [
    tests_DisambiguatingDescription
    tests_investigation
]