import { FileSystemTree_$union, FileSystemTree } from "./FileSystem/FileSystemTree.js";
import { append } from "./fable_modules/fable-library-ts/Array.js";

export function createGitKeepFile(): FileSystemTree_$union {
    return FileSystemTree.createFile(".gitkeep");
}

export function createReadmeFile(): FileSystemTree_$union {
    return FileSystemTree.createFile("README.md");
}

export function createEmptyFolder(name: string): FileSystemTree_$union {
    return FileSystemTree.createFolder(name, [createGitKeepFile()]);
}

export function createAssayFolder(assayName: string): FileSystemTree_$union {
    const dataset: FileSystemTree_$union = createEmptyFolder("dataset");
    const protocols: FileSystemTree_$union = createEmptyFolder("protocols");
    const readme: FileSystemTree_$union = createReadmeFile();
    const assayFile: FileSystemTree_$union = FileSystemTree.createFile("isa.assay.xlsx");
    return FileSystemTree.createFolder(assayName, [dataset, protocols, assayFile, readme]);
}

export function createStudyFolder(studyName: string): FileSystemTree_$union {
    const resources: FileSystemTree_$union = createEmptyFolder("resources");
    const protocols: FileSystemTree_$union = createEmptyFolder("protocols");
    const readme: FileSystemTree_$union = createReadmeFile();
    const studyFile: FileSystemTree_$union = FileSystemTree.createFile("isa.study.xlsx");
    return FileSystemTree.createFolder(studyName, [resources, protocols, studyFile, readme]);
}

export function createInvestigationFile(): FileSystemTree_$union {
    return FileSystemTree.createFile("isa.investigation.xlsx");
}

export function createAssaysFolder(assays: FileSystemTree_$union[]): FileSystemTree_$union {
    return FileSystemTree.createFolder("assays", append<FileSystemTree_$union>([createGitKeepFile()], assays));
}

export function createStudiesFolder(studies: FileSystemTree_$union[]): FileSystemTree_$union {
    return FileSystemTree.createFolder("studies", append<FileSystemTree_$union>([createGitKeepFile()], studies));
}

export function createWorkflowsFolder(workflows: FileSystemTree_$union[]): FileSystemTree_$union {
    return FileSystemTree.createFolder("workflows", append<FileSystemTree_$union>([createGitKeepFile()], workflows));
}

export function createRunsFolder(runs: FileSystemTree_$union[]): FileSystemTree_$union {
    return FileSystemTree.createFolder("runs", append<FileSystemTree_$union>([createGitKeepFile()], runs));
}

