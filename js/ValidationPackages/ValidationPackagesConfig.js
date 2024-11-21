import { value, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { join } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { item, fold, length, map, empty, singleton, append, delay, toList } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { sortBy } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { equals, compareArrays } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { rangeDouble } from "../fable_modules/fable-library-js.4.22.0/Range.js";
import { boxHashSeq, boxHashOption, boxHashArray } from "../Core/Helper/HashCodes.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ValidationPackagesConfig {
    constructor(validation_packages, arc_specification) {
        this._arc_specification = arc_specification;
        this._validation_packages = validation_packages;
    }
    get ValidationPackages() {
        const this$ = this;
        return this$._validation_packages;
    }
    set ValidationPackages(validation_packages) {
        const this$ = this;
        this$._validation_packages = validation_packages;
    }
    get ARCSpecification() {
        const this$ = this;
        return unwrap(this$._arc_specification);
    }
    set ARCSpecification(arc_specification) {
        const this$ = this;
        this$._arc_specification = arc_specification;
    }
    static make(validation_packages, arc_specification) {
        return new ValidationPackagesConfig(validation_packages, unwrap(arc_specification));
    }
    Copy() {
        const this$ = this;
        const validation_packages = this$.ValidationPackages;
        const arc_specification = this$.ARCSpecification;
        return ValidationPackagesConfig.make(validation_packages, arc_specification);
    }
    toString() {
        const this$ = this;
        return join("\n", toList(delay(() => append(singleton("{"), delay(() => append((this$.ARCSpecification != null) ? singleton(` ARCSpecification = ${value(this$.ARCSpecification)}`) : empty(), delay(() => append(singleton(" ValidationPackages = ["), delay(() => append(singleton(join(`;${"\n"}`, map(toString, this$.ValidationPackages))), delay(() => append(singleton("]"), delay(() => singleton("}"))))))))))))));
    }
    StructurallyEquals(other) {
        const this$ = this;
        const sort = (arg) => sortBy((vp) => [vp.Name, vp.Version], Array.from(arg), {
            Compare: compareArrays,
        });
        if (equals(this$.ARCSpecification, other.ARCSpecification)) {
            const a = sort(this$.ValidationPackages);
            const b = sort(other.ValidationPackages);
            return (length(a) === length(b)) && fold((acc, e) => {
                if (acc) {
                    return e;
                }
                else {
                    return false;
                }
            }, true, toList(delay(() => map((i) => equals(item(i, a), item(i, b)), rangeDouble(0, 1, length(a) - 1)))));
        }
        else {
            return false;
        }
    }
    ReferenceEquals(other) {
        const this$ = this;
        return this$ === other;
    }
    Equals(other) {
        let other_vp;
        const this$ = this;
        return (other instanceof ValidationPackagesConfig) && ((other_vp = other, this$.StructurallyEquals(other_vp)));
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.ARCSpecification), boxHashSeq(this$.ValidationPackages)]) | 0;
    }
}

export function ValidationPackagesConfig_$reflection() {
    return class_type("ARCtrl.ValidationPackages.ValidationPackagesConfig", undefined, ValidationPackagesConfig);
}

export function ValidationPackagesConfig_$ctor_376974AD(validation_packages, arc_specification) {
    return new ValidationPackagesConfig(validation_packages, arc_specification);
}

