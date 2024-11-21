import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { unwrap, value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Person extends LDObject {
    constructor(id, givenName, additionalType, familyName, email, identifier, affiliation, jobTitle, additionalName, address, telephone, faxNumber, disambiguatingDescription) {
        super(id, "schema.org/Person", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("givenName", givenName, this$.contents);
        setOptionalProperty("familyName", familyName, this$.contents);
        setOptionalProperty("email", email, this$.contents);
        setOptionalProperty("identifier", identifier, this$.contents);
        setOptionalProperty("affiliation", affiliation, this$.contents);
        setOptionalProperty("jobTitle", jobTitle, this$.contents);
        setOptionalProperty("additionalName", additionalName, this$.contents);
        setOptionalProperty("address", address, this$.contents);
        setOptionalProperty("telephone", telephone, this$.contents);
        setOptionalProperty("faxNumber", faxNumber, this$.contents);
        setOptionalProperty("disambiguatingDescription", disambiguatingDescription, this$.contents);
    }
    GetGivenName() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("givenName", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("givenName");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"givenName"}' is set on this '${"Person"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"givenName"}' set on this '${"Person"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getGivenName() {
        return (p) => p.GetGivenName();
    }
}

export function Person_$reflection() {
    return class_type("ARCtrl.ROCrate.Person", undefined, Person, LDObject_$reflection());
}

export function Person_$ctor_Z45412208(id, givenName, additionalType, familyName, email, identifier, affiliation, jobTitle, additionalName, address, telephone, faxNumber, disambiguatingDescription) {
    return new Person(id, givenName, additionalType, familyName, email, identifier, affiliation, jobTitle, additionalName, address, telephone, faxNumber, disambiguatingDescription);
}

