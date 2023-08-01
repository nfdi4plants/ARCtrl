import { Union } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { list_type, union_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_$reflection } from "../JsonTypes/OntologyAnnotation.js";
import { Protocol_get_empty, Protocol } from "../JsonTypes/Protocol.js";
import { ProcessParameterValue_$reflection } from "../JsonTypes/ProcessParameterValue.js";
import { MaterialAttributeValue_$reflection } from "../JsonTypes/MaterialAttributeValue.js";
import { FactorValue_$reflection } from "../JsonTypes/FactorValue.js";
import { map as map_1, exists, fold, singleton, empty, append } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { map, value as value_4, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Process_get_empty, Process } from "../JsonTypes/Process.js";
import { Assay_get_empty, Assay } from "../JsonTypes/Assay.js";
import { printf, toFail } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { Study } from "../JsonTypes/Study.js";

export class ProtocolTransformation extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["AddName", "AddProtocolType", "AddDescription"];
    }
}

export function ProtocolTransformation_$reflection() {
    return union_type("ISA.Builder.ProtocolTransformation", [], ProtocolTransformation, () => [[["Item", string_type]], [["Item", OntologyAnnotation_$reflection()]], [["Item", string_type]]]);
}

export function ProtocolTransformation__Transform_4D8AC666(this$, p) {
    switch (this$.tag) {
        case 1:
            return new Protocol(p.ID, p.Name, this$.fields[0], p.Description, p.Uri, p.Version, p.Parameters, p.Components, p.Comments);
        case 2:
            return new Protocol(p.ID, p.Name, p.ProtocolType, this$.fields[0], p.Uri, p.Version, p.Parameters, p.Components, p.Comments);
        default:
            return new Protocol(p.ID, this$.fields[0], p.ProtocolType, p.Description, p.Uri, p.Version, p.Parameters, p.Components, p.Comments);
    }
}

export function ProtocolTransformation__Equals_4D8AC666(this$, p) {
    const matchValue = p.Name;
    let matchResult, n_1, n$0027_1;
    if (this$.tag === 0) {
        if (matchValue != null) {
            if (this$.fields[0] === matchValue) {
                matchResult = 0;
                n_1 = this$.fields[0];
                n$0027_1 = matchValue;
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return true;
        default:
            return false;
    }
}

export class ProcessTransformation extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["AddName", "AddParameter", "AddCharacteristic", "AddFactor", "AddProtocol"];
    }
}

export function ProcessTransformation_$reflection() {
    return union_type("ISA.Builder.ProcessTransformation", [], ProcessTransformation, () => [[["Item", string_type]], [["Item", ProcessParameterValue_$reflection()]], [["Item", MaterialAttributeValue_$reflection()]], [["Item", FactorValue_$reflection()]], [["Item", list_type(ProtocolTransformation_$reflection())]]]);
}

export function ProcessTransformation__Transform_30EF9E7B(this$, p) {
    let pro;
    switch (this$.tag) {
        case 1: {
            const pv = this$.fields[0];
            const parameterValues = append(defaultArg(p.ParameterValues, empty()), singleton(pv));
            return new Process(p.ID, p.Name, (pro = defaultArg(p.ExecutesProtocol, Protocol_get_empty()), new Protocol(pro.ID, pro.Name, pro.ProtocolType, pro.Description, pro.Uri, pro.Version, append(defaultArg(pro.Parameters, empty()), singleton(value_4(pv.Category))), pro.Components, pro.Comments)), parameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
        }
        case 2:
            return new Process(p.ID, p.Name, p.ExecutesProtocol, p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, map((i) => i, p.Inputs), p.Outputs, p.Comments);
        case 3:
            return new Process(p.ID, p.Name, p.ExecutesProtocol, p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, map((i_1) => i_1, p.Outputs), p.Comments);
        case 4:
            return new Process(p.ID, p.Name, fold((pro_2, trans) => ProtocolTransformation__Transform_4D8AC666(trans, pro_2), defaultArg(p.ExecutesProtocol, Protocol_get_empty()), this$.fields[0]), p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
        default:
            return new Process(p.ID, this$.fields[0], p.ExecutesProtocol, p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
    }
}

export function ProcessTransformation__Equals_30EF9E7B(this$, p) {
    const matchValue = p.Name;
    let matchResult, n_1, n$0027_1;
    if (this$.tag === 0) {
        if (matchValue != null) {
            if (this$.fields[0] === matchValue) {
                matchResult = 0;
                n_1 = this$.fields[0];
                n$0027_1 = matchValue;
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return true;
        default:
            return false;
    }
}

export class AssayTransformation extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["AddFileName", "AddParameter", "AddCharacteristic", "AddFactor", "AddProcess"];
    }
}

export function AssayTransformation_$reflection() {
    return union_type("ISA.Builder.AssayTransformation", [], AssayTransformation, () => [[["Item", string_type]], [["Item", ProcessParameterValue_$reflection()]], [["Item", MaterialAttributeValue_$reflection()]], [["Item", FactorValue_$reflection()]], [["Item", list_type(ProcessTransformation_$reflection())]]]);
}

export function AssayTransformation__Transform_Z269B5B97(this$, a) {
    switch (this$.tag) {
        case 4: {
            const pts = this$.fields[0];
            const processes = defaultArg(a.ProcessSequence, empty());
            return new Assay(a.ID, a.FileName, a.MeasurementType, a.TechnologyType, a.TechnologyPlatform, a.DataFiles, a.Materials, a.CharacteristicCategories, a.UnitCategories, exists((p) => exists((trans) => ProcessTransformation__Equals_30EF9E7B(trans, p), pts), processes) ? map_1((p_1) => {
                if (exists((trans_1) => ProcessTransformation__Equals_30EF9E7B(trans_1, p_1), pts)) {
                    return fold((p_2, trans_2) => ProcessTransformation__Transform_30EF9E7B(trans_2, p_2), p_1, pts);
                }
                else {
                    return p_1;
                }
            }, processes) : append(processes, singleton(fold((p_3, trans_3) => ProcessTransformation__Transform_30EF9E7B(trans_3, p_3), Process_get_empty(), pts))), a.Comments);
        }
        case 0:
            return new Assay(a.ID, this$.fields[0], a.MeasurementType, a.TechnologyType, a.TechnologyPlatform, a.DataFiles, a.Materials, a.CharacteristicCategories, a.UnitCategories, a.ProcessSequence, a.Comments);
        default:
            return toFail(printf("Builder failed: Case %O Not implemented"))(this$);
    }
}

export function AssayTransformation__Equals_Z269B5B97(this$, a) {
    const matchValue = a.FileName;
    let matchResult, n_1, n$0027_1;
    if (this$.tag === 0) {
        if (matchValue != null) {
            if (this$.fields[0] === matchValue) {
                matchResult = 0;
                n_1 = this$.fields[0];
                n$0027_1 = matchValue;
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return true;
        default:
            return false;
    }
}

export class StudyTransformation extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["AddParameter", "AddCharacteristic", "AddFactor", "AddProcess", "AddAssay"];
    }
}

export function StudyTransformation_$reflection() {
    return union_type("ISA.Builder.StudyTransformation", [], StudyTransformation, () => [[["Item", ProcessParameterValue_$reflection()]], [["Item", MaterialAttributeValue_$reflection()]], [["Item", FactorValue_$reflection()]], [["Item", list_type(ProcessTransformation_$reflection())]], [["Item", list_type(AssayTransformation_$reflection())]]]);
}

export function StudyTransformation__Transform_Z27CB2981(this$, s) {
    switch (this$.tag) {
        case 3: {
            const pts = this$.fields[0];
            const processes = defaultArg(s.ProcessSequence, empty());
            return new Study(s.ID, s.FileName, s.Identifier, s.Title, s.Description, s.SubmissionDate, s.PublicReleaseDate, s.Publications, s.Contacts, s.StudyDesignDescriptors, s.Protocols, s.Materials, exists((p) => exists((trans) => ProcessTransformation__Equals_30EF9E7B(trans, p), pts), processes) ? map_1((p_1) => {
                if (exists((trans_1) => ProcessTransformation__Equals_30EF9E7B(trans_1, p_1), pts)) {
                    return fold((p_2, trans_2) => ProcessTransformation__Transform_30EF9E7B(trans_2, p_2), p_1, pts);
                }
                else {
                    return p_1;
                }
            }, processes) : append(processes, singleton(fold((p_3, trans_3) => ProcessTransformation__Transform_30EF9E7B(trans_3, p_3), Process_get_empty(), pts))), s.Assays, s.Factors, s.CharacteristicCategories, s.UnitCategories, s.Comments);
        }
        case 4: {
            const ats = this$.fields[0];
            const assays = defaultArg(s.Assays, empty());
            return new Study(s.ID, s.FileName, s.Identifier, s.Title, s.Description, s.SubmissionDate, s.PublicReleaseDate, s.Publications, s.Contacts, s.StudyDesignDescriptors, s.Protocols, s.Materials, s.ProcessSequence, exists((a) => exists((trans_4) => AssayTransformation__Equals_Z269B5B97(trans_4, a), ats), assays) ? map_1((a_1) => {
                if (exists((trans_5) => AssayTransformation__Equals_Z269B5B97(trans_5, a_1), ats)) {
                    return fold((a_2, trans_6) => AssayTransformation__Transform_Z269B5B97(trans_6, a_2), a_1, ats);
                }
                else {
                    return a_1;
                }
            }, assays) : append(assays, singleton(fold((a_3, trans_7) => AssayTransformation__Transform_Z269B5B97(trans_7, a_3), Assay_get_empty(), ats))), s.Factors, s.CharacteristicCategories, s.UnitCategories, s.Comments);
        }
        default:
            return toFail(printf("Builder failed: Case %O Not implemented"))(this$);
    }
}

