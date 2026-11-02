import { Publication, OntologyAnnotation, Comment } from "../../src/ARCtrl/index.ts"
import { expect, it, describe } from 'vitest';

export const mkDefaultPublication = () => new Publication(

);

describe('Publication', function () {
    it('Primary Constructor Minimal', function() {
        const publication = new Publication();
        expect(publication).toBeInstanceOf(Publication);
        expect(publication.PubMedID).toBeUndefined();
        expect(publication.DOI).toBeUndefined();
        expect(publication.Authors).toBeUndefined();
        expect(publication.Title).toBeUndefined();
        expect(publication.Status).toBeUndefined();
        expect(publication.Comments).toEqual([]);
    });
    it('Primary Constructor', function() {
        const publication = new Publication(
            "12345678",
            "10.1234/example",
            "Frey K.; Example A.",
            "An example publication",
            new OntologyAnnotation("published", "OBI", "OBI:0000605", [new Comment("status", "verified")]),
            [new Comment("source", "journal"), new Comment("year", "2026")]
        );
        expect(publication).toBeInstanceOf(Publication);
        expect(publication.PubMedID).toBe("12345678");
        expect(publication.DOI).toBe("10.1234/example");
        expect(publication.Authors).toBe("Frey K.; Example A.");
        expect(publication.Title).toBe("An example publication");
        expect(publication.Status).toBeInstanceOf(OntologyAnnotation);
        expect(publication.Status?.Name).toBe("published");
        expect(publication.Status?.TermSourceREF).toBe("OBI");
        expect(publication.Status?.TermAccessionNumber).toBe("OBI:0000605");
        expect(publication.Status?.Comments).toHaveLength(1);
        expect(publication.Status?.Comments[0].Name).toBe("status");
        expect(publication.Status?.Comments[0].Value).toBe("verified");
        expect(publication.Comments).toHaveLength(2);
        expect(publication.Comments[0].Name).toBe("source");
        expect(publication.Comments[0].Value).toBe("journal");
        expect(publication.Comments[1].Name).toBe("year");
        expect(publication.Comments[1].Value).toBe("2026");
    });
    it('Primary Constructor creates fresh comments', function() {
        const first = new Publication();
        const second = new Publication();
        first.Comments.push(new Comment("source", "journal"));
        expect(second.Comments).toEqual([]);
    });
});
