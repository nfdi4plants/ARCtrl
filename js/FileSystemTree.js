import { FileSystemTree } from "./FileSystem/FileSystemTree.js";
import { append } from "./fable_modules/fable-library.4.1.4/Array.js";

export function createGitKeepFile() {
    return FileSystemTree.createFile(".gitkeep");
}

export function createReadmeFile() {
    return FileSystemTree.createFile("README.md");
}

export function createEmptyFolder(name) {
    return FileSystemTree.createFolder(name, [createGitKeepFile()]);
}

export function createAssayFolder(assayName) {
    const dataset = createEmptyFolder("dataset");
    const protocols = createEmptyFolder("protocols");
    const readme = createReadmeFile();
    const assayFile = FileSystemTree.createFile("isa.assay.xlsx");
    return FileSystemTree.createFolder(assayName, [dataset, protocols, assayFile, readme]);
}

export function createStudyFolder(studyName) {
    const resources = createEmptyFolder("resources");
    const protocols = createEmptyFolder("protocols");
    const readme = createReadmeFile();
    const studyFile = FileSystemTree.createFile("isa.study.xlsx");
    return FileSystemTree.createFolder(studyName, [resources, protocols, studyFile, readme]);
}

export function createInvestigationFile() {
    return FileSystemTree.createFile("isa.investigation.xlsx");
}

export function createAssaysFolder(assays) {
    return FileSystemTree.createFolder("assays", append([createGitKeepFile()], assays));
}

export function createStudiesFolder(studies) {
    return FileSystemTree.createFolder("studies", append([createGitKeepFile()], studies));
}

export function createWorkflowsFolder(workflows) {
    return FileSystemTree.createFolder("workflows", append([createGitKeepFile()], workflows));
}

export function createRunsFolder(runs) {
    return FileSystemTree.createFolder("runs", append([createGitKeepFile()], runs));
}

