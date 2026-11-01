from datetime import datetime, timezone

from src.ARCtrl import Commit, FileSystem, FileSystemTree


def test_commit_primary_constructor():
    date = datetime(2026, 10, 29, 10, tzinfo=timezone.utc)
    commit = Commit("abc123", "Kevin", "kevin@example.org", date, "Initial ARC")
    assert isinstance(commit, Commit)
    assert commit.hash == "abc123"
    assert commit.user_name == "Kevin"
    assert commit.user_email == "kevin@example.org"
    assert commit.date is date
    assert commit.message == "Initial ARC"


def test_primary_constructor_file():
    file = FileSystemTree.create_file("README.md")
    assert isinstance(file, FileSystemTree)
    assert file.Name == "README.md"
    assert file.is_file is True
    assert file.is_folder is False
    assert list(file.ToFilePaths()) == ["README.md"]


def test_primary_constructor_empty_folder():
    folder = FileSystemTree.create_folder("root", [])
    assert isinstance(folder, FileSystemTree)
    assert folder.Name == "root"
    assert folder.is_folder is True
    assert folder.is_file is False
    assert list(folder.ToFilePaths()) == []


def test_primary_constructor_nested_folder():
    file = FileSystemTree.create_file("results.csv")
    data = FileSystemTree.create_folder("dataset", [file])
    root = FileSystemTree.create_folder("root", [data])
    assert root.Name == "root"
    assert root.TryGetChildByName("dataset") is data
    assert data.TryGetChildByName("results.csv") is file
    assert list(root.ToFilePaths()) == ["dataset/results.csv"]


def test_primary_constructor_minimal():
    tree = FileSystemTree.create_folder("root", [])
    fs = FileSystem(tree, [])
    assert isinstance(fs, FileSystem)
    assert fs.tree is tree
    assert list(fs.history) == []


def test_filesystem_primary_constructor():
    tree = FileSystemTree.create_folder("root", [FileSystemTree.create_file("README.md")])
    commit = Commit("abc123", "Kevin", "kevin@example.org", datetime(2026, 10, 29, 10, tzinfo=timezone.utc), "Initial ARC")
    history = [commit]
    fs = FileSystem(tree, history)
    assert isinstance(fs, FileSystem)
    assert fs.tree is tree
    assert list(fs.tree.ToFilePaths()) == ["README.md"]
    assert fs.history is history
    assert fs.history[0].message == "Initial ARC"
