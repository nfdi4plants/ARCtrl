import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { record_type, class_type, string_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export class Commit extends Record implements IEquatable<Commit>, IComparable<Commit> {
    readonly Hash: string;
    readonly UserName: string;
    readonly UserEmail: string;
    readonly Date: Date;
    readonly Message: string;
    constructor(Hash: string, UserName: string, UserEmail: string, Date$: Date, Message: string) {
        super();
        this.Hash = Hash;
        this.UserName = UserName;
        this.UserEmail = UserEmail;
        this.Date = Date$;
        this.Message = Message;
    }
}

export function Commit_$reflection(): TypeInfo {
    return record_type("FileSystem.Commit", [], Commit, () => [["Hash", string_type], ["UserName", string_type], ["UserEmail", string_type], ["Date", class_type("System.DateTime")], ["Message", string_type]]);
}

