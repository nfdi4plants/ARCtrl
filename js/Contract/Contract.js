import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { class_type, option_type, record_type, array_type, string_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";
import { defaultArg } from "../fable_modules/fable-library.4.1.4/Option.js";

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
    return record_type("Contract.CLITool", [], CLITool, () => [["Name", string_type], ["Arguments", array_type(string_type)]]);
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
        return new Contract("DELETE", path, void 0, void 0);
    }
    static createRead(path, dtoType) {
        return new Contract("READ", path, dtoType, void 0);
    }
    static createExecute(dto, { path }) {
        return new Contract("EXECUTE", defaultArg(path, ""), "Cli", dto);
    }
}

export function Contract_$reflection() {
    return record_type("Contract.Contract", [], Contract, () => [["Operation", string_type], ["Path", string_type], ["DTOType", option_type(string_type)], ["DTO", option_type(class_type("Contract.DTO"))]]);
}

