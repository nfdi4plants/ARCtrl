import { Union, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { ActivePatterns_$007CTermColumn$007C_$007C, ActivePatterns_$007CRegex$007C_$007C, tryParseIOTypeHeader } from "../Regex.js";
import { value, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { union_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { ProtocolParameter, ProtocolParameter_create_Z6C54B221 } from "../JsonTypes/ProtocolParameter.js";
import { Factor } from "../JsonTypes/Factor.js";
import { MaterialAttribute, MaterialAttribute_create_Z6C54B221 } from "../JsonTypes/MaterialAttribute.js";
import { Component, Component_create_61502994 } from "../JsonTypes/Component.js";

export type IOType_$union = 
    | IOType<0>
    | IOType<1>
    | IOType<2>
    | IOType<3>
    | IOType<4>
    | IOType<5>
    | IOType<6>

export type IOType_$cases = {
    0: ["Source", []],
    1: ["Sample", []],
    2: ["RawDataFile", []],
    3: ["DerivedDataFile", []],
    4: ["ImageFile", []],
    5: ["Material", []],
    6: ["FreeText", [string]]
}

export function IOType_Source() {
    return new IOType<0>(0, []);
}

export function IOType_Sample() {
    return new IOType<1>(1, []);
}

export function IOType_RawDataFile() {
    return new IOType<2>(2, []);
}

export function IOType_DerivedDataFile() {
    return new IOType<3>(3, []);
}

export function IOType_ImageFile() {
    return new IOType<4>(4, []);
}

export function IOType_Material() {
    return new IOType<5>(5, []);
}

export function IOType_FreeText(Item: string) {
    return new IOType<6>(6, [Item]);
}

export class IOType<Tag extends keyof IOType_$cases> extends Union<Tag, IOType_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: IOType_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Source", "Sample", "RawDataFile", "DerivedDataFile", "ImageFile", "Material", "FreeText"];
    }
    static get All(): IOType_$union[] {
        return [IOType_Source(), IOType_Sample(), IOType_RawDataFile(), IOType_DerivedDataFile(), IOType_ImageFile(), IOType_Material()];
    }
    get asInput(): string {
        const this$ = this as IOType_$union;
        const stringCreate = <$a>(x: $a): string => {
            let copyOfStruct: $a;
            return `Input [${(copyOfStruct = x, toString(copyOfStruct))}]`;
        };
        return (this$.tag === /* FreeText */ 6) ? stringCreate(this$.fields[0]) : stringCreate(this$);
    }
    get asOutput(): string {
        const this$ = this as IOType_$union;
        const stringCreate = <$a>(x: $a): string => {
            let copyOfStruct: $a;
            return `Output [${(copyOfStruct = x, toString(copyOfStruct))}]`;
        };
        return (this$.tag === /* FreeText */ 6) ? stringCreate(this$.fields[0]) : stringCreate(this$);
    }
    toString(): string {
        const this$ = this as IOType_$union;
        return (this$.tag === /* Sample */ 1) ? "Sample Name" : ((this$.tag === /* RawDataFile */ 2) ? "Raw Data File" : ((this$.tag === /* DerivedDataFile */ 3) ? "Derived Data File" : ((this$.tag === /* ImageFile */ 4) ? "Image File" : ((this$.tag === /* Material */ 5) ? "Material" : ((this$.tag === /* FreeText */ 6) ? this$.fields[0] : "Source Name")))));
    }
    static ofString(str: string): IOType_$union {
        return (str === "Source") ? IOType_Source() : ((str === "Source Name") ? IOType_Source() : ((str === "Sample") ? IOType_Sample() : ((str === "Sample Name") ? IOType_Sample() : ((str === "RawDataFile") ? IOType_RawDataFile() : ((str === "Raw Data File") ? IOType_RawDataFile() : ((str === "DerivedDataFile") ? IOType_DerivedDataFile() : ((str === "Derived Data File") ? IOType_DerivedDataFile() : ((str === "ImageFile") ? IOType_ImageFile() : ((str === "Image File") ? IOType_ImageFile() : ((str === "Material") ? IOType_Material() : IOType_FreeText(str)))))))))));
    }
    static tryOfHeaderString(str: string): Option<IOType_$union> {
        const matchValue: Option<string> = tryParseIOTypeHeader(str);
        if (matchValue == null) {
            return void 0;
        }
        else {
            const s: string = value(matchValue);
            return IOType.ofString(s);
        }
    }
}

export function IOType_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.IOType", [], IOType, () => [[], [], [], [], [], [], [["Item", string_type]]]);
}

export type CompositeHeader_$union = 
    | CompositeHeader<0>
    | CompositeHeader<1>
    | CompositeHeader<2>
    | CompositeHeader<3>
    | CompositeHeader<4>
    | CompositeHeader<5>
    | CompositeHeader<6>
    | CompositeHeader<7>
    | CompositeHeader<8>
    | CompositeHeader<9>
    | CompositeHeader<10>
    | CompositeHeader<11>
    | CompositeHeader<12>
    | CompositeHeader<13>

export type CompositeHeader_$cases = {
    0: ["Component", [OntologyAnnotation]],
    1: ["Characteristic", [OntologyAnnotation]],
    2: ["Factor", [OntologyAnnotation]],
    3: ["Parameter", [OntologyAnnotation]],
    4: ["ProtocolType", []],
    5: ["ProtocolDescription", []],
    6: ["ProtocolUri", []],
    7: ["ProtocolVersion", []],
    8: ["ProtocolREF", []],
    9: ["Performer", []],
    10: ["Date", []],
    11: ["Input", [IOType_$union]],
    12: ["Output", [IOType_$union]],
    13: ["FreeText", [string]]
}

export function CompositeHeader_Component(Item: OntologyAnnotation) {
    return new CompositeHeader<0>(0, [Item]);
}

export function CompositeHeader_Characteristic(Item: OntologyAnnotation) {
    return new CompositeHeader<1>(1, [Item]);
}

export function CompositeHeader_Factor(Item: OntologyAnnotation) {
    return new CompositeHeader<2>(2, [Item]);
}

export function CompositeHeader_Parameter(Item: OntologyAnnotation) {
    return new CompositeHeader<3>(3, [Item]);
}

export function CompositeHeader_ProtocolType() {
    return new CompositeHeader<4>(4, []);
}

export function CompositeHeader_ProtocolDescription() {
    return new CompositeHeader<5>(5, []);
}

export function CompositeHeader_ProtocolUri() {
    return new CompositeHeader<6>(6, []);
}

export function CompositeHeader_ProtocolVersion() {
    return new CompositeHeader<7>(7, []);
}

export function CompositeHeader_ProtocolREF() {
    return new CompositeHeader<8>(8, []);
}

export function CompositeHeader_Performer() {
    return new CompositeHeader<9>(9, []);
}

export function CompositeHeader_Date() {
    return new CompositeHeader<10>(10, []);
}

export function CompositeHeader_Input(Item: IOType_$union) {
    return new CompositeHeader<11>(11, [Item]);
}

export function CompositeHeader_Output(Item: IOType_$union) {
    return new CompositeHeader<12>(12, [Item]);
}

export function CompositeHeader_FreeText(Item: string) {
    return new CompositeHeader<13>(13, [Item]);
}

export class CompositeHeader<Tag extends keyof CompositeHeader_$cases> extends Union<Tag, CompositeHeader_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: CompositeHeader_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Component", "Characteristic", "Factor", "Parameter", "ProtocolType", "ProtocolDescription", "ProtocolUri", "ProtocolVersion", "ProtocolREF", "Performer", "Date", "Input", "Output", "FreeText"];
    }
    toString(): string {
        const this$ = this as CompositeHeader_$union;
        switch (this$.tag) {
            case /* Factor */ 2: {
                const oa_1: OntologyAnnotation = this$.fields[0];
                return `Factor [${oa_1.NameText}]`;
            }
            case /* Characteristic */ 1: {
                const oa_2: OntologyAnnotation = this$.fields[0];
                return `Characteristic [${oa_2.NameText}]`;
            }
            case /* Component */ 0: {
                const oa_3: OntologyAnnotation = this$.fields[0];
                return `Component [${oa_3.NameText}]`;
            }
            case /* ProtocolType */ 4:
                return "Protocol Type";
            case /* ProtocolREF */ 8:
                return "Protocol REF";
            case /* ProtocolDescription */ 5:
                return "Protocol Description";
            case /* ProtocolUri */ 6:
                return "Protocol Uri";
            case /* ProtocolVersion */ 7:
                return "Protocol Version";
            case /* Performer */ 9:
                return "Performer";
            case /* Date */ 10:
                return "Date";
            case /* Input */ 11: {
                const io: IOType_$union = this$.fields[0];
                return io.asInput;
            }
            case /* Output */ 12: {
                const io_1: IOType_$union = this$.fields[0];
                return io_1.asOutput;
            }
            case /* FreeText */ 13:
                return this$.fields[0];
            default: {
                const oa: OntologyAnnotation = this$.fields[0];
                return `Parameter [${oa.NameText}]`;
            }
        }
    }
    ToTerm(): OntologyAnnotation {
        const this$ = this as CompositeHeader_$union;
        switch (this$.tag) {
            case /* Factor */ 2:
                return this$.fields[0];
            case /* Characteristic */ 1:
                return this$.fields[0];
            case /* Component */ 0:
                return this$.fields[0];
            case /* ProtocolType */ 4:
                return OntologyAnnotation.fromString("Protocol Type");
            case /* ProtocolREF */ 8:
                return OntologyAnnotation.fromString("Protocol REF");
            case /* ProtocolDescription */ 5:
                return OntologyAnnotation.fromString("Protocol Description");
            case /* ProtocolUri */ 6:
                return OntologyAnnotation.fromString("Protocol Uri");
            case /* ProtocolVersion */ 7:
                return OntologyAnnotation.fromString("Protocol Version");
            case /* Performer */ 9:
                return OntologyAnnotation.fromString("Performer");
            case /* Date */ 10:
                return OntologyAnnotation.fromString("Date");
            case /* Input */ 11: {
                const io: IOType_$union = this$.fields[0];
                return OntologyAnnotation.fromString(io.asInput);
            }
            case /* Output */ 12: {
                const io_1: IOType_$union = this$.fields[0];
                return OntologyAnnotation.fromString(io_1.asOutput);
            }
            case /* FreeText */ 13: {
                const str: string = this$.fields[0];
                return OntologyAnnotation.fromString(str);
            }
            default:
                return this$.fields[0];
        }
    }
    static OfHeaderString(str: string): CompositeHeader_$union {
        const matchValue: string = str.trim();
        const activePatternResult: Option<any> = ActivePatterns_$007CRegex$007C_$007C("Input\\s\\[(?<iotype>.+)\\]", matchValue);
        if (activePatternResult != null) {
            const r: any = value(activePatternResult);
            const iotype: string = (r.groups && r.groups.iotype) || "";
            return CompositeHeader_Input(IOType.ofString(iotype));
        }
        else {
            const activePatternResult_1: Option<any> = ActivePatterns_$007CRegex$007C_$007C("Output\\s\\[(?<iotype>.+)\\]", matchValue);
            if (activePatternResult_1 != null) {
                const r_1: any = value(activePatternResult_1);
                const iotype_1: string = (r_1.groups && r_1.groups.iotype) || "";
                return CompositeHeader_Output(IOType.ofString(iotype_1));
            }
            else {
                const activePatternResult_2: Option<{ TermColumnType: string, TermName: string }> = ActivePatterns_$007CTermColumn$007C_$007C(matchValue);
                if (activePatternResult_2 != null) {
                    const r_2: { TermColumnType: string, TermName: string } = value(activePatternResult_2);
                    const matchValue_1: string = r_2.TermColumnType;
                    switch (matchValue_1) {
                        case "Parameter":
                        case "Parameter Value":
                            return CompositeHeader_Parameter(OntologyAnnotation.fromString(r_2.TermName));
                        case "Factor":
                        case "Factor Value":
                            return CompositeHeader_Factor(OntologyAnnotation.fromString(r_2.TermName));
                        case "Characteristic":
                        case "Characteristics":
                        case "Characteristics Value":
                            return CompositeHeader_Characteristic(OntologyAnnotation.fromString(r_2.TermName));
                        case "Component":
                            return CompositeHeader_Component(OntologyAnnotation.fromString(r_2.TermName));
                        default:
                            return CompositeHeader_FreeText(str);
                    }
                }
                else {
                    return (matchValue === "Date") ? CompositeHeader_Date() : ((matchValue === "Performer") ? CompositeHeader_Performer() : ((matchValue === "Protocol Description") ? CompositeHeader_ProtocolDescription() : ((matchValue === "Protocol Uri") ? CompositeHeader_ProtocolUri() : ((matchValue === "Protocol Version") ? CompositeHeader_ProtocolVersion() : ((matchValue === "Protocol Type") ? CompositeHeader_ProtocolType() : ((matchValue === "Protocol REF") ? CompositeHeader_ProtocolREF() : CompositeHeader_FreeText(matchValue)))))));
                }
            }
        }
    }
    get IsDeprecated(): boolean {
        let s: string, s_1: string, s_2: string, s_3: string;
        const this$ = this as CompositeHeader_$union;
        let matchResult: int32, s_4: string, s_5: string, s_6: string, s_7: string;
        if (this$.tag === /* FreeText */ 13) {
            if ((s = this$.fields[0], s.toLocaleLowerCase() === "sample name")) {
                matchResult = 0;
                s_4 = this$.fields[0];
            }
            else if ((s_1 = this$.fields[0], s_1.toLocaleLowerCase() === "source name")) {
                matchResult = 1;
                s_5 = this$.fields[0];
            }
            else if ((s_2 = this$.fields[0], s_2.toLocaleLowerCase() === "data file name")) {
                matchResult = 2;
                s_6 = this$.fields[0];
            }
            else if ((s_3 = this$.fields[0], s_3.toLocaleLowerCase() === "derived data file")) {
                matchResult = 3;
                s_7 = this$.fields[0];
            }
            else {
                matchResult = 4;
            }
        }
        else {
            matchResult = 4;
        }
        switch (matchResult) {
            case 0:
                return true;
            case 1:
                return true;
            case 2:
                return true;
            case 3:
                return true;
            default:
                return false;
        }
    }
    get IsCvParamColumn(): boolean {
        const this$ = this as CompositeHeader_$union;
        switch (this$.tag) {
            case /* Parameter */ 3:
            case /* Factor */ 2:
            case /* Characteristic */ 1:
            case /* Component */ 0:
                return true;
            default:
                return false;
        }
    }
    get IsTermColumn(): boolean {
        const this$ = this as CompositeHeader_$union;
        switch (this$.tag) {
            case /* Parameter */ 3:
            case /* Factor */ 2:
            case /* Characteristic */ 1:
            case /* Component */ 0:
            case /* ProtocolType */ 4:
                return true;
            default:
                return false;
        }
    }
    get IsFeaturedColumn(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* ProtocolType */ 4;
    }
    get GetFeaturedColumnAccession(): string {
        const this$ = this as CompositeHeader_$union;
        if (this$.tag === /* ProtocolType */ 4) {
            return "DPBO:1000161";
        }
        else {
            throw new Error(`Tried matching ${this$} in getFeaturedColumnAccession, but is not a featured column.`);
        }
    }
    get GetColumnAccessionShort(): string {
        const this$ = this as CompositeHeader_$union;
        switch (this$.tag) {
            case /* ProtocolType */ 4:
                return this$.GetFeaturedColumnAccession;
            case /* Parameter */ 3: {
                const oa: OntologyAnnotation = this$.fields[0];
                return oa.TermAccessionShort;
            }
            case /* Factor */ 2: {
                const oa_1: OntologyAnnotation = this$.fields[0];
                return oa_1.TermAccessionShort;
            }
            case /* Characteristic */ 1: {
                const oa_2: OntologyAnnotation = this$.fields[0];
                return oa_2.TermAccessionShort;
            }
            case /* Component */ 0: {
                const oa_3: OntologyAnnotation = this$.fields[0];
                return oa_3.TermAccessionShort;
            }
            default:
                throw new Error(`Tried matching ${this$}, but is not a column with an accession.`);
        }
    }
    get IsSingleColumn(): boolean {
        const this$ = this as CompositeHeader_$union;
        switch (this$.tag) {
            case /* FreeText */ 13:
            case /* Input */ 11:
            case /* Output */ 12:
            case /* ProtocolREF */ 8:
            case /* ProtocolDescription */ 5:
            case /* ProtocolUri */ 6:
            case /* ProtocolVersion */ 7:
            case /* Performer */ 9:
            case /* Date */ 10:
                return true;
            default:
                return false;
        }
    }
    get IsIOType(): boolean {
        const this$ = this as CompositeHeader_$union;
        let matchResult: int32, io: IOType_$union;
        switch (this$.tag) {
            case /* Input */ 11: {
                matchResult = 0;
                io = this$.fields[0];
                break;
            }
            case /* Output */ 12: {
                matchResult = 0;
                io = this$.fields[0];
                break;
            }
            default:
                matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return true;
            default:
                return false;
        }
    }
    get isInput(): boolean {
        const this$ = this as CompositeHeader_$union;
        if (this$.tag === /* Input */ 11) {
            const io: IOType_$union = this$.fields[0];
            return true;
        }
        else {
            return false;
        }
    }
    get isOutput(): boolean {
        const this$ = this as CompositeHeader_$union;
        if (this$.tag === /* Input */ 11) {
            const io: IOType_$union = this$.fields[0];
            return true;
        }
        else {
            return false;
        }
    }
    get isParameter(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* Parameter */ 3;
    }
    get isFactor(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* Factor */ 2;
    }
    get isCharacteristic(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* Characteristic */ 1;
    }
    get isComponent(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* Component */ 0;
    }
    get isProtocolType(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* ProtocolType */ 4;
    }
    get isProtocolREF(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* ProtocolREF */ 8;
    }
    get isProtocolDescription(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* ProtocolDescription */ 5;
    }
    get isProtocolUri(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* ProtocolUri */ 6;
    }
    get isProtocolVersion(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* ProtocolVersion */ 7;
    }
    get isPerformer(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* Performer */ 9;
    }
    get isDate(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* Date */ 10;
    }
    get isFreeText(): boolean {
        const this$ = this as CompositeHeader_$union;
        return this$.tag === /* FreeText */ 13;
    }
    TryParameter(): Option<ProtocolParameter> {
        const this$ = this as CompositeHeader_$union;
        return (this$.tag === /* Parameter */ 3) ? ProtocolParameter_create_Z6C54B221(void 0, this$.fields[0]) : void 0;
    }
    TryFactor(): Option<Factor> {
        const this$ = this as CompositeHeader_$union;
        if (this$.tag === /* Factor */ 2) {
            const oa: OntologyAnnotation = this$.fields[0];
            return Factor.create(void 0, void 0, oa);
        }
        else {
            return void 0;
        }
    }
    TryCharacteristic(): Option<MaterialAttribute> {
        const this$ = this as CompositeHeader_$union;
        return (this$.tag === /* Characteristic */ 1) ? MaterialAttribute_create_Z6C54B221(void 0, this$.fields[0]) : void 0;
    }
    TryComponent(): Option<Component> {
        const this$ = this as CompositeHeader_$union;
        return (this$.tag === /* Component */ 0) ? Component_create_61502994(void 0, void 0, void 0, this$.fields[0]) : void 0;
    }
}

export function CompositeHeader_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.CompositeHeader", [], CompositeHeader, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [], [], [], [], [], [], [], [["Item", IOType_$reflection()]], [["Item", IOType_$reflection()]], [["Item", string_type]]]);
}

