import { Option, value as value_1 } from "../../fable_modules/fable-library-ts/Option.js";
import { isIterable } from "../../fable_modules/fable-library-ts/Util.js";
import { nil, seq } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library-ts/Seq.js";

/**
 * Try to encode the given object using the given encoder, or return Encode.nil if the object is null
 * 
 * If the object is a sequence, encode each element using the given encoder and return the resulting sequence
 */
export function tryInclude<$a>(name: $a, encoder: ((arg0: any) => any), value: Option<any>): [$a, any] {
    let v: Iterable<any>, o: any;
    return [name, (value != null) ? (isIterable(value_1(value)) ? ((v = (value_1(value) as Iterable<any>), seq(map<any, any>(encoder, v)))) : ((o = value_1(value), encoder(o)))) : nil] as [$a, any];
}

