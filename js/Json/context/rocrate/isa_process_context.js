import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Process, ArcProcess, name, executesProtocol, performer, date, previousProcess, nextProcess, input, output, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Process = Process;
        this.ArcProcess = ArcProcess;
        this.name = name;
        this.executesProtocol = executesProtocol;
        this.performer = performer;
        this.date = date;
        this.previousProcess = previousProcess;
        this.nextProcess = nextProcess;
        this.input = input;
        this.output = output;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Process.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Process", string_type], ["ArcProcess", string_type], ["name", string_type], ["executesProtocol", string_type], ["performer", string_type], ["date", string_type], ["previousProcess", string_type], ["nextProcess", string_type], ["input", string_type], ["output", string_type], ["comments", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["bio", {
        Encode(helpers_1) {
            return helpers_1.encodeString("https://bioschemas.org/");
        },
    }], ["Process", {
        Encode(helpers_2) {
            return helpers_2.encodeString("bio:LabProcess");
        },
    }], ["name", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["executesProtocol", {
        Encode(helpers_4) {
            return helpers_4.encodeString("bio:executesLabProtocol");
        },
    }], ["parameterValues", {
        Encode(helpers_5) {
            return helpers_5.encodeString("bio:parameterValue");
        },
    }], ["performer", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:agent");
        },
    }], ["date", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:endTime");
        },
    }], ["inputs", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:object");
        },
    }], ["outputs", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:result");
        },
    }], ["comments", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:disambiguatingDescription");
        },
    }]];
    return {
        Encode(helpers_11) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_11)], values);
            return helpers_11.encodeObject(arg);
        },
    };
})();

