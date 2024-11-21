import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../OntologyAnnotation.js";
import { bind, map, defaultArg, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { Option_fromValueWithDefault } from "../Helper/Collections.js";
import { Record, toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, option_type, string_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ProtocolParameter extends Record {
    constructor(ID, ParameterName) {
        super();
        this.ID = ID;
        this.ParameterName = ParameterName;
    }
    static make(id, parameterName) {
        return new ProtocolParameter(id, parameterName);
    }
    static create(Id, ParameterName) {
        return ProtocolParameter.make(Id, ParameterName);
    }
    static get empty() {
        return ProtocolParameter.create();
    }
    static fromString(term, source, accession, comments) {
        const oa = OntologyAnnotation.create(term, source, accession, unwrap(comments));
        const parameterName = Option_fromValueWithDefault(new OntologyAnnotation(), oa);
        return ProtocolParameter.make(undefined, parameterName);
    }
    static toStringObject(pp) {
        const value = {
            TermAccessionNumber: "",
            TermName: "",
            TermSourceREF: "",
        };
        return defaultArg(map((oa) => OntologyAnnotation.toStringObject(oa), pp.ParameterName), value);
    }
    get NameText() {
        const this$ = this;
        return defaultArg(map((oa) => oa.NameText, this$.ParameterName), "");
    }
    get TryNameText() {
        const this$ = this;
        return unwrap(bind((oa) => oa.Name, this$.ParameterName));
    }
    MapCategory(f) {
        const this$ = this;
        return new ProtocolParameter(this$.ID, map(f, this$.ParameterName));
    }
    SetCategory(c) {
        const this$ = this;
        return new ProtocolParameter(this$.ID, c);
    }
    static tryGetNameText(pp) {
        return pp.TryNameText;
    }
    static getNameText(pp) {
        return defaultArg(ProtocolParameter.tryGetNameText(pp), "");
    }
    static nameEqualsString(name, pp) {
        return pp.NameText === name;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + this$.NameText;
    }
}

export function ProtocolParameter_$reflection() {
    return record_type("ARCtrl.Process.ProtocolParameter", [], ProtocolParameter, () => [["ID", option_type(string_type)], ["ParameterName", option_type(OntologyAnnotation_$reflection())]]);
}

