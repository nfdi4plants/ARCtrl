import { map } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { union_type, string_type, getUnionCases, name } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { Union, toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { ActivePatterns_$007CTermColumn$007C_$007C, Pattern_CommentPattern, Pattern_OutputPattern, Pattern_InputPattern, ActivePatterns_$007CRegex$007C_$007C, tryParseIOTypeHeader } from "../Helper/Regex.js";
import { printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../OntologyAnnotation.js";

export class IOType extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Source", "Sample", "Data", "Material", "FreeText"];
    }
    static get All() {
        return [new IOType(0, []), new IOType(1, []), new IOType(2, []), new IOType(3, [])];
    }
    static get Cases() {
        return map((x) => [x.tag, name(x)], getUnionCases(IOType_$reflection()));
    }
    get asInput() {
        const this$ = this;
        const stringCreate = (x) => {
            let copyOfStruct;
            return `Input [${(copyOfStruct = x, toString(copyOfStruct))}]`;
        };
        return (this$.tag === 4) ? stringCreate(this$.fields[0]) : stringCreate(this$);
    }
    get asOutput() {
        const this$ = this;
        const stringCreate = (x) => {
            let copyOfStruct;
            return `Output [${(copyOfStruct = x, toString(copyOfStruct))}]`;
        };
        return (this$.tag === 4) ? stringCreate(this$.fields[0]) : stringCreate(this$);
    }
    Merge(other) {
        const this$ = this;
        switch (this$.tag) {
            case 2:
                if (other.tag === 0) {
                    return new IOType(2, []);
                }
                else {
                    throw new Error(`Data IO column and ${other} can not be merged`);
                }
            case 1:
                switch (other.tag) {
                    case 0:
                        return new IOType(1, []);
                    case 1:
                        return new IOType(1, []);
                    default:
                        throw new Error(`Sample IO column and ${other} can not be merged`);
                }
            case 0:
                return (other.tag === 0) ? (new IOType(0, [])) : other;
            case 3:
                switch (other.tag) {
                    case 0:
                        return new IOType(3, []);
                    case 3:
                        return new IOType(3, []);
                    default:
                        throw new Error(`Material IO column and ${other} can not be merged`);
                }
            default:
                if (other.tag === 4) {
                    if (this$.fields[0] === other.fields[0]) {
                        return new IOType(4, [this$.fields[0]]);
                    }
                    else {
                        throw new Error(`FreeText IO column names ${this$.fields[0]} and ${other.fields[0]} do differ`);
                    }
                }
                else {
                    throw new Error(`FreeText IO column and ${other} can not be merged`);
                }
        }
    }
    toString() {
        const this$ = this;
        return (this$.tag === 1) ? "Sample Name" : ((this$.tag === 2) ? "Data" : ((this$.tag === 3) ? "Material" : ((this$.tag === 4) ? this$.fields[0] : "Source Name")));
    }
    static ofString(str) {
        return (str === "Source") ? (new IOType(0, [])) : ((str === "Source Name") ? (new IOType(0, [])) : ((str === "Sample") ? (new IOType(1, [])) : ((str === "Sample Name") ? (new IOType(1, [])) : ((str === "RawDataFile") ? (new IOType(2, [])) : ((str === "Raw Data File") ? (new IOType(2, [])) : ((str === "DerivedDataFile") ? (new IOType(2, [])) : ((str === "Derived Data File") ? (new IOType(2, [])) : ((str === "ImageFile") ? (new IOType(2, [])) : ((str === "Image File") ? (new IOType(2, [])) : ((str === "Data") ? (new IOType(2, [])) : ((str === "Material") ? (new IOType(3, [])) : (new IOType(4, [str])))))))))))));
    }
    static tryOfHeaderString(str) {
        const matchValue = tryParseIOTypeHeader(str);
        if (matchValue == null) {
            return undefined;
        }
        else {
            const s = matchValue;
            return IOType.ofString(s);
        }
    }
    static source() {
        return new IOType(0, []);
    }
    static sample() {
        return new IOType(1, []);
    }
    static data() {
        return new IOType(2, []);
    }
    static material() {
        return new IOType(3, []);
    }
    static freeText(s) {
        return new IOType(4, [s]);
    }
}

export function IOType_$reflection() {
    return union_type("ARCtrl.IOType", [], IOType, () => [[], [], [], [], [["Item", string_type]]]);
}

export class CompositeHeader extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Component", "Characteristic", "Factor", "Parameter", "ProtocolType", "ProtocolDescription", "ProtocolUri", "ProtocolVersion", "ProtocolREF", "Performer", "Date", "Input", "Output", "FreeText", "Comment"];
    }
    static get Cases() {
        return map((x) => [x.tag, name(x)], getUnionCases(CompositeHeader_$reflection()));
    }
    static jsGetColumnMetaType(inp) {
        return (inp === 0) ? 1 : ((inp === 1) ? 1 : ((inp === 2) ? 1 : ((inp === 3) ? 1 : ((inp === 4) ? 0 : ((inp === 5) ? 0 : ((inp === 6) ? 0 : ((inp === 7) ? 0 : ((inp === 8) ? 0 : ((inp === 9) ? 0 : ((inp === 10) ? 0 : ((inp === 11) ? 2 : ((inp === 12) ? 2 : ((inp === 13) ? 3 : ((inp === 14) ? 3 : toFail(printf("Cannot assign input `Tag` (%i) to `CompositeHeader`"))(inp)))))))))))))));
    }
    toString() {
        const this$ = this;
        return (this$.tag === 2) ? (`Factor [${this$.fields[0].NameText}]`) : ((this$.tag === 1) ? (`Characteristic [${this$.fields[0].NameText}]`) : ((this$.tag === 0) ? (`Component [${this$.fields[0].NameText}]`) : ((this$.tag === 4) ? "Protocol Type" : ((this$.tag === 8) ? "Protocol REF" : ((this$.tag === 5) ? "Protocol Description" : ((this$.tag === 6) ? "Protocol Uri" : ((this$.tag === 7) ? "Protocol Version" : ((this$.tag === 9) ? "Performer" : ((this$.tag === 10) ? "Date" : ((this$.tag === 11) ? this$.fields[0].asInput : ((this$.tag === 12) ? this$.fields[0].asOutput : ((this$.tag === 14) ? (`Comment [${this$.fields[0]}]`) : ((this$.tag === 13) ? this$.fields[0] : (`Parameter [${this$.fields[0].NameText}]`))))))))))))));
    }
    ToTerm() {
        const this$ = this;
        return (this$.tag === 2) ? this$.fields[0] : ((this$.tag === 1) ? this$.fields[0] : ((this$.tag === 0) ? this$.fields[0] : ((this$.tag === 4) ? OntologyAnnotation.create(toString(this$), undefined, this$.GetFeaturedColumnAccession) : ((this$.tag === 8) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 5) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 6) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 7) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 9) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 10) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 11) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 12) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 14) ? OntologyAnnotation.create(toString(this$)) : ((this$.tag === 13) ? OntologyAnnotation.create(toString(this$)) : this$.fields[0])))))))))))));
    }
    static OfHeaderString(str) {
        const matchValue = str.trim();
        const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_InputPattern, matchValue);
        if (activePatternResult != null) {
            const r = activePatternResult;
            const iotype = (r.groups && r.groups.iotype) || "";
            return new CompositeHeader(11, [IOType.ofString(iotype)]);
        }
        else {
            const activePatternResult_1 = ActivePatterns_$007CRegex$007C_$007C(Pattern_OutputPattern, matchValue);
            if (activePatternResult_1 != null) {
                const r_1 = activePatternResult_1;
                const iotype_1 = (r_1.groups && r_1.groups.iotype) || "";
                return new CompositeHeader(12, [IOType.ofString(iotype_1)]);
            }
            else {
                const activePatternResult_2 = ActivePatterns_$007CRegex$007C_$007C(Pattern_CommentPattern, matchValue);
                if (activePatternResult_2 != null) {
                    const r_2 = activePatternResult_2;
                    return new CompositeHeader(14, [(r_2.groups && r_2.groups.commentKey) || ""]);
                }
                else {
                    const activePatternResult_3 = ActivePatterns_$007CTermColumn$007C_$007C(matchValue);
                    if (activePatternResult_3 != null) {
                        const r_3 = activePatternResult_3;
                        const matchValue_1 = r_3.TermColumnType;
                        switch (matchValue_1) {
                            case "Parameter":
                            case "Parameter Value":
                                return new CompositeHeader(3, [OntologyAnnotation.create(r_3.TermName)]);
                            case "Factor":
                            case "Factor Value":
                                return new CompositeHeader(2, [OntologyAnnotation.create(r_3.TermName)]);
                            case "Characteristic":
                            case "Characteristics":
                            case "Characteristics Value":
                                return new CompositeHeader(1, [OntologyAnnotation.create(r_3.TermName)]);
                            case "Component":
                                return new CompositeHeader(0, [OntologyAnnotation.create(r_3.TermName)]);
                            default:
                                return new CompositeHeader(13, [str]);
                        }
                    }
                    else {
                        return (matchValue === "Date") ? (new CompositeHeader(10, [])) : ((matchValue === "Performer") ? (new CompositeHeader(9, [])) : ((matchValue === "Protocol Description") ? (new CompositeHeader(5, [])) : ((matchValue === "Protocol Uri") ? (new CompositeHeader(6, [])) : ((matchValue === "Protocol Version") ? (new CompositeHeader(7, [])) : ((matchValue === "Protocol Type") ? (new CompositeHeader(4, [])) : ((matchValue === "Protocol REF") ? (new CompositeHeader(8, [])) : (new CompositeHeader(13, [matchValue]))))))));
                    }
                }
            }
        }
    }
    get IsDeprecated() {
        const this$ = this;
        let matchResult, s_4, s_5, s_6, s_7;
        if (this$.tag === 13) {
            if (this$.fields[0].toLocaleLowerCase() === "sample name") {
                matchResult = 0;
                s_4 = this$.fields[0];
            }
            else if (this$.fields[0].toLocaleLowerCase() === "source name") {
                matchResult = 1;
                s_5 = this$.fields[0];
            }
            else if (this$.fields[0].toLocaleLowerCase() === "data file name") {
                matchResult = 2;
                s_6 = this$.fields[0];
            }
            else if (this$.fields[0].toLocaleLowerCase() === "derived data file") {
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
    get IsCvParamColumn() {
        const this$ = this;
        switch (this$.tag) {
            case 3:
            case 2:
            case 1:
            case 0:
                return true;
            default:
                return false;
        }
    }
    get IsTermColumn() {
        const this$ = this;
        switch (this$.tag) {
            case 3:
            case 2:
            case 1:
            case 0:
            case 4:
                return true;
            default:
                return false;
        }
    }
    get IsDataColumn() {
        const this$ = this;
        let matchResult, anythingElse;
        switch (this$.tag) {
            case 11: {
                if (this$.fields[0].tag === 2) {
                    matchResult = 0;
                }
                else {
                    matchResult = 1;
                    anythingElse = this$;
                }
                break;
            }
            case 12: {
                if (this$.fields[0].tag === 2) {
                    matchResult = 0;
                }
                else {
                    matchResult = 1;
                    anythingElse = this$;
                }
                break;
            }
            default: {
                matchResult = 1;
                anythingElse = this$;
            }
        }
        switch (matchResult) {
            case 0:
                return true;
            default:
                return false;
        }
    }
    get IsFeaturedColumn() {
        const this$ = this;
        return this$.tag === 4;
    }
    get GetFeaturedColumnAccession() {
        const this$ = this;
        if (this$.tag === 4) {
            return "DPBO:1000161";
        }
        else {
            throw new Error(`Tried matching ${this$} in getFeaturedColumnAccession, but is not a featured column.`);
        }
    }
    get GetColumnAccessionShort() {
        const this$ = this;
        switch (this$.tag) {
            case 4:
                return this$.GetFeaturedColumnAccession;
            case 3:
                return this$.fields[0].TermAccessionShort;
            case 2:
                return this$.fields[0].TermAccessionShort;
            case 1:
                return this$.fields[0].TermAccessionShort;
            case 0:
                return this$.fields[0].TermAccessionShort;
            default:
                throw new Error(`Tried matching ${this$}, but is not a column with an accession.`);
        }
    }
    get IsSingleColumn() {
        const this$ = this;
        switch (this$.tag) {
            case 13:
            case 11:
            case 12:
            case 14:
            case 8:
            case 5:
            case 6:
            case 7:
            case 9:
            case 10:
                return true;
            default:
                return false;
        }
    }
    get IsIOType() {
        const this$ = this;
        let matchResult, io;
        switch (this$.tag) {
            case 11: {
                matchResult = 0;
                io = this$.fields[0];
                break;
            }
            case 12: {
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
    get isInput() {
        const this$ = this;
        return this$.tag === 11;
    }
    get isOutput() {
        const this$ = this;
        return this$.tag === 12;
    }
    get isParameter() {
        const this$ = this;
        return this$.tag === 3;
    }
    get isFactor() {
        const this$ = this;
        return this$.tag === 2;
    }
    get isCharacteristic() {
        const this$ = this;
        return this$.tag === 1;
    }
    get isComponent() {
        const this$ = this;
        return this$.tag === 0;
    }
    get isProtocolType() {
        const this$ = this;
        return this$.tag === 4;
    }
    get isProtocolREF() {
        const this$ = this;
        return this$.tag === 8;
    }
    get isProtocolDescription() {
        const this$ = this;
        return this$.tag === 5;
    }
    get isProtocolUri() {
        const this$ = this;
        return this$.tag === 6;
    }
    get isProtocolVersion() {
        const this$ = this;
        return this$.tag === 7;
    }
    get isProtocolColumn() {
        const this$ = this;
        switch (this$.tag) {
            case 8:
            case 5:
            case 6:
            case 7:
            case 4:
                return true;
            default:
                return false;
        }
    }
    get isPerformer() {
        const this$ = this;
        return this$.tag === 9;
    }
    get isDate() {
        const this$ = this;
        return this$.tag === 10;
    }
    get isComment() {
        const this$ = this;
        return this$.tag === 14;
    }
    get isFreeText() {
        const this$ = this;
        return this$.tag === 13;
    }
    TryInput() {
        const this$ = this;
        return (this$.tag === 11) ? this$.fields[0] : undefined;
    }
    TryOutput() {
        const this$ = this;
        return (this$.tag === 12) ? this$.fields[0] : undefined;
    }
    TryIOType() {
        const this$ = this;
        let matchResult, io;
        switch (this$.tag) {
            case 12: {
                matchResult = 0;
                io = this$.fields[0];
                break;
            }
            case 11: {
                matchResult = 0;
                io = this$.fields[0];
                break;
            }
            default:
                matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return io;
            default:
                return undefined;
        }
    }
    get IsUnique() {
        const this$ = this;
        switch (this$.tag) {
            case 4:
            case 8:
            case 5:
            case 6:
            case 7:
            case 9:
            case 10:
            case 11:
            case 12:
                return true;
            default:
                return false;
        }
    }
    Copy() {
        const this$ = this;
        return (this$.tag === 3) ? (new CompositeHeader(3, [this$.fields[0].Copy()])) : ((this$.tag === 2) ? (new CompositeHeader(2, [this$.fields[0].Copy()])) : ((this$.tag === 1) ? (new CompositeHeader(1, [this$.fields[0].Copy()])) : ((this$.tag === 0) ? (new CompositeHeader(0, [this$.fields[0].Copy()])) : this$)));
    }
    static component(oa) {
        return new CompositeHeader(0, [oa]);
    }
    static characteristic(oa) {
        return new CompositeHeader(1, [oa]);
    }
    static factor(oa) {
        return new CompositeHeader(2, [oa]);
    }
    static parameter(oa) {
        return new CompositeHeader(3, [oa]);
    }
    static protocolType() {
        return new CompositeHeader(4, []);
    }
    static protocolDescription() {
        return new CompositeHeader(5, []);
    }
    static protocolUri() {
        return new CompositeHeader(6, []);
    }
    static protocolVersion() {
        return new CompositeHeader(7, []);
    }
    static protocolREF() {
        return new CompositeHeader(8, []);
    }
    static performer() {
        return new CompositeHeader(9, []);
    }
    static date() {
        return new CompositeHeader(10, []);
    }
    static input(io) {
        return new CompositeHeader(11, [io]);
    }
    static output(io) {
        return new CompositeHeader(12, [io]);
    }
    static freeText(s) {
        return new CompositeHeader(13, [s]);
    }
    static comment(s) {
        return new CompositeHeader(14, [s]);
    }
}

export function CompositeHeader_$reflection() {
    return union_type("ARCtrl.CompositeHeader", [], CompositeHeader, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [], [], [], [], [], [], [], [["Item", IOType_$reflection()]], [["Item", IOType_$reflection()]], [["Item", string_type]], [["Item", string_type]]]);
}

