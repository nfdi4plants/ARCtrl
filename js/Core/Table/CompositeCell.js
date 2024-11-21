import { unwrap, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../OntologyAnnotation.js";
import { Data_$reflection, Data } from "../Data.js";
import { Union } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { union_type, string_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class CompositeCell extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Term", "FreeText", "Unitized", "Data"];
    }
    get isUnitized() {
        const this$ = this;
        return this$.tag === 2;
    }
    get isTerm() {
        const this$ = this;
        return this$.tag === 0;
    }
    get isFreeText() {
        const this$ = this;
        return this$.tag === 1;
    }
    get isData() {
        const this$ = this;
        return this$.tag === 3;
    }
    GetEmptyCell() {
        const this$ = this;
        return (this$.tag === 2) ? CompositeCell.emptyUnitized : ((this$.tag === 1) ? CompositeCell.emptyFreeText : ((this$.tag === 3) ? CompositeCell.emptyData : CompositeCell.emptyTerm));
    }
    GetContent() {
        const this$ = this;
        switch (this$.tag) {
            case 0: {
                const oa = this$.fields[0];
                return [oa.NameText, defaultArg(oa.TermSourceREF, ""), defaultArg(oa.TermAccessionNumber, "")];
            }
            case 2: {
                const oa_1 = this$.fields[1];
                return [this$.fields[0], oa_1.NameText, defaultArg(oa_1.TermSourceREF, ""), defaultArg(oa_1.TermAccessionNumber, "")];
            }
            case 3: {
                const d = this$.fields[0];
                return [defaultArg(d.Name, ""), defaultArg(d.Format, ""), defaultArg(d.SelectorFormat, "")];
            }
            default:
                return [this$.fields[0]];
        }
    }
    ToUnitizedCell() {
        const this$ = this;
        return (this$.tag === 1) ? (new CompositeCell(2, ["", OntologyAnnotation.create(this$.fields[0])])) : ((this$.tag === 0) ? (new CompositeCell(2, ["", this$.fields[0]])) : ((this$.tag === 3) ? (new CompositeCell(2, ["", OntologyAnnotation.create(this$.fields[0].NameText)])) : this$));
    }
    ToTermCell() {
        const this$ = this;
        return (this$.tag === 2) ? (new CompositeCell(0, [this$.fields[1]])) : ((this$.tag === 1) ? (new CompositeCell(0, [OntologyAnnotation.create(this$.fields[0])])) : ((this$.tag === 3) ? (new CompositeCell(0, [new OntologyAnnotation(this$.fields[0].NameText)])) : this$));
    }
    ToFreeTextCell() {
        const this$ = this;
        return (this$.tag === 0) ? (new CompositeCell(1, [this$.fields[0].NameText])) : ((this$.tag === 2) ? (new CompositeCell(1, [this$.fields[1].NameText])) : ((this$.tag === 3) ? (new CompositeCell(1, [this$.fields[0].NameText])) : this$));
    }
    ToDataCell() {
        const this$ = this;
        return (this$.tag === 1) ? CompositeCell.createDataFromString(this$.fields[0]) : ((this$.tag === 0) ? CompositeCell.createDataFromString(this$.fields[0].NameText) : ((this$.tag === 3) ? this$ : CompositeCell.createDataFromString(this$.fields[1].NameText)));
    }
    get AsUnitized() {
        const this$ = this;
        if (this$.tag === 2) {
            return [this$.fields[0], this$.fields[1]];
        }
        else {
            throw new Error("Not a Unitized cell.");
        }
    }
    get AsTerm() {
        const this$ = this;
        if (this$.tag === 0) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a Term Cell.");
        }
    }
    get AsFreeText() {
        const this$ = this;
        if (this$.tag === 1) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a FreeText Cell.");
        }
    }
    get AsData() {
        const this$ = this;
        if (this$.tag === 3) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a Data Cell.");
        }
    }
    static createTerm(oa) {
        return new CompositeCell(0, [oa]);
    }
    static createTermFromString(name, tsr, tan) {
        return new CompositeCell(0, [OntologyAnnotation.create(unwrap(name), unwrap(tsr), unwrap(tan))]);
    }
    static createUnitized(value, oa) {
        return new CompositeCell(2, [value, defaultArg(oa, new OntologyAnnotation())]);
    }
    static createUnitizedFromString(value, name, tsr, tan) {
        const tupledArg = [value, OntologyAnnotation.create(unwrap(name), unwrap(tsr), unwrap(tan))];
        return new CompositeCell(2, [tupledArg[0], tupledArg[1]]);
    }
    static createFreeText(value) {
        return new CompositeCell(1, [value]);
    }
    static createData(d) {
        return new CompositeCell(3, [d]);
    }
    static createDataFromString(value, format, selectorFormat) {
        return new CompositeCell(3, [Data.create(undefined, value, undefined, unwrap(format), unwrap(selectorFormat))]);
    }
    static get emptyTerm() {
        return new CompositeCell(0, [new OntologyAnnotation()]);
    }
    static get emptyFreeText() {
        return new CompositeCell(1, [""]);
    }
    static get emptyUnitized() {
        return new CompositeCell(2, ["", new OntologyAnnotation()]);
    }
    static get emptyData() {
        return new CompositeCell(3, [Data.create()]);
    }
    UpdateWithOA(oa) {
        const this$ = this;
        switch (this$.tag) {
            case 2:
                return CompositeCell.createUnitized(this$.fields[0], oa);
            case 1:
                return CompositeCell.createFreeText(oa.NameText);
            case 3: {
                const d = this$.fields[0];
                d.Name = oa.NameText;
                return new CompositeCell(3, [d]);
            }
            default:
                return CompositeCell.createTerm(oa);
        }
    }
    static updateWithOA(oa, cell) {
        return cell.UpdateWithOA(oa);
    }
    toString() {
        const this$ = this;
        return (this$.tag === 1) ? this$.fields[0] : ((this$.tag === 2) ? (`${this$.fields[0]} ${this$.fields[1].NameText}`) : ((this$.tag === 3) ? (`${this$.fields[0].NameText}`) : (`${this$.fields[0].NameText}`)));
    }
    Copy() {
        const this$ = this;
        return (this$.tag === 1) ? (new CompositeCell(1, [this$.fields[0]])) : ((this$.tag === 2) ? (new CompositeCell(2, [this$.fields[0], this$.fields[1].Copy()])) : ((this$.tag === 3) ? (new CompositeCell(3, [this$.fields[0].Copy()])) : (new CompositeCell(0, [this$.fields[0].Copy()]))));
    }
    ValidateAgainstHeader(header, raiseException) {
        const this$ = this;
        const raiseExeption = defaultArg(raiseException, false);
        const cell = this$;
        if (header.IsDataColumn && (cell.isData ? true : cell.isFreeText)) {
            return true;
        }
        else if (header.IsDataColumn) {
            if (raiseExeption) {
                throw new Error(`Invalid combination of header \`${header}\` and cell \`${cell}\`, Data header should have either Data or Freetext cells`);
            }
            return false;
        }
        else if (header.IsTermColumn && (cell.isTerm ? true : cell.isUnitized)) {
            return true;
        }
        else if (!header.IsTermColumn && cell.isFreeText) {
            return true;
        }
        else {
            if (raiseExeption) {
                throw new Error(`Invalid combination of header \`${header}\` and cell \`${cell}\``);
            }
            return false;
        }
    }
    static term(oa) {
        return new CompositeCell(0, [oa]);
    }
    static freeText(s) {
        return new CompositeCell(1, [s]);
    }
    static unitized(v, oa) {
        return new CompositeCell(2, [v, oa]);
    }
    static data(d) {
        return new CompositeCell(3, [d]);
    }
}

export function CompositeCell_$reflection() {
    return union_type("ARCtrl.CompositeCell", [], CompositeCell, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", string_type]], [["Item1", string_type], ["Item2", OntologyAnnotation_$reflection()]], [["Item", Data_$reflection()]]]);
}

