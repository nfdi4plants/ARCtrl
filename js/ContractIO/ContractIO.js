import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "../fable_modules/Fable.Promise.3.2.0/Promise.fs.js";
import { promise } from "../fable_modules/Fable.Promise.3.2.0/PromiseImpl.fs.js";
import { combine } from "../FileSystem/Path.js";
import { deleteFileOrDirectoryAsync, renameFileOrDirectoryAsync, writeFileXlsxAsync, writeFileTextAsync, ensureDirectoryOfFileAsync, readFileTextAsync, readFileXlsxAsync } from "./FileSystemHelper.js";
import { Contract } from "../Contract/Contract.js";
import { FSharpResult$2 } from "../fable_modules/fable-library-js.4.22.0/Result.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { append, fold, map } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { curry2 } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { sequential } from "../CrossAsync.js";

export function fulfillReadContractAsync(basePath, c) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const matchValue = c.DTOType;
        let matchResult;
        if (matchValue != null) {
            switch (matchValue) {
                case "ISA_Assay":
                case "ISA_Investigation":
                case "ISA_Study":
                case "ISA_Datamap": {
                    matchResult = 0;
                    break;
                }
                case "PlainText": {
                    matchResult = 1;
                    break;
                }
                default:
                    matchResult = 2;
            }
        }
        else {
            matchResult = 2;
        }
        switch (matchResult) {
            case 0: {
                const path = combine(basePath, c.Path);
                return readFileXlsxAsync(path).then((_arg) => (Promise.resolve(new FSharpResult$2(0, [new Contract(c.Operation, c.Path, c.DTOType, _arg)]))));
            }
            case 1: {
                const path_1 = combine(basePath, c.Path);
                return readFileTextAsync(path_1).then((_arg_1) => (Promise.resolve(new FSharpResult$2(0, [new Contract(c.Operation, c.Path, c.DTOType, _arg_1)]))));
            }
            default:
                return Promise.resolve(new FSharpResult$2(1, [toText(printf("Contract %s is not an ISA contract"))(c.Path)]));
        }
    }).catch((_arg_2) => {
        let arg_2;
        return Promise.resolve(new FSharpResult$2(1, [(arg_2 = _arg_2.message, toText(printf("Error reading contract %s: %s"))(c.Path)(arg_2))]));
    }))));
}

export function fullfillContractBatchAsyncBy(contractF, basePath, cs) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => {
        let arg;
        return ((arg = map(curry2(contractF)(basePath), cs), sequential()(arg))).then((_arg) => {
            const res = fold((acc, cr) => {
                const copyOfStruct = acc;
                if (copyOfStruct.tag === 1) {
                    const copyOfStruct_1 = cr;
                    if (copyOfStruct_1.tag === 1) {
                        return new FSharpResult$2(1, [append(copyOfStruct.fields[0], [copyOfStruct_1.fields[0]])]);
                    }
                    else {
                        return new FSharpResult$2(1, [copyOfStruct.fields[0]]);
                    }
                }
                else {
                    const copyOfStruct_2 = cr;
                    if (copyOfStruct_2.tag === 1) {
                        return new FSharpResult$2(1, [[copyOfStruct_2.fields[0]]]);
                    }
                    else {
                        return new FSharpResult$2(0, [append(copyOfStruct.fields[0], [copyOfStruct_2.fields[0]])]);
                    }
                }
            }, new FSharpResult$2(0, [[]]), _arg);
            return Promise.resolve(res);
        });
    }));
}

export function fulfillWriteContractAsync(basePath, c) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const matchValue = c.DTO;
        if (matchValue == null) {
            const path_2 = combine(basePath, c.Path);
            return ensureDirectoryOfFileAsync(path_2).then(() => (writeFileTextAsync(path_2, "").then(() => (Promise.resolve(new FSharpResult$2(0, [c]))))));
        }
        else {
            const wb = matchValue;
            const path = combine(basePath, c.Path);
            return ensureDirectoryOfFileAsync(path).then(() => (writeFileXlsxAsync(path, wb).then(() => (Promise.resolve(new FSharpResult$2(0, [c]))))));
        }
    }).catch((_arg_6) => {
        let arg_2;
        return Promise.resolve(new FSharpResult$2(1, [(arg_2 = _arg_6.message, toText(printf("Error writing contract %s: %s"))(c.Path)(arg_2))]));
    }))));
}

export function fulfillUpdateContractAsync(basePath, c) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const matchValue = c.DTO;
        if (matchValue == null) {
            const path_2 = combine(basePath, c.Path);
            return ensureDirectoryOfFileAsync(path_2).then(() => (writeFileTextAsync(path_2, "").then(() => (Promise.resolve(new FSharpResult$2(0, [c]))))));
        }
        else {
            const wb = matchValue;
            const path = combine(basePath, c.Path);
            return ensureDirectoryOfFileAsync(path).then(() => (writeFileXlsxAsync(path, wb).then(() => (Promise.resolve(new FSharpResult$2(0, [c]))))));
        }
    }).catch((_arg_6) => {
        let arg_2;
        return Promise.resolve(new FSharpResult$2(1, [(arg_2 = _arg_6.message, toText(printf("Error updating contract %s: %s"))(c.Path)(arg_2))]));
    }))));
}

export function fullfillRenameContractAsync(basePath, c) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const matchValue = c.DTO;
        let matchResult, t_1, t_2;
        if (matchValue != null) {
            if (typeof matchValue === "string") {
                if (matchValue === c.Path) {
                    matchResult = 0;
                    t_1 = matchValue;
                }
                else {
                    matchResult = 1;
                    t_2 = matchValue;
                }
            }
            else {
                matchResult = 2;
            }
        }
        else {
            matchResult = 2;
        }
        switch (matchResult) {
            case 0:
                return Promise.resolve(new FSharpResult$2(1, [toText(printf("Rename Contract %s old and new Path are the same"))(c.Path)]));
            case 1: {
                const newPath = combine(basePath, t_2);
                const oldPath = combine(basePath, c.Path);
                return renameFileOrDirectoryAsync(oldPath, newPath).then(() => (Promise.resolve(new FSharpResult$2(0, [c]))));
            }
            default:
                return Promise.resolve(new FSharpResult$2(1, [toText(printf("Rename Contract %s does not contain new Path"))(c.Path)]));
        }
    }).catch((_arg_1) => {
        let arg_3;
        return Promise.resolve(new FSharpResult$2(1, [(arg_3 = _arg_1.message, toText(printf("Error renaming contract %s: %s"))(c.Path)(arg_3))]));
    }))));
}

export function fullfillDeleteContractAsync(basePath, c) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const path = combine(basePath, c.Path);
        return deleteFileOrDirectoryAsync(path).then(() => (Promise.resolve(new FSharpResult$2(0, [c]))));
    }).catch((_arg_1) => {
        let arg_1;
        return Promise.resolve(new FSharpResult$2(1, [(arg_1 = _arg_1.message, toText(printf("Error deleting contract %s: %s"))(c.Path)(arg_1))]));
    }))));
}

export function fullFillContract(basePath, c) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const matchValue = c.Operation;
        return (matchValue === "READ") ? (fulfillReadContractAsync(basePath, c)) : ((matchValue === "CREATE") ? (fulfillWriteContractAsync(basePath, c)) : ((matchValue === "UPDATE") ? (fulfillUpdateContractAsync(basePath, c)) : ((matchValue === "DELETE") ? (fullfillDeleteContractAsync(basePath, c)) : ((matchValue === "RENAME") ? (fullfillRenameContractAsync(basePath, c)) : (Promise.resolve(new FSharpResult$2(1, [toText(printf("Operation %A not supported"))(c.Operation)])))))));
    }));
}

export function fullFillContractBatchAsync(basePath, cs) {
    return fullfillContractBatchAsyncBy(fullFillContract, basePath, cs);
}

