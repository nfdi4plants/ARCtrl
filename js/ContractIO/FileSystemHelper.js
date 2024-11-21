import { writeFileBinary, writeFileText, deleteDirectory, deleteFile, moveDirectory, moveFile, readFileBinary, readFileText, getSubFiles, getSubDirectories, fileExists as fileExists_1, ensureDirectoryOfFile, ensureDirectory, createDirectory, directoryExists as directoryExists_1 } from "../../src/ARCtrl/ContractIO/FileSystem.js";
import { FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static, FsSpreadsheet_FsWorkbook__FsWorkbook_fromXlsxFile_Static_Z721C83C5 } from "../fable_modules/FsSpreadsheet.Js.6.3.0-alpha.4/FsExtensions.fs.js";
import { replace, substring } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "../fable_modules/Fable.Promise.3.2.0/Promise.fs.js";
import { toArray, append, concat, map, empty, isEmpty } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { promise } from "../fable_modules/Fable.Promise.3.2.0/PromiseImpl.fs.js";
import { sequential } from "../CrossAsync.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Array.js";

export const directoryExistsAsync = directoryExists_1;

export const createDirectoryAsync = createDirectory;

export const ensureDirectoryAsync = ensureDirectory;

export const ensureDirectoryOfFileAsync = ensureDirectoryOfFile;

export const fileExistsAsync = fileExists_1;

export const getSubDirectoriesAsync = getSubDirectories;

export const getSubFilesAsync = getSubFiles;

export const readFileTextAsync = readFileText;

export const readFileBinaryAsync = readFileBinary;

export function readFileXlsxAsync(path) {
    return FsSpreadsheet_FsWorkbook__FsWorkbook_fromXlsxFile_Static_Z721C83C5(path);
}

export const moveFileAsync = moveFile;

export const moveDirectoryAsync = moveDirectory;

export const deleteFileAsync = deleteFile;

export const deleteDirectoryAsync = deleteDirectory;

export const writeFileTextAsync = writeFileText;

export const writeFileBinaryAsync = writeFileBinary;

export function writeFileXlsxAsync(path, wb) {
    return FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static(path, wb);
}

/**
 * Return the absolute path relative to the directoryPath
 */
export function makeRelative(directoryPath, path) {
    if (((directoryPath === ".") ? true : (directoryPath === "/")) ? true : (directoryPath === "")) {
        return path;
    }
    else if (path.startsWith(directoryPath)) {
        return substring(path, directoryPath.length);
    }
    else {
        return path;
    }
}

export function standardizeSlashes(path) {
    return replace(path, "\\", "/");
}

export function getAllFilePathsAsync(directoryPath) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => {
        const allFiles = (dirs) => PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (isEmpty(dirs) ? (Promise.resolve(empty())) : (sequential()(map(getSubFilesAsync, dirs)).then((_arg) => {
            const subFiles_1 = concat(_arg);
            return sequential()(map(getSubDirectoriesAsync, dirs)).then((_arg_1) => (sequential()(map(allFiles, _arg_1)).then((_arg_2) => {
                const subDirContents_1 = concat(_arg_2);
                return Promise.resolve(append(subDirContents_1, subFiles_1));
            })));
        })))));
        return allFiles([directoryPath]).then((_arg_3) => {
            const allFilesRelative = map_1((arg_2) => standardizeSlashes(makeRelative(directoryPath, arg_2)), toArray(_arg_3));
            return Promise.resolve(allFilesRelative);
        });
    }));
}

export function renameFileOrDirectoryAsync(oldPath, newPath) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fileExistsAsync(oldPath).then((_arg) => (directoryExistsAsync(oldPath).then((_arg_1) => {
        if (_arg) {
            return moveFileAsync(oldPath, newPath);
        }
        else if (_arg_1) {
            return moveDirectoryAsync(oldPath, newPath);
        }
        else {
            return Promise.resolve();
        }
    }))))));
}

export function deleteFileOrDirectoryAsync(path) {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (fileExistsAsync(path).then((_arg) => (directoryExistsAsync(path).then((_arg_1) => {
        if (_arg) {
            return deleteFileAsync(path);
        }
        else if (_arg_1) {
            return deleteDirectoryAsync(path);
        }
        else {
            return Promise.resolve();
        }
    }))))));
}

