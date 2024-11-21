import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { setOptionalProperty } from "../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { LDObject_$reflection, LDObject, LDContext_$ctor } from "./LDObject.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ArcROCrateMetadata extends LDObject {
    constructor(about) {
        super("ro-crate-metadata", "CreativeWork");
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@5"] = 1;
        setOptionalProperty("about", about, this$.contents);
        const conformsTo = new DynamicObj();
        conformsTo.SetProperty("@id", "https://w3id.org/ro/crate/1.1");
        this$.contents.SetProperty("conformsTo", conformsTo);
        const context = LDContext_$ctor();
        context.SetProperty("sdo", "http://schema.org/");
        context.SetProperty("arc", "http://purl.org/nfdi4plants/ontology/");
        context.SetProperty("CreativeWork", "sdo:CreativeWork");
        context.SetProperty("about", "sdo:about");
        context.SetProperty("conformsTo", "sdo:conformsTo");
        this$.contents.SetProperty("@context", context);
    }
}

export function ArcROCrateMetadata_$reflection() {
    return class_type("ARCtrl.ROCrate.ArcROCrateMetadata", undefined, ArcROCrateMetadata, LDObject_$reflection());
}

export function ArcROCrateMetadata_$ctor_Z475E1643(about) {
    return new ArcROCrateMetadata(about);
}

