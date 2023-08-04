import { value as value_1 } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { isIterable } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { nil, seq } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library.4.1.4/Seq.js";

/**
 * Try to encode the given object using the given encoder, or return Encode.nil if the object is null
 * 
 * If the object is a sequence, encode each element using the given encoder and return the resulting sequence
 */
export function tryInclude(name, encoder, value) {
    let v, o;
    return [name, (value != null) ? (isIterable(value_1(value)) ? ((v = value_1(value), seq(map(encoder, v)))) : ((o = value_1(value), encoder(o)))) : nil];
}

