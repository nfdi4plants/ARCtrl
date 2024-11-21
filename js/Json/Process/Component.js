import { genID, decoder, encoder } from "../PropertyValue.js";
import { Component, Component_decomposeName_Z721C83C5, Component__get_ComponentName, Component_createAsPV } from "../../Core/Process/Component.js";
import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryInclude } from "../Encode.js";
import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder } from "../OntologyAnnotation.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Decode_uri } from "../Decode.js";

export const ROCrate_encoder = encoder;

export const ROCrate_decoder = decoder(Component_createAsPV);

export function ISAJson_genID(c) {
    return genID(c);
}

export function ISAJson_encoder(idMap, c) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("componentName", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), Component__get_ComponentName(c)), tryInclude("componentType", (oa) => OntologyAnnotation_ISAJson_encoder(idMap, oa), c.ComponentType)]));
    return {
        Encode(helpers_1) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_1)], values);
            return helpers_1.encodeObject(arg);
        },
    };
}

export const ISAJson_decoder = object((get$) => {
    let objectArg_1;
    let name;
    const objectArg = get$.Optional;
    name = objectArg.Field("componentName", Decode_uri);
    let patternInput_1;
    if (name == null) {
        patternInput_1 = [undefined, undefined];
    }
    else {
        const patternInput = Component_decomposeName_Z721C83C5(name);
        patternInput_1 = [patternInput[0], patternInput[1]];
    }
    return new Component(patternInput_1[0], patternInput_1[1], (objectArg_1 = get$.Optional, objectArg_1.Field("componentType", OntologyAnnotation_ISAJson_decoder)));
});

