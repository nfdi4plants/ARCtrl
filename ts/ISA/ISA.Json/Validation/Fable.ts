import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { bool_type, option_type, array_type, record_type, string_type, obj_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { value, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { map } from "../../../fable_modules/fable-library-ts/Array.js";
import { singleton } from "../../../fable_modules/fable-library-ts/AsyncBuilder.js";
import { awaitPromise } from "../../../fable_modules/fable-library-ts/Async.js";
import * as JsonValidation from "../../../../src/ISA/ISA.Json/Validation/JsonValidation.js";

export class ValidationError extends Record implements IEquatable<ValidationError> {
    readonly path: any;
    readonly property: string;
    readonly message: string;
    readonly schema: any;
    readonly instance: any;
    readonly name: string;
    readonly argument: any;
    readonly stack: string;
    constructor(path: any, property: string, message: string, schema: any, instance: any, name: string, argument: any, stack: string) {
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

export function ValidationError_$reflection(): TypeInfo {
    return record_type("ISA.Json.Fable.ValidationError", [], ValidationError, () => [["path", obj_type], ["property", string_type], ["message", string_type], ["schema", obj_type], ["instance", obj_type], ["name", string_type], ["argument", obj_type], ["stack", string_type]]);
}

export function ValidationError__ToErrorString(this$: ValidationError): string {
    return `Property ${this$.property} (${this$.instance}) ${this$.message}.`;
}

export class ValidatorResult extends Record implements IEquatable<ValidatorResult> {
    readonly instance: any;
    readonly schema: any;
    readonly options: any;
    readonly path: any[];
    readonly propertyPath: string;
    readonly errors: ValidationError[];
    readonly throwError: Option<any>;
    readonly throFirst: Option<any>;
    readonly throwAll: Option<any>;
    readonly disableFormat: boolean;
    constructor(instance: any, schema: any, options: any, path: any[], propertyPath: string, errors: ValidationError[], throwError: Option<any>, throFirst: Option<any>, throwAll: Option<any>, disableFormat: boolean) {
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

export function ValidatorResult_$reflection(): TypeInfo {
    return record_type("ISA.Json.Fable.ValidatorResult", [], ValidatorResult, () => [["instance", obj_type], ["schema", obj_type], ["options", obj_type], ["path", array_type(obj_type)], ["propertyPath", string_type], ["errors", array_type(ValidationError_$reflection())], ["throwError", option_type(obj_type)], ["throFirst", option_type(obj_type)], ["throwAll", option_type(obj_type)], ["disableFormat", bool_type]]);
}

export function ValidatorResult__ToValidationResult(this$: ValidatorResult): [boolean, string[]] {
    if (this$.errors.length === 0) {
        return [true, []] as [boolean, string[]];
    }
    else {
        return [false, map<ValidationError, string>(ValidationError__ToErrorString, this$.errors)] as [boolean, string[]];
    }
}

export interface IValidate {
    helloWorld(): string,
    validateAgainstSchema(jsonString: string, schemaUrl: string): Promise<ValidatorResult>
}

export function validate(schemaURL: string, objectString: string): any {
    let validationResult: Option<ValidatorResult> = void 0;
    return singleton.Delay<[boolean, string[]]>((): any => singleton.Bind<void, [boolean, string[]]>(awaitPromise(JsonValidation.validateAgainstSchema(objectString, schemaURL).then<void>((o: ValidatorResult): void => {
        validationResult = o;
    })), (): any => {
        const output: [boolean, string[]] = ValidatorResult__ToValidationResult(value(validationResult));
        return singleton.Return<[boolean, string[]]>(output);
    }));
}

