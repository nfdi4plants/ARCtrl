import { defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { CompositeHeader_$reflection, CompositeHeader_$union } from "./CompositeHeader.js";
import { CompositeCell_$reflection, CompositeCell_$union } from "./CompositeCell.js";
import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, array_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class CompositeColumn extends Record implements IEquatable<CompositeColumn> {
    readonly Header: CompositeHeader_$union;
    readonly Cells: CompositeCell_$union[];
    constructor(Header: CompositeHeader_$union, Cells: CompositeCell_$union[]) {
        super();
        this.Header = Header;
        this.Cells = Cells;
    }
    static create(header: CompositeHeader_$union, cells?: CompositeCell_$union[]): CompositeColumn {
        return new CompositeColumn(header, defaultArg(cells, []));
    }
    validate(raiseException?: boolean): boolean {
        const this$: CompositeColumn = this;
        const raiseExeption: boolean = defaultArg(raiseException, false);
        const header: CompositeHeader_$union = this$.Header;
        const cells: CompositeCell_$union[] = this$.Cells;
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
            if (raiseExeption) {
                throw new Error(`Invalid combination of header \`${header}\` and cells \`${cells[0]}\``);
            }
            return false;
        }
    }
}

export function CompositeColumn_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.CompositeColumn", [], CompositeColumn, () => [["Header", CompositeHeader_$reflection()], ["Cells", array_type(CompositeCell_$reflection())]]);
}

