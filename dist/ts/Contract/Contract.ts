import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { class_type, option_type, record_type, array_type, string_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";
import { Option, defaultArg } from "../fable_modules/fable-library-ts/Option.js";

export type DTOType = 
    | "ISA_Assay"
    | "ISA_Study"
    | "ISA_Investigation"
    | "JSON"
    | "Markdown"
    | "CWL"
    | "PlainText"
    | "Cli"

export class CLITool extends Record implements IEquatable<CLITool>, IComparable<CLITool> {
    readonly Name: string;
    readonly Arguments: string[];
    constructor(Name: string, Arguments: string[]) {
        super();
        this.Name = Name;
        this.Arguments = Arguments;
    }
    static create(name: string, arguments$: string[]): CLITool {
        return new CLITool(name, arguments$);
    }
}

export function CLITool_$reflection(): TypeInfo {
    return record_type("ARCtrl.Contract.CLITool", [], CLITool, () => [["Name", string_type], ["Arguments", array_type(string_type)]]);
}

export type DTO = 
    | any
    | string
    | CLITool

export type Operation = 
    | "CREATE"
    | "UPDATE"
    | "DELETE"
    | "READ"
    | "EXECUTE"

export class Contract extends Record implements IEquatable<Contract> {
    readonly Operation: Operation;
    readonly Path: string;
    readonly DTOType: Option<DTOType>;
    readonly DTO: Option<DTO>;
    constructor(Operation: Operation, Path: string, DTOType: Option<DTOType>, DTO: Option<DTO>) {
        super();
        this.Operation = Operation;
        this.Path = Path;
        this.DTOType = DTOType;
        this.DTO = DTO;
    }
    static create(op: Operation, path: string, { dtoType, dto }: {dtoType?: DTOType, dto?: DTO }): Contract {
        return new Contract(op, path, dtoType, dto);
    }
    static createCreate(path: string, dtoType: DTOType, dto?: DTO): Contract {
        return new Contract("CREATE", path, dtoType, dto);
    }
    static createUpdate(path: string, dtoType: DTOType, dto: DTO): Contract {
        return new Contract("UPDATE", path, dtoType, dto);
    }
    static createDelete(path: string): Contract {
        return new Contract("DELETE", path, void 0, void 0);
    }
    static createRead(path: string, dtoType: DTOType): Contract {
        return new Contract("READ", path, dtoType, void 0);
    }
    static createExecute(dto: CLITool, { path }: {path?: string }): Contract {
        return new Contract("EXECUTE", defaultArg(path, ""), "Cli", dto);
    }
}

export function Contract_$reflection(): TypeInfo {
    return record_type("ARCtrl.Contract.Contract", [], Contract, () => [["Operation", string_type], ["Path", string_type], ["DTOType", option_type(string_type)], ["DTO", option_type(class_type("ARCtrl.Contract.DTO"))]]);
}

