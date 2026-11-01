import { Comment } from "../../src/ARCtrl/index.ts"
import { expect, it, describe } from 'vitest';

describe('Comment', function () {
    it('Primary Constructor Minimal', function() {
        const comment = new Comment();
        expect(comment).toBeInstanceOf(Comment);
        expect(comment.Name).toBeUndefined();
        expect(comment.Value).toBeUndefined();
    });
    it('Primary Constructor', function() {
        const comment = new Comment(
            "isa-column-order",
            "2"
        );
        expect(comment).toBeInstanceOf(Comment);
        expect(comment.Name).toBe("isa-column-order");
        expect(comment.Value).toBe("2");
    });
});
