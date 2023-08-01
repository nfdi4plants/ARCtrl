import { split as split_1 } from "../FileSystem/Path.js";
import { $007CInvestigationPath$007C_$007C } from "./Contracts.ArcInvestigation.js";
import { value, Option } from "../fable_modules/fable-library-ts/Option.js";
import { Contract } from "../Contract/Contract.js";
import { $007CAssayPath$007C_$007C } from "./Contracts.ArcAssay.js";
import { $007CStudyPath$007C_$007C } from "./Contracts.ArcStudy.js";

/**
 * Tries to create READ contract with DTOType = ISA_Assay, ISA_Study or ISA_Investigation from given path relative to ARC root.
 */
export function tryISAReadContractFromPath(path: string): Option<Contract> {
    const split: string[] = split_1(path);
    const activePatternResult: Option<string> = $007CInvestigationPath$007C_$007C(split);
    if (activePatternResult != null) {
        const p: string = value(activePatternResult);
        return Contract.createRead(p, "ISA_Investigation");
    }
    else {
        const activePatternResult_1: Option<string> = $007CAssayPath$007C_$007C(split);
        if (activePatternResult_1 != null) {
            const p_1: string = value(activePatternResult_1);
            return Contract.createRead(p_1, "ISA_Assay");
        }
        else {
            const activePatternResult_2: Option<string> = $007CStudyPath$007C_$007C(split);
            if (activePatternResult_2 != null) {
                const p_2: string = value(activePatternResult_2);
                return Contract.createRead(p_2, "ISA_Study");
            }
            else {
                return void 0;
            }
        }
    }
}

