module ISAJsonTests


open ISADotNet
open ISADotNet.Json


#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

#endif

open TestingUtils
open JsonSchemaValidation

module JsonExtensions =


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

let testProcessInput =

    testList "ProcessInputTests" [
        testCase "ReadMaterial" (fun () -> 
            
            let s = 
                """
                {
                "@id": "#material/extract-C-0.07-aliquot10",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot10",
                "type": "Extract Name"
                }
                """

            let result = ProcessInput.fromString s

            let expected = 
                Material.create("#material/extract-C-0.07-aliquot10","extract-C-0.07-aliquot10",MaterialType.ExtractName,Characteristics = [])
                |> ProcessInput.Material
            Expect.equal result expected ""

        )

    ]

let testProtocolFile =

    testList "ProtocolJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Protocol.fromString TestFiles.Protocol.protocol |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Protocol.fromString TestFiles.Protocol.protocol

            let writingSuccess = 
                try 
                    Protocol.toString p |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let p = Protocol.fromString TestFiles.Protocol.protocol

            let s = Protocol.toString p

            Expect.matchingProtocol s
        )

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Protocol.fromString TestFiles.Protocol.protocol
                |> Protocol.toString

            let i = 
                TestFiles.Protocol.protocol
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.equal o i "Written protocol file does not match read protocol file"
        )
        |> testSequenced
    ]

let testProcessFile =

    testList "ProcessJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Process.fromString TestFiles.Process.process' |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Process.fromString TestFiles.Process.process'

            let writingSuccess = 
                try 
                    Process.toString p |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let p = Process.fromString TestFiles.Process.process'

            let s = Process.toString p

            Expect.matchingProcess s
        )

        testCase "OutputMatchesInput" (fun () ->

            let o =
                Process.fromString TestFiles.Process.process'
                |> Process.toString

            let i = 
                TestFiles.Process.process'
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            mySequenceEqual o i "Written process file does not match read process file"
        )
        |> testSequenced
    ]

let testPersonFile =

    testList "PersonJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Person.fromString TestFiles.Person.person |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Person.fromString TestFiles.Person.person

            let writingSuccess = 
                try 
                    Person.toString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let a = Person.fromString TestFiles.Person.person

            let s = Person.toString a

            Expect.matchingPerson s
        )

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Person.fromString TestFiles.Person.person
                |> Person.toString

            let i = 
                TestFiles.Person.person
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            mySequenceEqual o i "Written person file does not match read person file"
        )
        |> testSequenced
    ]

let testPublicationFile =

    testList "PublicationJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Publication.fromString TestFiles.Publication.publication |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Publication.fromString TestFiles.Publication.publication

            let writingSuccess = 
                try 
                    Publication.toString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let a = Publication.fromString TestFiles.Publication.publication

            let s = Publication.toString a

            Expect.matchingPublication s
        )

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Publication.fromString TestFiles.Publication.publication
                |> Publication.toString

            let i = 
                TestFiles.Publication.publication
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            mySequenceEqual o i "Written Publication file does not match read publication file"
        )
        |> testSequenced
    ]

let testAssayFile =

    testList "AssayJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Assay.fromString TestFiles.Assay.assay |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Assay.fromString TestFiles.Assay.assay

            let writingSuccess = 
                try 
                    Assay.toString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let a = Assay.fromString TestFiles.Assay.assay

            let s = Assay.toString a

            Expect.matchingAssay s
        )

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Assay.fromString TestFiles.Assay.assay
                |> Assay.toString

            let i = 
                TestFiles.Assay.assay
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            mySequenceEqual o i "Written assay file does not match read assay file"
        )
        |> testSequenced
    ]

let testInvestigationFile = 

    testList "InvestigationJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Investigation.fromString TestFiles.Investigation.investigation |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccess" (fun () ->

            let i = Investigation.fromString TestFiles.Investigation.investigation

            let writingSuccess = 
                try 
                    Investigation.toString i |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let i = Investigation.fromString TestFiles.Investigation.investigation

            let s = Investigation.toString i

            Expect.matchingInvestigation s
        )

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Investigation.fromString TestFiles.Investigation.investigation
                |> Investigation.toString

            let i = 
                TestFiles.Investigation.investigation
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            mySequenceEqual o i "Written investigation file does not match read investigation file"
        )
        |> testSequenced

        testCase "HandleEmptyRemarks" (fun () ->

            let json = "{}"
            
            let i = Investigation.fromString json

            Expect.equal i.Remarks List.empty "Remark list should be an empty list."
        )
        |> testSequenced

        testCase "FullInvestigation" (fun () ->
                  
            let comment = 
                Comment.make (Some "MyComment") (Some "Key") (Some "Value")

            let ontologySourceReference =
                OntologySourceReference.make
                    (Some "bla bla")
                    (Some "filePath.txt")
                    (Some "OO")
                    (Some "1.3.3")
                    (Some [comment])

            let publicationStatus = 
                OntologyAnnotation.make 
                    (Some "OntologyTerm/Published")
                    (Some (AnnotationValue.Text "published"))
                    (Some "pso")
                    (Some "http://purl.org/spar/pso/published")
                    (Some [comment])

            let publication =
                Publication.make
                    (Some "12345678")
                    (Some "11.1111/abcdef123456789")
                    (Some "Lukas Weil, Other Gzúy")
                    (Some "Fair is great")
                    (Some publicationStatus)
                    (Some [comment])

            let role = 
                OntologyAnnotation.make 
                    (Some "OntologyTerm/SoftwareDeveloperRole")
                    (Some (AnnotationValue.Text "software developer role"))
                    (Some "swo")
                    (Some "http://www.ebi.ac.uk/swo/SWO_0000392")
                    (Some [comment])

            let person =
                Person.make
                    (Some "Persons/LukasWeil")
                    (Some "Weil")
                    (Some "Lukas")
                    (Some "H")
                    (Some "weil@email.com")
                    (Some "0123 456789")
                    (Some "9876 543210")
                    (Some "fantasyStreet 23, 123 Town")
                    (Some "Universiteee")
                    (Some [role])
                    (Some [comment])

            let characteristic = 
                MaterialAttribute.make 
                    (Some "Characteristic/Organism")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Organism")
                            (Some (AnnotationValue.Text "organism"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                            (Some [comment])
                    ))

            let characteristicValue = 
                MaterialAttributeValue.make 
                    (Some "CharacteristicValue/Arabidopsis")
                    (Some characteristic)
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Organism")
                            (Some (AnnotationValue.Text "Arabidopsis thaliana"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                            (Some [comment])
                        |> Value.Ontology
                    ))
                    None

            let studyDesignDescriptor = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/TimeSeries")
                    (Some (AnnotationValue.Text "Time Series Analysis"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C18235")               
                    (Some [comment])

            let protocolType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/GrowthProtocol")
                    (Some (AnnotationValue.Text "growth protocol"))
                    (Some "dfbo")
                    (Some "http://purl.obolibrary.org/obo/DFBO_1000162")
                    (Some [comment])

            let parameter = 
                ProtocolParameter.make
                    (Some "Parameter/Temperature")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Temperature")
                            (Some (AnnotationValue.Text "temperature unit"))
                            (Some "uo")
                            (Some "http://purl.obolibrary.org/obo/UO_0000005")
                            (Some [comment])
                    ))

            let parameterUnit =              
                OntologyAnnotation.make
                    (Some "OntologyTerm/DegreeCelsius")
                    (Some (AnnotationValue.Text "degree celsius"))
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000027")
                    (Some [comment])

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
                            (Some "OntologyTerm/RTPCR")
                            (Some (AnnotationValue.Text "real-time PCR machine"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0001110")
                            (Some [comment])
                        |> Value.Ontology
                    ))
                    None
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/PCR")
                            (Some (AnnotationValue.Text "PCR instrument"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0000989")
                            (Some [comment])
                    ))
                
            let protocol = 
                Protocol.make 
                    (Some "Protocol/MyProtocol")
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
                        (Some "Factor/Time")
                        (Some "Time")
                        (Some (
                            OntologyAnnotation.make
                                (Some "OntologyTerm/Time")
                                (Some (AnnotationValue.Text "time"))
                                (Some "pato")
                                (Some "http://purl.obolibrary.org/obo/PATO_0000165")
                                (Some [comment])
                        ))
                        (Some [comment])

            let factorUnit = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/Hour")
                    (Some (AnnotationValue.Text "hour"))
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000032")
                    (Some [comment])
                    

            let factorValue = 
                FactorValue.make
                    (Some "FactorValue/4hours")
                    (Some factor)
                    (Some (Value.Float 4.5))
                    (Some factorUnit)

            let source =
                Source.make
                    (Some "Source/MySource")
                    (Some "MySource")
                    (Some [characteristicValue])

            let sample = 
                Sample.make
                    (Some "Sample/MySample")
                    (Some "MySample")
                    (Some [characteristicValue])
                    (Some [factorValue])
                    (Some [source])

            let data = 
                Data.make
                    (Some "Data/MyData")
                    (Some "MyData")
                    (Some DataFile.DerivedDataFile)
                    (Some [comment])
        
            let material = 
                Material.make
                    (Some "Material/MyMaterial")
                    (Some "MyMaterial")
                    (Some MaterialType.ExtractName)
                    (Some [characteristicValue])
                    None

            let derivedMaterial = 
                Material.make
                    (Some "Material/MyDerivedMaterial")
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
                    (Some "Process/MyProcess1")
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
                    (Some "Process/MyProcess2")
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


            let measurementType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/LFQuantification")
                    (Some (AnnotationValue.Text "LC/MS Label-Free Quantification"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C161813")
                    (Some [comment])

            let technologyType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/TOF")
                    (Some (AnnotationValue.Text "Time-of-Flight"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C70698")
                    (Some [comment])

            let assayMaterials =
                AssayMaterials.make
                    (Some [sample])
                    (Some [material;derivedMaterial])

            let assay = 
                Assay.make
                    (Some "Assay/MyAssay")
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
                    (Some "Study/MyStudy")
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
                    (Some "Investigations/MyInvestigation")
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

            let s = Investigation.toString investigation

            Expect.matchingInvestigation s


            let reReadInvestigation = Investigation.fromString s
            let reWrittenInvestigation = Investigation.toString reReadInvestigation

            let i = 
                s 
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                reWrittenInvestigation
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            mySequenceEqual o i "Written investigation file does not match read investigation file"

        )
        |> testSequenced
    ]

let main = 
    testList "APITests" [
        testProcessInput     
        testProtocolFile
        testProcessFile
        testPersonFile
        testPublicationFile
        testAssayFile
        testInvestigationFile
    ]