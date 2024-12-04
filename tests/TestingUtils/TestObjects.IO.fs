module TestObjects.IO

open ARCtrl.ArcPathHelper

let testBaseFolder =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    "./tests/TestingUtils"
    #else
    //"../TestingUtils"
    __SOURCE_DIRECTORY__
    #endif

let testObjectsBaseFolder = combine testBaseFolder "TestObjects.IO"

let testResultsFolder =
    #if !FABLE_COMPILER
    combineMany [| testBaseFolder;"TestResults";"NET"|]
    #endif
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    combineMany [| testBaseFolder;"TestResults";"js"|]
    #endif
    #if FABLE_COMPILER_PYTHON
    combineMany [| testBaseFolder;"TestResults";"py"|]
    #endif

let testContractsFolder = combine testObjectsBaseFolder "Contracts"

let testSubPathsFolder = combine testObjectsBaseFolder "Path_findSubPaths"

let testSimpleARC = combine testObjectsBaseFolder "SimpleARC"
let testSimpleARC_Output = combine testResultsFolder "TestResults/SimpleARC"


let simpleTextFilePath = combine testObjectsBaseFolder "SimpleText.txt"
let simpleWorkbookPath = combine testObjectsBaseFolder "TestWorkbook.xlsx"