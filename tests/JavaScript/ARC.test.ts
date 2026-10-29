import {
    ARC, ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcWorkflow, Comment, CWL,
    FileSystem, FileSystemTree, OntologySourceReference, Person, Publication, Remark, Commit, License,
} from "../../src/ARCtrl/index.ts";
import { expect, it, describe } from "vitest";

describe("ARC", function () {
    it("primary constructor minimal", function () {
        const arc = new ARC("My cool ARC");
        expect(arc).toBeInstanceOf(ARC);
        expect(arc).toBeInstanceOf(ArcInvestigation);
        expect(arc.Identifier).toBe("My cool ARC");
        expect(arc.Title).toBeUndefined();
        expect(arc.Description).toBeUndefined();
        expect(arc.SubmissionDate).toBeUndefined();
        expect(arc.PublicReleaseDate).toBeUndefined();
        expect(arc.OntologySourceReferences).toStrictEqual([]);
        expect(arc.Publications).toStrictEqual([]);
        expect(arc.Contacts).toStrictEqual([]);
        expect(arc.Assays).toStrictEqual([]);
        expect(arc.Studies).toStrictEqual([]);
        expect(arc.Workflows).toStrictEqual([]);
        expect(arc.Runs).toStrictEqual([]);
        expect(arc.RegisteredStudyIdentifiers).toStrictEqual([]);
        expect(arc.Comments).toStrictEqual([]);
        expect(arc.Remarks).toStrictEqual([]);
        expect(arc.License).toBeUndefined();
        expect(arc.FileSystem).toBeInstanceOf(FileSystem);
        expect(arc.FileSystem.History).toStrictEqual([]);
        expect(arc.FileSystem.Tree.ToFilePaths()).toContain("isa.investigation.xlsx");
    });
    it("primary constructor", function () {
        const arc = new ARC(
            "My cool ARC",
            "My cool ARC title",
            "My cool ARC description",
            "2026-10-20",
            "2026-10-29",
            [
                new OntologySourceReference(
                    "My cool description", "my/cool/filepath", "My cool file name", "2.0.0-alpha.01+meta.b6",
                ),
                new OntologySourceReference(
                    "Another description",
                    "another/cool/filepath",
                    "Another cool file name",
                    "2.0.0-alpha.01+meta.b6",
                ),
            ],
        );
        expect(arc).toBeInstanceOf(ARC);
        expect(arc.Identifier).toBe("My cool ARC");
        expect(arc.Title).toBe("My cool ARC title");
        expect(arc.Description).toBe("My cool ARC description");
        expect(arc.SubmissionDate).toBe("2026-10-20");
        expect(arc.PublicReleaseDate).toBe("2026-10-29");
        expect(arc.OntologySourceReferences).toHaveLength(2);
        expect(arc.OntologySourceReferences[0].Description).toBe("My cool description");
        expect(arc.OntologySourceReferences[1].Description).toBe("Another description");
    });

    it("primary constructor with all nested types", function () {
        const ontologySource = new OntologySourceReference("Ontology description", "ontology.obo", "OBI", "1.0");
        const publication = new Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study");
        const contact = new Person("0000-0002-1825-0097", "Frey", "Kevin");
        const assay = new ArcAssay("assay-1");
        const study = new ArcStudy("study-1");
        const workflow = new ArcWorkflow("workflow-1");
        const run = new ArcRun("run-1");
        const cwl = CWL.CWLProcessingUnit.commandLineTool(new CWL.CWLToolDescription([]));
        const comment = new Comment("lastUpdated", "2026-10-29");
        const remark = new Remark(7, "Investigation remark");
        const commit = new Commit("abc123", "Kevin", "kevin@example.org", new Date("2026-10-29T10:00:00Z"), "Initial ARC");
        const fs = new FileSystem(new FileSystemTree(1, ["root", [new FileSystemTree(0, ["README.md"])]]), [commit]);
        const license = new License("fulltext", "Custom license text", "COPYING");

        study.RegisteredAssayIdentifiers.push("assay-1");
        run.WorkflowIdentifiers.push("workflow-1");
        workflow.CWLDescription = cwl;
        run.CWLDescription = cwl;

        const arc = new ARC(
            "My cool ARC",
            "My cool ARC title",
            "My cool ARC description",
            "2026-10-20",
            "2026-10-29",
            [ontologySource],
            [publication],
            [contact],
            [assay],
            [study],
            [workflow],
            [run],
            ["study-1"],
            [comment],
            [remark],
            fs,
            license,
        );

        expect(arc).toBeInstanceOf(ARC);
        expect(arc.Identifier).toBe("My cool ARC");
        expect(arc.Title).toBe("My cool ARC title");
        expect(arc.Description).toBe("My cool ARC description");
        expect(arc.SubmissionDate).toBe("2026-10-20");
        expect(arc.PublicReleaseDate).toBe("2026-10-29");
        expect(arc.OntologySourceReferences).toStrictEqual([ontologySource]);
        expect(arc.Publications).toStrictEqual([publication]);
        expect(arc.Contacts).toStrictEqual([contact]);
        expect(arc.Assays).toStrictEqual([assay]);
        expect(arc.Studies).toStrictEqual([study]);
        expect(arc.Workflows).toStrictEqual([workflow]);
        expect(arc.Runs).toStrictEqual([run]);
        expect(arc.RegisteredStudyIdentifiers).toStrictEqual(["study-1"]);
        expect(arc.Comments).toStrictEqual([comment]);
        expect(arc.Remarks).toStrictEqual([remark]);
        expect(arc.License).toBe(license);
        expect(arc.FileSystem.History).toStrictEqual([commit]);
        expect(assay.Investigation).toBe(arc);
        expect(study.Investigation).toBe(arc);
        expect(workflow.Investigation).toBe(arc);
        expect(run.Investigation).toBe(arc);
        expect(arc.RegisteredStudies).toStrictEqual([study]);
        expect(study.RegisteredAssays).toStrictEqual([assay]);
        expect(run.Workflows).toStrictEqual([workflow]);

        const paths = arc.FileSystem.Tree.ToFilePaths();
        expect(paths).toContain("README.md");
        expect(paths).toContain("COPYING");
        expect(paths).toContain("isa.investigation.xlsx");
        expect(paths).toContain("assays/assay-1/isa.assay.xlsx");
        expect(paths).toContain("studies/study-1/isa.study.xlsx");
        expect(paths).toContain("workflows/workflow-1/workflow.cwl");
        expect(paths).toContain("runs/run-1/run.cwl");
    });

    it("does not share default collections or filesystem between instances", function () {
        const first = new ARC("first");
        const second = new ARC("second");
        first.Comments.push(new Comment("key", "value"));
        first.FileSystem.History[0] = new Commit("abc123", "Kevin", "kevin@example.org", new Date("2026-10-29T10:00:00Z"), "Initial ARC");
        expect(second.Comments).toStrictEqual([]);
        expect(second.FileSystem.History).toStrictEqual([]);
    });
});
