import { OntologyAnnotation, Comment } from "../../src/ARCtrl/index.ts";
import { expect, it, describe } from 'vitest';

describe('OntologyAnnotation', function () {
    it('Primary Constructor Minimal', function() {
        const annotation = new OntologyAnnotation();
        expect(annotation).toBeInstanceOf(OntologyAnnotation);
        expect(annotation.Name).toBeUndefined();
        expect(annotation.TermSourceREF).toBeUndefined();
        expect(annotation.TermAccessionNumber).toBeUndefined();
        expect(annotation.Comments).toEqual([]);
    });
    it('Primary Constructor', function() {
        const annotation = new OntologyAnnotation(
            "Homo sapiens",
            "NCBITaxon",
            "NCBITaxon:9606",
            [new Comment("source", "curated"), new Comment("version", "2026")]
        );
        expect(annotation).toBeInstanceOf(OntologyAnnotation);
        expect(annotation.Name).toBe("Homo sapiens");
        expect(annotation.TermSourceREF).toBe("NCBITaxon");
        expect(annotation.TermAccessionNumber).toBe("NCBITaxon:9606");
        expect(annotation.Comments).toHaveLength(2);
        expect(annotation.Comments[0].Name).toBe("source");
        expect(annotation.Comments[0].Value).toBe("curated");
        expect(annotation.Comments[1].Name).toBe("version");
        expect(annotation.Comments[1].Value).toBe("2026");
    });
    it('Primary Constructor creates fresh comments', function() {
        const first = new OntologyAnnotation();
        const second = new OntologyAnnotation();
        first.Comments.push(new Comment("source", "curated"));
        expect(second.Comments).toEqual([]);
    });
});
