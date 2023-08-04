import { singleton, ofArray, head, tail, isEmpty, map } from "../../fable_modules/fable-library.4.1.4/List.js";
import { CompositeCell } from "../ISA/ArcTypes/CompositeCell.js";
import { printf, toFail } from "../../fable_modules/fable-library.4.1.4/String.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.3.3.0/Cells/FsCell.fs.js";

export function fromFsCells(cells) {
    const cellValues = map((c) => c.Value, cells);
    let matchResult, v, v1, v2, v3, v1_1, v2_1, v3_1, v4;
    if (!isEmpty(cellValues)) {
        if (!isEmpty(tail(cellValues))) {
            if (!isEmpty(tail(tail(cellValues)))) {
                if (!isEmpty(tail(tail(tail(cellValues))))) {
                    if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                        matchResult = 2;
                        v1_1 = head(cellValues);
                        v2_1 = head(tail(cellValues));
                        v3_1 = head(tail(tail(cellValues)));
                        v4 = head(tail(tail(tail(cellValues))));
                    }
                    else {
                        matchResult = 3;
                    }
                }
                else {
                    matchResult = 1;
                    v1 = head(cellValues);
                    v2 = head(tail(cellValues));
                    v3 = head(tail(tail(cellValues)));
                }
            }
            else {
                matchResult = 3;
            }
        }
        else {
            matchResult = 0;
            v = head(cellValues);
        }
    }
    else {
        matchResult = 3;
    }
    switch (matchResult) {
        case 0:
            return CompositeCell.createFreeText(v);
        case 1:
            return CompositeCell.createTermFromString(v1, v2, v3);
        case 2:
            return CompositeCell.createUnitizedFromString(v1_1, v2_1, v3_1, v4);
        default:
            return toFail(printf("Dafuq"));
    }
}

export function toFsCells(isTerm, hasUnit, cell) {
    switch (cell.tag) {
        case 0:
            if (hasUnit) {
                return ofArray([new FsCell(cell.fields[0].NameText), new FsCell(""), new FsCell(cell.fields[0].TermSourceREFString), new FsCell(cell.fields[0].TermAccessionOntobeeUrl)]);
            }
            else {
                return ofArray([new FsCell(cell.fields[0].NameText), new FsCell(cell.fields[0].TermSourceREFString), new FsCell(cell.fields[0].TermAccessionOntobeeUrl)]);
            }
        case 2:
            return ofArray([new FsCell(cell.fields[0]), new FsCell(cell.fields[1].NameText), new FsCell(cell.fields[1].TermSourceREFString), new FsCell(cell.fields[1].TermAccessionOntobeeUrl)]);
        default:
            if (hasUnit) {
                return ofArray([new FsCell(cell.fields[0]), new FsCell(""), new FsCell(""), new FsCell("")]);
            }
            else if (isTerm) {
                return ofArray([new FsCell(cell.fields[0]), new FsCell(""), new FsCell("")]);
            }
            else {
                return singleton(new FsCell(cell.fields[0]));
            }
    }
}

