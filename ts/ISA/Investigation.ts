import { value as value_4, Option } from "../fable_modules/fable-library-ts/Option.js";
import { Study_$reflection, Study } from "./Study.js";
import { Assay } from "./Assay.js";
import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { FSharpList } from "../fable_modules/fable-library-ts/List.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { record_type, list_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export class Investigation extends Record implements IEquatable<Investigation>, IComparable<Investigation> {
    readonly Studies: FSharpList<Study>;
    constructor(Studies: FSharpList<Study>) {
        super();
        this.Studies = Studies;
    }
    static tryGetStudyByID(studyIdentifier: string, investigation: Investigation): Option<Study> {
        throw new Error();
    }
    static updateStudyByID(study: Study, studyIdentifier: string, investigation: Investigation): Investigation {
        Investigation.tryGetStudyByID(studyIdentifier, investigation);
        throw new Error();
    }
    static addStudy(study: Study, investigation: Investigation): Investigation {
        throw new Error();
    }
    static addAssay(assay: Assay, studyIdentifier: string, investigation: Investigation): Investigation {
        const matchValue: Option<Study> = Investigation.tryGetStudyByID(studyIdentifier, investigation);
        if (matchValue == null) {
            (): Study => Study.create();
            (arg_7: Study): ((arg0: Investigation) => Investigation) => ((arg_8: Investigation): Investigation => Investigation.addStudy(arg_7, arg_8));
        }
        else {
            const s: Study = value_4(matchValue);
            (arg_2: Assay): ((arg0: Study) => Study) => ((arg_3: Study): Study => Study.addAssay(arg_2, arg_3));
            (arg_4: Study): ((arg0: string) => ((arg0: Investigation) => Investigation)) => ((arg_5: string): ((arg0: Investigation) => Investigation) => ((arg_6: Investigation): Investigation => Investigation.updateStudyByID(arg_4, arg_5, arg_6)));
        }
        throw new Error();
    }
}

export function Investigation_$reflection(): TypeInfo {
    return record_type("ISA.Investigation", [], Investigation, () => [["Studies", list_type(Study_$reflection())]]);
}

