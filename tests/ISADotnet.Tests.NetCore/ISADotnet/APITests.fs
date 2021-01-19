module APITests

open Expecto

open ISADotNet.API
open TestingUtils
open Update

module TestTypes = 

    type StringType = {
        StringField: string
    }

    type IntType = {
        IntField: int
    }

    type ListType = {
        StringList: string list
        IntList: int list
    }

    type ArrayType = {
        StringArray: string []
        FloatArray: float []
    }

    type SeqType = {
        StringSeq: seq<string>
        StringOptSeq: seq<string option>
    }

    type RecordTypeType = {
        StringType: StringType
    }

    type MapType = {
        MapField: Map<string,int>
    }

open TestTypes

[<Tests>]
let testUpdate = 

    testList "UpdateTests" [
        testCase "StringType UpdateByExisting" (fun () ->
            let eOld = {StringField = "Hello"}
            let eExisting = {StringField = "New Hello"}
            let eEmpty = {StringField = ""}
    
            let shouldUpate = UpdateByExisting.updateRecordType eOld eExisting
            let shouldNotUpdate = UpdateByExisting.updateRecordType eOld eEmpty
    
            /// eExisting is not empty and should update eOld
            Expect.isTrue (shouldUpate = eExisting) "Record Type with string failed with 'UpdateByExisting' by not updating old record type."
            /// eEmpty is empty and should not update eOld
            Expect.isTrue (shouldNotUpdate = eOld) "Record Type with string failed with 'UpdateByExisting' by updating with new empty record type."
        )
    
        testCase "StringType UpdateAllAppendLists" (fun () ->
            let eOld = {StringField = "Hello"}
            let eExisting = {StringField = "New Hello"}
    
            let shouldUpate = UpdateAllAppendLists.updateRecordType eOld eExisting
    
            /// eExisting is not a list and should update by replacing eOld    
            Expect.isTrue (shouldUpate = eExisting ) "Record Type with string failed with 'UpdateAllAppendLists'"
        )
    
        testCase "IntType UpdateByExisting" (fun () ->
            let eOld = {IntField = 5}
            let eExisting = {IntField = 12}
    
            let shouldUpate = UpdateByExisting.updateRecordType eOld eExisting

            /// eExisting is not empty and should update eOld
            Expect.isTrue (shouldUpate = eExisting ) "IntType failed with 'UpdateByExisting'"
        )
    
        testCase "IntType UpdateAllAppendLists" (fun () ->
            let eOld = {IntField = 2}
            let eExisting = {IntField = 5}
    
            let shouldUpate = UpdateAllAppendLists.updateRecordType eOld eExisting
    
            /// eExisting is not a list and should update by replacing eOld
            Expect.isTrue (shouldUpate = eExisting ) "Record Type with int failed with 'UpdateAllAppendLists'"
        )
    
        testCase "ListType UpdateByExisting" (fun () ->
            let eOld = {
                StringList = ["This"; "Is"; "a"; "Test"]
                IntList = [0 .. 10]
            }
            let eNew = {
                StringList = ["A"; "New"; "Varient"]
                IntList = []
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
    
            /// new IntList is empty and should not update
            Expect.isTrue (update.IntList = eOld.IntList) "ListType UpdateByExisting; failed by updating by empty list"
            Expect.isTrue (update.StringList = eNew.StringList) "ListType UpdateByExisting; failed by not updating StringList"
        )
    
        testCase "ListType UpdateAllAppendLists" (fun () ->
            let eOld = {
                StringList = ["This"; "Is"; "a"; "Test"]
                IntList = [0 .. 10]
            }
            let eNew = {
                StringList = ["A"; "New"; "Varient"]
                IntList = []
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
    
            /// new IntList is empty and should not update
            Expect.isTrue (update.IntList = List.append eNew.IntList eOld.IntList) "ListType UpdateAllAppendLists; failed by not correctly appending IntList"
            Expect.isTrue (update.StringList = List.append eNew.StringList eOld.StringList) "ListType UpdateAllAppendLists; failed by not correctly appending StringList"
        )
    
        testCase "ArrayType UpdateByExisting" (fun () ->
            let eOld = {
                StringArray = [|"This"; "Is"; "a"; "Test"|]
                FloatArray = [|0. .. 10.|]
            }
            let eNew = {
                StringArray = [|"A"; "New"; "Varient"|]
                FloatArray = [||]
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
    
            /// new IntList is empty and should not update
            Expect.isTrue (update.FloatArray = eOld.FloatArray) "ArrayType UpdateByExisting; failed by updating by empty list"
            Expect.isTrue (update.StringArray = eNew.StringArray) "ArrayType UpdateByExisting; failed by not updating StringList"
        )
    
        testCase "ArrayType UpdateAllAppendLists" (fun () ->
            let eOld = {
                StringArray = [|"This"; "Is"; "a"; "Test"|]
                FloatArray = [|0. .. 10.|]
            }
            let eNew = {
                StringArray = [|"A"; "New"; "Varient"|]
                FloatArray = [||]
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
    
            /// new IntList is empty and should not update
            Expect.isTrue (update.FloatArray = Array.append eNew.FloatArray eOld.FloatArray) "ArrayType UpdateAllAppendLists; failed by not correctly appending IntList"
            Expect.isTrue (update.StringArray = Array.append eNew.StringArray eOld.StringArray) "ArrayType UpdateAllAppendLists; failed by not correctly appending StringList"
        )
    
        testCase "SeqType UpdateByExisting" (fun () ->
            let eOld = {
                StringSeq = seq ["This"; "Is"; "a"; "Test"]
                StringOptSeq = seq [Some "Input"; None; Some "noNone"]
            }
            let eNew = {
                StringSeq = seq ["A"; "New"; "Varient"]
                StringOptSeq = seq []
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
    
            /// new IntList is empty and should not update
            Expect.isTrue (update.StringOptSeq = eOld.StringOptSeq) "SeqType UpdateByExisting; failed by updating by empty list"
            Expect.isTrue (update.StringSeq = eNew.StringSeq) "SeqType UpdateByExisting; failed by not updating StringList"
        )
    
        testCase "SeqType UpdateAllAppendLists" (fun () ->
            let eOld = {
                StringSeq = seq ["This"; "Is"; "A"; "Test"]
                StringOptSeq = seq [Some "Input"; None; Some "noNone"]
            }
            let eNew = {
                StringSeq = seq ["A"; "New"; "Varient"]
                StringOptSeq = seq []
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
    
            /// Here the values are passed hard coded as Expect seems to interact strangely when comparing seqs.
            Expect.isTrue (update.StringSeq = seq ["A"; "New"; "Varient"; "This"; "Is"; "A"; "Test"]) "ArrayType UpdateAllAppendLists; failed by not correctly appending list1"
            Expect.isTrue (update.StringOptSeq = seq [Some "Input"; None; Some "noNone"]) "ArrayType UpdateAllAppendLists; failed by not correctly appending list2"
        )
    
        testCase "RecordTypeType UpdateByExisting" (fun () ->
            let eOld = {
                StringType = {StringField = "This is a string"} 
            }
            let eNew = {
                StringType = {StringField = "This is a new string"} 
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
    
            /// record types will never be checked if they are empty or not, so they will always be replaced
            Expect.isTrue (update.StringType = eNew.StringType) "RecordTypeType UpdateByExisting"
        )
    
        testCase "RecordTypeType UpdateAllAppendLists" (fun () ->
            let eOld = {
                StringType = {StringField = "This is a string"} 
            }
            let eNew = {
                StringType = {StringField = "This is a new string"} 
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
    
            /// record types will never be checked if they have lists to append or not, so they will always be replaced
            Expect.isTrue (update.StringType = eNew.StringType) "RecordTypeType UpdateAllAppendLists"
        )
    
        testCase "MapType UpdateByExisting" (fun () ->
            let eOld = {
                MapField = ["key", 2123] |> Map.ofSeq 
            }
            let eNew = {
                MapField = ["new key", 42] |> Map.ofSeq 
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
    
            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.isTrue (update.MapField = eNew.MapField) "MapType UpdateByExisting"
        )
    
        testCase "MapType UpdateAllAppendLists" (fun () ->
            let eOld = {
                MapField = ["key", 2123] |> Map.ofSeq 
            }
            let eNew = {
                MapField = ["new key", 42] |> Map.ofSeq 
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
    
            /// record types will never be checked if they have lists to append or not, so they will always be replaced
            Expect.isTrue (update.MapField = eNew.MapField) "MapType UpdateAllAppendLists"
        )
    
    ]
    