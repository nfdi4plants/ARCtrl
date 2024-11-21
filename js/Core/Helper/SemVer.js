import { toString, Record } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, option_type, string_type, int32_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { ActivePatterns_$007CRegex$007C_$007C } from "./Regex.js";
import { parse } from "../../fable_modules/fable-library-js.4.22.0/Int32.js";
import { value as value_3, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { StringBuilder__Append_Z721C83C5, StringBuilder_$ctor } from "../../fable_modules/fable-library-js.4.22.0/System.Text.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";

export const SemVerAux_Pattern = "^(?<major>\\d+)(\\.(?<minor>\\d+))?(\\.(?<patch>\\d+))?(-(?<pre>[0-9A-Za-z-\\.]+))?(\\+(?<build>[0-9A-Za-z-\\.]+))?$";

export class SemVer extends Record {
    constructor(Major, Minor, Patch, PreRelease, Metadata) {
        super();
        this.Major = (Major | 0);
        this.Minor = (Minor | 0);
        this.Patch = (Patch | 0);
        this.PreRelease = PreRelease;
        this.Metadata = Metadata;
    }
}

export function SemVer_$reflection() {
    return record_type("ARCtrl.Helper.SemVer.SemVer", [], SemVer, () => [["Major", int32_type], ["Minor", int32_type], ["Patch", int32_type], ["PreRelease", option_type(string_type)], ["Metadata", option_type(string_type)]]);
}

export function SemVer_make(major, minor, patch, pre, meta) {
    return new SemVer(major, minor, patch, pre, meta);
}

export function SemVer_create_Z55658624(major, minor, patch, pre, meta) {
    return new SemVer(major, minor, patch, pre, meta);
}

export function SemVer_tryOfString_Z721C83C5(str) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(SemVerAux_Pattern, str);
    if (activePatternResult != null) {
        const m = activePatternResult;
        const g = m;
        return SemVer_create_Z55658624(parse((g.groups && g.groups.major) || "", 511, false, 32), parse((g.groups && g.groups.minor) || "", 511, false, 32), parse((g.groups && g.groups.patch) || "", 511, false, 32), unwrap(((g.groups && g.groups.pre) != null) ? ((g.groups && g.groups.pre) || "") : undefined), unwrap(((g.groups && g.groups.build) != null) ? ((g.groups && g.groups.build) || "") : undefined));
    }
    else {
        return undefined;
    }
}

export function SemVer__AsString(this$) {
    let arg_3, arg_4;
    const sb = StringBuilder_$ctor();
    StringBuilder__Append_Z721C83C5(sb, toText(printf("%i.%i.%i"))(this$.Major)(this$.Minor)(this$.Patch));
    if (this$.PreRelease != null) {
        StringBuilder__Append_Z721C83C5(sb, (arg_3 = value_3(this$.PreRelease), toText(printf("-%s"))(arg_3)));
    }
    if (this$.Metadata != null) {
        StringBuilder__Append_Z721C83C5(sb, (arg_4 = value_3(this$.Metadata), toText(printf("+%s"))(arg_4)));
    }
    return toString(sb);
}

