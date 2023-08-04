import { Union } from "../../../fable_modules/fable-library-ts/Types.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { list_type, union_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Protocol_get_empty, Protocol } from "../JsonTypes/Protocol.js";
import { map, defaultArg, value as value_4, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { ProcessParameterValue_$reflection, ProcessParameterValue } from "../JsonTypes/ProcessParameterValue.js";
import { MaterialAttributeValue_$reflection, MaterialAttributeValue } from "../JsonTypes/MaterialAttributeValue.js";
import { FactorValue_$reflection, FactorValue } from "../JsonTypes/FactorValue.js";
import { map as map_1, exists, fold, singleton, empty, append, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { ProtocolParameter } from "../JsonTypes/ProtocolParameter.js";
import { Process_get_empty, Process } from "../JsonTypes/Process.js";
import { ProcessInput_$union } from "../JsonTypes/ProcessInput.js";
import { ProcessOutput_$union } from "../JsonTypes/ProcessOutput.js";
import { Assay_get_empty, Assay } from "../JsonTypes/Assay.js";
import { printf, toFail } from "../../../fable_modules/fable-library-ts/String.js";
import { Study } from "../JsonTypes/Study.js";

export type ProtocolTransformation_$union = 
    | ProtocolTransformation<0>
    | ProtocolTransformation<1>
    | ProtocolTransformation<2>

export type ProtocolTransformation_$cases = {
    0: ["AddName", [string]],
    1: ["AddProtocolType", [OntologyAnnotation]],
    2: ["AddDescription", [string]]
}

export function ProtocolTransformation_AddName(Item: string) {
    return new ProtocolTransformation<0>(0, [Item]);
}

export function ProtocolTransformation_AddProtocolType(Item: OntologyAnnotation) {
    return new ProtocolTransformation<1>(1, [Item]);
}

export function ProtocolTransformation_AddDescription(Item: string) {
    return new ProtocolTransformation<2>(2, [Item]);
}

export class ProtocolTransformation<Tag extends keyof ProtocolTransformation_$cases> extends Union<Tag, ProtocolTransformation_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: ProtocolTransformation_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["AddName", "AddProtocolType", "AddDescription"];
    }
}

export function ProtocolTransformation_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.Builder.ProtocolTransformation", [], ProtocolTransformation, () => [[["Item", string_type]], [["Item", OntologyAnnotation_$reflection()]], [["Item", string_type]]]);
}

export function ProtocolTransformation__Transform_Z5F51792E(this$: ProtocolTransformation_$union, p: Protocol): Protocol {
    switch (this$.tag) {
        case /* AddProtocolType */ 1:
            return new Protocol(p.ID, p.Name, this$.fields[0], p.Description, p.Uri, p.Version, p.Parameters, p.Components, p.Comments);
        case /* AddDescription */ 2:
            return new Protocol(p.ID, p.Name, p.ProtocolType, this$.fields[0], p.Uri, p.Version, p.Parameters, p.Components, p.Comments);
        default:
            return new Protocol(p.ID, this$.fields[0], p.ProtocolType, p.Description, p.Uri, p.Version, p.Parameters, p.Components, p.Comments);
    }
}

export function ProtocolTransformation__Equals_Z5F51792E(this$: ProtocolTransformation_$union, p: Protocol): boolean {
    const matchValue: Option<string> = p.Name;
    let matchResult: int32, n_1: string, n$0027_1: string;
    if (this$.tag === /* AddName */ 0) {
        if (matchValue != null) {
            if (this$.fields[0] === value_4(matchValue)) {
                matchResult = 0;
                n_1 = this$.fields[0];
                n$0027_1 = value_4(matchValue);
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

export type ProcessTransformation_$union = 
    | ProcessTransformation<0>
    | ProcessTransformation<1>
    | ProcessTransformation<2>
    | ProcessTransformation<3>
    | ProcessTransformation<4>

export type ProcessTransformation_$cases = {
    0: ["AddName", [string]],
    1: ["AddParameter", [ProcessParameterValue]],
    2: ["AddCharacteristic", [MaterialAttributeValue]],
    3: ["AddFactor", [FactorValue]],
    4: ["AddProtocol", [FSharpList<ProtocolTransformation_$union>]]
}

export function ProcessTransformation_AddName(Item: string) {
    return new ProcessTransformation<0>(0, [Item]);
}

export function ProcessTransformation_AddParameter(Item: ProcessParameterValue) {
    return new ProcessTransformation<1>(1, [Item]);
}

export function ProcessTransformation_AddCharacteristic(Item: MaterialAttributeValue) {
    return new ProcessTransformation<2>(2, [Item]);
}

export function ProcessTransformation_AddFactor(Item: FactorValue) {
    return new ProcessTransformation<3>(3, [Item]);
}

export function ProcessTransformation_AddProtocol(Item: FSharpList<ProtocolTransformation_$union>) {
    return new ProcessTransformation<4>(4, [Item]);
}

export class ProcessTransformation<Tag extends keyof ProcessTransformation_$cases> extends Union<Tag, ProcessTransformation_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: ProcessTransformation_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["AddName", "AddParameter", "AddCharacteristic", "AddFactor", "AddProtocol"];
    }
}

export function ProcessTransformation_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.Builder.ProcessTransformation", [], ProcessTransformation, () => [[["Item", string_type]], [["Item", ProcessParameterValue_$reflection()]], [["Item", MaterialAttributeValue_$reflection()]], [["Item", FactorValue_$reflection()]], [["Item", list_type(ProtocolTransformation_$reflection())]]]);
}

export function ProcessTransformation__Transform_716E708F(this$: ProcessTransformation_$union, p: Process): Process {
    let pro: Protocol;
    switch (this$.tag) {
        case /* AddParameter */ 1: {
            const pv: ProcessParameterValue = this$.fields[0];
            const parameterValues: FSharpList<ProcessParameterValue> = append(defaultArg(p.ParameterValues, empty<ProcessParameterValue>()), singleton(pv));
            return new Process(p.ID, p.Name, (pro = defaultArg(p.ExecutesProtocol, Protocol_get_empty()), new Protocol(pro.ID, pro.Name, pro.ProtocolType, pro.Description, pro.Uri, pro.Version, append(defaultArg(pro.Parameters, empty<ProtocolParameter>()), singleton(value_4(pv.Category))), pro.Components, pro.Comments)), parameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
        }
        case /* AddCharacteristic */ 2: {
            const c: MaterialAttributeValue = this$.fields[0];
            return new Process(p.ID, p.Name, p.ExecutesProtocol, p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, map<FSharpList<ProcessInput_$union>, FSharpList<ProcessInput_$union>>((i: FSharpList<ProcessInput_$union>): FSharpList<ProcessInput_$union> => i, p.Inputs), p.Outputs, p.Comments);
        }
        case /* AddFactor */ 3: {
            const f: FactorValue = this$.fields[0];
            return new Process(p.ID, p.Name, p.ExecutesProtocol, p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, map<FSharpList<ProcessOutput_$union>, FSharpList<ProcessOutput_$union>>((i_1: FSharpList<ProcessOutput_$union>): FSharpList<ProcessOutput_$union> => i_1, p.Outputs), p.Comments);
        }
        case /* AddProtocol */ 4: {
            const pts: FSharpList<ProtocolTransformation_$union> = this$.fields[0];
            return new Process(p.ID, p.Name, fold<ProtocolTransformation_$union, Protocol>((pro_2: Protocol, trans: ProtocolTransformation_$union): Protocol => ProtocolTransformation__Transform_Z5F51792E(trans, pro_2), defaultArg(p.ExecutesProtocol, Protocol_get_empty()), pts), p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
        }
        default:
            return new Process(p.ID, this$.fields[0], p.ExecutesProtocol, p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
    }
}

export function ProcessTransformation__Equals_716E708F(this$: ProcessTransformation_$union, p: Process): boolean {
    const matchValue: Option<string> = p.Name;
    let matchResult: int32, n_1: string, n$0027_1: string;
    if (this$.tag === /* AddName */ 0) {
        if (matchValue != null) {
            if (this$.fields[0] === value_4(matchValue)) {
                matchResult = 0;
                n_1 = this$.fields[0];
                n$0027_1 = value_4(matchValue);
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

export type AssayTransformation_$union = 
    | AssayTransformation<0>
    | AssayTransformation<1>
    | AssayTransformation<2>
    | AssayTransformation<3>
    | AssayTransformation<4>

export type AssayTransformation_$cases = {
    0: ["AddFileName", [string]],
    1: ["AddParameter", [ProcessParameterValue]],
    2: ["AddCharacteristic", [MaterialAttributeValue]],
    3: ["AddFactor", [FactorValue]],
    4: ["AddProcess", [FSharpList<ProcessTransformation_$union>]]
}

export function AssayTransformation_AddFileName(Item: string) {
    return new AssayTransformation<0>(0, [Item]);
}

export function AssayTransformation_AddParameter(Item: ProcessParameterValue) {
    return new AssayTransformation<1>(1, [Item]);
}

export function AssayTransformation_AddCharacteristic(Item: MaterialAttributeValue) {
    return new AssayTransformation<2>(2, [Item]);
}

export function AssayTransformation_AddFactor(Item: FactorValue) {
    return new AssayTransformation<3>(3, [Item]);
}

export function AssayTransformation_AddProcess(Item: FSharpList<ProcessTransformation_$union>) {
    return new AssayTransformation<4>(4, [Item]);
}

export class AssayTransformation<Tag extends keyof AssayTransformation_$cases> extends Union<Tag, AssayTransformation_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: AssayTransformation_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["AddFileName", "AddParameter", "AddCharacteristic", "AddFactor", "AddProcess"];
    }
}

export function AssayTransformation_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.Builder.AssayTransformation", [], AssayTransformation, () => [[["Item", string_type]], [["Item", ProcessParameterValue_$reflection()]], [["Item", MaterialAttributeValue_$reflection()]], [["Item", FactorValue_$reflection()]], [["Item", list_type(ProcessTransformation_$reflection())]]]);
}

export function AssayTransformation__Transform_722A269D(this$: AssayTransformation_$union, a: Assay): Assay {
    switch (this$.tag) {
        case /* AddProcess */ 4: {
            const pts: FSharpList<ProcessTransformation_$union> = this$.fields[0];
            const processes: FSharpList<Process> = defaultArg(a.ProcessSequence, empty<Process>());
            return new Assay(a.ID, a.FileName, a.MeasurementType, a.TechnologyType, a.TechnologyPlatform, a.DataFiles, a.Materials, a.CharacteristicCategories, a.UnitCategories, exists<Process>((p: Process): boolean => exists<ProcessTransformation_$union>((trans: ProcessTransformation_$union): boolean => ProcessTransformation__Equals_716E708F(trans, p), pts), processes) ? map_1<Process, Process>((p_1: Process): Process => {
                if (exists<ProcessTransformation_$union>((trans_1: ProcessTransformation_$union): boolean => ProcessTransformation__Equals_716E708F(trans_1, p_1), pts)) {
                    return fold<ProcessTransformation_$union, Process>((p_2: Process, trans_2: ProcessTransformation_$union): Process => ProcessTransformation__Transform_716E708F(trans_2, p_2), p_1, pts);
                }
                else {
                    return p_1;
                }
            }, processes) : append(processes, singleton(fold<ProcessTransformation_$union, Process>((p_3: Process, trans_3: ProcessTransformation_$union): Process => ProcessTransformation__Transform_716E708F(trans_3, p_3), Process_get_empty(), pts))), a.Comments);
        }
        case /* AddFileName */ 0:
            return new Assay(a.ID, this$.fields[0], a.MeasurementType, a.TechnologyType, a.TechnologyPlatform, a.DataFiles, a.Materials, a.CharacteristicCategories, a.UnitCategories, a.ProcessSequence, a.Comments);
        default:
            return toFail(printf("Builder failed: Case %O Not implemented"))(this$);
    }
}

export function AssayTransformation__Equals_722A269D(this$: AssayTransformation_$union, a: Assay): boolean {
    const matchValue: Option<string> = a.FileName;
    let matchResult: int32, n_1: string, n$0027_1: string;
    if (this$.tag === /* AddFileName */ 0) {
        if (matchValue != null) {
            if (this$.fields[0] === value_4(matchValue)) {
                matchResult = 0;
                n_1 = this$.fields[0];
                n$0027_1 = value_4(matchValue);
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

export type StudyTransformation_$union = 
    | StudyTransformation<0>
    | StudyTransformation<1>
    | StudyTransformation<2>
    | StudyTransformation<3>
    | StudyTransformation<4>

export type StudyTransformation_$cases = {
    0: ["AddParameter", [ProcessParameterValue]],
    1: ["AddCharacteristic", [MaterialAttributeValue]],
    2: ["AddFactor", [FactorValue]],
    3: ["AddProcess", [FSharpList<ProcessTransformation_$union>]],
    4: ["AddAssay", [FSharpList<AssayTransformation_$union>]]
}

export function StudyTransformation_AddParameter(Item: ProcessParameterValue) {
    return new StudyTransformation<0>(0, [Item]);
}

export function StudyTransformation_AddCharacteristic(Item: MaterialAttributeValue) {
    return new StudyTransformation<1>(1, [Item]);
}

export function StudyTransformation_AddFactor(Item: FactorValue) {
    return new StudyTransformation<2>(2, [Item]);
}

export function StudyTransformation_AddProcess(Item: FSharpList<ProcessTransformation_$union>) {
    return new StudyTransformation<3>(3, [Item]);
}

export function StudyTransformation_AddAssay(Item: FSharpList<AssayTransformation_$union>) {
    return new StudyTransformation<4>(4, [Item]);
}

export class StudyTransformation<Tag extends keyof StudyTransformation_$cases> extends Union<Tag, StudyTransformation_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: StudyTransformation_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["AddParameter", "AddCharacteristic", "AddFactor", "AddProcess", "AddAssay"];
    }
}

export function StudyTransformation_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.Builder.StudyTransformation", [], StudyTransformation, () => [[["Item", ProcessParameterValue_$reflection()]], [["Item", MaterialAttributeValue_$reflection()]], [["Item", FactorValue_$reflection()]], [["Item", list_type(ProcessTransformation_$reflection())]], [["Item", list_type(AssayTransformation_$reflection())]]]);
}

export function StudyTransformation__Transform_7312BC8B(this$: StudyTransformation_$union, s: Study): Study {
    switch (this$.tag) {
        case /* AddProcess */ 3: {
            const pts: FSharpList<ProcessTransformation_$union> = this$.fields[0];
            const processes: FSharpList<Process> = defaultArg(s.ProcessSequence, empty<Process>());
            return new Study(s.ID, s.FileName, s.Identifier, s.Title, s.Description, s.SubmissionDate, s.PublicReleaseDate, s.Publications, s.Contacts, s.StudyDesignDescriptors, s.Protocols, s.Materials, exists<Process>((p: Process): boolean => exists<ProcessTransformation_$union>((trans: ProcessTransformation_$union): boolean => ProcessTransformation__Equals_716E708F(trans, p), pts), processes) ? map_1<Process, Process>((p_1: Process): Process => {
                if (exists<ProcessTransformation_$union>((trans_1: ProcessTransformation_$union): boolean => ProcessTransformation__Equals_716E708F(trans_1, p_1), pts)) {
                    return fold<ProcessTransformation_$union, Process>((p_2: Process, trans_2: ProcessTransformation_$union): Process => ProcessTransformation__Transform_716E708F(trans_2, p_2), p_1, pts);
                }
                else {
                    return p_1;
                }
            }, processes) : append(processes, singleton(fold<ProcessTransformation_$union, Process>((p_3: Process, trans_3: ProcessTransformation_$union): Process => ProcessTransformation__Transform_716E708F(trans_3, p_3), Process_get_empty(), pts))), s.Assays, s.Factors, s.CharacteristicCategories, s.UnitCategories, s.Comments);
        }
        case /* AddAssay */ 4: {
            const ats: FSharpList<AssayTransformation_$union> = this$.fields[0];
            const assays: FSharpList<Assay> = defaultArg(s.Assays, empty<Assay>());
            return new Study(s.ID, s.FileName, s.Identifier, s.Title, s.Description, s.SubmissionDate, s.PublicReleaseDate, s.Publications, s.Contacts, s.StudyDesignDescriptors, s.Protocols, s.Materials, s.ProcessSequence, exists<Assay>((a: Assay): boolean => exists<AssayTransformation_$union>((trans_4: AssayTransformation_$union): boolean => AssayTransformation__Equals_722A269D(trans_4, a), ats), assays) ? map_1<Assay, Assay>((a_1: Assay): Assay => {
                if (exists<AssayTransformation_$union>((trans_5: AssayTransformation_$union): boolean => AssayTransformation__Equals_722A269D(trans_5, a_1), ats)) {
                    return fold<AssayTransformation_$union, Assay>((a_2: Assay, trans_6: AssayTransformation_$union): Assay => AssayTransformation__Transform_722A269D(trans_6, a_2), a_1, ats);
                }
                else {
                    return a_1;
                }
            }, assays) : append(assays, singleton(fold<AssayTransformation_$union, Assay>((a_3: Assay, trans_7: AssayTransformation_$union): Assay => AssayTransformation__Transform_722A269D(trans_7, a_3), Assay_get_empty(), ats))), s.Factors, s.CharacteristicCategories, s.UnitCategories, s.Comments);
        }
        default:
            return toFail(printf("Builder failed: Case %O Not implemented"))(this$);
    }
}

