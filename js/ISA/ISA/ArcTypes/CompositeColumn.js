import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { min } from "../../../fable_modules/fable-library.4.1.4/Double.js";
import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { CompositeHeader_$reflection } from "./CompositeHeader.js";
import { CompositeCell_$reflection } from "./CompositeCell.js";
import { record_type, array_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class CompositeColumn extends Record {
    constructor(Header, Cells) {
        super();
        this.Header = Header;
        this.Cells = Cells;
    }
    static create(header, cells) {
        return new CompositeColumn(header, defaultArg(cells, []));
    }
    validate(raiseException) {
        const this$ = this;
        const raiseExeption = defaultArg(raiseException, false);
        const header = this$.Header;
        const cells = this$.Cells;
        if (cells.length === 0) {
            return true;
        }
        else if (header.IsTermColumn && (cells[0].isTerm ? true : cells[0].isUnitized)) {
            return true;
        }
        else if (!header.IsTermColumn && cells[0].isFreeText) {
            return true;
        }
        else {
            const c = cells;
            if (raiseExeption) {
                throw new Error(`Invalid combination of header \`${header}\` and cells \`${c[min(c.length, 3)]}\``);
            }
            return false;
        }
    }
}

export function CompositeColumn_$reflection() {
    return record_type("ISA.CompositeColumn", [], CompositeColumn, () => [["Header", CompositeHeader_$reflection()], ["Cells", array_type(CompositeCell_$reflection())]]);
}

