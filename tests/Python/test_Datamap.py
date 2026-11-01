from src.ARCtrl import Comment, Datamap, DataContext, DataFile, OntologyAnnotation


def test_constructs_from_an_empty_context_array():
    datamap = Datamap([])
    assert datamap.DataContexts == []
    assert datamap.StaticHash == 0


def test_retains_contexts_in_positional_order_with_nested_data_and_ontology_fields():
    contexts = [
        DataContext(
            'context-1', 'data/results.csv#row=1', DataFile.derived_data_file(), 'text/csv', 'RFC 7111',
            OntologyAnnotation('mass', 'MS', 'MS:1000001'),
            OntologyAnnotation('gram', 'UO', 'UO:0000021'),
            OntologyAnnotation('sample', 'OBI', 'OBI:0000747'),
            'Mass', 'A measured mass', 'run-1', [Comment('quality', 'checked')],
        ),
        DataContext('context-2', 'data/image.png'),
    ]
    datamap = Datamap(contexts)
    assert datamap.DataContexts is contexts
    assert len(datamap.DataContexts) == 2
    assert datamap.StaticHash == 0
    first = datamap.DataContexts[0]
    assert first.ID == 'context-1'
    assert first.Name == 'data/results.csv#row=1'
    assert first.FilePath == 'data/results.csv'
    assert first.Selector == 'row=1'
    assert first.DataType.tag == 1
    assert list(first.DataType.fields) == []
    assert first.Format == 'text/csv'
    assert first.SelectorFormat == 'RFC 7111'
    assert first.Explication.Name == 'mass'
    assert first.Explication.TermSourceREF == 'MS'
    assert first.Explication.TermAccessionNumber == 'MS:1000001'
    assert first.Unit.Name == 'gram'
    assert first.Unit.TermSourceREF == 'UO'
    assert first.Unit.TermAccessionNumber == 'UO:0000021'
    assert first.ObjectType.Name == 'sample'
    assert first.ObjectType.TermSourceREF == 'OBI'
    assert first.ObjectType.TermAccessionNumber == 'OBI:0000747'
    assert first.Label == 'Mass'
    assert first.Description == 'A measured mass'
    assert first.GeneratedBy == 'run-1'
    assert first.Comments[0].Name == 'quality'
    assert first.Comments[0].Value == 'checked'
    assert datamap.DataContexts[1].ID == 'context-2'
    assert datamap.DataContexts[1].Name == 'data/image.png'
