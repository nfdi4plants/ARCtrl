import { toArray, collect, empty, map, singleton, append, delay, toList } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { tuple2, map as map_1, list } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { decoder as decoder_1, encoder as encoder_1 } from "./CompositeHeader.js";
import { decoder as decoder_2, encoder as encoder_2 } from "./CompositeCell.js";
import { empty as empty_2, ofSeq } from "../../fable_modules/fable-library-js.4.22.0/Map.js";
import { int32ToString, equals, arrayHash, equalArrays, compareArrays } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { Helpers_prependPath, array as array_2, string, int, tuple2 as tuple2_1, map$0027, list as list_1, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { empty as empty_1 } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { Dictionary } from "../../fable_modules/fable-library-js.4.22.0/MutableMap.js";
import { ArcTable } from "../../Core/Table/ArcTable.js";
import { decodeCell, encodeCell } from "./CellTable.js";
import { addToDict, getItemFromDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { rangeDouble } from "../../fable_modules/fable-library-js.4.22.0/Range.js";
import { fill, setItem, fold, iterateIndexed, map as map_2 } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library-js.4.22.0/Result.js";
import { ErrorReason$1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Types.fs.js";
import { decodeString, encodeString } from "../StringTable.js";

export function encoder(table) {
    const values = toList(delay(() => {
        let value_4;
        return append(singleton(["name", (value_4 = table.Name, {
            Encode(helpers_2) {
                return helpers_2.encodeString(value_4);
            },
        })]), delay(() => append((table.Headers.length !== 0) ? singleton(["header", list(toList(delay(() => map(encoder_1, table.Headers))))]) : empty(), delay(() => ((table.Values.size !== 0) ? singleton(["values", map_1((tupledArg) => tuple2((value) => ({
            Encode(helpers) {
                return helpers.encodeSignedIntegralNumber(value);
            },
        }), (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeSignedIntegralNumber(value_2);
            },
        }), tupledArg[0], tupledArg[1]), encoder_2, ofSeq(toList(delay(() => collect((matchValue) => {
            const activePatternResult = matchValue;
            return singleton([activePatternResult[0], activePatternResult[1]]);
        }, table.Values))), {
            Compare: compareArrays,
        }))]) : empty())))));
    }));
    return {
        Encode(helpers_3) {
            const arg = map((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_3)], values);
            return helpers_3.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let arg_1, objectArg, arg_3, objectArg_1, objectArg_2;
    let decodedHeader;
    const collection = defaultArg((arg_1 = list_1(decoder_1), (objectArg = get$.Optional, objectArg.Field("header", arg_1))), empty_1());
    decodedHeader = Array.from(collection);
    const decodedValues = new Dictionary(defaultArg((arg_3 = map$0027(tuple2_1(int, int), decoder_2), (objectArg_1 = get$.Optional, objectArg_1.Field("values", arg_3))), empty_2({
        Compare: compareArrays,
    })), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    });
    return ArcTable.create((objectArg_2 = get$.Required, objectArg_2.Field("name", string)), decodedHeader, decodedValues);
});

export function encoderCompressedColumn(columnIndex, rowCount, cellTable, table) {
    if (table.Headers[columnIndex].IsIOType ? true : (rowCount < 100)) {
        const values = toArray(delay(() => map((r) => encodeCell(cellTable, getItemFromDict(table.Values, [columnIndex, r])), rangeDouble(0, 1, rowCount - 1))));
        return {
            Encode(helpers) {
                const arg = map_2((v) => v.Encode(helpers), values);
                return helpers.encodeArray(arg);
            },
        };
    }
    else {
        let current = getItemFromDict(table.Values, [columnIndex, 0]);
        let from = 0;
        const values_3 = toArray(delay(() => append(collect((i) => {
            let values_1, value, value_1;
            const next = getItemFromDict(table.Values, [columnIndex, i]);
            return !equals(next, current) ? append(singleton((values_1 = [["f", (value = (from | 0), {
                Encode(helpers_1) {
                    return helpers_1.encodeSignedIntegralNumber(value);
                },
            })], ["t", (value_1 = ((i - 1) | 0), {
                Encode(helpers_2) {
                    return helpers_2.encodeSignedIntegralNumber(value_1);
                },
            })], ["v", encodeCell(cellTable, current)]], {
                Encode(helpers_3) {
                    const arg_1 = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_3)], values_1);
                    return helpers_3.encodeObject(arg_1);
                },
            })), delay(() => {
                current = next;
                from = (i | 0);
                return empty();
            })) : empty();
        }, rangeDouble(1, 1, rowCount - 1)), delay(() => {
            let values_2, value_2, value_3;
            return singleton((values_2 = [["f", (value_2 = (from | 0), {
                Encode(helpers_4) {
                    return helpers_4.encodeSignedIntegralNumber(value_2);
                },
            })], ["t", (value_3 = ((rowCount - 1) | 0), {
                Encode(helpers_5) {
                    return helpers_5.encodeSignedIntegralNumber(value_3);
                },
            })], ["v", encodeCell(cellTable, current)]], {
                Encode(helpers_6) {
                    const arg_2 = map((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_6)], values_2);
                    return helpers_6.encodeObject(arg_2);
                },
            }));
        }))));
        return {
            Encode(helpers_7) {
                const arg_3 = map_2((v_3) => v_3.Encode(helpers_7), values_3);
                return helpers_7.encodeArray(arg_3);
            },
        };
    }
}

export function decoderCompressedColumn(cellTable, table, columnIndex) {
    return {
        Decode(helper, column) {
            const matchValue = array_2(decodeCell(cellTable)).Decode(helper, column);
            if (matchValue.tag === 1) {
                const rangeDecoder = object((get$) => {
                    let from;
                    const objectArg = get$.Required;
                    from = objectArg.Field("f", int);
                    let to_;
                    const objectArg_1 = get$.Required;
                    to_ = objectArg_1.Field("t", int);
                    let value;
                    const arg_5 = decodeCell(cellTable);
                    const objectArg_2 = get$.Required;
                    value = objectArg_2.Field("v", arg_5);
                    for (let i = from; i <= to_; i++) {
                        addToDict(table.Values, [columnIndex, i], value);
                    }
                });
                const matchValue_1 = array_2(rangeDecoder).Decode(helper, column);
                return (matchValue_1.tag === 1) ? (new FSharpResult$2(1, [matchValue_1.fields[0]])) : (new FSharpResult$2(0, [undefined]));
            }
            else {
                iterateIndexed((r, cell) => {
                    addToDict(table.Values, [columnIndex, r], cell);
                }, matchValue.fields[0]);
                return new FSharpResult$2(0, [undefined]);
            }
        },
    };
}

export function arrayi(decoderi) {
    return {
        Decode(helpers, value) {
            if (helpers.isArray(value)) {
                let i = -1;
                const tokens = helpers.asArray(value);
                return fold((acc, value_1) => {
                    let tupledArg;
                    i = ((i + 1) | 0);
                    if (acc.tag === 0) {
                        const acc_1 = acc.fields[0];
                        const matchValue = decoderi(i).Decode(helpers, value_1);
                        if (matchValue.tag === 0) {
                            setItem(acc_1, i, matchValue.fields[0]);
                            return new FSharpResult$2(0, [acc_1]);
                        }
                        else {
                            return new FSharpResult$2(1, [(tupledArg = matchValue.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1]))]);
                        }
                    }
                    else {
                        return acc;
                    }
                }, new FSharpResult$2(0, [fill(new Array(tokens.length), 0, tokens.length, null)]), tokens);
            }
            else {
                return new FSharpResult$2(1, [["", new ErrorReason$1(0, ["an array", value])]]);
            }
        },
    };
}

export function encoderCompressed(stringTable, oaTable, cellTable, table) {
    const values_1 = toList(delay(() => append(singleton(["n", encodeString(stringTable, table.Name)]), delay(() => append((table.Headers.length !== 0) ? singleton(["h", list(toList(delay(() => map(encoder_1, table.Headers))))]) : empty(), delay(() => {
        if (table.Values.size !== 0) {
            const rowCount = table.RowCount | 0;
            const columns = toArray(delay(() => map((c) => encoderCompressedColumn(c, rowCount, cellTable, table), rangeDouble(0, 1, table.ColumnCount - 1))));
            return singleton(["c", {
                Encode(helpers) {
                    const arg = map_2((v) => v.Encode(helpers), columns);
                    return helpers.encodeArray(arg);
                },
            }]);
        }
        else {
            return empty();
        }
    }))))));
    return {
        Encode(helpers_1) {
            const arg_1 = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_1)], values_1);
            return helpers_1.encodeObject(arg_1);
        },
    };
}

export function decoderCompressed(stringTable, oaTable, cellTable) {
    return object((get$) => {
        let arg_1, objectArg, arg_3, objectArg_1, arg_5, objectArg_2;
        let decodedHeader;
        const collection = defaultArg((arg_1 = list_1(decoder_1), (objectArg = get$.Optional, objectArg.Field("h", arg_1))), empty_1());
        decodedHeader = Array.from(collection);
        const table = ArcTable.create((arg_3 = decodeString(stringTable), (objectArg_1 = get$.Required, objectArg_1.Field("n", arg_3))), decodedHeader, new Dictionary([], {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
        (arg_5 = arrayi((columnIndex) => decoderCompressedColumn(cellTable, table, columnIndex)), (objectArg_2 = get$.Optional, objectArg_2.Field("c", arg_5)));
        return table;
    });
}

