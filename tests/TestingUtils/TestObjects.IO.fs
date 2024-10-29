module TestObjects.IO

let testResultsFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestResults")

let testInputFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects/Contracts")
let testOutputFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestResults/Contracts")

let testSubPathsFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects/Path_findSubPaths")

let testSimpleARC = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects/SimpleARC")
let testSimpleARC_Output = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestResults/SimpleARC")