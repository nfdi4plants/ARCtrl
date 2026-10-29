import { Remark } from "../../src/ARCtrl/ts/Core/Comment.ts";
import { expect, it, describe } from 'vitest';

describe('Remark', function () {
    it('Primary Constructor Minimal', function() {
        const remark = new Remark(0, "");
        expect(remark).toBeInstanceOf(Remark);
        expect(remark.Line).toBe(0);
        expect(remark.Value).toBe("");
    });
    it('Primary Constructor', function() {
        const remark = new Remark(12, "A metadata remark");
        expect(remark).toBeInstanceOf(Remark);
        expect(remark.Line).toBe(12);
        expect(remark.Value).toBe("A metadata remark");
    });
});
