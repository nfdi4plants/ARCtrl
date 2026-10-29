import { equal, deepEqual, notEqual } from 'assert';
import {
    XlsxController, ArcAssay, ArcStudy, ArcWorkflow, ArcRun, ArcInvestigation, ArcTable,
    Datamap, DataContext, OntologyAnnotation, OntologySourceReference, Person, Publication, Comment, Component, ScalarValue,
} from "../../src/ARCtrl/index.ts";
import { expect, it, describe } from 'vitest';
import {mkDefaultAssay} from "./ArcAssay.test.ts";
import {mkDefaultTable} from "./ArcTable.test.ts";
import {mkDefaultStudy} from "./ArcStudy.test.ts";

describe('XlsxController', function () {
    it("Assay", function () {
        let assay = mkDefaultAssay()
        let fswb = XlsxController.Assay.toFsWorkbook(assay)
        let assay2 = XlsxController.Assay.fromFsWorkbook(fswb)
        equal(assay.Identifier, assay2.Identifier, "Assay identifier before and after write-read should be same.")
        expect(assay2).toBeInstanceOf(ArcAssay);
        expect(assay2.Title, "Title").toBe(assay.Title);
        expect(assay2.Description, "Description").toBe(assay.Description);
        expect(assay2.MeasurementType, "MeasurementType").toEqual(assay.MeasurementType);
        expect(assay2.TechnologyType, "TechnologyType").toEqual(assay.TechnologyType);
        expect(assay2.TechnologyPlatform, "TechnologyPlatform").toEqual(assay.TechnologyPlatform);
        expect(assay2.Performers, "Performers").toStrictEqual(assay.Performers);
        expect(assay2.Comments, "Comments").toStrictEqual(assay.Comments);
        expect(assay2.Investigation, "Investigation").toBe(assay.Investigation);
    });
    it("Assay - empty table removed on read", function () {
        let assay = mkDefaultAssay()
        expect(assay.TableCount).toBe(1)
        let fswb = XlsxController.Assay.toFsWorkbook(assay)
        let assay2 = XlsxController.Assay.fromFsWorkbook(fswb)
        expect(assay2.TableCount).toBe(0)
    });
    it("Assay - filled table preserved on read", function () {
        let assay = mkDefaultAssay()
        assay.AddTable(mkDefaultTable())
        expect(assay.TableCount).toBe(2)
        let fswb = XlsxController.Assay.toFsWorkbook(assay)
        let assay2 = XlsxController.Assay.fromFsWorkbook(fswb)
        expect(assay2.TableCount).toBe(1)
        expect(assay2.Tables[0].Name).toBe("Filled Measurements");
    });
    it.todo("Assay - Datamap roundtrip", function () {
        let assay = mkDefaultAssay()
        let fswb = XlsxController.Assay.toFsWorkbook(assay)
        let assay2 = XlsxController.Assay.fromFsWorkbook(fswb)
        // console.log(1, assay.Datamap)
        // console.log(2, assay2.Datamap)
        /// This currently fails with a faulty parse for datacontext:
        /// assay: DataContext {
        //   _id: 'data-1',
        //   _selector: undefined,
        //   _filePath: 'dataset/results.csv',
        //   _dataType: undefined,
        //   _format: undefined,
        //   _selectorFormat: undefined,
        //   _comments: [],
        //   _explication: undefined,
        //   _unit: undefined,
        //   _objectType: undefined,
        //   _label: undefined,
        //   _description: undefined,
        //   _generatedBy: undefined
        // }
        // assay2: DataContext {
        //   _id: undefined,
        //   _selector: undefined,
        //   _filePath: 'dataset/results.csv',
        //   _dataType: undefined,
        //   _format: '',
        //   _selectorFormat: '',
        //   _comments: [],
        //   _explication: [OntologyAnnotation],
        //   _unit: [OntologyAnnotation],
        //   _objectType: [OntologyAnnotation],
        //   _label: undefined,
        //   _description: undefined,
        //   _generatedBy: undefined
        // }
        expect(assay2.Datamap).toStrictEqual(new Datamap([]));
    });
    it("Study", function () {
        // Implement the test for Study here
        let study = mkDefaultStudy()
        let fswb = XlsxController.Study.toFsWorkbook(study)
        let [study2, _] = XlsxController.Study.fromFsWorkbook(fswb)
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
    });
    it("Study - empty table is dropped", function () {
        let study = mkDefaultStudy()
        expect(study.TableCount).toBe(1)
        expect(study.Tables[0].Name).toBe("Measurements")
        let fswb = XlsxController.Study.toFsWorkbook(study)
        let [study2, _] = XlsxController.Study.fromFsWorkbook(fswb)
        expect(study2.TableCount).toBe(0)
        expect(study2.Tables).toStrictEqual([])
    })
    it("Study - filled table is preserved", function () {
        let study = mkDefaultStudy()
        study.AddTable(mkDefaultTable())
        expect(study.TableCount).toBe(2)
        expect(study.Tables[0].Name).toBe("Measurements")
        expect(study.Tables[1].Name).toBe("Filled Measurements")
        let fswb = XlsxController.Study.toFsWorkbook(study)
        let [study2, _] = XlsxController.Study.fromFsWorkbook(fswb)
        expect(study2.TableCount).toBe(1)
        expect(study2.Tables[0].Name).toBe("Filled Measurements")
    })
    it.todo("Study - Datamap", function () {
        // same as for assay
        let study = mkDefaultStudy()
        let fswb = XlsxController.Study.toFsWorkbook(study)
        let [study2, _] = XlsxController.Study.fromFsWorkbook(fswb)
        expect(study2.Datamap).toStrictEqual(study.Datamap)
    })
    it("Workflow", function () {
        const workflow = new ArcWorkflow(
            "workflow-1",
            "Workflow title",
            "Workflow description",
            new OntologyAnnotation("data analysis", "OBI", "http://purl.obolibrary.org/obo/OBI_0200000"),
            "workflows/workflow-1/workflow.cwl",
            "1.2.3",
            ["subworkflow-1", "subworkflow-2"],
            [
                new OntologyAnnotation("temperature", "PATO", "PATO:0000146"),
                new OntologyAnnotation("duration", "PATO", "PATO:0001309"),
            ],
            [new Component(new ScalarValue(3, ["instrument"]), undefined, new OntologyAnnotation("equipment", "OBI", "OBI:0000968"))],
            undefined,
            [new Person("0000-0002-1825-0097", "Frey", "Kevin")],
            undefined,
            [new Comment("lastUpdated", "2026-10-29")],
        );
        const fswb = XlsxController.Workflow.toFsWorkbook(workflow);
        const workflow2 = XlsxController.Workflow.fromFsWorkbook(fswb);

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
        expect(workflow2.Contacts).toStrictEqual(workflow.Contacts);
        expect(workflow2.Comments).toStrictEqual(workflow.Comments);
    });
    it("Run", function () {
        const run = new ArcRun(
            "run-1",
            "Run title",
            "Run description",
            new OntologyAnnotation("metabolite profiling", "OBI", "http://purl.obolibrary.org/obo/OBI_0000366"),
            new OntologyAnnotation("mass spectrometry", "OBI", "http://purl.obolibrary.org/obo/OBI_0000470"),
            new OntologyAnnotation("mass spectrometer"),
            ["workflow-1", "workflow-2"],
            [],
            undefined,
            [new Person("0000-0002-1825-0097", "Frey", "Kevin")],
            undefined,
            [],
            [new Comment("lastUpdated", "2026-10-29")],
        );
        const fswb = XlsxController.Run.toFsWorkbook(run);
        const run2 = XlsxController.Run.fromFsWorkbook(fswb);

        expect(run2).toBeInstanceOf(ArcRun);
        expect(run2.Identifier).toBe(run.Identifier);
        expect(run2.Title).toBe(run.Title);
        expect(run2.Description).toBe(run.Description);
        expect(run2.MeasurementType).toStrictEqual(run.MeasurementType);
        expect(run2.TechnologyType).toStrictEqual(run.TechnologyType);
        expect(run2.TechnologyPlatform).toStrictEqual(run.TechnologyPlatform);
        expect(run2.WorkflowIdentifiers).toStrictEqual(run.WorkflowIdentifiers);
        expect(run2.Performers).toStrictEqual(run.Performers);
        expect(run2.Comments).toStrictEqual(run.Comments);
        expect(run2.Tables).toStrictEqual([]);
    });
    it("Run - empty table is dropped", function () {
        const run = new ArcRun("run-1");
        run.AddTable(new ArcTable("Measurements"));
        expect(run.TableCount).toBe(1);

        const fswb = XlsxController.Run.toFsWorkbook(run);
        const run2 = XlsxController.Run.fromFsWorkbook(fswb);

        expect(run2.TableCount).toBe(0);
        expect(run2.Tables).toStrictEqual([]);
    });
    it("Run - filled table is preserved", function () {
        const run = new ArcRun("run-1");
        const table = mkDefaultTable();
        run.AddTable(new ArcTable("Measurements"));
        run.AddTable(table);
        expect(run.TableCount).toBe(2);

        const fswb = XlsxController.Run.toFsWorkbook(run);
        const run2 = XlsxController.Run.fromFsWorkbook(fswb);

        expect(run2.TableCount).toBe(1);
        expect(run2.Tables[0]).toBeInstanceOf(ArcTable);
        expect(run2.Tables[0].Name).toBe(table.Name);
        expect(run2.Tables[0].ColumnCount).toBe(table.ColumnCount);
        expect(run2.Tables[0].RowCount).toBe(table.RowCount);
        expect(run2.Tables[0].GetCellAt(3, 0).fields).toStrictEqual(["first row"]);
        expect(run2.Tables[0].GetCellAt(3, 1).fields).toStrictEqual(["second row"]);
        const parameterIndex = run2.Tables[0].Headers.findIndex(header => header.tag === 3);
        expect(parameterIndex).toBeGreaterThanOrEqual(0);
        const unitized = run2.Tables[0].GetCellAt(parameterIndex, 0);
        expect(unitized.tag).toBe(2);
        if (unitized.tag === 2) {
            expect(unitized.fields[0]).toBe("5");
            expect(unitized.fields[1].Name).toBe("gram");
            expect(unitized.fields[1].TermSourceREF).toBe("UO");
        }
    });
    it("Investigation", function () {
        const study = mkDefaultStudy();
        const assay = mkDefaultAssay();
        const investigation = new ArcInvestigation(
            "investigation-1",
            "Investigation title",
            "Investigation description",
            "2026-10-20",
            "2026-10-29",
            [new OntologySourceReference("Ontology for Biomedical Investigations", "http://purl.obolibrary.org/obo/obi.owl", "OBI", "2026-10-01")],
            [new Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")],
            [new Person("0000-0002-1825-0097", "Frey", "Kevin")],
            [assay],
            [study],
            [],
            [],
            [study.Identifier],
            [new Comment("lastUpdated", "2026-10-29")],
        );
        const fswb = XlsxController.Investigation.toFsWorkbook(investigation);
        const investigation2 = XlsxController.Investigation.fromFsWorkbook(fswb);

        expect(investigation2).toBeInstanceOf(ArcInvestigation);
        expect(investigation2.Identifier).toBe(investigation.Identifier);
        expect(investigation2.Title).toBe(investigation.Title);
        expect(investigation2.Description).toBe(investigation.Description);
        expect(investigation2.SubmissionDate).toBe(investigation.SubmissionDate);
        expect(investigation2.PublicReleaseDate).toBe(investigation.PublicReleaseDate);
        expect(investigation2.OntologySourceReferences).toStrictEqual(investigation.OntologySourceReferences);
        expect(investigation2.Publications).toStrictEqual(investigation.Publications);
        expect(investigation2.Contacts).toStrictEqual(investigation.Contacts);
        expect(investigation2.Comments).toStrictEqual(investigation.Comments);
        expect(investigation2.RegisteredStudyIdentifiers).toStrictEqual([study.Identifier]);
        expect(investigation2.Studies).toHaveLength(1);
        expect(investigation2.Studies[0]).toBeInstanceOf(ArcStudy);
        expect(investigation2.Studies[0].Identifier).toBe(study.Identifier);
        expect(investigation2.Studies[0].Title).toBe(study.Title);
        expect(investigation2.Studies[0].RegisteredAssayIdentifiers).toStrictEqual([assay.Identifier]);
        expect(investigation2.Studies[0].Investigation).toBe(investigation2);
        expect(investigation2.Assays).toHaveLength(1);
        expect(investigation2.Assays[0]).toBeInstanceOf(ArcAssay);
        expect(investigation2.Assays[0].Identifier).toBe(assay.Identifier);
        expect(investigation2.Assays[0].Title).toBe(assay.Title);
        expect(investigation2.Assays[0].Investigation).toBe(investigation2);
    });
    it("Investigation - no registered studies", function () {
        const investigation = new ArcInvestigation("investigation-1");
        const fswb = XlsxController.Investigation.toFsWorkbook(investigation);
        const investigation2 = XlsxController.Investigation.fromFsWorkbook(fswb);

        expect(investigation2.Identifier).toBe(investigation.Identifier);
        expect(investigation2.RegisteredStudyIdentifiers).toStrictEqual([]);
        expect(investigation2.Studies).toStrictEqual([]);
        expect(investigation2.Assays).toStrictEqual([]);
    });
    it("Datamap - empty", function () {
        const datamap = new Datamap([]);
        const fswb = XlsxController.Datamap.toFsWorkbook(datamap);
        const datamap2 = XlsxController.Datamap.fromFsWorkbook(fswb);

        expect(datamap2).toBeInstanceOf(Datamap);
        expect(datamap2.DataContexts).toStrictEqual([]);
    });
    it("Datamap - populated contexts", function () {
        // Spreadsheet columns preserve data locations and metadata, but do not contain IDs or DataType.
        const datamap = new Datamap([
            new DataContext(
                undefined, "data/results.csv#row=1", undefined, "text/csv", "RFC 7111",
                new OntologyAnnotation("mass", "MS", "MS:1000001"),
                new OntologyAnnotation("gram", "UO", "UO:0000021"),
                new OntologyAnnotation("sample", "OBI", "OBI:0000747"),
                "Mass", "A measured mass", "run-1", [new Comment("quality", "checked")],
            ),
            new DataContext(
                undefined, "data/results.csv#row=2", undefined, "text/csv", "RFC 7111",
                new OntologyAnnotation("mass", "MS", "MS:1000001"),
                new OntologyAnnotation("gram", "UO", "UO:0000021"),
                new OntologyAnnotation("sample", "OBI", "OBI:0000747"),
                "Second mass", "Another measured mass", "run-2", [new Comment("quality", "reviewed")],
            ),
        ]);
        const fswb = XlsxController.Datamap.toFsWorkbook(datamap);
        const datamap2 = XlsxController.Datamap.fromFsWorkbook(fswb);

        expect(datamap2).toBeInstanceOf(Datamap);
        expect(datamap2.DataContexts).toHaveLength(2);
        datamap.DataContexts.forEach((context, index) => {
            const context2 = datamap2.DataContexts[index];
            expect(context2).toBeInstanceOf(DataContext);
            expect(context2.Name).toBe(context.Name);
            expect(context2.FilePath).toBe(context.FilePath);
            expect(context2.Selector).toBe(context.Selector);
            expect(context2.Format).toBe(context.Format);
            expect(context2.SelectorFormat).toBe(context.SelectorFormat);
            expect(context2.Explication).toStrictEqual(context.Explication);
            expect(context2.Unit).toStrictEqual(context.Unit);
            expect(context2.ObjectType).toStrictEqual(context.ObjectType);
            expect(context2.Label).toBe(context.Label);
            expect(context2.Description).toBe(context.Description);
            expect(context2.GeneratedBy).toBe(context.GeneratedBy);
            expect(context2.Comments).toStrictEqual(context.Comments);
        });
    });
});
