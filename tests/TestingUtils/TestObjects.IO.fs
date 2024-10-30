module TestObjects.IO

open ARCtrl.ArcPathHelper

let testObjectsBaseFolder = combine __SOURCE_DIRECTORY__ "TestObjects.IO"

let testResultsFolder =
    #if !FABLE_COMPILER
    combineMany [| __SOURCE_DIRECTORY__;"TestResults";"NET"|]
    #endif

let testContractsFolder = combine testObjectsBaseFolder "Contracts"

let testSubPathsFolder = combine testObjectsBaseFolder "Path_findSubPaths"

let testSimpleARC = combine testObjectsBaseFolder "SimpleARC"
let testSimpleARC_Output = combine testResultsFolder "TestResults/SimpleARC"


let simpleTextFilePath = combine testObjectsBaseFolder "SimpleText.txt"