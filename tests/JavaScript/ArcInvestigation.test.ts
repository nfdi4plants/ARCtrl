import {
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow, Comment,
    CWL, DataContext, Datamap, OntologyAnnotation, OntologySourceReference, Person, Publication,
} from "../../src/ARCtrl/index.ts";
import { Remark } from "../../src/ARCtrl/index.ts";
import { describe, expect, it } from "vitest";



describe("ArcInvestigation", function () {
    it("primary constructor minimal", function () {
        const investigation = new ArcInvestigation("investigation-1");

        expect(investigation).toBeInstanceOf(ArcInvestigation);
        expect(investigation.Identifier).toBe("investigation-1");
        expect(investigation.Title).toBeUndefined();
        expect(investigation.Description).toBeUndefined();
        expect(investigation.SubmissionDate).toBeUndefined();
        expect(investigation.PublicReleaseDate).toBeUndefined();
        expect(investigation.OntologySourceReferences).toStrictEqual([]);
        expect(investigation.Publications).toStrictEqual([]);
        expect(investigation.Contacts).toStrictEqual([]);
        expect(investigation.Assays).toStrictEqual([]);
        expect(investigation.Studies).toStrictEqual([]);
        expect(investigation.Workflows).toStrictEqual([]);
        expect(investigation.Runs).toStrictEqual([]);
        expect(investigation.RegisteredStudyIdentifiers).toStrictEqual([]);
        expect(investigation.Comments).toStrictEqual([]);
        expect(investigation.Remarks).toStrictEqual([]);
    });

    it("primary constructor", function () {
        const ontologySources = [
            new OntologySourceReference("First ontology"),
            new OntologySourceReference("Second ontology"),
        ];
        const publication = new Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study");
        const contact = new Person("0000-0002-1825-0097", "Frey", "Kevin");
        const assay = new ArcAssay("assay-1");
        const study = new ArcStudy("study-1");
        const workflow = new ArcWorkflow("workflow-1");
        const run = new ArcRun("run-1");
        const comment = new Comment("lastUpdated", "2026-10-29");
        const remark = new Remark(7, "Investigation remark");

        study.RegisteredAssayIdentifiers.push("assay-1");
        run.WorkflowIdentifiers.push("workflow-1");

        const investigation = new ArcInvestigation(
            "My cool ARC",
            "My cool ARC title",
            "My cool ARC description",
            "2026-10-20",
            "2026-10-29",
            ontologySources,
            [publication],
            [contact],
            [assay],
            [study],
            [workflow],
            [run],
            ["study-1"],
            [comment],
            [remark],
        );

        expect(investigation).toBeInstanceOf(ArcInvestigation);
        expect(investigation.Identifier).toBe("My cool ARC");
        expect(investigation.Title).toBe("My cool ARC title");
        expect(investigation.Description).toBe("My cool ARC description");
        expect(investigation.SubmissionDate).toBe("2026-10-20");
        expect(investigation.PublicReleaseDate).toBe("2026-10-29");
        expect(investigation.OntologySourceReferences).toStrictEqual(ontologySources);
        expect(investigation.Publications).toStrictEqual([publication]);
        expect(investigation.Contacts).toStrictEqual([contact]);
        expect(investigation.Assays).toStrictEqual([assay]);
        expect(investigation.Studies).toStrictEqual([study]);
        expect(investigation.Workflows).toStrictEqual([workflow]);
        expect(investigation.Runs).toStrictEqual([run]);
        expect(investigation.RegisteredStudyIdentifiers).toStrictEqual(["study-1"]);
        expect(investigation.Comments).toStrictEqual([comment]);
        expect(investigation.Remarks).toStrictEqual([remark]);
        expect(assay.Investigation).toBe(investigation);
        expect(study.Investigation).toBe(investigation);
        expect(workflow.Investigation).toBe(investigation);
        expect(run.Investigation).toBe(investigation);
        expect(investigation.RegisteredStudies).toStrictEqual([study]);
        expect(study.RegisteredAssays).toStrictEqual([assay]);
        expect(run.Workflows).toStrictEqual([workflow]);
    });

    it("creates independent default comments", function () {
        const first = new ArcInvestigation("first");
        const second = new ArcInvestigation("second");

        first.Comments.push(new Comment("key", "value"));

        expect(second.Comments).toStrictEqual([]);
    });
});
