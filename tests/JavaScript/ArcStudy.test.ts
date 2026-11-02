import {
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow, Comment,
    CWL, DataContext, Datamap, OntologyAnnotation, OntologySourceReference, Person, Publication,
} from "../../src/ARCtrl/index.ts";
import { describe, expect, it } from "vitest";

export const mkDefaultStudy = () => {
    const publication = new Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study");
    const contact = new Person("0000-0002-1825-0097", "Frey", "Kevin");
    const studyDesign = new OntologyAnnotation("metabolite profiling");
    const table = new ArcTable("Measurements");
    const datamap = new Datamap([new DataContext("data-1", "dataset/results.csv")]);
    const comment = new Comment("lastUpdated", "2026-10-29");
    const study = new ArcStudy(
        "study-1",
        "Study title",
        "Study description",
        "2026-10-20",
        "2026-10-29",
        [publication],
        [contact],
        [studyDesign],
        [table],
        datamap,
        ["assay-1"],
        [comment],
    );
    return study;
}

describe("ArcStudy", function () {
    it("primary constructor minimal", function () {
        const study = new ArcStudy("study-1");

        expect(study).toBeInstanceOf(ArcStudy);
        expect(study.Identifier).toBe("study-1");
        expect(study.Title).toBeUndefined();
        expect(study.Description).toBeUndefined();
        expect(study.SubmissionDate).toBeUndefined();
        expect(study.PublicReleaseDate).toBeUndefined();
        expect(study.Publications).toStrictEqual([]);
        expect(study.Contacts).toStrictEqual([]);
        expect(study.StudyDesignDescriptors).toStrictEqual([]);
        expect(study.Tables).toStrictEqual([]);
        expect(study.Datamap).toBeUndefined();
        expect(study.RegisteredAssayIdentifiers).toStrictEqual([]);
        expect(study.Comments).toStrictEqual([]);
        expect(study.Investigation).toBeUndefined();
    });

    it("primary constructor", function () {
        const publication = new Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study");
        const contact = new Person("0000-0002-1825-0097", "Frey", "Kevin");
        const studyDesign = new OntologyAnnotation("metabolite profiling");
        const table = new ArcTable("Measurements");
        const datamap = new Datamap([new DataContext("data-1", "dataset/results.csv")]);
        const comment = new Comment("lastUpdated", "2026-10-29");
        const study = mkDefaultStudy()

        expect(study).toBeInstanceOf(ArcStudy);
        expect(study.Identifier).toBe("study-1");
        expect(study.Title).toBe("Study title");
        expect(study.Description).toBe("Study description");
        expect(study.SubmissionDate).toBe("2026-10-20");
        expect(study.PublicReleaseDate).toBe("2026-10-29");
        expect(study.Publications).toStrictEqual([publication]);
        expect(study.Contacts).toStrictEqual([contact]);
        expect(study.StudyDesignDescriptors).toStrictEqual([studyDesign]);
        expect(study.Tables).toStrictEqual([table]);
        expect(study.Datamap).toStrictEqual(datamap);
        expect(study.RegisteredAssayIdentifiers).toStrictEqual(["assay-1"]);
        expect(study.Comments).toStrictEqual([comment]);
    });

    it("creates independent default comments", function () {
        const first = new ArcStudy("first");
        const second = new ArcStudy("second");

        first.Comments.push(new Comment("key", "value"));

        expect(second.Comments).toStrictEqual([]);
    });
});
