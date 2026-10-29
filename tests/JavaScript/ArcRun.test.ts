import {
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow, Comment,
    CWL, DataContext, Datamap, OntologyAnnotation, OntologySourceReference, Person, Publication,
} from "../../src/ARCtrl/index.ts";
import { describe, expect, it } from "vitest";


describe("ArcRun", function () {
    it("primary constructor minimal", function () {
        const run = new ArcRun("run-1");

        expect(run).toBeInstanceOf(ArcRun);
        expect(run.Identifier).toBe("run-1");
        expect(run.Title).toBeUndefined();
        expect(run.Description).toBeUndefined();
        expect(run.MeasurementType).toBeUndefined();
        expect(run.TechnologyType).toBeUndefined();
        expect(run.TechnologyPlatform).toBeUndefined();
        expect(run.WorkflowIdentifiers).toStrictEqual([]);
        expect(run.Tables).toStrictEqual([]);
        expect(run.Datamap).toBeUndefined();
        expect(run.Performers).toStrictEqual([]);
        expect(run.CWLDescription).toBeUndefined();
        expect(run.CWLInput).toStrictEqual([]);
        expect(run.Comments).toStrictEqual([]);
        expect(run.Investigation).toBeUndefined();
    });

    it("primary constructor", function () {
        const measurementType = new OntologyAnnotation("metabolite profiling");
        const technologyType = new OntologyAnnotation("mass spectrometry");
        const technologyPlatform = new OntologyAnnotation("mass spectrometer");
        const table = new ArcTable("Measurements");
        const datamap = new Datamap([new DataContext("data-1", "dataset/results.csv")]);
        const performer = new Person("0000-0002-1825-0097", "Frey", "Kevin");
        const cwl = CWL.CWLProcessingUnit.commandLineTool(new CWL.CWLToolDescription([], "v1.2", ["echo"]));
        const input = new CWL.CWLParameterReference("message", ["hello"]);
        const comment = new Comment("lastUpdated", "2026-10-29");
        const run = new ArcRun(
            "run-1",
            "Run title",
            "Run description",
            measurementType,
            technologyType,
            technologyPlatform,
            ["workflow-1"],
            [table],
            datamap,
            [performer],
            cwl,
            [input],
            [comment],
        );

        expect(run).toBeInstanceOf(ArcRun);
        expect(run.Identifier).toBe("run-1");
        expect(run.Title).toBe("Run title");
        expect(run.Description).toBe("Run description");
        expect(run.MeasurementType).toBe(measurementType);
        expect(run.TechnologyType).toBe(technologyType);
        expect(run.TechnologyPlatform).toBe(technologyPlatform);
        expect(run.WorkflowIdentifiers).toStrictEqual(["workflow-1"]);
        expect(run.Tables).toStrictEqual([table]);
        expect(run.Datamap).toBe(datamap);
        expect(run.Performers).toStrictEqual([performer]);
        expect(run.CWLDescription).toBe(cwl);
        expect(run.CWLInput).toStrictEqual([input]);
        expect(run.Comments).toStrictEqual([comment]);
    });

    it("creates independent default comments", function () {
        const first = new ArcRun("first");
        const second = new ArcRun("second");

        first.Comments.push(new Comment("key", "value"));

        expect(second.Comments).toStrictEqual([]);
    });
});

