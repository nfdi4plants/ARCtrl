import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { ProtocolParameter_getNameText_2762A46F } from "../ISA/JsonTypes/ProtocolParameter.js";
import { Value_getText_Z277CD705 } from "../ISA/JsonTypes/Value.js";
import { list as list_2, toString, nil, object as object_22 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { map, choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { Protocol_decoder, Protocol_encoder, ProtocolParameter_decoder, ProtocolParameter_encoder } from "./Protocol.js";
import { Value_decoder, Value_encoder } from "./Factor.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { list as list_1, string, object as object_23 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { ProcessParameterValue } from "../ISA/JsonTypes/ProcessParameterValue.js";
import { uri, fromString } from "./Decode.js";
import { ProcessInput } from "../ISA/JsonTypes/ProcessInput.js";
import { Data_decoder, Sample_decoder, Source_decoder, Source_encoder, Data_encoder, Sample_encoder } from "./Data.js";
import { Material_decoder, Material_encoder } from "./Material.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library.4.1.4/Choice.js";
import { ProcessOutput } from "../ISA/JsonTypes/ProcessOutput.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { decoder as decoder_4, encoder } from "./Comment.js";
import { Process } from "../ISA/JsonTypes/Process.js";

export function ProcessParameterValue_genID(p) {
    const matchValue = p.Value;
    const matchValue_1 = p.Category;
    let matchResult, c, v;
    if (matchValue != null) {
        if (matchValue_1 != null) {
            matchResult = 0;
            c = matchValue_1;
            v = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return (("#Param_" + replace(ProtocolParameter_getNameText_2762A46F(c), " ", "_")) + "_") + replace(Value_getText_Z277CD705(v), " ", "_");
        default:
            return "#EmptyParameterValue";
    }
}

export function ProcessParameterValue_encoder(options, oa) {
    return object_22(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = ProcessParameterValue_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : empty(), delay(() => {
            let value_2, s_1;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_2 = "ProcessParameterValue", (typeof value_2 === "string") ? ((s_1 = value_2, s_1)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("category", (oa_1) => ProtocolParameter_encoder(options, oa_1), oa["Category"])), delay(() => append(singleton(tryInclude("value", (value_4) => Value_encoder(options, value_4), oa["Value"])), delay(() => singleton(tryInclude("unit", (oa_2) => OntologyAnnotation_encoder(options, oa_2), oa["Unit"]))))))));
        }));
    }))));
}

export function ProcessParameterValue_decoder(options) {
    return (path) => ((v) => object_23((get$) => {
        let arg_1, objectArg, objectArg_1, arg_5, objectArg_2;
        return new ProcessParameterValue((arg_1 = ProtocolParameter_decoder(options), (objectArg = get$.Optional, objectArg.Field("category", uncurry2(arg_1)))), (objectArg_1 = get$.Optional, objectArg_1.Field("value", (s, json) => Value_decoder(options, s, json))), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field("unit", uncurry2(arg_5)))));
    }, path, v));
}

export function ProcessParameterValue_fromString(s) {
    return fromString(uncurry2(ProcessParameterValue_decoder(ConverterOptions_$ctor())), s);
}

export function ProcessParameterValue_toString(p) {
    return toString(2, ProcessParameterValue_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function ProcessParameterValue_toStringLD(p) {
    let returnVal;
    return toString(2, ProcessParameterValue_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function ProcessInput_encoder(options, value) {
    if (value instanceof ProcessInput) {
        if (value.tag === 1) {
            return Sample_encoder(options, value.fields[0]);
        }
        else if (value.tag === 2) {
            return Data_encoder(options, value.fields[0]);
        }
        else if (value.tag === 3) {
            return Material_encoder(options, value.fields[0]);
        }
        else {
            return Source_encoder(options, value.fields[0]);
        }
    }
    else {
        return nil;
    }
}

export function ProcessInput_decoder(options, s, json) {
    const matchValue = Source_decoder(options, s, json);
    if (matchValue.tag === 1) {
        const matchValue_1 = Sample_decoder(options, s, json);
        if (matchValue_1.tag === 1) {
            const matchValue_2 = Data_decoder(options, s, json);
            if (matchValue_2.tag === 1) {
                const matchValue_3 = Material_decoder(options, s, json);
                if (matchValue_3.tag === 1) {
                    return new FSharpResult$2(1, [matchValue_3.fields[0]]);
                }
                else {
                    return new FSharpResult$2(0, [new ProcessInput(3, [matchValue_3.fields[0]])]);
                }
            }
            else {
                return new FSharpResult$2(0, [new ProcessInput(2, [matchValue_2.fields[0]])]);
            }
        }
        else {
            return new FSharpResult$2(0, [new ProcessInput(1, [matchValue_1.fields[0]])]);
        }
    }
    else {
        return new FSharpResult$2(0, [new ProcessInput(0, [matchValue.fields[0]])]);
    }
}

export function ProcessInput_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => ProcessInput_decoder(options, s_1, json)))), s);
}

export function ProcessInput_toString(m) {
    return toString(2, ProcessInput_encoder(ConverterOptions_$ctor(), m));
}

export function ProcessInput_toStringLD(m) {
    let returnVal;
    return toString(2, ProcessInput_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

export function ProcessOutput_encoder(options, value) {
    if (value instanceof ProcessOutput) {
        if (value.tag === 1) {
            return Data_encoder(options, value.fields[0]);
        }
        else if (value.tag === 2) {
            return Material_encoder(options, value.fields[0]);
        }
        else {
            return Sample_encoder(options, value.fields[0]);
        }
    }
    else {
        return nil;
    }
}

export function ProcessOutput_decoder(options, s, json) {
    const matchValue = Sample_decoder(options, s, json);
    if (matchValue.tag === 1) {
        const matchValue_1 = Data_decoder(options, s, json);
        if (matchValue_1.tag === 1) {
            const matchValue_2 = Material_decoder(options, s, json);
            if (matchValue_2.tag === 1) {
                return new FSharpResult$2(1, [matchValue_2.fields[0]]);
            }
            else {
                return new FSharpResult$2(0, [new ProcessOutput(2, [matchValue_2.fields[0]])]);
            }
        }
        else {
            return new FSharpResult$2(0, [new ProcessOutput(1, [matchValue_1.fields[0]])]);
        }
    }
    else {
        return new FSharpResult$2(0, [new ProcessOutput(0, [matchValue.fields[0]])]);
    }
}

export function ProcessOutput_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => ProcessOutput_decoder(options, s_1, json)))), s);
}

export function ProcessOutput_toString(m) {
    return toString(2, ProcessOutput_encoder(ConverterOptions_$ctor(), m));
}

export function Process_genID(p) {
    const matchValue = p.ID;
    if (matchValue == null) {
        const matchValue_1 = p.Name;
        if (matchValue_1 == null) {
            return "#EmptyProcess";
        }
        else {
            return "#Process_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Process_encoder(options, oa) {
    return object_22(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Process_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Process", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["Name"])), delay(() => append(singleton(tryInclude("executesProtocol", (oa_1) => Protocol_encoder(options, oa_1), oa["ExecutesProtocol"])), delay(() => append(singleton(tryInclude("parameterValues", (oa_2) => ProcessParameterValue_encoder(options, oa_2), oa["ParameterValues"])), delay(() => append(singleton(tryInclude("performer", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, oa["Performer"])), delay(() => append(singleton(tryInclude("date", (value_13) => {
                let s_5;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_5 = value_14, s_5)) : nil;
            }, oa["Date"])), delay(() => append(singleton(tryInclude("previousProcess", (oa_3) => Process_encoder(options, oa_3), oa["PreviousProcess"])), delay(() => append(singleton(tryInclude("nextProcess", (oa_4) => Process_encoder(options, oa_4), oa["NextProcess"])), delay(() => append(singleton(tryInclude("inputs", (value_16) => ProcessInput_encoder(options, value_16), oa["Inputs"])), delay(() => append(singleton(tryInclude("outputs", (value_17) => ProcessOutput_encoder(options, value_17), oa["Outputs"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), oa["Comments"]))))))))))))))))))))));
        }));
    }))));
}

export function Process_decoder(options) {
    return (path_7) => ((v) => object_23((get$) => {
        let objectArg, objectArg_1, arg_5, objectArg_2, arg_7, decoder, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, objectArg_8, objectArg_9, arg_21, decoder_3, objectArg_10;
        return new Process((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (arg_5 = Protocol_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field("executesProtocol", uncurry2(arg_5)))), (arg_7 = ((decoder = ProcessParameterValue_decoder(options), (path_1) => ((value_1) => list_1(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field("parameterValues", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field("performer", string)), (objectArg_5 = get$.Optional, objectArg_5.Field("date", string)), (arg_13 = Process_decoder(options), (objectArg_6 = get$.Optional, objectArg_6.Field("previousProcess", uncurry2(arg_13)))), (arg_15 = Process_decoder(options), (objectArg_7 = get$.Optional, objectArg_7.Field("nextProcess", uncurry2(arg_15)))), (objectArg_8 = get$.Optional, objectArg_8.Field("inputs", (path_4, value_4) => list_1((s_1, json_1) => ProcessInput_decoder(options, s_1, json_1), path_4, value_4))), (objectArg_9 = get$.Optional, objectArg_9.Field("outputs", (path_5, value_5) => list_1((s_2, json_2) => ProcessOutput_decoder(options, s_2, json_2), path_5, value_5))), (arg_21 = ((decoder_3 = decoder_4(options), (path_6) => ((value_6) => list_1(uncurry2(decoder_3), path_6, value_6)))), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", uncurry2(arg_21)))));
    }, path_7, v));
}

export function Process_fromString(s) {
    return fromString(uncurry2(Process_decoder(ConverterOptions_$ctor())), s);
}

export function Process_toString(p) {
    return toString(2, Process_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Process_toStringLD(p) {
    let returnVal;
    return toString(2, Process_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function ProcessSequence_fromString(s) {
    let decoder;
    return fromString(uncurry2((decoder = Process_decoder(ConverterOptions_$ctor()), (path) => ((value) => list_1(uncurry2(decoder), path, value)))), s);
}

export function ProcessSequence_toString(p) {
    let options;
    return toString(2, list_2(map((options = ConverterOptions_$ctor(), (oa) => Process_encoder(options, oa)), p)));
}

/**
 * exports in json-ld format
 */
export function ProcessSequence_toStringLD(p) {
    let options, returnVal;
    return toString(2, list_2(map((options = ((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal))), (oa) => Process_encoder(options, oa)), p)));
}

