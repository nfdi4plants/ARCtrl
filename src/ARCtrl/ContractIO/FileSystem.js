// const fs = require('fs/promises');
// const pathModule = require('path');

import fs from 'fs/promises'
import * as pathModule from 'path'

// Check if a directory exists
export async function directoryExists(path) {
    try {
        const stats = await fs.stat(path);
        return stats.isDirectory();
    } catch (err) {
        if (err.code === 'ENOENT') return false;
        throw err;
    }
}

// Create a directory
export async function createDirectory(path) {
    await fs.mkdir(path, { recursive: true });
}

// Ensure a directory exists
export async function ensureDirectory(path) {
    if (!(await directoryExists(path))) {
        await createDirectory(path);
    }
}

// Ensure the directory for a file exists
export async function ensureDirectoryOfFile(filePath) {
    const dir = pathModule.dirname(filePath);
    await ensureDirectory(dir);
}

// Check if a file exists
export async function fileExists(path) {
    try {
        const stats = await fs.stat(path);
        return stats.isFile();
    } catch (err) {
        if (err.code === 'ENOENT') return false;
        throw err;
    }
}

// Get subdirectories in a directory combined with the input path
export async function getSubDirectories(path) {
    const entries = await fs.readdir(path, { withFileTypes: true });
    return entries.filter(entry => entry.isDirectory()).map(entry => pathModule.join(path, entry.name));
}

// write a function which returns the path of all files in a directory combined with the input path
export async function getSubFiles(path) {
    const entries = await fs.readdir(path, { withFileTypes: true });
    return entries.filter(entry => entry.isFile()).map(entry => pathModule.join(path, entry.name));
    }

// Move a file
export async function moveFile(oldPath, newPath) {
    await fs.rename(oldPath, newPath);
}

// Move a directory
export async function moveDirectory(oldPath, newPath) {
    await fs.rename(oldPath, newPath);
}

// Delete a file
export async function deleteFile(path) {
    try {
        await fs.unlink(path);
    }
    catch (err) {
        if (err.code !== 'ENOENT') throw err;
    }
}

// Delete a directory (and its contents)
export async function deleteDirectory(path) {
    await fs.rm(path, { recursive: true, force: true });
}

// Read file as text
export async function readFileText(path) {
    return await fs.readFile(path, 'utf-8');
}

// Read file as binary
export async function readFileBinary(path) {
    return await fs.readFile(path);
}

// Write text to a file
export async function writeFileText(path, text) {
    await fs.writeFile(path, text, 'utf-8');
}

// Write binary data to a file
export async function writeFileBinary(path, bytes) {
    await fs.writeFile(path, bytes);
}