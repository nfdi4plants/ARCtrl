import { split as split_1 } from "../FileSystem/Path.js";
import { $007CInvestigationPath$007C_$007C } from "./ArcInvestigation.js";
import { Contract } from "./Contract.js";
import { $007CAssayPath$007C_$007C } from "./ArcAssay.js";
import { $007CStudyPath$007C_$007C } from "./ArcStudy.js";
import { $007CDatamapPath$007C_$007C } from "./Datamap.js";

/**
 * Tries to create READ contract with DTOType = ISA_Assay, ISA_Study or ISA_Investigation from given path relative to ARC root.
 */
export function tryISAReadContractFromPath(path) {
    const split = split_1(path);
    const activePatternResult = $007CInvestigationPath$007C_$007C(split);
    if (activePatternResult != null) {
        const p = activePatternResult;
        return Contract.createRead(p, "ISA_Investigation");
    }
    else {
        const activePatternResult_1 = $007CAssayPath$007C_$007C(split);
        if (activePatternResult_1 != null) {
            const p_1 = activePatternResult_1;
            return Contract.createRead(p_1, "ISA_Assay");
        }
        else {
            const activePatternResult_2 = $007CStudyPath$007C_$007C(split);
            if (activePatternResult_2 != null) {
                const p_2 = activePatternResult_2;
                return Contract.createRead(p_2, "ISA_Study");
            }
            else {
                const activePatternResult_3 = $007CDatamapPath$007C_$007C(split);
                if (activePatternResult_3 != null) {
                    const p_3 = activePatternResult_3;
                    return Contract.createRead(p_3, "ISA_Datamap");
                }
                else {
                    return undefined;
                }
            }
        }
    }
}

