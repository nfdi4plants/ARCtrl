module ArcAssayTests


#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ISA
open TestingUtils
open ISA.Spreadsheet

let testColumnHeaderFunctions = 

    testList "ColumnHeaderFunctionTests" [
        //testCase "RawKindTest" (fun () ->

        //    let headerString = "RawHeader"

        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "RawHeader" None None

        //    Expect.equal header testHeader "Should have used header string as kind"
        //)
        //testCase "NumberedRawKindTest" (fun () ->

        //    let headerString = "RawHeader (#5)"

        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "RawHeader" None (Some 5)

        //    Expect.equal header testHeader "Number was not parsed correctly"
        //)
        //testCase "Name" (fun () ->

        //    let headerString = "NamedHeader [Name]"

        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Name" "" "" |> Some) None

        //    Expect.equal header testHeader "Dit not parse Name correctly"
        //)
        //testCase "NameWithNumber" (fun () ->

        //    let headerString = "NamedHeader [Name#5]"

        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

        //    let testComment = Comment.fromString "Number" "5"
        //    let testOntology = OntologyAnnotation.make None (Some (AnnotationValue.Text "Name")) None None (Some [testComment])

        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "NamedHeader" (Some testOntology) (Some 5)

        //    Expect.equal header testHeader "Did not parse Name correctly"
        //)
        //testCase "NameWithBrackets" (fun () ->
        
        //    let headerString = "NamedHeader [Name [Stuff]]"
        
        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString
            
        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Name [Stuff]" "" "" |> Some) None
            
        //    Expect.equal header testHeader "Dit not parse Name correctly"
        //)
        //testCase "NameWithHashtag" (fun () ->
            
        //    let headerString = "NamedHeader [Name#Stuff]"
            
        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString
                
        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Name#Stuff" "" "" |> Some) None
                
        //    Expect.equal header testHeader "Dit not parse Name correctly"
        //)
        //testCase "AccessionWithNumber" (fun () ->

        //    let headerString = "Term Accession Number (MS:1000031#2)"

        //    let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

        //    let testComment = Comment.fromString "Number" "2"
        //    let testOntology = OntologyAnnotation.fromString "" "MS" "http://purl.obolibrary.org/obo/MS_1000031"
        //    let testHeader = AnnotationColumn.ColumnHeader.create headerString "Term Accession Number" (Some testOntology) (Some 2)

        //    Expect.equal header testHeader "Dit not parse Name correctly"
        //)
    ]

let testNodeGetterFunctions =

    //let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"

    //let assayFilePath = System.IO.Path.Combine(sourceDirectory,"AssayTableTestFile.xlsx")
    
    //let doc = Spreadsheet.fromFile assayFilePath false
    //let sst = Spreadsheet.tryGetSharedStringTable doc
    //let wsp = Spreadsheet.tryGetWorksheetPartBySheetIndex 0u doc |> Option.get
    //let table = Table.tryGetByNameBy (fun s -> s.Contains "annotationTable") wsp |> Option.get

    //let m = Table.toSparseValueMatrix sst (Worksheet.getSheetData wsp.Worksheet) table
        
    testList "NodeGetterTests" [
        //testCase "RetreiveValueFromMatrix" (fun () ->
        //    let tryGetValue k (dict:System.Collections.Generic.Dictionary<'K,'V>) = 
        //        let b,v = dict.TryGetValue(k)
        //        if b then Some v
        //        else None

        //    let v = tryGetValue (0,"Unit") m

        //    Expect.isSome v "Value could not be retrieved from matrix"

        //    let expectedValue = "square centimeter"

        //    Expect.equal v.Value expectedValue "Value retrieved from matrix is not correct"

        //)

        //testCase "GetUnitGetter" (fun () ->

        //    let headers = ["Unit";"Term Source REF (TO:0002637)";"Term Accession Number (TO:0002637)"]

        //    let unitGetterOption = AnnotationNode.tryGetUnitGetterFunction headers

        //    Expect.isSome unitGetterOption "Unit Getter was not returned even though headers should have matched"

        //    let unitGetter = unitGetterOption.Value

        //    let unit = unitGetter m 0

        //    let expectedUnit = OntologyAnnotation.fromString "square centimeter" "UO" "http://purl.obolibrary.org/obo/UO_0000081"

        //    Expect.equal unit expectedUnit "Retrieved Unit is wrong"
        //)
        //testCase "GetUnitGetterWrongHeaders" (fun () ->

        //    let headers = ["Parameter [square centimeter]";"Term Source REF (UO:0000081)";"Term Accession Number (UO:0000081)"]

        //    let unitGetterOption = AnnotationNode.tryGetUnitGetterFunction headers

        //    Expect.isNone unitGetterOption "Unit Getter was returned even though headers should not have matched"
        //)
        //testCase "GetCharacteristicsGetter" (fun () ->

        //    let headers = ["Characteristics [leaf size]";"Unit";"Term Source REF (TO:0002637)";"Term Accession Number (TO:0002637)"]

        //    let characteristicGetterOption = AnnotationNode.tryGetCharacteristicGetter 0 headers

        //    Expect.isSome characteristicGetterOption "Characteristic Getter was not returned even though headers should have matched"

        //    let characteristic,characteristicValueGetter = characteristicGetterOption.Value

        //    Expect.isSome characteristic "CharacteristGetter was returned but not characteristic was returned"

        //    let expectedCharacteristic = MaterialAttribute.fromStringWithValueIndex "leaf size" "TO"  "http://purl.obolibrary.org/obo/TO_0002637" 0

        //    Expect.equal characteristic.Value expectedCharacteristic "Retrieved Characteristic is wrong"

        //    let expectedUnit = OntologyAnnotation.fromString "square centimeter" "UO" "http://purl.obolibrary.org/obo/UO_0000081" |> Some

        //    let expectedValue = Value.fromOptions (Some "10") None None

        //    let expectedCharacteristicValue = MaterialAttributeValue.make None (Some expectedCharacteristic) expectedValue expectedUnit

        //    let characteristicValue = characteristicValueGetter m 0

        //    Expect.equal characteristicValue expectedCharacteristicValue "Retrieved Characteristic Value was not correct"
        //)
        //testCase "GetCharacteristicsGetterWrongHeaders" (fun () ->

        //    let headers = ["Parameter [square centimeter] (#h; #tUO:0000081; #u)";"Term Source REF [square centimeter] (#h; #tUO:0000081; #u)";"Term Accession Number [square centimeter] (#h; #tUO:0000081; #u)"]

        //    let unitGetterOption = AnnotationNode.tryGetCharacteristicGetter 0 headers

        //    Expect.isNone unitGetterOption "Characteristic Getter was returned even though headers should not have matched"
        //)
        //testCase "GetFactorGetter" (fun () ->

        //    let headers = ["Factor [time]";"Unit";"Term Source REF (PATO:0000165)";"Term Accession Number (PATO:0000165)"]

        //    let factorGetterOption = AnnotationNode.tryGetFactorGetter 0 headers

        //    Expect.isSome factorGetterOption "Factor Getter was not returned even though headers should have matched"
            
        //    let factor,factorValueGetter = factorGetterOption.Value

        //    Expect.isSome factor "FactorGetter was returned but no factor was returned"

        //    let expectedFactor = Factor.fromStringWithValueIndex "time" "time" "PATO" "http://purl.obolibrary.org/obo/PATO_0000165" 0

        //    Expect.equal factor.Value expectedFactor "Retrieved Factor is wrong"
            
        //    let expectedUnit = OntologyAnnotation.fromString "hour" "UO" "http://purl.obolibrary.org/obo/UO_0000032" |> Some

        //    let expectedValue = Value.fromOptions (Some "5") None None

        //    let expectedFactorValue = FactorValue.make None (Some expectedFactor) expectedValue expectedUnit

        //    let factorValue = factorValueGetter m 0

        //    Expect.equal factorValue expectedFactorValue "Retrieved Factor Value was not correct"
        //)
        //testCase "GetFactorGetterWrongHeaders" (fun () ->

        //    let headers = ["Parameter [square centimeter] (#h; #tUO:0000081; #u)";"Term Source REF [square centimeter] (#h; #tUO:0000081; #u)";"Term Accession Number [square centimeter] (#h; #tUO:0000081; #u)"]

        //    let unitGetterOption = AnnotationNode.tryGetFactorGetter 0 headers

        //    Expect.isNone unitGetterOption "Facotr Getter was returned even though headers should not have matched"
        //)
        //testCase "GetProtocolREFGetter" (fun () ->
            
        //    let headers = ["Protocol REF"]
            
        //    let refGetterOption = AnnotationNode.tryGetProtocolREFGetter 0 headers
            
        //    Expect.isSome refGetterOption "Protocol Ref Getter was not returned even though headers should have matched"
                        
        //    let protocolREFGetter = refGetterOption.Value

        //    let expectedProtocolREF = "MyProtocol"
            
        //    let protocolREF = protocolREFGetter m 0
            
        //    Expect.equal protocolREF expectedProtocolREF "Retrieved Protocol Type was not correct"
        //)
        //testCase "GetProtocolTypeGetter" (fun () ->
        
        //    let headers = ["Protocol Type";"Term Source REF";"Term Accession Number"]
        
        //    let typeGetterOption = AnnotationNode.tryGetProtocolTypeGetter 0 headers
        
        //    Expect.isSome typeGetterOption "Protocol Type Getter was not returned even though headers should have matched"
                    
        //    let protocolTypeGetter = typeGetterOption.Value

        //    let comments = [ValueIndex.createOrderComment 0]
        //    let expectedProtocolType = OntologyAnnotation.fromStringWithComments "Growth Protocol" "NFDI4PSO" "http://purl.obolibrary.org/obo/NFDI4PSO_1002416" comments
        
        //    let protocolType = protocolTypeGetter m 0
        
        //    Expect.equal protocolType expectedProtocolType "Retrieved Protocol Type was not correct"
        //)
        //testCase "GetParameterGetter" (fun () ->

        //    let headers = ["Parameter [temperature unit]";"Unit ";"Term Source REF (UO:0000005)";"Term Accession Number (UO:0000005)"]

        //    let parameterGetterOption = AnnotationNode.tryGetParameterGetter 0 headers

        //    Expect.isSome parameterGetterOption "Parameter Getter was not returned even though headers should have matched"
            
        //    let parameter,parameterValueGetter = parameterGetterOption.Value

        //    Expect.isSome parameter "ParameterGetter was returned but no parameter was returned"

        //    let expectedParameter = ProtocolParameter.fromStringWithValueIndex "temperature unit" "UO" "http://purl.obolibrary.org/obo/UO_0000005" 0

        //    Expect.equal parameter.Value expectedParameter "Retrieved Parameter is wrong"

        //    let expectedUnit = OntologyAnnotation.fromString "degree Celsius" "UO" "http://purl.obolibrary.org/obo/UO_0000027" |> Some

        //    let expectedValue = Value.fromOptions (Some "27") None None

        //    let expectedParameterValue = ProcessParameterValue.make (Some expectedParameter) expectedValue expectedUnit

        //    let parameterValue = parameterValueGetter m 0

        //    Expect.equal parameterValue expectedParameterValue "Retrieved Parameter Value was not correct"
        //)
        //testCase "GetParameterGetterNoUnit" (fun () ->

        //    let headers = ["Parameter [measurement device]";"Term Source REF (OBI:0000832)";"Term Accession Number (OBI:0000832)"]

        //    let parameterGetterOption = AnnotationNode.tryGetParameterGetter 0 headers

        //    Expect.isSome parameterGetterOption "Parameter Getter was not returned even though headers should have matched"
            
        //    let parameter,parameterValueGetter = parameterGetterOption.Value

        //    Expect.isSome parameter "ParameterGetter was returned but no parameter was returned"

        //    Expect.isSome parameter.Value.ParameterName.Value.TermSourceREF "Parameter has no TermSourceRef"

        //    let expectedParameter = ProtocolParameter.fromStringWithValueIndex "measurement device" "OBI" "http://purl.obolibrary.org/obo/OBI_0000832" 0

        //    Expect.equal parameter.Value expectedParameter "Retrieved Parameter is wrong"
            
        //    let expectedValue = Value.fromOptions (Some "Bruker NMR probe") (Some "OBI") (Some "http://purl.obolibrary.org/obo/OBI_0000561")

        //    let expectedParameterValue = ProcessParameterValue.make (Some expectedParameter) expectedValue None

        //    let parameterValue = parameterValueGetter m 0

        //    Expect.equal parameterValue expectedParameterValue "Retrieved Unitless Parameter Value was not correct"
        //)
        //testCase "GetComponentGetter" (fun () ->

        //    let headers = ["Component [weight]";"Unit   ";"Term Source REF (PATO:0000128)";"Term Accession Number (PATO:0000128)"]

        //    let parameterGetterOption = AnnotationNode.tryGetComponentGetter 0 headers

        //    Expect.isSome parameterGetterOption "Component Getter was not returned even though headers should have matched"
            
        //    let componentGetter = parameterGetterOption.Value

        //    let expectedName = "12 gram (UO:0000021)"

        //    let expectedValue = Value.fromOptions (Some "12") None None

        //    let expectedUnit = OntologyAnnotation.fromString "gram" "UO" "http://purl.obolibrary.org/obo/UO_0000021" |> Some

        //    let comments = [ValueIndex.createOrderComment 0]
        //    let expectedType = OntologyAnnotation.fromStringWithComments "weight" "PATO" "http://purl.obolibrary.org/obo/PATO_0000128" comments |> Some

        //    let expectedComponent = Component.make (Some expectedName) expectedValue expectedUnit expectedType

        //    let componentV = componentGetter m 0

        //    Expect.equal componentV expectedComponent "Retrieved Component was not correct"
        //)
        //testCase "GetComponentGetterNoUnit" (fun () ->

        //    let headers = ["Component [instrument model]";"Term Source REF (MS:1000031)";"Term Accession Number (MS:1000031)"]

        //    let parameterGetterOption = AnnotationNode.tryGetComponentGetter 0 headers

        //    Expect.isSome parameterGetterOption "Component Getter was not returned even though headers should have matched"
            
        //    let componentGetter = parameterGetterOption.Value

        //    let expectedName = "Orbitrap Fusion (MS:1002416)" |> Some

        //    let expectedValue = Value.fromOptions (Some "Orbitrap Fusion") (Some "MS") (Some "http://purl.obolibrary.org/obo/MS_1002416")

        //    let expectedUnit = None

        //    let comments = [ValueIndex.createOrderComment 0]
        //    let expectedType = OntologyAnnotation.fromStringWithComments "instrument model" "MS" "http://purl.obolibrary.org/obo/MS_1000031" comments |> Some

        //    let expectedComponent = Component.make expectedName expectedValue expectedUnit expectedType

        //    let componentV = componentGetter m 0

        //    Expect.equal componentV expectedComponent "Retrieved Component was not correct"

        //)
        //testCase "GetParameterGetterUserSpecific" (fun () ->

        //    let headers = ["Parameter [heating block]";"Term Source REF (OBI:0400108)";"Term Accession Number (OBI:0400108)"]

        //    let parameterGetterOption = AnnotationNode.tryGetParameterGetter 0 headers

        //    Expect.isSome parameterGetterOption "Parameter Getter was not returned even though headers should have matched"
            
        //    let parameter,parameterValueGetter = parameterGetterOption.Value

        //    Expect.isSome parameter "ParameterGetter was returned but no parameter was returned"

        //    let expectedParameter = ProtocolParameter.fromStringWithValueIndex "heating block" "OBI" "http://purl.obolibrary.org/obo/OBI_0400108" 0

        //    Expect.equal parameter.Value expectedParameter "Retrieved Parameter is wrong"
            
        //    let expectedValue = Value.fromOptions (Some "Freds old stove") None None

        //    let expectedParameterValue = ProcessParameterValue.make (Some expectedParameter) expectedValue None

        //    let parameterValue = parameterValueGetter m 0

        //    Expect.equal parameterValue expectedParameterValue "Retrieved Unitless Parameter Value was not correct"
        //)
        //testCase "GetParameterGetterWrongHeaders" (fun () ->

        //    let headers = ["Factor [square centimeter]";"Term Source REF (UO:0000081)";"Term Accession Number (UO:0000081)"]

        //    let unitGetterOption = AnnotationNode.tryGetParameterGetter 0 headers

        //    Expect.isNone unitGetterOption "Facotr Getter was returned even though headers should not have matched"
        //)
    ]

let testProcessGetter =

    //let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"

    //let assayFilePath = System.IO.Path.Combine(sourceDirectory,"AssayTableTestFile.xlsx")
    
    //let doc = Spreadsheet.fromFile assayFilePath false
    //let sst = Spreadsheet.tryGetSharedStringTable doc
    //let wsp = Spreadsheet.tryGetWorksheetPartBySheetIndex 0u doc |> Option.get
    //let table = Table.tryGetByNameBy (fun s -> s.Contains "annotationTable") wsp |> Option.get

    //let m = Table.toSparseValueMatrix sst (Worksheet.getSheetData wsp.Worksheet) table

    //let characteristicHeaders = ["Characteristics [leaf size]";"Unit";"Term Source REF (TO:0002637)";"Term Accession Number (TO:0002637)"]
    //let expectedCharacteristic = MaterialAttribute.fromStringWithValueIndex "leaf size" "TO" "http://purl.obolibrary.org/obo/TO_0002637" 0
    //let expectedCharacteristicUnit = OntologyAnnotation.fromString "square centimeter" "UO" "http://purl.obolibrary.org/obo/UO_0000081" |> Some
    //let expectedCharacteristicValue = MaterialAttributeValue.make None (Some expectedCharacteristic) (Value.fromOptions (Some "10") None None) expectedCharacteristicUnit

    //let factorHeaders = ["Factor [time]";"Unit (#2)";"Term Source REF (PATO:0000165)";"Term Accession Number (PATO:0000165)"]
    //let expectedFactor = Factor.fromStringWithValueIndex "time" "time" "PATO" "http://purl.obolibrary.org/obo/PATO_0000165" 1
    //let expectedFactorUnit = OntologyAnnotation.fromString "hour" "UO"  "http://purl.obolibrary.org/obo/UO_0000032"|> Some    
    //let expectedFactorValue = FactorValue.make None (Some expectedFactor) (Value.fromOptions (Some "5") None None) expectedFactorUnit

    //let parameterHeaders = ["Parameter [temperature unit]";"Unit (#3)";"Term Source REF (UO:0000005)";"Term Accession Number (UO:0000005)"]
    //let expectedParameter = ProtocolParameter.fromStringWithValueIndex "temperature unit" "UO" "http://purl.obolibrary.org/obo/UO_0000005" 2
    //let expectedParameterUnit = OntologyAnnotation.fromString "degree Celsius" "UO" "http://purl.obolibrary.org/obo/UO_0000027"  |> Some    
    //let expectedParameterValue = ProcessParameterValue.make (Some expectedParameter) (Value.fromOptions (Some "27") None None) expectedParameterUnit


    //let sourceHeader = ["Source Name"]
    //let expectedSourceName = "Source1"

    //let sampleHeader = ["Sample Name"]
    //let expectedSampleName = "Sample1"

    testList "ProcessGetterTests" [
        //testCase "ProcessGetterSourceParamsSample" (fun () ->

        //    let headers = sourceHeader @ characteristicHeaders @ factorHeaders @ parameterHeaders @ sampleHeader

        //    let expectedSource = Source.make None (Some expectedSourceName) (Some [expectedCharacteristicValue])
        //    let expectedInput = ProcessInput.Source expectedSource
        //    let expectedOutput = ProcessOutput.Sample (Sample.make None (Some expectedSampleName) None (Some [expectedFactorValue]) (Some [expectedSource])  )

        //    let expectedProtocol = Protocol.make None None None None None None (Some [expectedParameter]) None None |> Protocol.setRowIndex 0

        //    let expectedProcess = Process.make None None (Some expectedProtocol) (Some [expectedParameterValue]) None None None None (Some [expectedInput]) (Some [expectedOutput]) None

        //    let processGetter = AnnotationTable.getProcessGetter "" (headers |> AnnotationNode.splitIntoNodes)            

        //    let processV = processGetter m 0

        //    Expect.equal processV expectedProcess "Process was retrieved incorrectly"
    
        //    let characteristics =   API.Process.getCharacteristics processV
        //    let factors =           API.Process.getFactors processV
        //    let protocol =         processV.ExecutesProtocol.Value

        //    mySequenceEqual characteristics [expectedCharacteristic] "Characteristics were parsed incorrectly"
        //    mySequenceEqual factors [expectedFactor] "Factors were parsed incorrectly"
        //    Expect.equal protocol expectedProtocol "Protocol was parsed incorrectly"

        //)
        //testCase "ProcessGetterSampleParams" (fun () ->

        //    let headers = 
        //        sampleHeader @ characteristicHeaders @ factorHeaders @ parameterHeaders
        //        //|> AnnotationTable.splitBySamples
        //        //|> Seq.head

        //    let expectedOutput = ProcessOutput.Sample (Sample.make None None None (Some [expectedFactorValue]) None  )

        //    let expectedInput = ProcessInput.Sample (Sample.make None (Some expectedSampleName) (Some [expectedCharacteristicValue]) None None  )


        //    let expectedProtocol = Protocol.make None None None None None None (Some [expectedParameter]) None None |> Protocol.setRowIndex 0
             
        //    let expectedProcess = Process.make None None (Some expectedProtocol) (Some [expectedParameterValue]) None None None None (Some [expectedInput]) (Some [expectedOutput]) None

        //    let processGetter = AnnotationTable.getProcessGetter "" (headers |> AnnotationNode.splitIntoNodes)

        //    let processV = processGetter m 0

        //    let characteristics =   API.Process.getCharacteristics processV
        //    let factors =           API.Process.getFactors processV
        //    let protocol =         processV.ExecutesProtocol.Value

        //    mySequenceEqual characteristics [expectedCharacteristic] "Characteristics were parsed incorrectly"
        //    mySequenceEqual factors [expectedFactor] "Factors were parsed incorrectly"
        //    Expect.equal protocol expectedProtocol "Protocol was parsed incorrectly"

        //    Expect.equal processV expectedProcess "Process was retrieved incorrectly"
    
        //)
    ]

let testProcessComparisonFunctions = 

    //let parameters = 
    //    [
    //    ProtocolParameter.make None (Some (OntologyAnnotation.fromString "Term1" "" ""))
    //    ProtocolParameter.make None (Some (OntologyAnnotation.fromString "Term2" "" ""))
    //    ]

    //let parameterValues1 =
    //    [
    //        ProcessParameterValue.make (Some parameters.[0]) (Value.fromOptions (Some "Value1") None None) None
    //        ProcessParameterValue.make (Some parameters.[1]) (Value.fromOptions (Some "Value2") None None) None
    //    ]

    //let parameterValues2 = 
    //    [
    //        ProcessParameterValue.make (Some parameters.[0]) (Value.fromOptions (Some "Value1") None None) None
    //        ProcessParameterValue.make (Some parameters.[1]) (Value.fromOptions (Some "Value3") None None) None
    //    ]

    //let characteristic =  MaterialAttribute.make None (Some (OntologyAnnotation.fromString "Term4" "" ""))
    //let characteristicValue = MaterialAttributeValue.make None (Some characteristic) (Value.fromOptions (Some "Value4") None None) None

    //let factor =  Factor.make None (Some "Factor") (Some (OntologyAnnotation.fromString "Term5" "" "")) None
    //let factorValue = FactorValue.make None (Some factor) (Value.fromOptions (Some "Value5") None None) None


    //let protocol1 = Protocol.make None (Some "Protocol1") None None None None (Some parameters) None None
    //let protocol2 = Protocol.make None (Some "Protocol2") None None None None (Some parameters) None None

    //let sample1 = Sample.make None (Some "Sample1") None None None
    //let sample2 = Sample.make None (Some "Sample2") None None None
    //let sample3 = Sample.make None (Some "Sample3") None None None
    //let sample4 = Sample.make None (Some "Sample4") None None None

    //let source1 = Source.make None (Some "Source1") None
    //let source2 = Source.make None (Some "Source2") None
    //let source3 = Source.make None (Some "Source3") None
    //let source4 = Source.make None (Some "Source4") None

    testList "ProcessComparisonTests" [
        //testCase "MergeProcesses" (fun () ->

        //    let process1 = Process.make None (Some "Process_1") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source1]) (Some [ProcessOutput.Sample sample1]) None
        //    let process2 = Process.make None (Some "Process_2") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source2]) (Some [ProcessOutput.Sample sample2]) None
        //    let process3 = Process.make None (Some "Process_3") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source3]) (Some [ProcessOutput.Sample sample3]) None
        //    let process4 = Process.make None (Some "Process_4") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source4]) (Some [ProcessOutput.Sample sample4]) None

        //    let mergedProcesses = AnnotationTable.mergeIdenticalProcesses "Process" [process1;process2;process3;process4]

        //    Expect.hasLength mergedProcesses 1 "Should have merged all 4 Processes as they execute with the same params"

        //    let mergedProcess = Seq.head mergedProcesses

        //    Expect.isSome mergedProcess.Inputs "Inputs were dropped"
        //    Expect.isSome mergedProcess.Outputs "Outputs were dropped"

        //    mySequenceEqual mergedProcess.Inputs.Value [ProcessInput.Source source1;ProcessInput.Source source2;ProcessInput.Source source3;ProcessInput.Source source4] "Inputs were not merged correctly"
        //    mySequenceEqual mergedProcess.Outputs.Value [ProcessOutput.Sample sample1;ProcessOutput.Sample sample2;ProcessOutput.Sample sample3;ProcessOutput.Sample sample4] "Inputs were not merged correctly"
        //)        
        //testCase "MergeProcessesDifferentParams" (fun () ->

        //    let process1 = Process.make None (Some "Process_1") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source1]) (Some [ProcessOutput.Sample sample1]) None
        //    let process2 = Process.make None (Some "Process_2") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source2]) (Some [ProcessOutput.Sample sample2]) None
        //    let process3 = Process.make None (Some "Process_3") (Some protocol1) (Some parameterValues2) None None None None (Some [ProcessInput.Source source3]) (Some [ProcessOutput.Sample sample3]) None
        //    let process4 = Process.make None (Some "Process_4") (Some protocol1) (Some parameterValues2) None None None None (Some [ProcessInput.Source source4]) (Some [ProcessOutput.Sample sample4]) None

        //    let mergedProcesses = AnnotationTable.mergeIdenticalProcesses "Process" [process1;process2;process3;process4]

        //    Expect.hasLength mergedProcesses 2 "Processes executed with two different parameter values, therefore they should be merged to two different processes"

        //    let mergedProcess1 = Seq.item 0 mergedProcesses
        //    let mergedProcess2 = Seq.item 1 mergedProcesses

        //    Expect.isSome mergedProcess1.Inputs "Inputs were dropped for mergedProcess1"
        //    Expect.isSome mergedProcess1.Outputs "Outputs were dropped for mergedProcess1"

        //    mySequenceEqual mergedProcess1.Inputs.Value [ProcessInput.Source source1;ProcessInput.Source source2] "Inputs were not merged correctly for mergedProcess1"
        //    mySequenceEqual mergedProcess1.Outputs.Value [ProcessOutput.Sample sample1;ProcessOutput.Sample sample2] "Inputs were not merged correctly for mergedProcess1"
        
        //    Expect.isSome mergedProcess2.Inputs "Inputs were dropped for mergedProcess2"
        //    Expect.isSome mergedProcess2.Outputs "Outputs were dropped for mergedProcess2"

        //    mySequenceEqual mergedProcess2.Inputs.Value [ProcessInput.Source source3;ProcessInput.Source source4] "Inputs were not merged correctly for mergedProcess2"
        //    mySequenceEqual mergedProcess2.Outputs.Value [ProcessOutput.Sample sample3;ProcessOutput.Sample sample4] "Inputs were not merged correctly for mergedProcess2"
        
        //)
        //testCase "MergeProcessesDifferentProtocols" (fun () ->

        //    let process1 = Process.make None (Some "Process_1") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source1]) (Some [ProcessOutput.Sample sample1]) None
        //    let process2 = Process.make None (Some "Process_2") (Some protocol2) (Some parameterValues1) None None None None (Some [ProcessInput.Source source2]) (Some [ProcessOutput.Sample sample2]) None
        //    let process3 = Process.make None (Some "Process_3") (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source3]) (Some [ProcessOutput.Sample sample3]) None
        //    let process4 = Process.make None (Some "Process_4") (Some protocol2) (Some parameterValues1) None None None None (Some [ProcessInput.Source source4]) (Some [ProcessOutput.Sample sample4]) None

        //    let mergedProcesses = AnnotationTable.mergeIdenticalProcesses "Process" [process1;process2;process3;process4]

        //    Expect.hasLength mergedProcesses 2 "Processes executed with two different protocols, therefore they should be merged to two different processes"

        //    let mergedProcess1 = Seq.item 0 mergedProcesses
        //    let mergedProcess2 = Seq.item 1 mergedProcesses

        //    Expect.isSome mergedProcess1.Inputs "Inputs were dropped for mergedProcess1"
        //    Expect.isSome mergedProcess1.Outputs "Outputs were dropped for mergedProcess1"

        //    mySequenceEqual mergedProcess1.Inputs.Value [ProcessInput.Source source1;ProcessInput.Source source3] "Inputs were not merged correctly for mergedProcess1"
        //    mySequenceEqual mergedProcess1.Outputs.Value [ProcessOutput.Sample sample1;ProcessOutput.Sample sample3] "Inputs were not merged correctly for mergedProcess1"
        
        //    Expect.isSome mergedProcess2.Inputs "Inputs were dropped for mergedProcess2"
        //    Expect.isSome mergedProcess2.Outputs "Outputs were dropped for mergedProcess2"

        //    mySequenceEqual mergedProcess2.Inputs.Value [ProcessInput.Source source2;ProcessInput.Source source4] "Inputs were not merged correctly for mergedProcess2"
        //    mySequenceEqual mergedProcess2.Outputs.Value [ProcessOutput.Sample sample2;ProcessOutput.Sample sample4] "Inputs were not merged correctly for mergedProcess2"
        
        //)
        //testCase "IndexIdenticalProcessesByProtocolName" (fun () ->

        //    let process1 = Process.make None None (Some protocol1) (Some parameterValues1) None None None None (Some [ProcessInput.Source source1]) (Some [ProcessOutput.Sample sample1]) None
        //    let process2 = Process.make None None (Some protocol1) (Some parameterValues2) None None None None (Some [ProcessInput.Source source2]) (Some [ProcessOutput.Sample sample2]) None
        //    let process3 = Process.make None None (Some protocol2) (Some parameterValues1) None None None None (Some [ProcessInput.Source source3]) (Some [ProcessOutput.Sample sample3]) None
        //    let process4 = Process.make None None (Some protocol2) (Some parameterValues2) None None None None (Some [ProcessInput.Source source4]) (Some [ProcessOutput.Sample sample4]) None

        //    let indexedProcesses = AnnotationTable.mergeIdenticalProcesses "Process" [process1;process2;process3;process4]

        //    let names = indexedProcesses |> Seq.map (fun p -> Option.defaultValue "" p.Name)

        //    let expectedNames = ["Process_0";"Process_1";"Process_2";"Process_3"]

        //    mySequenceEqual names expectedNames "Processes were not indexed correctly"
        //)
        //testCase "MergeSampleInfoTransformToSource" (fun () ->
            
        //    let sourceWithSampleName = ProcessInput.Source (Source.make None (Some "Sample1") None)

        //    let process1 = Process.make None None None None None None None None (Some [ProcessInput.Source source1]) (Some [ProcessOutput.Sample sample1]) None
        //    let process2 = Process.make None None None None None None None None (Some [sourceWithSampleName]) (Some [ProcessOutput.Sample sample2]) None

        //    let updatedProcesses = AnnotationTable.updateSamplesByThemselves [process1;process2]

        //    let expectedProcessSequence =
        //        [
        //            process1
        //            Process.make None None None None None None None None (Some ([ProcessInput.Sample sample1])) (Some [ProcessOutput.Sample sample2]) None
                
        //        ]

        //    mySequenceEqual updatedProcesses expectedProcessSequence "Source with same name as sample should have been converted to sample"        
        //)
        //testCase "MergeSampleInfo" (fun () ->
            
        //    let outputOfFirst = ProcessOutput.Sample (Sample.make None (Some "Sample1") None (Some [factorValue]) None)
        //    let inputOfSecond = ProcessInput.Source (Source.make None (Some "Sample1") (Some [characteristicValue]))

        //    let process1 = Process.make None None None None None None None None (Some [ProcessInput.Source source1]) (Some [outputOfFirst]) None
        //    let process2 = Process.make None None None None None None None None (Some [inputOfSecond]) (Some [ProcessOutput.Sample sample2]) None

        //    let updatedProcesses = AnnotationTable.updateSamplesByThemselves [process1;process2]


        //    let outputs = (Seq.head updatedProcesses).Outputs
        //    Expect.isSome outputs "Outputs of first Process are empty"
        //    let outputSample = 
        //        match outputs.Value with
        //        | [ProcessOutput.Sample s] -> s
        //        | _ -> failwithf "Expected a single sample for outputs but got %A" outputs


        //    let inputs = (Seq.item 1 updatedProcesses).Inputs
        //    Expect.isSome inputs "Inputs of second Process are empty"
        //    let inputSample = 
        //        match inputs.Value with
        //        | [ProcessInput.Sample s] -> s
        //        | _ -> failwithf "Expected a single sample for inputs but got %A" inputs
            
        //    let expectedSample = Sample.make None (Some "Sample1") (Some [characteristicValue]) (Some [factorValue]) None

        //    Expect.equal inputSample outputSample "The information of the output of the first process and the input of the second process was not equalized"      
        //    Expect.equal outputSample expectedSample "Values were not correctly merged"
        //)
    ]

let testMetaDataFunctions = 

    testList "AssayMetadataTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet TestObjects.Assay.assayMetadata |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = ArcAssay.fromMetadataSheet TestObjects.Assay.assayMetadata

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInput" (fun () ->
           
            let o = 
                TestObjects.Assay.assayMetadata
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.workSheetEqual o TestObjects.Assay.assayMetadata "Written assay metadata does not match read assay metadata"
        )

        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet TestObjects.Assay.assayMetadataEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessEmpty" (fun () ->

            let a = ArcAssay.fromMetadataSheet TestObjects.Assay.assayMetadataEmpty

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInputEmpty" (fun () ->
           
            let o = 
                TestObjects.Assay.assayMetadataEmpty
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.workSheetEqual o TestObjects.Assay.assayMetadataEmpty "Written Empty assay metadata does not match read assay metadata"
        )
        ]

let testAssayFileReader = 

    //let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    //let sinkDirectory = __SOURCE_DIRECTORY__ + @"/TestResult/"
    //let assayFilePath = System.IO.Path.Combine(sourceDirectory,"AssayTestFile.xlsx")

    //let expectedPersons =
    //    [
    //    Contacts.fromString "Weil" "Lukas" "H." "" "" "" "" "" "researcher" "http://purl.obolibrary.org/obo/MS_1001271" "MS" ([Comment.fromString "Worksheet" "GreatAssay"])
    //    Contacts.fromString "Leil" "Wukas" "" "" "" "" "" "" "" "" "" ([Comment.fromString "Worksheet" "SecondAssay"])
    //    ]

    //let technologyType =
    //    OntologyAnnotation.fromString "mass spectrometry" "MS" "http://purl.obolibrary.org/obo/MS_1000268"
        
    //let fileName = @"GreatAssay\assay.isa.xlsx"

    //let temperatureUnit = ProtocolParameter.fromStringWithValueIndex "temperature unit" "UO"  "http://purl.obolibrary.org/obo/UO_0000005" 0
    

    //let temperature = ProtocolParameter.fromStringWithValueIndex "temperature" "" "" 2

    //let peptidase = ProtocolParameter.fromStringWithValueIndex "enzyme unit" "UO" "http://purl.obolibrary.org/obo/UO_0000181" 1

    //let time1 = ProtocolParameter.fromStringWithValueIndex "time unit" "UO" "http://purl.obolibrary.org/obo/UO_0000003" 3

    //let time2 = Factor.fromStringWithValueIndex "time unit" "time unit" "UO" "http://purl.obolibrary.org/obo/UO_0000003" 4

    //let leafSize = MaterialAttribute.fromStringWithValueIndex "leaf size" "TO" "http://purl.obolibrary.org/obo/TO_0002637" 0

    //let temperatureUnit2 = ProtocolParameter.fromStringWithValueIndex "temperature unit" "UO"  "http://purl.obolibrary.org/obo/UO_0000005" 1

    testList "AssayFileReaderTests" [
        //testCase "ReaderSuccess" (fun () -> 
                       
        //    let readingSuccess = 
        //        try 
        //            Assay.fromFile assayFilePath |> ignore
        //            Result.Ok "DidRun"
        //        with
        //        | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

        //    Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        //)
        //testCase "ReadsCorrectly" (fun () ->        
            
        //    let persons,assay = Assay.fromFile assayFilePath

        //    let expectedProtocols = 
        //        [
        //        Protocol.make None (Some "GreatAssay") None None None None (Some [temperatureUnit;peptidase;temperature;time1]) None None |> Protocol.setRowRange (0,2)
        //        Protocol.make None (Some "SecondAssay") None None None None (Some [temperatureUnit2]) None None |> Protocol.setRowRange (0,2)
        //        ]
                

        //    let expectedFactors = [time2]

        //    let factors = API.Assay.getFactors assay
        //    let protocols = API.Assay.getProtocols assay

        //    mySequenceEqual factors expectedFactors        "Factors were read incorrectly"
        //    mySequenceEqual protocols expectedProtocols    "Protocols were read incorrectly"
        //    mySequenceEqual persons expectedPersons        "Persons were read incorrectly from metadata sheet"

        //    Expect.isSome assay.FileName "FileName was not read"
        //    Expect.equal assay.FileName.Value fileName "FileName was not read correctly"

        //    Expect.isSome assay.TechnologyType "Technology Type was not read"
        //    Expect.equal assay.TechnologyType.Value technologyType "Technology Type was not read correctly"

        //    Expect.isSome assay.CharacteristicCategories "Characteristics were not read"
        //    Expect.equal assay.CharacteristicCategories.Value [leafSize] "Characteristics were not read correctly"

        //    Expect.isSome assay.ProcessSequence "Processes were not read"
        //    assay.ProcessSequence.Value
        //    |> Seq.map (fun p -> Option.defaultValue "" p.Name)
        //    |> fun names -> mySequenceEqual names ["GreatAssay_0";"GreatAssay_1";"SecondAssay_0"] "Process names do not match"

        //)
        //testCase "AroundTheWorldComponents" (fun () ->        

        //    let xlsxFilePath = System.IO.Path.Combine(sourceDirectory,"20220802_TermCols_Assay.xlsx")
        //    let jsonFilePath = System.IO.Path.Combine(sourceDirectory,"20220802_TermCols_Assay.json")
        //    let xlsxOutFilePath = System.IO.Path.Combine(sinkDirectory,"20220802_TermCols_Assay.xlsx")

        //    let ref = Json.Assay.fromFile jsonFilePath
        //    let p,a = Assay.fromFile xlsxFilePath

        //    Expect.equal a.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length "Assay read from xlsx and json do not match. Process sequence does not have the same length"

        //    (a.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay read from xlsx and json do not match. Protocol does not match"
        //        Expect.equal p refP "Assay read from xlsx and json do not match. Process does not match"
        //    ) 

        //    Assay.toFile xlsxOutFilePath p a

        //    let _,a' = Assay.fromFile xlsxOutFilePath


        //    Expect.equal a'.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length "Assay written to xlsx and read in again does no longer match json. Process sequence does not have the same length"

        //    (a'.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay written to xlsx and read in again does no longer match json. Protocol does not match"
        //        Expect.equal p refP "Assay written to xlsx and read in again does no longer match json. Process does not match"
        //    ) 
        //    mySequenceEqual a'.ProcessSequence.Value ref.ProcessSequence.Value ""

        //)
        //testCase "AroundTheWorldProtocolType" (fun () ->        

        //    let xlsxFilePath = System.IO.Path.Combine(sourceDirectory,"20220803_ProtocolType_Assay.xlsx")
        //    let jsonFilePath = System.IO.Path.Combine(sourceDirectory,"20220803_ProtocolType_Assay.json")
        //    let xlsxOutFilePath = System.IO.Path.Combine(sinkDirectory,"20220803_ProtocolType_Assay.xlsx")

        //    let ref = Json.Assay.fromFile jsonFilePath
        //    let p,a = Assay.fromFile xlsxFilePath

        //    Expect.equal a.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length $"Assay read from xlsx and json do not match. Process sequence does not have the same length"


        //    (a.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay read from xlsx and json do not match. Protocol does not match"
        //        Expect.equal p refP "Assay read from xlsx and json do not match. Process does not match"
        //    ) 

        //    Assay.toFile xlsxOutFilePath p a

        //    let _,a' = Assay.fromFile xlsxOutFilePath


        //    Expect.equal a'.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length "Assay written to xlsx and read in again does no longer match json. Process sequence does not have the same length"

        //    (a'.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay written to xlsx and read in again does no longer match json. Protocol does not match"
        //        Expect.equal p refP "Assay written to xlsx and read in again does no longer match json. Process does not match"
        //    ) 
        //    mySequenceEqual a'.ProcessSequence.Value ref.ProcessSequence.Value ""

        //)
    ]


let main = 
    testList "AssayFile" [
        testMetaDataFunctions
    ]