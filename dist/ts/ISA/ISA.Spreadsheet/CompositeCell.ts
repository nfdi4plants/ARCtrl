import { singleton, ofArray, head, tail, isEmpty, FSharpList, map } from "../../fable_modules/fable-library-ts/List.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.3.3.0/Cells/FsCell.fs.js";
import { CompositeCell_$union, CompositeCell } from "../ISA/ArcTypes/CompositeCell.js";
import { printf, toFail } from "../../fable_modules/fable-library-ts/String.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";

export function fromFsCells(cells: FSharpList<FsCell>): CompositeCell_$union {
    const cellValues: FSharpList<string> = map<FsCell, string>((c: FsCell): string => c.Value, cells);
    let matchResult: int32, v: string, v1: string, v2: string, v3: string, v1_1: string, v2_1: string, v3_1: string, v4: string;
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
            return CompositeCell.createFreeText(v!);
        case 1:
            return CompositeCell.createTermFromString(v1!, v2!, v3!);
        case 2:
            return CompositeCell.createUnitizedFromString(v1_1!, v2_1!, v3_1!, v4!);
        default:
            return toFail(printf("Dafuq"));
    }
}

export function toFsCells(isTerm: boolean, hasUnit: boolean, cell: CompositeCell_$union): FSharpList<FsCell> {
    let v: OntologyAnnotation, v_1: string, v_2: string;
    switch (cell.tag) {
        case /* Term */ 0:
            if ((v = cell.fields[0], hasUnit)) {
                const v_6: OntologyAnnotation = cell.fields[0];
                return ofArray([new FsCell(v_6.NameText), new FsCell(""), new FsCell(v_6.TermSourceREFString), new FsCell(v_6.TermAccessionOntobeeUrl)]);
            }
            else {
                const v_7: OntologyAnnotation = cell.fields[0];
                return ofArray([new FsCell(v_7.NameText), new FsCell(v_7.TermSourceREFString), new FsCell(v_7.TermAccessionOntobeeUrl)]);
            }
        case /* Unitized */ 2: {
            const unit: OntologyAnnotation = cell.fields[1];
            const v_8: string = cell.fields[0];
            return ofArray([new FsCell(v_8), new FsCell(unit.NameText), new FsCell(unit.TermSourceREFString), new FsCell(unit.TermAccessionOntobeeUrl)]);
        }
        default:
            if ((v_1 = cell.fields[0], hasUnit)) {
                const v_3: string = cell.fields[0];
                return ofArray([new FsCell(v_3), new FsCell(""), new FsCell(""), new FsCell("")]);
            }
            else if ((v_2 = cell.fields[0], isTerm)) {
                const v_4: string = cell.fields[0];
                return ofArray([new FsCell(v_4), new FsCell(""), new FsCell("")]);
            }
            else {
                const v_5: string = cell.fields[0];
                return singleton(new FsCell(v_5));
            }
    }
}

