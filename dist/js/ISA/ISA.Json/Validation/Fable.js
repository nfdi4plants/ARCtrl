import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { bool_type, option_type, array_type, record_type, string_type, obj_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { map } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { singleton } from "../../../fable_modules/fable-library.4.1.4/AsyncBuilder.js";
import { awaitPromise } from "../../../fable_modules/fable-library.4.1.4/Async.js";
import * as JsonValidation from "../../../../../src/ISA/ISA.Json/Validation/JsonValidation.js";
import { value } from "../../../fable_modules/fable-library.4.1.4/Option.js";

export class ValidationError extends Record {
    constructor(path, property, message, schema, instance, name, argument, stack) {
        super();
        this.path = path;
        this.property = property;
        this.message = message;
        this.schema = schema;
        this.instance = instance;
        this.name = name;
        this.argument = argument;
        this.stack = stack;
    }
}

export function ValidationError_$reflection() {
    return record_type("ARCtrl.ISA.Json.Fable.ValidationError", [], ValidationError, () => [["path", obj_type], ["property", string_type], ["message", string_type], ["schema", obj_type], ["instance", obj_type], ["name", string_type], ["argument", obj_type], ["stack", string_type]]);
}

export function ValidationError__ToErrorString(this$) {
    return `Property ${this$.property} (${this$.instance}) ${this$.message}.`;
}

export class ValidatorResult extends Record {
    constructor(instance, schema, options, path, propertyPath, errors, throwError, throFirst, throwAll, disableFormat) {
        super();
        this.instance = instance;
        this.schema = schema;
        this.options = options;
        this.path = path;
        this.propertyPath = propertyPath;
        this.errors = errors;
        this.throwError = throwError;
        this.throFirst = throFirst;
        this.throwAll = throwAll;
        this.disableFormat = disableFormat;
    }
}

export function ValidatorResult_$reflection() {
    return record_type("ARCtrl.ISA.Json.Fable.ValidatorResult", [], ValidatorResult, () => [["instance", obj_type], ["schema", obj_type], ["options", obj_type], ["path", array_type(obj_type)], ["propertyPath", string_type], ["errors", array_type(ValidationError_$reflection())], ["throwError", option_type(obj_type)], ["throFirst", option_type(obj_type)], ["throwAll", option_type(obj_type)], ["disableFormat", bool_type]]);
}

export function ValidatorResult__ToValidationResult(this$) {
    if (this$.errors.length === 0) {
        return [true, []];
    }
    else {
        return [false, map(ValidationError__ToErrorString, this$.errors)];
    }
}

export function validate(schemaURL, objectString) {
    let validationResult = void 0;
    return singleton.Delay(() => singleton.Bind(awaitPromise(JsonValidation.validateAgainstSchema(objectString, schemaURL).then((o) => {
        validationResult = o;
    })), () => {
        const output = ValidatorResult__ToValidationResult(value(validationResult));
        return singleton.Return(output);
    }));
}

