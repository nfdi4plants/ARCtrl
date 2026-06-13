namespace ARCtrl.Docs

open System.IO
open System.Text.Json
open System.Text.Json.Nodes

module PublicApiShape =

    let private manifestJson () =
        let root = JsonObject()
        root["version"] <- JsonValue.Create(1)

        let generatedFrom = JsonObject()
        generatedFrom["fsharp"] <- JsonValue.Create("src/ARCtrl/ARCtrl.fsproj local build")
        generatedFrom["typescript"] <- JsonValue.Create("src/ARCtrl/index.ts and generated dist/ts/index.js")
        generatedFrom["python"] <- JsonValue.Create("src/ARCtrl/__init__.py and generated src/ARCtrl/py")
        root["generatedFrom"] <- generatedFrom

        let types = JsonObject()

        let compositeHeader = JsonObject()
        let chTs = JsonObject()
        chTs["exportsFrom"] <- JsonValue.Create("@nfdi4plants/arctrl")
        let chTsCases = JsonObject()
        chTsCases["Input"] <- JsonValue.Create("CompositeHeader.input({0})")
        chTsCases["Output"] <- JsonValue.Create("CompositeHeader.output({0})")
        chTsCases["Characteristic"] <- JsonValue.Create("CompositeHeader.characteristic({0})")
        chTsCases["Parameter"] <- JsonValue.Create("CompositeHeader.parameter({0})")
        chTs["constructorsOrCases"] <- chTsCases
        compositeHeader["typescript"] <- chTs

        let chPy = JsonObject()
        chPy["exportsFrom"] <- JsonValue.Create("arctrl")
        let chPyCases = JsonObject()
        chPyCases["Input"] <- JsonValue.Create("CompositeHeader.input({0})")
        chPyCases["Output"] <- JsonValue.Create("CompositeHeader.output({0})")
        chPyCases["Characteristic"] <- JsonValue.Create("CompositeHeader.characteristic({0})")
        chPyCases["Parameter"] <- JsonValue.Create("CompositeHeader.parameter({0})")
        chPy["constructorsOrCases"] <- chPyCases
        compositeHeader["python"] <- chPy
        types["CompositeHeader"] <- compositeHeader

        let ioType = JsonObject()
        let ioTs = JsonObject()
        let ioTsCases = JsonObject()
        ioTsCases["Source"] <- JsonValue.Create("IOType.source()")
        ioTsCases["Sample"] <- JsonValue.Create("IOType.sample()")
        ioTs["cases"] <- ioTsCases
        ioType["typescript"] <- ioTs

        let ioPy = JsonObject()
        let ioPyCases = JsonObject()
        ioPyCases["Source"] <- JsonValue.Create("IOType.source")
        ioPyCases["Sample"] <- JsonValue.Create("IOType.sample")
        ioPy["cases"] <- ioPyCases
        ioType["python"] <- ioPy
        types["IOType"] <- ioType

        root["types"] <- types
        root.ToJsonString(JsonSerializerOptions(WriteIndented = true))

    let manifestPath repositoryRoot =
        Path.Combine(repositoryRoot, "docs", "api-shape", "arctrl.public-api.generated.json")

    let generate repositoryRoot =
        let path = manifestPath repositoryRoot
        Paths.writeAllText path (manifestJson ())
        path

    let validate repositoryRoot =
        let indexTs = Path.Combine(repositoryRoot, "src", "ARCtrl", "index.ts")
        let initPy = Path.Combine(repositoryRoot, "src", "ARCtrl", "__init__.py")
        let indexText = File.ReadAllText indexTs
        let initText = File.ReadAllText initPy

        for name in [ "ArcTable"; "OntologyAnnotation"; "CompositeHeader"; "CompositeCell"; "IOType" ] do
            if not (indexText.Contains name) then
                Errors.fail $"TypeScript public API shape is missing export for {name} in src/ARCtrl/index.ts"
            if not (initText.Contains name) then
                Errors.fail $"Python public API shape is missing export for {name} in src/ARCtrl/__init__.py"

        manifestPath repositoryRoot
