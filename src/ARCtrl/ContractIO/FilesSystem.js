const fs = require('fs/promises');
const pathModule = require('path');
const fsSync = require('fs'); // For synchronous checks like `existsSync`

// Check if a directory exists
async function directoryExists(path) {
    try {
        const stats = await fs.stat(path);
        return stats.isDirectory();
    } catch (err) {
        if (err.code === 'ENOENT') return false;
        throw err;
    }
}

// Create a directory
async function createDirectory(path) {
    await fs.mkdir(path, { recursive: true });
}

// Ensure a directory exists
async function ensureDirectory(path) {
    if (!(await directoryExists(path))) {
        await createDirectory(path);
    }
}

// Ensure the directory for a file exists
async function ensureDirectoryOfFile(filePath) {
    const dir = pathModule.dirname(filePath);
    await ensureDirectory(dir);
}

// Check if a file exists
async function fileExists(path) {
    try {
        const stats = await fs.stat(path);
        return stats.isFile();
    } catch (err) {
        if (err.code === 'ENOENT') return false;
        throw err;
    }
}

// Get subdirectories in a directory
async function getSubDirectories(path) {
    const entries = await fs.readdir(path, { withFileTypes: true });
    return entries.filter(entry => entry.isDirectory()).map(entry => entry.name);
}

// Get files in a directory
async function getSubFiles(path) {
    const entries = await fs.readdir(path, { withFileTypes: true });
    return entries.filter(entry => entry.isFile()).map(entry => entry.name);
}

// Move a file
async function moveFile(oldPath, newPath) {
    await fs.rename(oldPath, newPath);
}

// Move a directory
async function moveDirectory(oldPath, newPath) {
    await fs.rename(oldPath, newPath);
}

// Delete a file
async function deleteFile(path) {
    await fs.unlink(path);
}

// Delete a directory (and its contents)
async function deleteDirectory(path) {
    await fs.rm(path, { recursive: true, force: true });
}

// Read file as text
async function readFileText(path) {
    return await fs.readFile(path, 'utf-8');
}

// Read file as binary
async function readFileBinary(path) {
    return await fs.readFile(path);
}

// Write text to a file
async function writeFileText(path, text) {
    await fs.writeFile(path, text, 'utf-8');
}

// Write binary data to a file
async function writeFileBinary(path, bytes) {
    await fs.writeFile(path, bytes);
}