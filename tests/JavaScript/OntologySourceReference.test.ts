import { OntologySourceReference, Comment } from "../../src/ARCtrl/index.ts"
import { expect, it, describe } from 'vitest';

export const mkDefaultOSR = () => new OntologySourceReference(
    "My cool description",
    "my/cool/filepath",
    "My cool file name",
    "2.0.0-alpha.01+meta.b6",
    [
        new Comment(
            "isa-column-order",
            "2"
        ),
        new Comment(
            "lastUpdated",
            "2026-10-29"
        )
    ]
);

describe('OntologySourceReference', function () {
    it('Primary Constructor Minimal', function() {
        const osr = new OntologySourceReference();
        expect(osr).toBeInstanceOf(OntologySourceReference);
        expect(osr.Description).toBeUndefined();
        expect(osr.File).toBeUndefined();
        expect(osr.Name).toBeUndefined();
        expect(osr.Version).toBeUndefined();
        expect(osr.Comments).toEqual([]);
    });
    it('Primary Constructor', function() {
        const osr = mkDefaultOSR()
        expect(osr).toBeInstanceOf(OntologySourceReference);
        expect(osr.Description).toBe("My cool description");
        expect(osr.File).toBe("my/cool/filepath");
        expect(osr.Name).toBe("My cool file name");
        expect(osr.Version).toBe("2.0.0-alpha.01+meta.b6");
        expect(osr.Comments).toHaveLength(2);
        expect(osr.Comments[0].Name).toBe("isa-column-order");
        expect(osr.Comments[0].Value).toBe("2");
        expect(osr.Comments[1].Name).toBe("lastUpdated");
        expect(osr.Comments[1].Value).toBe("2026-10-29");
    });
    it('Primary Constructor creates fresh comments', function() {
        const first = new OntologySourceReference();
        const second = new OntologySourceReference();
        first.Comments.push(new Comment("source", "manual"));
        expect(second.Comments).toEqual([]);
    });
});
