import { equal, deepEqual, notEqual } from 'assert';
import { XlsxController, ArcAssay } from "./index.js"
import { expect, it, describe } from 'vitest';

describe('XlsxController', function () {
    it("Assay", function () {
        let assay = new ArcAssay("My Delightful Assay")
        let fswb = XlsxController.Assay.toFsWorkbook(assay)
        let assay2 = XlsxController.Assay.fromFsWorkbook(fswb)
        equal(assay.Identifier, assay2.Identifier, "Assay identifier before and after write-read should be same.")
    });
});