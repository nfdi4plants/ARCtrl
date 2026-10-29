import { FileSystem, FileSystemTree } from "../../src/ARCtrl/index.ts";
import { Commit } from "../../src/ARCtrl/ts/FileSystem/Commit.ts";
import { describe, expect, it } from "vitest";

describe("Commit", function () {
    it("primary constructor", function () {
        const date = new Date("2026-10-29T10:00:00Z");
        const commit = new Commit("abc123", "Kevin", "kevin@example.org", date, "Initial ARC");
        expect(commit).toBeInstanceOf(Commit);
        expect(commit.Hash).toBe("abc123");
        expect(commit.UserName).toBe("Kevin");
        expect(commit.UserEmail).toBe("kevin@example.org");
        expect(commit.Date).toBe(date);
        expect(commit.Message).toBe("Initial ARC");
    });
});

describe("FileSystemTree", function () {
    it("primary constructor file", function () {
        const file = new FileSystemTree(0, ["README.md"]);
        expect(file).toBeInstanceOf(FileSystemTree);
        expect(file.Name).toBe("README.md");
        expect(file.isFile).toBe(true);
        expect(file.isFolder).toBe(false);
        expect(file.ToFilePaths()).toStrictEqual(["README.md"]);
    });
    it("primary constructor empty folder", function () {
        const folder = new FileSystemTree(1, ["root", []]);
        expect(folder).toBeInstanceOf(FileSystemTree);
        expect(folder.Name).toBe("root");
        expect(folder.isFolder).toBe(true);
        expect(folder.isFile).toBe(false);
        expect(folder.ToFilePaths()).toStrictEqual([]);
    });
    it("primary constructor nested folder", function () {
        const file = new FileSystemTree(0, ["results.csv"]);
        const data = new FileSystemTree(1, ["dataset", [file]]);
        const root = new FileSystemTree(1, ["root", [data]]);
        expect(root.Name).toBe("root");
        expect(root.TryGetChildByName("dataset")).toBe(data);
        expect(data.TryGetChildByName("results.csv")).toBe(file);
        expect(root.ToFilePaths()).toStrictEqual(["dataset/results.csv"]);
    });
});

describe("FileSystem", function () {
    it("primary constructor minimal", function () {
        const tree = new FileSystemTree(1, ["root", []]);
        const fs = new FileSystem(tree, []);
        expect(fs).toBeInstanceOf(FileSystem);
        expect(fs.Tree).toBe(tree);
        expect(fs.History).toStrictEqual([]);
    });
    it("primary constructor", function () {
        const tree = new FileSystemTree(1, ["root", [new FileSystemTree(0, ["README.md"])]]);
        const commit = new Commit("abc123", "Kevin", "kevin@example.org", new Date("2026-10-29T10:00:00Z"), "Initial ARC");
        const history = [commit];
        const fs = new FileSystem(tree, history);
        expect(fs).toBeInstanceOf(FileSystem);
        expect(fs.Tree).toBe(tree);
        expect(fs.Tree.ToFilePaths()).toStrictEqual(["README.md"]);
        expect(fs.History).toBe(history);
        expect(fs.History[0].Message).toBe("Initial ARC");
    });
});
