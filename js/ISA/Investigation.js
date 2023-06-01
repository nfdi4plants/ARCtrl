import { Study_$reflection, Study } from "./Study.js";
import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, list_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";

export class Investigation extends Record {
    constructor(Studies) {
        super();
        this.Studies = Studies;
    }
    static tryGetStudyByID(studyIdentifier, investigation) {
        throw new Error();
    }
    static updateStudyByID(study, studyIdentifier, investigation) {
        Investigation.tryGetStudyByID(studyIdentifier, investigation);
        throw new Error();
    }
    static addStudy(study, investigation) {
        throw new Error();
    }
    static addAssay(assay, studyIdentifier, investigation) {
        const matchValue = Investigation.tryGetStudyByID(studyIdentifier, investigation);
        if (matchValue == null) {
            () => Study.create();
            (arg_7) => ((arg_8) => Investigation.addStudy(arg_7, arg_8));
        }
        else {
            const s = matchValue;
            (arg_2) => ((arg_3) => Study.addAssay(arg_2, arg_3));
            (arg_4) => ((arg_5) => ((arg_6) => Investigation.updateStudyByID(arg_4, arg_5, arg_6)));
        }
        throw new Error();
    }
}

export function Investigation_$reflection() {
    return record_type("ISA.Investigation", [], Investigation, () => [["Studies", list_type(Study_$reflection())]]);
}

