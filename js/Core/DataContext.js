import { hash, boxHashSeq, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { Value as Value_1 } from "./Value.js";
import { Data_$reflection, Data } from "./Data.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";

export class DataContext extends Data {
    constructor(id, name, dataType, format, selectorFormat, explication, unit, objectType, label, description, generatedBy, comments) {
        super(unwrap(id), unwrap(name), unwrap(dataType), unwrap(format), unwrap(selectorFormat), unwrap(comments));
        this._explication = explication;
        this._unit = unit;
        this._objectType = objectType;
        this._label = label;
        this._description = description;
        this._generatedBy = generatedBy;
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.ID), boxHashOption(this$.Name), boxHashOption(this$.DataType), boxHashOption(this$.Format), boxHashOption(this$.SelectorFormat), boxHashSeq(this$.Comments), boxHashOption(DataContext__get_Explication(this$)), boxHashOption(DataContext__get_Unit(this$)), boxHashOption(DataContext__get_ObjectType(this$)), boxHashOption(DataContext__get_Label(this$)), boxHashOption(DataContext__get_Description(this$)), boxHashOption(DataContext__get_GeneratedBy(this$))]) | 0;
    }
    Equals(obj) {
        const this$ = this;
        return hash(this$) === hash(obj);
    }
    AlternateName() {
        const this$ = this;
        return DataContext__get_Label(this$);
    }
    MeasurementMethod() {
        const this$ = this;
        return DataContext__get_GeneratedBy(this$);
    }
    GetCategory() {
        const this$ = this;
        return DataContext__get_Explication(this$);
    }
    GetValue() {
        const this$ = this;
        return map((Item) => (new Value_1(0, [Item])), DataContext__get_ObjectType(this$));
    }
    GetUnit() {
        const this$ = this;
        return DataContext__get_Unit(this$);
    }
    GetAdditionalType() {
        return "DataContext";
    }
    Description() {
        const this$ = this;
        return DataContext__get_Description(this$);
    }
}

export function DataContext_$reflection() {
    return class_type("ARCtrl.DataContext", undefined, DataContext, Data_$reflection());
}

export function DataContext_$ctor_Z780A8A2A(id, name, dataType, format, selectorFormat, explication, unit, objectType, label, description, generatedBy, comments) {
    return new DataContext(id, name, dataType, format, selectorFormat, explication, unit, objectType, label, description, generatedBy, comments);
}

export function DataContext__get_Explication(this$) {
    return this$._explication;
}

export function DataContext__set_Explication_279AAFF2(this$, explication) {
    this$._explication = explication;
}

export function DataContext__get_Unit(this$) {
    return this$._unit;
}

export function DataContext__set_Unit_279AAFF2(this$, unit) {
    this$._unit = unit;
}

export function DataContext__get_ObjectType(this$) {
    return this$._objectType;
}

export function DataContext__set_ObjectType_279AAFF2(this$, objectType) {
    this$._objectType = objectType;
}

export function DataContext__get_Label(this$) {
    return this$._label;
}

export function DataContext__set_Label_6DFDD678(this$, label) {
    this$._label = label;
}

export function DataContext__get_Description(this$) {
    return this$._description;
}

export function DataContext__set_Description_6DFDD678(this$, description) {
    this$._description = description;
}

export function DataContext__get_GeneratedBy(this$) {
    return this$._generatedBy;
}

export function DataContext__set_GeneratedBy_6DFDD678(this$, generatedBy) {
    this$._generatedBy = generatedBy;
}

export function DataContext__AsData(this$) {
    return new Data(unwrap(this$.ID), unwrap(this$.Name), unwrap(this$.DataType), unwrap(this$.Format), unwrap(this$.SelectorFormat), this$.Comments);
}

export function DataContext_fromData_Z7B4D7BF5(data, explication, unit, objectType, label, description, generatedBy) {
    return DataContext_$ctor_Z780A8A2A(unwrap(data.ID), unwrap(data.Name), unwrap(data.DataType), unwrap(data.Format), unwrap(data.SelectorFormat), unwrap(explication), unwrap(unit), unwrap(objectType), unwrap(label), unwrap(description), unwrap(generatedBy), data.Comments);
}

export function DataContext_createAsPV(alternateName, measurementMethod, description, category, value, unit) {
    let oa, v;
    return DataContext_$ctor_Z780A8A2A(undefined, undefined, undefined, undefined, undefined, unwrap(category), unwrap(unit), unwrap((value == null) ? undefined : ((value.tag === 0) ? ((oa = value.fields[0], oa)) : ((v = value, new OntologyAnnotation(v.Text))))), unwrap(alternateName), unwrap(description), unwrap(measurementMethod));
}

export function DataContext__Copy(this$) {
    const copy = DataContext_$ctor_Z780A8A2A();
    copy.ID = this$.ID;
    copy.Name = this$.Name;
    copy.DataType = this$.DataType;
    copy.Format = this$.Format;
    copy.SelectorFormat = this$.SelectorFormat;
    DataContext__set_Explication_279AAFF2(copy, DataContext__get_Explication(this$));
    DataContext__set_Unit_279AAFF2(copy, DataContext__get_Unit(this$));
    DataContext__set_ObjectType_279AAFF2(copy, DataContext__get_ObjectType(this$));
    DataContext__set_Description_6DFDD678(copy, DataContext__get_Description(this$));
    DataContext__set_GeneratedBy_6DFDD678(copy, DataContext__get_GeneratedBy(this$));
    copy.Comments = this$.Comments;
    return copy;
}

