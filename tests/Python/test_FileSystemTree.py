from src.ARCtrl import FileSystemTree

class TestFactories:

    def test_from_file_paths(self):
        tree = FileSystemTree.from_file_paths(["assays/a/isa.assay.xlsx"])
        assert tree.ContainsChildWithName("assays")

    def test_create_root_folder(self):
        tree = FileSystemTree.create_root_folder([])
        assert tree.Name == FileSystemTree.ROOT_NAME
