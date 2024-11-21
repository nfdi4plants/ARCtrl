import { value as value_1, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { join } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { empty, singleton, append, delay, toList } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { stringHash, equals } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { boxHashOption, boxHashArray } from "../Core/Helper/HashCodes.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ValidationPackage {
    constructor(name, version) {
        this.version = version;
        this._name = name;
        this._version = this.version;
    }
    get Name() {
        const this$ = this;
        return this$._name;
    }
    set Name(name) {
        const this$ = this;
        this$._name = name;
    }
    get Version() {
        const this$ = this;
        return unwrap(this$._version);
    }
    set Version(version) {
        const this$ = this;
        this$._version = version;
    }
    static make(name, version) {
        return new ValidationPackage(name, unwrap(version));
    }
    Copy() {
        const this$ = this;
        const name = this$.Name;
        const version = this$.Version;
        return ValidationPackage.make(name, version);
    }
    toString() {
        const this$ = this;
        return join("\n", toList(delay(() => append(singleton("{"), delay(() => append(singleton(` Name = ${this$.Name}`), delay(() => append((this$.version != null) ? singleton(` Version = ${value_1(this$.Version)}`) : empty(), delay(() => singleton("}"))))))))));
    }
    Equals(obj) {
        let other_vp;
        const this$ = this;
        return (obj instanceof ValidationPackage) && ((other_vp = obj, (other_vp.Name === this$.Name) && equals(other_vp.Version, this$.Version)));
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([stringHash(this$.Name), boxHashOption(this$.Version)]) | 0;
    }
}

export function ValidationPackage_$reflection() {
    return class_type("ARCtrl.ValidationPackages.ValidationPackage", undefined, ValidationPackage);
}

export function ValidationPackage_$ctor_27AED5E3(name, version) {
    return new ValidationPackage(name, version);
}

