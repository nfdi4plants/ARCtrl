import { defaultArg } from "./fable_modules/fable-library-js.4.22.0/Option.js";
import { singleton } from "./fable_modules/fable-library-js.4.22.0/AsyncBuilder.js";
import { downloadFile } from "./WebRequest/WebRequest.js";
import { Templates_fromJsonString } from "./JsonIO/Table/Templates.js";
import { startAsPromise } from "./fable_modules/fable-library-js.4.22.0/Async.js";
import { class_type } from "./fable_modules/fable-library-js.4.22.0/Reflection.js";

export function getTemplates(url) {
    const url_1 = defaultArg(url, "https://github.com/nfdi4plants/Swate-templates/releases/download/latest/templates_v2.0.0.json");
    return singleton.Delay(() => singleton.Bind(downloadFile(url_1), (_arg) => {
        const mapResult = Templates_fromJsonString(_arg);
        return singleton.Return(mapResult);
    }));
}

export class WebController {
    constructor() {
    }
    static getTemplates(url) {
        return startAsPromise(singleton.Delay(() => singleton.Bind(getTemplates(url), (_arg) => singleton.Return(_arg))));
    }
}

export function WebController_$reflection() {
    return class_type("ARCtrl.Template.Web.WebController", undefined, WebController);
}

