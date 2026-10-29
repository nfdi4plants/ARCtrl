
import {
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow, Comment,
    CWL, DataContext, Datamap, OntologyAnnotation, OntologySourceReference, Person, Publication,
} from "../../src/ARCtrl/index.ts";
import { describe, expect, it } from "vitest";

describe("ArcAssay", function () {
    it("primary constructor minimal", function () {
        const assay = new ArcAssay("assay-1");

        expect(assay).toBeInstanceOf(ArcAssay);
        expect(assay.Identifier).toBe("assay-1");
        expect(assay.Title).toBeUndefined();
        expect(assay.Description).toBeUndefined();
        expect(assay.MeasurementType).toBeUndefined();
        expect(assay.TechnologyType).toBeUndefined();
        expect(assay.TechnologyPlatform).toBeUndefined();
        expect(assay.Tables).toStrictEqual([]);
        expect(assay.Datamap).toBeUndefined();
        expect(assay.Performers).toStrictEqual([]);
        expect(assay.Comments).toStrictEqual([]);
        expect(assay.Investigation).toBeUndefined();
    });

    it("primary constructor", function () {
        const measurementType = new OntologyAnnotation("metabolite profiling");
        const technologyType = new OntologyAnnotation("mass spectrometry");
        const technologyPlatform = new OntologyAnnotation("mass spectrometer");
        const table = new ArcTable("Measurements");
        const datamap = new Datamap([new DataContext("data-1", "dataset/results.csv")]);
        const performer = new Person("0000-0002-1825-0097", "Frey", "Kevin");
        const comment = new Comment("lastUpdated", "2026-10-29");
        const assay = new ArcAssay(
            "assay-1",
            "Assay title",
            "Assay description",
            measurementType,
            technologyType,
            technologyPlatform,
            [table],
            datamap,
            [performer],
            [comment],
        );

        expect(assay).toBeInstanceOf(ArcAssay);
        expect(assay.Identifier).toBe("assay-1");
        expect(assay.Title).toBe("Assay title");
        expect(assay.Description).toBe("Assay description");
        expect(assay.MeasurementType).toBe(measurementType);
        expect(assay.TechnologyType).toBe(technologyType);
        expect(assay.TechnologyPlatform).toBe(technologyPlatform);
        expect(assay.Tables).toStrictEqual([table]);
        expect(assay.Datamap).toBe(datamap);
        expect(assay.Performers).toStrictEqual([performer]);
        expect(assay.Comments).toStrictEqual([comment]);
    });

    it("creates independent default comments", function () {
        const first = new ArcAssay("first");
        const second = new ArcAssay("second");

        first.Comments.push(new Comment("key", "value"));

        expect(second.Comments).toStrictEqual([]);
    });
});