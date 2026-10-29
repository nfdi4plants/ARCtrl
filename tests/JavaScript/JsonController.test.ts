import { deepEqual } from 'assert';
import {
    ARC, ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow, Comment,
    Component, CompositeCell, CompositeHeader, Data, DataContext, DataFile, Datamap, IOType, JsonController, ScalarValue,
    OntologyAnnotation, OntologySourceReference, Organisation, Person, Publication, ROCrate, Template,
} from "../../src/ARCtrl/index.ts"
import { expect, it, describe } from 'vitest';
import { mkDefaultAssay } from "./ArcAssay.test.ts";
import { mkDefaultStudy } from "./ArcStudy.test.ts";
import { mkDefaultTable } from "./ArcTable.test.ts";

const mkDataContext = () => new DataContext(
    "context-1", "dataset/results.csv#row=2", new DataFile(1, []), "text/csv", "RFC 7111",
    new OntologyAnnotation("mass", "MS", "MS:1000001"),
    new OntologyAnnotation("gram", "UO", "UO:0000021"),
    new OntologyAnnotation("sample", "OBI", "OBI:0000747"),
    "Sample mass", "Measured sample mass", "run-1", [new Comment("quality", "checked")],
);

const mkPerson = () => new Person(
    "0000-0002-1825-0097", "Frey", "Kevin", "K. F.", "kevin@example.org",
    "+49 123 456", "+49 123 457", "Example Street 1", "Example University",
    [new OntologyAnnotation("investigator", "OBI", "OBI:0000202")],
    [new Comment("department", "biology")],
);

const mkWorkflow = () => new ArcWorkflow(
    "workflow-1", "Workflow title", "Workflow description", new OntologyAnnotation("data analysis"),
    "workflows/workflow-1/workflow.cwl", "1.2.3", ["subworkflow-1"],
    [new OntologyAnnotation("temperature")],
    [new Component(new ScalarValue(3, ["instrument"]), undefined, new OntologyAnnotation("equipment"))],
    new Datamap([mkDataContext()]), [mkPerson()], undefined, [new Comment("version", "reviewed")],
);

const mkRun = () => new ArcRun(
    "run-1", "Run title", "Run description", new OntologyAnnotation("metabolite profiling"),
    new OntologyAnnotation("mass spectrometry"), new OntologyAnnotation("mass spectrometer"),
    ["workflow-1"], [mkDefaultTable()], new Datamap([mkDataContext()]), [mkPerson()],
    undefined, undefined, [new Comment("quality", "checked")],
);

const mkInvestigation = () => {
    const assay = mkDefaultAssay();
    assay.AddTable(mkDefaultTable());
    const study = mkDefaultStudy();
    study.AddTable(mkDefaultTable());
    return new ArcInvestigation(
        "investigation-1", "Investigation title", "Investigation description", "2026-10-20", "2026-10-29",
        [new OntologySourceReference("Ontology description", "ontology.obo", "OBI", "1.0")],
        [new Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")],
        [mkPerson()], [assay], [study], undefined, undefined, [study.Identifier],
        [new Comment("quality", "checked")],
    );
};

const expectTablesEqual = (actual: ArcTable[], expected: ArcTable[]) => {
    expect(actual).toHaveLength(expected.length);
    expected.forEach((table, index) => {
        const table2 = actual[index];
        expect(table2).toBeInstanceOf(ArcTable);
        expect(table2.Name).toBe(table.Name);
        expect(table2.Headers).toStrictEqual(table.Headers);
        expect(table2.RowCount).toBe(table.RowCount);
        expect(table2.ColumnCount).toBe(table.ColumnCount);
        expect(table2.Columns).toStrictEqual(table.Columns);
    });
};

describe('JsonController', function () {
    it("OntologyAnnotation", function () {
        let oa = new OntologyAnnotation("instrument model", "MS", "MS:08239081")
        let json = JsonController.OntologyAnnotation.toJsonString(oa)
        let oa2 = JsonController.OntologyAnnotation.fromJsonString(json)
        deepEqual(oa, oa2, "OntologyAnnotation before and after write-read should be same.")
    });

    it("Person", function () {
        let person = mkPerson()
        let json = JsonController.Person.toJsonString(person)
        let person2 = JsonController.Person.fromJsonString(json)
        expect(person2).toBeInstanceOf(Person);
        expect(person2).toStrictEqual(person);
    });

    it("Datamap", function () {
        let datamap = new Datamap([mkDataContext(), new DataContext("context-2", "dataset/image.png")])
        let json = JsonController.Datamap.toJsonString(datamap)
        let datamap2 = JsonController.Datamap.fromJsonString(json)
        expect(datamap2).toBeInstanceOf(Datamap);
        expect(datamap2.DataContexts).toStrictEqual(datamap.DataContexts);
        expect(datamap2.DataContexts[0]).toBeInstanceOf(DataContext);
    });

    it("Datamap - empty", function () {
        let datamap = new Datamap([])
        let json = JsonController.Datamap.toJsonString(datamap)
        let datamap2 = JsonController.Datamap.fromJsonString(json)
        expect(datamap2).toBeInstanceOf(Datamap);
        expect(datamap2.DataContexts).toStrictEqual([]);
    });

    for (const compressed of [false, true]) {
        const suffix = compressed ? " - compressed" : "";

        it("Assay" + suffix, function () {
            let assay = mkDefaultAssay()
            assay.AddTable(mkDefaultTable())
            let json = compressed
                ? JsonController.Assay.toCompressedJsonString(assay)
                : JsonController.Assay.toJsonString(assay)
            let assay2 = compressed
                ? JsonController.Assay.fromCompressedJsonString(json)
                : JsonController.Assay.fromJsonString(json)
            expect(assay2).toBeInstanceOf(ArcAssay);
            expect(assay2.Identifier).toBe(assay.Identifier);
            expect(assay2.Title).toBe(assay.Title);
            expect(assay2.Description).toBe(assay.Description);
            expect(assay2.MeasurementType).toStrictEqual(assay.MeasurementType);
            expect(assay2.TechnologyType).toStrictEqual(assay.TechnologyType);
            expect(assay2.TechnologyPlatform).toStrictEqual(assay.TechnologyPlatform);
            expect(assay2.Performers).toStrictEqual(assay.Performers);
            expect(assay2.Comments).toStrictEqual(assay.Comments);
            expect(assay2.Datamap).toStrictEqual(assay.Datamap);
            expectTablesEqual(assay2.Tables, assay.Tables);
            expect(assay2.Investigation).toBeUndefined();
        });

        it("Study" + suffix, function () {
            let study = mkDefaultStudy()
            study.AddTable(mkDefaultTable())
            let json = compressed
                ? JsonController.Study.toCompressedJsonString(study)
                : JsonController.Study.toJsonString(study)
            let study2 = compressed
                ? JsonController.Study.fromCompressedJsonString(json)
                : JsonController.Study.fromJsonString(json)
            expect(study2).toBeInstanceOf(ArcStudy);
            expect(study2.Identifier).toBe(study.Identifier);
            expect(study2.Title).toBe(study.Title);
            expect(study2.Description).toBe(study.Description);
            expect(study2.SubmissionDate).toBe(study.SubmissionDate);
            expect(study2.PublicReleaseDate).toBe(study.PublicReleaseDate);
            expect(study2.Publications).toStrictEqual(study.Publications);
            expect(study2.Contacts).toStrictEqual(study.Contacts);
            expect(study2.StudyDesignDescriptors).toStrictEqual(study.StudyDesignDescriptors);
            expect(study2.RegisteredAssayIdentifiers).toStrictEqual(study.RegisteredAssayIdentifiers);
            expect(study2.Comments).toStrictEqual(study.Comments);
            expect(study2.Datamap).toStrictEqual(study.Datamap);
            expectTablesEqual(study2.Tables, study.Tables);
            expect(study2.Investigation).toBeUndefined();
        });

        it("Workflow" + suffix, function () {
            let workflow = mkWorkflow()
            let json = compressed
                ? JsonController.Workflow.toCompressedJsonString(workflow)
                : JsonController.Workflow.toJsonString(workflow)
            let workflow2 = compressed
                ? JsonController.Workflow.fromCompressedJsonString(json)
                : JsonController.Workflow.fromJsonString(json)
            expect(workflow2).toBeInstanceOf(ArcWorkflow);
            expect(workflow2.Identifier).toBe(workflow.Identifier);
            expect(workflow2.Title).toBe(workflow.Title);
            expect(workflow2.Description).toBe(workflow.Description);
            expect(workflow2.WorkflowType).toStrictEqual(workflow.WorkflowType);
            expect(workflow2.URI).toBe(workflow.URI);
            expect(workflow2.Version).toBe(workflow.Version);
            expect(workflow2.SubWorkflowIdentifiers).toStrictEqual(workflow.SubWorkflowIdentifiers);
            expect(workflow2.Parameters).toStrictEqual(workflow.Parameters);
            expect(workflow2.Components).toStrictEqual(workflow.Components);
            expect(workflow2.Datamap).toStrictEqual(workflow.Datamap);
            expect(workflow2.Contacts).toStrictEqual(workflow.Contacts);
            expect(workflow2.Comments).toStrictEqual(workflow.Comments);
            expect(workflow2.Investigation).toBeUndefined();
        });

        it("Run" + suffix, function () {
            let run = mkRun()
            let json = compressed
                ? JsonController.Run.toCompressedJsonString(run)
                : JsonController.Run.toJsonString(run)
            let run2 = compressed
                ? JsonController.Run.fromCompressedJsonString(json)
                : JsonController.Run.fromJsonString(json)
            expect(run2).toBeInstanceOf(ArcRun);
            expect(run2.Identifier).toBe(run.Identifier);
            expect(run2.Title).toBe(run.Title);
            expect(run2.Description).toBe(run.Description);
            expect(run2.MeasurementType).toStrictEqual(run.MeasurementType);
            expect(run2.TechnologyType).toStrictEqual(run.TechnologyType);
            expect(run2.TechnologyPlatform).toStrictEqual(run.TechnologyPlatform);
            expect(run2.WorkflowIdentifiers).toStrictEqual(run.WorkflowIdentifiers);
            expectTablesEqual(run2.Tables, run.Tables);
            expect(run2.Datamap).toStrictEqual(run.Datamap);
            expect(run2.Performers).toStrictEqual(run.Performers);
            expect(run2.Comments).toStrictEqual(run.Comments);
            expect(run2.Investigation).toBeUndefined();
        });

        it("Investigation" + suffix, function () {
            let investigation = mkInvestigation()
            let json = compressed
                ? JsonController.Investigation.toCompressedJsonString(investigation)
                : JsonController.Investigation.toJsonString(investigation)
            let investigation2 = compressed
                ? JsonController.Investigation.fromCompressedJsonString(json)
                : JsonController.Investigation.fromJsonString(json)
            expect(investigation2).toBeInstanceOf(ArcInvestigation);
            expect(investigation2.Identifier).toBe(investigation.Identifier);
            expect(investigation2.Title).toBe(investigation.Title);
            expect(investigation2.Description).toBe(investigation.Description);
            expect(investigation2.SubmissionDate).toBe(investigation.SubmissionDate);
            expect(investigation2.PublicReleaseDate).toBe(investigation.PublicReleaseDate);
            expect(investigation2.OntologySourceReferences).toStrictEqual(investigation.OntologySourceReferences);
            expect(investigation2.Publications).toStrictEqual(investigation.Publications);
            expect(investigation2.Contacts).toStrictEqual(investigation.Contacts);
            expect(investigation2.RegisteredStudyIdentifiers).toStrictEqual(investigation.RegisteredStudyIdentifiers);
            expect(investigation2.Comments).toStrictEqual(investigation.Comments);
            expect(investigation2.Assays).toHaveLength(1);
            expect(investigation2.Studies).toHaveLength(1);
            expect(investigation2.Assays[0]).toBeInstanceOf(ArcAssay);
            expect(investigation2.Studies[0]).toBeInstanceOf(ArcStudy);
            expect(JSON.parse(JsonController.Assay.toJsonString(investigation2.Assays[0])))
                .toStrictEqual(JSON.parse(JsonController.Assay.toJsonString(investigation.Assays[0])));
            expect(JSON.parse(JsonController.Study.toJsonString(investigation2.Studies[0])))
                .toStrictEqual(JSON.parse(JsonController.Study.toJsonString(investigation.Studies[0])));
            expectTablesEqual(investigation2.Assays[0].Tables, investigation.Assays[0].Tables);
            expectTablesEqual(investigation2.Studies[0].Tables, investigation.Studies[0].Tables);
            expect(investigation2.Assays[0].Investigation).toBe(investigation2);
            expect(investigation2.Studies[0].Investigation).toBe(investigation2);
            expect(investigation2.RegisteredStudies[0]).toBe(investigation2.Studies[0]);
            expect(investigation2.Studies[0].RegisteredAssays[0]).toBe(investigation2.Assays[0]);
        });

        it("ArcTable" + suffix, function () {
            let table = mkDefaultTable()
            let json = compressed
                ? JsonController.ArcTable.toCompressedJsonString(table)
                : JsonController.ArcTable.toJsonString(table)
            let table2 = compressed
                ? JsonController.ArcTable.fromCompressedJsonString(json)
                : JsonController.ArcTable.fromJsonString(json)
            expect(table2).toBeInstanceOf(ArcTable);
            expect(table2.Name).toBe(table.Name);
            expect(table2.Headers).toStrictEqual(table.Headers);
            expect(table2.RowCount).toBe(table.RowCount);
            expect(table2.ColumnCount).toBe(table.ColumnCount);
            expect(table2.Columns).toStrictEqual(table.Columns);
        });

        it("Template" + suffix, function () {
            let template = new Template(
                "12345678-1234-1234-1234-123456789abc", mkDefaultTable(), "Measurements", "Measurement template",
                new Organisation(1, ["Example organisation"]), "1.2.3", [mkPerson()],
                [new OntologyAnnotation("repository")], [new OntologyAnnotation("metabolomics")],
                new Date("2026-10-29T10:00:00Z"),
            )
            let json = compressed
                ? JsonController.Template.toCompressedJsonString(template)
                : JsonController.Template.toJsonString(template)
            let template2 = compressed
                ? JsonController.Template.fromCompressedJsonString(json)
                : JsonController.Template.fromJsonString(json)
            expect(template2).toBeInstanceOf(Template);
            expect(template2.Id).toBe(template.Id);
            expect(template2.Name).toBe(template.Name);
            expect(template2.Description).toBe(template.Description);
            expect(template2.Organisation).toStrictEqual(template.Organisation);
            expect(template2.Version).toBe(template.Version);
            expect(template2.Authors).toStrictEqual(template.Authors);
            expect(template2.EndpointRepositories).toStrictEqual(template.EndpointRepositories);
            expect(template2.Tags).toStrictEqual(template.Tags);
            expect(template2.LastUpdated.getTime()).toBe(template.LastUpdated.getTime());
            expectTablesEqual([template2.Table], [template.Table]);
        });
    }

    it("Comment", function () {
        let comment = new Comment("quality", "checked")
        let json = JsonController.Comment.toJsonString(comment)
        let comment2 = JsonController.Comment.fromJsonString(json)
        expect(comment2).toBeInstanceOf(Comment);
        expect(comment2).toStrictEqual(comment);
    });

    it("OntologySourceReference", function () {
        let reference = new OntologySourceReference(
            "Ontology description", "ontology.obo", "OBI", "1.0", [new Comment("source", "curated")],
        )
        let json = JsonController.OntologySourceReference.toJsonString(reference)
        let reference2 = JsonController.OntologySourceReference.fromJsonString(json)
        expect(reference2).toBeInstanceOf(OntologySourceReference);
        expect(reference2).toStrictEqual(reference);
    });

    it("Publication", function () {
        let publication = new Publication(
            "12345678", "10.1000/example", "Kevin Frey", "ARC study",
            new OntologyAnnotation("published", "OBI", "OBI:0000605"), [new Comment("year", "2026")],
        )
        let json = JsonController.Publication.toJsonString(publication)
        let publication2 = JsonController.Publication.fromJsonString(json)
        expect(publication2).toBeInstanceOf(Publication);
        expect(publication2).toStrictEqual(publication);
    });

    it("Data", function () {
        let data = new Data(
            "data-1", "dataset/results.csv#row=2", new DataFile(1, []), "text/csv", "RFC 7111",
            [new Comment("quality", "checked")],
        )
        let json = JsonController.Data.toJsonString(data)
        let data2 = JsonController.Data.fromJsonString(json)
        expect(data2).toBeInstanceOf(Data);
        expect(data2).toStrictEqual(data);
    });

    it("DataContext", function () {
        let context = mkDataContext()
        let json = JsonController.DataContext.toJsonString(context)
        let context2 = JsonController.DataContext.fromJsonString(json)
        expect(context2).toBeInstanceOf(DataContext);
        expect(context2).toStrictEqual(context);
    });

    it("DataContext - empty", function () {
        let context = new DataContext()
        let json = JsonController.DataContext.toJsonString(context)
        let context2 = JsonController.DataContext.fromJsonString(json)
        expect(context2).toStrictEqual(context);
    });

    for (const format of ["ISA", "RO-Crate"]) {
        it("Comment - " + format, function () {
            let comment = new Comment("quality", "checked")
            let json = format === "ISA"
                ? JsonController.Comment.toISAJsonString(comment, 2)
                : JsonController.Comment.toROCrateJsonString(comment, 2)
            let comment2 = format === "ISA"
                ? JsonController.Comment.fromISAJsonString(json)
                : JsonController.Comment.fromROCrateJsonString(json)
            expect(json).toContain('\n  "');
            expect(comment2).toBeInstanceOf(Comment);
            expect(comment2).toStrictEqual(comment);
        });

        it("OntologySourceReference - " + format, function () {
            let reference = new OntologySourceReference(
                "Ontology description", "ontology.obo", "OBI", "1.0", [new Comment("source", "curated")],
            )
            let json = format === "ISA"
                ? JsonController.OntologySourceReference.toISAJsonString(reference, 2)
                : JsonController.OntologySourceReference.toROCrateJsonString(reference, 2)
            let reference2 = format === "ISA"
                ? JsonController.OntologySourceReference.fromISAJsonString(json)
                : JsonController.OntologySourceReference.fromROCrateJsonString(json)
            expect(json).toContain('\n  "');
            expect(reference2).toBeInstanceOf(OntologySourceReference);
            expect(reference2).toStrictEqual(reference);
        });

        it("Publication - " + format, function () {
            let publication = new Publication(
                "12345678", "10.1000/example", "Frey, Kevin", "ARC study",
                new OntologyAnnotation("published", "OBI", "OBI:0000605"), [new Comment("year", "2026")],
            )
            let json = format === "ISA"
                ? JsonController.Publication.toISAJsonString(publication, 2, true)
                : JsonController.Publication.toROCrateJsonString(publication, 2)
            let publication2 = format === "ISA"
                ? JsonController.Publication.fromISAJsonString(json)
                : JsonController.Publication.fromROCrateJsonString(json)
            expect(json).toContain('\n  "');
            expect(publication2).toBeInstanceOf(Publication);
            expect(publication2).toStrictEqual(publication);
        });

        it("Data - " + format, function () {
            let data = new Data(
                "data-1", "dataset/results.csv#row=2", new DataFile(1, []),
                format === "ISA" ? undefined : "text/csv", format === "ISA" ? undefined : "RFC 7111",
                [new Comment("quality", "checked")],
            )
            let json = format === "ISA"
                ? JsonController.Data.toISAJsonString(data, 2, true)
                : JsonController.Data.toROCrateJsonString(data, 2)
            let data2 = format === "ISA"
                ? JsonController.Data.fromISAJsonString(json)
                : JsonController.Data.fromROCrateJsonString(json)
            expect(json).toContain('\n  "');
            expect(data2).toBeInstanceOf(Data);
            expect(data2).toStrictEqual(data);
        });
    }

    it.each([DataFile.derivedDataFile(), DataFile.rawDataFile(), DataFile.imageFile()] as const)("DataFile - case %s", function (dataFile) {
        let isaJson = JsonController.DataFile.toISAJsonString(dataFile)
        let roCrateJson = JsonController.DataFile.toROCrateJsonString(dataFile)
        expect(JsonController.DataFile.fromISAJsonString(isaJson)).toStrictEqual(dataFile);
        expect(JsonController.DataFile.fromROCrateJsonString(roCrateJson)).toStrictEqual(dataFile);
    });

    it.each([
        IOType.source(), IOType.sample(), IOType.data(), IOType.material(), IOType.freeText("Custom input"),
    ])("IOType - %s", function (ioType) {
        let json = JsonController.IOType.toJsonString(ioType)
        let ioType2 = JsonController.IOType.fromJsonString(json)
        expect(ioType2).toBeInstanceOf(IOType);
        expect(ioType2).toStrictEqual(ioType);
    });

    it.each([
        CompositeCell.term(new OntologyAnnotation("organism", "OBI", "OBI:0100026", [new Comment("term", "curated")])),
        CompositeCell.freeText("A value with \"quotes\", a newline\n and ünicode"),
        CompositeCell.unitized("5", new OntologyAnnotation("gram", "UO", "UO:0000021")),
        new CompositeCell(3, [new Data("data-1", "dataset/results.csv#row=2", new DataFile(1, []))]),
    ])("CompositeCell - %s", function (cell) {
        let json = JsonController.CompositeCell.toJsonString(cell)
        let cell2 = JsonController.CompositeCell.fromJsonString(json)
        expect(cell2).toBeInstanceOf(CompositeCell);
        expect(cell2).toStrictEqual(cell);
    });

    it.each([
        CompositeHeader.parameter(new OntologyAnnotation("temperature", "PATO", "PATO:0000146")),
        CompositeHeader.characteristic(new OntologyAnnotation("organism")),
        CompositeHeader.factor(new OntologyAnnotation("treatment")),
        CompositeHeader.component(new OntologyAnnotation("instrument")),
        CompositeHeader.protocolType(), CompositeHeader.protocolREF(), CompositeHeader.protocolDescription(),
        CompositeHeader.protocolUri(), CompositeHeader.protocolVersion(), CompositeHeader.performer(), CompositeHeader.date(),
        CompositeHeader.input(IOType.source()), CompositeHeader.output(IOType.data()),
        CompositeHeader.freeText("Notes"), CompositeHeader.comment("quality"),
    ])("CompositeHeader - %s", function (header) {
        let json = JsonController.CompositeHeader.toJsonString(header)
        let header2 = JsonController.CompositeHeader.fromJsonString(json)
        expect(header2).toBeInstanceOf(CompositeHeader);
        expect(header2).toStrictEqual(header);
    });

    it("JSON indentation", function () {
        let comment = new Comment("quality", "checked")
        let json = JsonController.Comment.toJsonString(comment, 4)
        expect(json).toContain('\n    "');
        expect(JsonController.Comment.fromJsonString(json)).toStrictEqual(comment);
    });

    it("invalid JSON is rejected", function () {
        expect(() => JsonController.Comment.fromJsonString("invalid JSON")).toThrow();
        expect(() => JsonController.DataContext.fromJsonString("{}")).toThrow();
        expect(() => JsonController.CompositeCell.fromJsonString('{"celltype":"Unknown","values":[]}')).toThrow();
        expect(() => JsonController.ArcTable.fromJsonString("{}")).toThrow();
        expect(() => JsonController.Template.fromJsonString("{}")).toThrow();
    });

    it("LDNode - RO-Crate", function () {
        let node = new ROCrate.LDNode("#person-1", ["Person"])
        node.SetProperty("name", "Kevin Frey")
        let json = JsonController.LDNode.toROCrateJsonString()(node)
        let node2 = JsonController.LDNode.fromROCrateJsonString(json)
        expect(node2).toBeInstanceOf(ROCrate.LDNode);
        expect(JSON.parse(JsonController.LDNode.toROCrateJsonString()(node2))).toStrictEqual(JSON.parse(json));
        expect(JSON.parse(json)).toMatchObject({ "@id": "#person-1", name: "Kevin Frey" });
    });

    it("LDGraph - RO-Crate", function () {
        let node = new ROCrate.LDNode("#person-1", ["Person"])
        node.SetProperty("name", "Kevin Frey")
        let graph = new ROCrate.LDGraph(undefined, [node])
        let json = JsonController.LDGraph.toROCrateJsonString()(graph)
        let graph2 = JsonController.LDGraph.fromROCrateJsonString(json)
        expect(graph2).toBeInstanceOf(ROCrate.LDGraph);
        expect(graph2.Nodes).toHaveLength(1);
        expect(JSON.parse(JsonController.LDGraph.toROCrateJsonString()(graph2))).toStrictEqual(JSON.parse(json));
    });

    it("ARC - RO-Crate", function () {
        let arc = new ARC("arc-1", "ARC title", "ARC description")
        let json = JsonController.ARC.toROCrateJsonString()(arc)
        let arc2 = JsonController.ARC.fromROCrateJsonString(json)
        expect(arc2).toBeInstanceOf(ARC);
        expect(arc2.Identifier).toBe(arc.Identifier);
        expect(arc2.Title).toBe(arc.Title);
        expect(arc2.Description).toBe(arc.Description);
    });
});
