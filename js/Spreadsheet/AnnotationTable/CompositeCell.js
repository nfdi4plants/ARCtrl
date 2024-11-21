import { defaultArg, bind, unwrap, map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { item } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { CompositeCell } from "../../Core/Table/CompositeCell.js";
import { Option_fromValueWithDefault } from "../../Core/Helper/Collections.js";

export function termFromStringCells(tsrCol, tanCol, cellValues) {
    const tan = map((i) => item(i, cellValues), tanCol);
    const tsr = map((i_1) => item(i_1, cellValues), tsrCol);
    return CompositeCell.createTermFromString(item(0, cellValues), unwrap(tsr), unwrap(tan));
}

export function unitizedFromStringCells(unitCol, tsrCol, tanCol, cellValues) {
    const unit = item(unitCol, cellValues);
    const tan = map((i) => item(i, cellValues), tanCol);
    const tsr = map((i_1) => item(i_1, cellValues), tsrCol);
    return CompositeCell.createUnitizedFromString(item(0, cellValues), unit, unwrap(tsr), unwrap(tan));
}

export function freeTextFromStringCells(cellValues) {
    return CompositeCell.createFreeText(item(0, cellValues));
}

export function dataFromStringCells(format, selectorFormat, cellValues) {
    const format_1 = bind((i) => Option_fromValueWithDefault("", item(i, cellValues)), format);
    const selectorFormat_1 = bind((i_1) => Option_fromValueWithDefault("", item(i_1, cellValues)), selectorFormat);
    return CompositeCell.createDataFromString(item(0, cellValues), unwrap(format_1), unwrap(selectorFormat_1));
}

export function toStringCells(isTerm, hasUnit, cell) {
    switch (cell.tag) {
        case 0:
            if (hasUnit) {
                return [cell.fields[0].NameText, "", defaultArg(cell.fields[0].TermSourceREF, ""), cell.fields[0].TermAccessionOntobeeUrl];
            }
            else {
                return [cell.fields[0].NameText, defaultArg(cell.fields[0].TermSourceREF, ""), cell.fields[0].TermAccessionOntobeeUrl];
            }
        case 2:
            return [cell.fields[0], cell.fields[1].NameText, defaultArg(cell.fields[1].TermSourceREF, ""), cell.fields[1].TermAccessionOntobeeUrl];
        case 3: {
            const format = defaultArg(cell.fields[0].Format, "");
            const selectorFormat = defaultArg(cell.fields[0].SelectorFormat, "");
            return [defaultArg(cell.fields[0].Name, ""), format, selectorFormat];
        }
        default:
            if (hasUnit) {
                return [cell.fields[0], "", "", ""];
            }
            else if (isTerm) {
                return [cell.fields[0], "", ""];
            }
            else {
                return [cell.fields[0]];
            }
    }
}

