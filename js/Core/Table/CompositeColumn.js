import { unwrap, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { fold, empty, singleton, collect, delay, toArray, exists } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { CompositeCell_$reflection, CompositeCell } from "./CompositeCell.js";
import { Record } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { CompositeHeader_$reflection } from "./CompositeHeader.js";
import { record_type, array_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class CompositeColumn extends Record {
    constructor(Header, Cells) {
        super();
        this.Header = Header;
        this.Cells = Cells;
    }
    static create(header, cells) {
        return new CompositeColumn(header, defaultArg(cells, []));
    }
    Validate(raiseException) {
        const this$ = this;
        return !exists((c) => !c.ValidateAgainstHeader(this$.Header, unwrap(raiseException)), this$.Cells);
    }
    TryGetColumnUnits() {
        const this$ = this;
        const arr = toArray(delay(() => collect((cell) => (cell.isUnitized ? singleton(cell.AsUnitized[1]) : empty()), this$.Cells)));
        return (arr.length === 0) ? undefined : arr;
    }
    GetDefaultEmptyCell() {
        const this$ = this;
        if (!this$.Header.IsTermColumn) {
            return CompositeCell.emptyFreeText;
        }
        else {
            let patternInput;
            const arg = [0, 0];
            patternInput = fold((tupledArg, cell) => {
                const units = tupledArg[0] | 0;
                const terms = tupledArg[1] | 0;
                if (cell.isUnitized) {
                    return [units + 1, terms];
                }
                else {
                    return [units, terms + 1];
                }
            }, [arg[0], arg[1]], this$.Cells);
            return (patternInput[1] >= patternInput[0]) ? CompositeCell.emptyTerm : CompositeCell.emptyUnitized;
        }
    }
    get IsUnique() {
        const this$ = this;
        return this$.Header.IsUnique;
    }
}

export function CompositeColumn_$reflection() {
    return record_type("ARCtrl.CompositeColumn", [], CompositeColumn, () => [["Header", CompositeHeader_$reflection()], ["Cells", array_type(CompositeCell_$reflection())]]);
}

