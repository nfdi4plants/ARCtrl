import { Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type, option_type, record_type, array_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";

export class CLITool extends Record {
    constructor(Name, Arguments) {
        super();
        this.Name = Name;
        this.Arguments = Arguments;
    }
    static create(name, arguments$) {
        return new CLITool(name, arguments$);
    }
}

export function CLITool_$reflection() {
    return record_type("ARCtrl.Contract.CLITool", [], CLITool, () => [["Name", string_type], ["Arguments", array_type(string_type)]]);
}

export function DTO__get_isSpreadsheet(this$) {
    return true;
}

export function DTO__get_isText(this$) {
    if (typeof this$ === "string") {
        return true;
    }
    else {
        return false;
    }
}

export function DTO__get_isCLITool(this$) {
    if (this$ instanceof CLITool) {
        return true;
    }
    else {
        return false;
    }
}

export function DTO__AsSpreadsheet(this$) {
    return this$;
}

export function DTO__AsText(this$) {
    if (typeof this$ === "string") {
        return this$;
    }
    else {
        throw new Error("Not text");
    }
}

export function DTO__AsCLITool(this$) {
    if (this$ instanceof CLITool) {
        return this$;
    }
    else {
        throw new Error("Not a CLI tool");
    }
}

export class Contract extends Record {
    constructor(Operation, Path, DTOType, DTO) {
        super();
        this.Operation = Operation;
        this.Path = Path;
        this.DTOType = DTOType;
        this.DTO = DTO;
    }
    static create(op, path, { dtoType, dto }) {
        return new Contract(op, path, dtoType, dto);
    }
    static createCreate(path, dtoType, dto) {
        return new Contract("CREATE", path, dtoType, dto);
    }
    static createUpdate(path, dtoType, dto) {
        return new Contract("UPDATE", path, dtoType, dto);
    }
    static createDelete(path) {
        return new Contract("DELETE", path, undefined, undefined);
    }
    static createRead(path, dtoType) {
        return new Contract("READ", path, dtoType, undefined);
    }
    static createExecute(dto, path) {
        return new Contract("EXECUTE", defaultArg(path, ""), "Cli", dto);
    }
    static createRename(oldPath, newPath) {
        return new Contract("RENAME", oldPath, undefined, newPath);
    }
}

export function Contract_$reflection() {
    return record_type("ARCtrl.Contract.Contract", [], Contract, () => [["Operation", string_type], ["Path", string_type], ["DTOType", option_type(string_type)], ["DTO", option_type(class_type("ARCtrl.Contract.DTO"))]]);
}

