import { Union, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { ActivePatterns_$007CTermColumn$007C_$007C, ActivePatterns_$007CRegex$007C_$007C, tryParseIOTypeHeader } from "../Regex.js";
import { union_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation__get_TermAccessionShort, OntologyAnnotation_fromString_2EB0E147, OntologyAnnotation__get_NameText } from "../JsonTypes/OntologyAnnotation.js";
import { ProtocolParameter_create_Z6C54B221 } from "../JsonTypes/ProtocolParameter.js";
import { Factor_create_3A99E5B8 } from "../JsonTypes/Factor.js";
import { MaterialAttribute_create_Z6C54B221 } from "../JsonTypes/MaterialAttribute.js";
import { Component_create_61502994 } from "../JsonTypes/Component.js";

export class IOType extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Source", "Sample", "RawDataFile", "DerivedDataFile", "ImageFile", "Material", "FreeText"];
    }
    static get All() {
        return [new IOType(0, []), new IOType(1, []), new IOType(2, []), new IOType(3, []), new IOType(4, []), new IOType(5, [])];
    }
    get asInput() {
        const this$ = this;
        const stringCreate = (x) => {
            let copyOfStruct;
            return `Input [${(copyOfStruct = x, toString(copyOfStruct))}]`;
        };
        return (this$.tag === 6) ? stringCreate(this$.fields[0]) : stringCreate(this$);
    }
    get asOutput() {
        const this$ = this;
        const stringCreate = (x) => {
            let copyOfStruct;
            return `Output [${(copyOfStruct = x, toString(copyOfStruct))}]`;
        };
        return (this$.tag === 6) ? stringCreate(this$.fields[0]) : stringCreate(this$);
    }
    toString() {
        const this$ = this;
        return (this$.tag === 1) ? "Sample Name" : ((this$.tag === 2) ? "Raw Data File" : ((this$.tag === 3) ? "Derived Data File" : ((this$.tag === 4) ? "Image File" : ((this$.tag === 5) ? "Material" : ((this$.tag === 6) ? this$.fields[0] : "Source Name")))));
    }
    static ofString(str) {
        return (str === "Source") ? (new IOType(0, [])) : ((str === "Source Name") ? (new IOType(0, [])) : ((str === "Sample") ? (new IOType(1, [])) : ((str === "Sample Name") ? (new IOType(1, [])) : ((str === "RawDataFile") ? (new IOType(2, [])) : ((str === "Raw Data File") ? (new IOType(2, [])) : ((str === "DerivedDataFile") ? (new IOType(3, [])) : ((str === "Derived Data File") ? (new IOType(3, [])) : ((str === "ImageFile") ? (new IOType(4, [])) : ((str === "Image File") ? (new IOType(4, [])) : ((str === "Material") ? (new IOType(5, [])) : (new IOType(6, [str]))))))))))));
    }
    static tryOfHeaderString(str) {
        const matchValue = tryParseIOTypeHeader(str);
        if (matchValue == null) {
            return void 0;
        }
        else {
            const s = matchValue;
            return IOType.ofString(s);
        }
    }
}

export function IOType_$reflection() {
    return union_type("ARCtrl.ISA.IOType", [], IOType, () => [[], [], [], [], [], [], [["Item", string_type]]]);
}

export class CompositeHeader extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Component", "Characteristic", "Factor", "Parameter", "ProtocolType", "ProtocolDescription", "ProtocolUri", "ProtocolVersion", "ProtocolREF", "Performer", "Date", "Input", "Output", "FreeText"];
    }
    toString() {
        const this$ = this;
        return (this$.tag === 2) ? (`Factor [${OntologyAnnotation__get_NameText(this$.fields[0])}]`) : ((this$.tag === 1) ? (`Characteristic [${OntologyAnnotation__get_NameText(this$.fields[0])}]`) : ((this$.tag === 0) ? (`Component [${OntologyAnnotation__get_NameText(this$.fields[0])}]`) : ((this$.tag === 4) ? "Protocol Type" : ((this$.tag === 8) ? "Protocol REF" : ((this$.tag === 5) ? "Protocol Description" : ((this$.tag === 6) ? "Protocol Uri" : ((this$.tag === 7) ? "Protocol Version" : ((this$.tag === 9) ? "Performer" : ((this$.tag === 10) ? "Date" : ((this$.tag === 11) ? this$.fields[0].asInput : ((this$.tag === 12) ? this$.fields[0].asOutput : ((this$.tag === 13) ? this$.fields[0] : (`Parameter [${OntologyAnnotation__get_NameText(this$.fields[0])}]`)))))))))))));
    }
    ToTerm() {
        const this$ = this;
        return (this$.tag === 2) ? this$.fields[0] : ((this$.tag === 1) ? this$.fields[0] : ((this$.tag === 0) ? this$.fields[0] : ((this$.tag === 4) ? OntologyAnnotation_fromString_2EB0E147("Protocol Type") : ((this$.tag === 8) ? OntologyAnnotation_fromString_2EB0E147("Protocol REF") : ((this$.tag === 5) ? OntologyAnnotation_fromString_2EB0E147("Protocol Description") : ((this$.tag === 6) ? OntologyAnnotation_fromString_2EB0E147("Protocol Uri") : ((this$.tag === 7) ? OntologyAnnotation_fromString_2EB0E147("Protocol Version") : ((this$.tag === 9) ? OntologyAnnotation_fromString_2EB0E147("Performer") : ((this$.tag === 10) ? OntologyAnnotation_fromString_2EB0E147("Date") : ((this$.tag === 11) ? OntologyAnnotation_fromString_2EB0E147(this$.fields[0].asInput) : ((this$.tag === 12) ? OntologyAnnotation_fromString_2EB0E147(this$.fields[0].asOutput) : ((this$.tag === 13) ? OntologyAnnotation_fromString_2EB0E147(this$.fields[0]) : this$.fields[0]))))))))))));
    }
    static OfHeaderString(str) {
        const matchValue = str.trim();
        const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("Input\\s\\[(?<iotype>.+)\\]", matchValue);
        if (activePatternResult != null) {
            const r = activePatternResult;
            const iotype = (r.groups && r.groups.iotype) || "";
            return new CompositeHeader(11, [IOType.ofString(iotype)]);
        }
        else {
            const activePatternResult_1 = ActivePatterns_$007CRegex$007C_$007C("Output\\s\\[(?<iotype>.+)\\]", matchValue);
            if (activePatternResult_1 != null) {
                const r_1 = activePatternResult_1;
                const iotype_1 = (r_1.groups && r_1.groups.iotype) || "";
                return new CompositeHeader(12, [IOType.ofString(iotype_1)]);
            }
            else {
                const activePatternResult_2 = ActivePatterns_$007CTermColumn$007C_$007C(matchValue);
                if (activePatternResult_2 != null) {
                    const r_2 = activePatternResult_2;
                    const matchValue_1 = r_2.TermColumnType;
                    switch (matchValue_1) {
                        case "Parameter":
                        case "Parameter Value":
                            return new CompositeHeader(3, [OntologyAnnotation_fromString_2EB0E147(r_2.TermName)]);
                        case "Factor":
                        case "Factor Value":
                            return new CompositeHeader(2, [OntologyAnnotation_fromString_2EB0E147(r_2.TermName)]);
                        case "Characteristic":
                        case "Characteristics":
                        case "Characteristics Value":
                            return new CompositeHeader(1, [OntologyAnnotation_fromString_2EB0E147(r_2.TermName)]);
                        case "Component":
                            return new CompositeHeader(0, [OntologyAnnotation_fromString_2EB0E147(r_2.TermName)]);
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
                return OntologyAnnotation__get_TermAccessionShort(this$.fields[0]);
            case 2:
                return OntologyAnnotation__get_TermAccessionShort(this$.fields[0]);
            case 1:
                return OntologyAnnotation__get_TermAccessionShort(this$.fields[0]);
            case 0:
                return OntologyAnnotation__get_TermAccessionShort(this$.fields[0]);
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
        return this$.tag === 11;
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
    get isPerformer() {
        const this$ = this;
        return this$.tag === 9;
    }
    get isDate() {
        const this$ = this;
        return this$.tag === 10;
    }
    get isFreeText() {
        const this$ = this;
        return this$.tag === 13;
    }
    TryParameter() {
        const this$ = this;
        return (this$.tag === 3) ? ProtocolParameter_create_Z6C54B221(void 0, this$.fields[0]) : void 0;
    }
    TryFactor() {
        const this$ = this;
        return (this$.tag === 2) ? Factor_create_3A99E5B8(void 0, void 0, this$.fields[0]) : void 0;
    }
    TryCharacteristic() {
        const this$ = this;
        return (this$.tag === 1) ? MaterialAttribute_create_Z6C54B221(void 0, this$.fields[0]) : void 0;
    }
    TryComponent() {
        const this$ = this;
        return (this$.tag === 0) ? Component_create_61502994(void 0, void 0, void 0, this$.fields[0]) : void 0;
    }
}

export function CompositeHeader_$reflection() {
    return union_type("ARCtrl.ISA.CompositeHeader", [], CompositeHeader, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [["Item", OntologyAnnotation_$reflection()]], [], [], [], [], [], [], [], [["Item", IOType_$reflection()]], [["Item", IOType_$reflection()]], [["Item", string_type]]]);
}

