module Tests.DataMap

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_empty() = DataMap.init()
    let create_Datacontext (i:int) =
        DataContext(
            $"id_string_{i}",
            "My Name",
            DataFile.DerivedDataFile,
            "My Format",
            "My Selector Format",
            OntologyAnnotation("Explication", "MS", "MS:123456"),
            OntologyAnnotation("Unit", "MS", "MS:123456"),
            OntologyAnnotation("ObjectType", "MS", "MS:123456"),
            "My Label",
            "My Description",
            "Kevin F",
            (ResizeArray [Comment.create("Hello", "World")])
        )
    let create_filled() = 
        DataMap(ResizeArray [
            for i in 1 .. 3 do
                create_Datacontext i            
        ])
