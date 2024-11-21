import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { equals } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { Config, YAMLElement } from "../fable_modules/YAMLicious.0.0.3/YAMLiciousTypes.fs.js";
import { resizearray, string, tryInclude } from "../fable_modules/YAMLicious.0.0.3/Encode.fs.js";
import { ValidationPackage_decoder, ValidationPackage_encoder } from "./ValidationPackage.js";
import { singleton, collect, delay, toList } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { YAMLContent_create_27AED5E3 } from "../fable_modules/YAMLicious.0.0.3/YAMLiciousTypes.fs.js";
import { string as string_1, resizearray as resizearray_1, object } from "../fable_modules/YAMLicious.0.0.3/Decode.fs.js";
import { ValidationPackagesConfig } from "../ValidationPackages/ValidationPackagesConfig.js";
import { unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { read } from "../fable_modules/YAMLicious.0.0.3/Reader.fs.js";
import { defaultWhitespace } from "./Encode.js";
import { write } from "../fable_modules/YAMLicious.0.0.3/Writer.fs.js";

export function ValidationPackagesConfig_encoder(validationpackage) {
    const objSeq = choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, new YAMLElement(5, []))) {
            return undefined;
        }
        else {
            return [tupledArg[0], v];
        }
    }, ofArray([tryInclude("arc_specification", string, validationpackage.ARCSpecification), ["validation_packages", resizearray(ValidationPackage_encoder, validationpackage.ValidationPackages)]]));
    return new YAMLElement(3, [toList(delay(() => collect((matchValue) => singleton(new YAMLElement(0, [YAMLContent_create_27AED5E3(matchValue[0]), matchValue[1]])), objSeq)))]);
}

export const ValidationPackagesConfig_decoder = (value_2) => object((get$) => {
    let objectArg, objectArg_1;
    return new ValidationPackagesConfig((objectArg = get$.Required, objectArg.Field("validation_packages", (value) => resizearray_1(ValidationPackage_decoder, value))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("arc_specification", string_1))));
}, value_2);

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_fromYamlString_Static_Z721C83C5(s) {
    return ValidationPackagesConfig_decoder(read(s));
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toYamlString_Static_71136F3F(whitespace) {
    return (vp) => {
        const element = ValidationPackagesConfig_encoder(vp);
        const whitespace_1 = defaultWhitespace(whitespace) | 0;
        return write(element, (c) => (new Config(whitespace_1, c.Level)));
    };
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toYamlString_71136F3F(this$, whitespace) {
    return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toYamlString_Static_71136F3F(unwrap(whitespace))(this$);
}

