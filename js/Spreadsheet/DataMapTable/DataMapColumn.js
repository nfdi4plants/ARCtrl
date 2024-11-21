import { toFsCells, fromFsCells } from "./DataMapHeader.js";
import { transpose, singleton, ofArray, map } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { tryFind, singleton as singleton_1, append, delay, map as map_1, collect, toList } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { bind, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Cells/FsCell.fs.js";
import { DataContext__get_GeneratedBy, DataContext__get_Description, DataContext__get_ObjectType, DataContext__get_Unit, DataContext__get_Explication } from "../../Core/DataContext.js";

export function setFromFsColumns(dc, columns) {
    const cellParser = fromFsCells(map((c) => c.Item(1), columns));
    for (let i = 0; i <= (dc.length - 1); i++) {
        cellParser(dc[i])(map((c_1) => c_1.Item(i + 2), columns));
    }
    return dc;
}

export function toFsColumns(dc) {
    const commentKeys = toList(distinct(collect((dc_1) => map_1((c) => defaultArg(c.Name, ""), dc_1.Comments), dc), {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    }));
    const headers = toFsCells(commentKeys);
    const createTerm = (oa) => {
        if (oa == null) {
            return ofArray([new FsCell(""), new FsCell(""), new FsCell("")]);
        }
        else {
            const oa_1 = oa;
            return ofArray([new FsCell(defaultArg(oa_1.Name, "")), new FsCell(defaultArg(oa_1.TermSourceREF, "")), new FsCell(defaultArg(oa_1.TermAccessionNumber, ""))]);
        }
    };
    const createText = (s) => singleton(new FsCell(defaultArg(s, "")));
    return transpose(toList(delay(() => append(singleton_1(headers), delay(() => map_1((dc_4) => {
        const dc_3 = dc_4;
        return toList(delay(() => {
            let dc_2;
            return append((dc_2 = dc_3, ofArray([new FsCell(defaultArg(dc_2.Name, "")), new FsCell(defaultArg(dc_2.Format, "")), new FsCell(defaultArg(dc_2.SelectorFormat, ""))])), delay(() => append(createTerm(DataContext__get_Explication(dc_3)), delay(() => append(createTerm(DataContext__get_Unit(dc_3)), delay(() => append(createTerm(DataContext__get_ObjectType(dc_3)), delay(() => append(createText(DataContext__get_Description(dc_3)), delay(() => append(createText(DataContext__get_GeneratedBy(dc_3)), delay(() => map((key) => (new FsCell(defaultArg(bind((c_2) => c_2.Value, tryFind((c_1) => (defaultArg(c_1.Name, "") === key), dc_3.Comments)), ""))), commentKeys)))))))))))));
        }));
    }, dc))))));
}

