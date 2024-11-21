import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { array_type, union_type, record_type, bool_type, option_type, string_type, class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { FSharpRef, Union, Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { setProperty } from "../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";

export class FileInstance extends DynamicObj {
    constructor() {
        super();
    }
}

export function FileInstance_$reflection() {
    return class_type("ARCtrl.CWL.FileInstance", undefined, FileInstance, DynamicObj_$reflection());
}

export function FileInstance_$ctor() {
    return new FileInstance();
}

export class DirectoryInstance extends DynamicObj {
    constructor() {
        super();
    }
}

export function DirectoryInstance_$reflection() {
    return class_type("ARCtrl.CWL.DirectoryInstance", undefined, DirectoryInstance, DynamicObj_$reflection());
}

export function DirectoryInstance_$ctor() {
    return new DirectoryInstance();
}

export class DirentInstance extends Record {
    constructor(Entry, Entryname, Writable) {
        super();
        this.Entry = Entry;
        this.Entryname = Entryname;
        this.Writable = Writable;
    }
}

export function DirentInstance_$reflection() {
    return record_type("ARCtrl.CWL.DirentInstance", [], DirentInstance, () => [["Entry", string_type], ["Entryname", option_type(string_type)], ["Writable", option_type(bool_type)]]);
}

export class CWLType extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["File", "Directory", "Dirent", "String", "Int", "Long", "Float", "Double", "Boolean", "Stdout", "Null", "Array"];
    }
}

export function CWLType_$reflection() {
    return union_type("ARCtrl.CWL.CWLType", [], CWLType, () => [[["Item", FileInstance_$reflection()]], [["Item", DirectoryInstance_$reflection()]], [["Item", DirentInstance_$reflection()]], [], [], [], [], [], [], [], [], [["Item", CWLType_$reflection()]]]);
}

export class InputRecordSchema extends DynamicObj {
    constructor() {
        super();
    }
}

export function InputRecordSchema_$reflection() {
    return class_type("ARCtrl.CWL.InputRecordSchema", undefined, InputRecordSchema, DynamicObj_$reflection());
}

export function InputRecordSchema_$ctor() {
    return new InputRecordSchema();
}

export class InputEnumSchema extends DynamicObj {
    constructor() {
        super();
    }
}

export function InputEnumSchema_$reflection() {
    return class_type("ARCtrl.CWL.InputEnumSchema", undefined, InputEnumSchema, DynamicObj_$reflection());
}

export function InputEnumSchema_$ctor() {
    return new InputEnumSchema();
}

export class InputArraySchema extends DynamicObj {
    constructor() {
        super();
    }
}

export function InputArraySchema_$reflection() {
    return class_type("ARCtrl.CWL.InputArraySchema", undefined, InputArraySchema, DynamicObj_$reflection());
}

export function InputArraySchema_$ctor() {
    return new InputArraySchema();
}

export class SchemaDefRequirementType extends DynamicObj {
    constructor(types, definitions) {
        super();
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@47"] = 1;
        setProperty("types", definitions, this$.contents);
    }
}

export function SchemaDefRequirementType_$reflection() {
    return class_type("ARCtrl.CWL.SchemaDefRequirementType", undefined, SchemaDefRequirementType, DynamicObj_$reflection());
}

export function SchemaDefRequirementType_$ctor_541DA560(types, definitions) {
    return new SchemaDefRequirementType(types, definitions);
}

export class SoftwarePackage extends Record {
    constructor(Package, Version, Specs) {
        super();
        this.Package = Package;
        this.Version = Version;
        this.Specs = Specs;
    }
}

export function SoftwarePackage_$reflection() {
    return record_type("ARCtrl.CWL.SoftwarePackage", [], SoftwarePackage, () => [["Package", string_type], ["Version", option_type(array_type(string_type))], ["Specs", option_type(array_type(string_type))]]);
}

