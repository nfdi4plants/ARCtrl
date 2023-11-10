import { addToDict } from "fable-library/MapUtil.js";
import { Dictionary } from "fable-library/MutableMap.js";
import { ofSeq } from "fable-library/Map.js";
import { defaultArg } from "fable-library/Option.js";
import { map, collect, delay, toList, empty, head } from "fable-library/Seq.js";
import { arrayHash, equalArrays, compareArrays } from "fable-library/Util.js";
import { class_type } from "fable-library/Reflection.js";
import { rangeDouble } from "fable-library/Range.js";

export class TestClass {
    constructor(arg) {
        const arg_1 = defaultArg(arg, empty());
        this.cells = (new Dictionary(ofSeq(arg_1, {
            Compare: compareArrays,
        }), {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
        this.cellArr = Array.from(arg_1);
    }
    get Cells() {
        const this$ = this;
        return this$.cells;
    }
    set Cells(newCells) {
        const this$ = this;
        this$.cells = newCells;
    }
    get CellArr() {
        const this$ = this;
        return this$.cellArr;
    }
    set CellArr(newCellArr) {
        const this$ = this;
        this$.cellArr = newCellArr;
    }
    AddItem(index, v) {
        const this$ = this;
        addToDict(this$.Cells, index, v);
    }
    RemoveItem(index) {
        const this$ = this;
        return this$.Cells.delete(index);
    }
    RemoveFirst() {
        const this$ = this;
        const index = head(this$.Cells.keys());
        return this$.RemoveItem(index);
    }
    get CellCount() {
        const this$ = this;
        return this$.Cells.size | 0;
    }
    get CellCountArr() {
        const this$ = this;
        return this$.CellArr.length | 0;
    }
    RemoveFirstArr() {
        const this$ = this;
        const value = this$.CellArr.splice(0, 1);
    }
}

export function TestClass_$reflection() {
    return class_type("BasicDictionary.TestClass", void 0, TestClass);
}

export function TestClass_$ctor_19766857(arg) {
    return new TestClass(arg);
}

export const testInstance = new TestClass(toList(delay(() => collect((i) => map((j) => [[i, j], `|${i},${j}|`], rangeDouble(0, 1, 10)), rangeDouble(0, 1, 10)))));