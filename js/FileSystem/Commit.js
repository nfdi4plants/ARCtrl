import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, class_type, string_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";

export class Commit extends Record {
    constructor(Hash, UserName, UserEmail, Date$, Message) {
        super();
        this.Hash = Hash;
        this.UserName = UserName;
        this.UserEmail = UserEmail;
        this.Date = Date$;
        this.Message = Message;
    }
}

export function Commit_$reflection() {
    return record_type("FileSystem.Commit", [], Commit, () => [["Hash", string_type], ["UserName", string_type], ["UserEmail", string_type], ["Date", class_type("System.DateTime")], ["Message", string_type]]);
}

