import { License } from "../../src/ARCtrl/ts/License.ts";
import { describe, expect, it } from "vitest";

describe("License", function () {
    it("primary constructor minimal", function () {
        const license = new License("fulltext", "Permission is hereby granted.");
        expect(license).toBeInstanceOf(License);
        expect(license.Type).toBe("fulltext");
        expect(license.Content).toBe("Permission is hereby granted.");
        expect(license.Path).toBe("LICENSE");
    });
    it("primary constructor", function () {
        const license = new License("fulltext", "Custom license text", "COPYING");
        expect(license).toBeInstanceOf(License);
        expect(license.Type).toBe("fulltext");
        expect(license.Content).toBe("Custom license text");
        expect(license.Path).toBe("COPYING");
    });
});
