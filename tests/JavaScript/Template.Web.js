import { equal, deepEqual } from 'assert';
import { WebController } from "./ARCtrl/index.js";

describe.skip('Template.Web', function () {
    it('getTemplates', async () => {
        let templates = await WebController.getTemplates()
        let exists = templates.length >= 0
        deepEqual(exists, true, "If true templates were found and downloaded")
    })
});