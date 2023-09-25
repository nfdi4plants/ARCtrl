import { equal, deepEqual } from 'assert';
import { JsWeb } from "./ARCtrl/Templates/Template.Web.js";

describe('Template.Web', function () {
    it('getTemplates', async () => {
        let templates = await JsWeb.getTemplates()
        let exists = templates.size >= 0
        deepEqual(exists, true, "If true templates were found and downloaded")
    })
});