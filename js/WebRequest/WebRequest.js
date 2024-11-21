import { downloadFile as downloadFile_1, isNode } from "./WebRequest.Node.js";
import { singleton } from "../fable_modules/fable-library-js.4.22.0/AsyncBuilder.js";
import { Http_get } from "../fable_modules/Fable.SimpleHttp.3.5.0/Http.fs.js";
import { printf, toFail } from "../fable_modules/fable-library-js.4.22.0/String.js";
import "isomorphic-fetch";

export function downloadFile(url) {
    if (isNode()) {
        return downloadFile_1(url);
    }
    else {
        return singleton.Delay(() => singleton.Bind(Http_get(url), (_arg) => {
            const statusCode = _arg[0] | 0;
            const responseText = _arg[1];
            return singleton.Return((statusCode === 200) ? responseText : toFail(printf("Status %d => %s"))(statusCode)(responseText));
        }));
    }
}

