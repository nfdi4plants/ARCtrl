import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { setOptionalProperty } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class LabProtocol extends LDObject {
    constructor(id, additionalType, name, intendedUse, description, url, comment, version, labEquipment, reagent, computationalTool) {
        super(id, "bioschemas.org/LabProtocol", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setOptionalProperty("name", name, this$.contents);
        setOptionalProperty("intendedUse", intendedUse, this$.contents);
        setOptionalProperty("description", description, this$.contents);
        setOptionalProperty("url", url, this$.contents);
        setOptionalProperty("comment", comment, this$.contents);
        setOptionalProperty("version", version, this$.contents);
        setOptionalProperty("labEquipment", labEquipment, this$.contents);
        setOptionalProperty("reagent", reagent, this$.contents);
        setOptionalProperty("computationalTool", computationalTool, this$.contents);
    }
}

export function LabProtocol_$reflection() {
    return class_type("ARCtrl.ROCrate.LabProtocol", undefined, LabProtocol, LDObject_$reflection());
}

export function LabProtocol_$ctor_3514295B(id, additionalType, name, intendedUse, description, url, comment, version, labEquipment, reagent, computationalTool) {
    return new LabProtocol(id, additionalType, name, intendedUse, description, url, comment, version, labEquipment, reagent, computationalTool);
}

