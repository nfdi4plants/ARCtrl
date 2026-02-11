module Tests.WorkflowGraphTestHelpers

open ARCtrl
open ARCtrl.CWL
open ARCtrl.FileSystem
open TestingUtils
open CrossAsync

let workflowFixturePath =
    ArcPathHelper.combineMany [|
        TestObjects.IO.testSimpleARCWithCWL
        "workflows"
        "ProteomIQon"
        "workflow.cwl"
    |]

let runFixturePath =
    ArcPathHelper.combineMany [|
        TestObjects.IO.testSimpleARCWithCWL
        "runs"
        "tests"
        "run.cwl"
    |]

let loadProcessingUnitFromPath (path: string) =
    crossAsync {
        let! content = FileSystemHelper.readFileTextAsync path
        return Decode.decodeCWLProcessingUnit content
    }

let buildRunResolverFromFixtures () =
    crossAsync {
        let! relativePaths = FileSystemHelper.getAllFilePathsAsync TestObjects.IO.testSimpleARCWithCWL
        let cwlRelativePaths =
            relativePaths
            |> Array.filter (fun p -> p.EndsWith(".cwl", System.StringComparison.OrdinalIgnoreCase))
        let! resolvedEntries =
            cwlRelativePaths
            |> Array.map (fun relativePath ->
                crossAsync {
                    let absolutePath = ArcPathHelper.combine TestObjects.IO.testSimpleARCWithCWL relativePath
                    let! content = FileSystemHelper.readFileTextAsync absolutePath
                    let processingUnit = Decode.decodeCWLProcessingUnit content
                    return ArcPathHelper.normalizePathKey relativePath, processingUnit
                }
            )
            |> CrossAsync.all
        let map = resolvedEntries |> Map.ofArray
        return fun (path: string) -> map |> Map.tryFind (ArcPathHelper.normalizePathKey path)
    }
