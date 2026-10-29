import {
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow, Comment,
    CWL, DataContext, Datamap, OntologyAnnotation, OntologySourceReference, Person, Publication,
} from "../../src/ARCtrl/index.ts";
import { Component } from "../../src/ARCtrl/ts/Core/Process/Component.ts";
import { ScalarValue } from "../../src/ARCtrl/ts/Core/ScalarValue.ts";
import { describe, expect, it } from "vitest";


describe("ArcWorkflow", function () {
    it("primary constructor minimal", function () {
        const workflow = new ArcWorkflow("workflow-1");

        expect(workflow).toBeInstanceOf(ArcWorkflow);
        expect(workflow.Identifier).toBe("workflow-1");
        expect(workflow.Title).toBeUndefined();
        expect(workflow.Description).toBeUndefined();
        expect(workflow.WorkflowType).toBeUndefined();
        expect(workflow.URI).toBeUndefined();
        expect(workflow.Version).toBeUndefined();
        expect(workflow.SubWorkflowIdentifiers).toStrictEqual([]);
        expect(workflow.Parameters).toStrictEqual([]);
        expect(workflow.Components).toStrictEqual([]);
        expect(workflow.Datamap).toBeUndefined();
        expect(workflow.Contacts).toStrictEqual([]);
        expect(workflow.CWLDescription).toBeUndefined();
        expect(workflow.Comments).toStrictEqual([]);
        expect(workflow.Investigation).toBeUndefined();
    });

    it("primary constructor", function () {
        const workflowType = new OntologyAnnotation("data analysis");
        const parameter = new OntologyAnnotation("temperature");
        const component = new Component(new ScalarValue(3, ["instrument"]), undefined, new OntologyAnnotation("equipment"));
        const datamap = new Datamap([new DataContext("data-1", "dataset/results.csv")]);
        const contact = new Person("0000-0002-1825-0097", "Frey", "Kevin");
        const cwl = new CWL.CWLProcessingUnit(0, [new CWL.CWLToolDescription([], "v1.2", ["echo"])]);
        const comment = new Comment("lastUpdated", "2026-10-29");
        const workflow = new ArcWorkflow(
            "workflow-1",
            "Workflow title",
            "Workflow description",
            workflowType,
            "workflows/workflow-1/workflow.cwl",
            "1.2.3",
            ["subworkflow-1"],
            [parameter],
            [component],
            datamap,
            [contact],
            cwl,
            [comment],
        );

        expect(workflow).toBeInstanceOf(ArcWorkflow);
        expect(workflow.Identifier).toBe("workflow-1");
        expect(workflow.Title).toBe("Workflow title");
        expect(workflow.Description).toBe("Workflow description");
        expect(workflow.WorkflowType).toBe(workflowType);
        expect(workflow.URI).toBe("workflows/workflow-1/workflow.cwl");
        expect(workflow.Version).toBe("1.2.3");
        expect(workflow.SubWorkflowIdentifiers).toStrictEqual(["subworkflow-1"]);
        expect(workflow.Parameters).toStrictEqual([parameter]);
        expect(workflow.Components).toStrictEqual([component]);
        expect(workflow.Datamap).toBe(datamap);
        expect(workflow.Contacts).toStrictEqual([contact]);
        expect(workflow.CWLDescription).toBe(cwl);
        expect(workflow.Comments).toStrictEqual([comment]);
    });

    it("creates independent default comments", function () {
        const first = new ArcWorkflow("first");
        const second = new ArcWorkflow("second");

        first.Comments.push(new Comment("key", "value"));

        expect(second.Comments).toStrictEqual([]);
    });
});

