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

    type OptionalStringType = {
        OptionalString: string option    
    }

    type OptionalListType = {
        OptionalList: string list option    
    }

    type OptionalRecordTypeType = {
        OptionalRecordType: StringType option    
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
            Expect.equal shouldUpate eExisting "Record Type with string failed with 'UpdateByExisting' by not updating old record type."
            /// eEmpty is empty and should not update eOld
            Expect.equal shouldNotUpdate eOld "Record Type with string failed with 'UpdateByExisting' by updating with new empty record type."
        )
    
        testCase "StringType UpdateAllAppendLists" (fun () ->
            let eOld = {StringField = "Hello"}
            let eExisting = {StringField = "New Hello"}
    
            let shouldUpate = UpdateAllAppendLists.updateRecordType eOld eExisting
    
            /// eExisting is not a list and should update by replacing eOld    
            Expect.equal shouldUpate eExisting "Record Type with string failed with 'UpdateAllAppendLists'"
        )
    
        testCase "IntType UpdateByExisting" (fun () ->
            let eOld = {IntField = 5}
            let eExisting = {IntField = 12}
    
            let shouldUpate = UpdateByExisting.updateRecordType eOld eExisting

            /// eExisting is not empty and should update eOld
            Expect.equal shouldUpate  eExisting "IntType failed with 'UpdateByExisting'"
        )
    
        testCase "IntType UpdateAllAppendLists" (fun () ->
            let eOld = {IntField = 2}
            let eExisting = {IntField = 5}
    
            let shouldUpate = UpdateAllAppendLists.updateRecordType eOld eExisting
    
            /// eExisting is not a list and should update by replacing eOld
            Expect.equal shouldUpate eExisting "Record Type with int failed with 'UpdateAllAppendLists'"
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
            Expect.equal update.IntList eOld.IntList "ListType UpdateByExisting; failed by updating by empty list"
            Expect.equal update.StringList eNew.StringList "ListType UpdateByExisting; failed by not updating StringList"
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
            Expect.sequenceEqual update.IntList (List.append eOld.IntList eNew.IntList) "ListType UpdateAllAppendLists; failed by not correctly appending IntList"
            Expect.sequenceEqual update.StringList (List.append eOld.StringList eNew.StringList) "ListType UpdateAllAppendLists; failed by not correctly appending StringList"
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
            Expect.equal update.FloatArray eOld.FloatArray "ArrayType UpdateByExisting; failed by updating by empty list"
            Expect.equal update.StringArray eNew.StringArray "ArrayType UpdateByExisting; failed by not updating StringList"
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
            Expect.sequenceEqual update.FloatArray (Array.append eOld.FloatArray eNew.FloatArray) "ArrayType UpdateAllAppendLists; failed by not correctly appending IntList"
            Expect.sequenceEqual update.StringArray (Array.append eOld.StringArray eNew.StringArray) "ArrayType UpdateAllAppendLists; failed by not correctly appending StringList"
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
            Expect.equal update.StringOptSeq eOld.StringOptSeq "SeqType UpdateByExisting; failed by updating by empty list"
            Expect.equal update.StringSeq eNew.StringSeq "SeqType UpdateByExisting; failed by not updating StringList"
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
            Expect.sequenceEqual update.StringSeq       (Seq.append eOld.StringSeq eNew.StringSeq) "ArrayType UpdateAllAppendLists; failed by not correctly appending list1"
            Expect.sequenceEqual update.StringOptSeq    (Seq.append eOld.StringOptSeq eNew.StringOptSeq) "ArrayType UpdateAllAppendLists; failed by not correctly appending list2"
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
            Expect.equal update.StringType eNew.StringType "RecordTypeType UpdateByExisting"
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
            Expect.equal update.StringType eNew.StringType "RecordTypeType UpdateAllAppendLists"
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
            Expect.equal update.MapField eNew.MapField "MapType UpdateByExisting"
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
            Expect.equal update.MapField eNew.MapField "MapType UpdateAllAppendLists"
        )
    
        testCase "OptionalStringType UpdateByExisting" (fun () ->
            let eOld = {
                OptionalString = Some "Value"
            }
            let eNew = {
                OptionalString = Some "NewValue"
            }
            let eNewEmpty = {
                OptionalString = None         
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
            let updateEmpty = UpdateByExisting.updateRecordType eOld eNewEmpty

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalString  eNew.OptionalString "Optional string was not updated correctly"
            Expect.equal updateEmpty.OptionalString eOld.OptionalString "Optional string was updated with empty value even though \"UpdateByExisting\" option is used"
        )

        testCase "OptionStringType UpdateAllAppendLists" (fun () ->
            let eOld = {
                OptionalString = Some "Value"
            }
            let eNew = {
                OptionalString = Some "NewValue"
            }
            let eNewEmpty = {
                OptionalString = None         
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
            let updateEmpty = UpdateAllAppendLists.updateRecordType eOld eNewEmpty

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalString  eNew.OptionalString "Optional string was not updated correctly"
            Expect.equal updateEmpty.OptionalString eNewEmpty.OptionalString "Optional string was not updated with empty value even though \"UpdateAllAppendLists\" option is used"
        )

        testCase "OptionalListType UpdateByExisting" (fun () ->
            let eOld = {
                OptionalList = Some ["Value"]
            }
            let eNew = {
                OptionalList = Some ["NewValue"]
            }
            let eNewEmpty = {
                OptionalList = None         
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
            let updateEmpty = UpdateByExisting.updateRecordType eOld eNewEmpty

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalList  eNew.OptionalList "Optional List was not updated correctly"
            Expect.equal updateEmpty.OptionalList eOld.OptionalList "Optional List was updated with empty value even though \"UpdateByExisting\" option is used"
        )

        testCase "OptionalListType UpdateAll" (fun () ->
            let eOld = {
                OptionalList = Some ["Value"]
            }
            let eNew = {
                OptionalList = Some ["NewValue"]
            }
            let eNewEmpty = {
                OptionalList = None         
            }
    
            let update = UpdateAll.updateRecordType eOld eNew
            let updateEmpty = UpdateAll.updateRecordType eOld eNewEmpty

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalList  eNew.OptionalList "Optional List was not updated correctly"
            Expect.equal updateEmpty.OptionalList eNewEmpty.OptionalList "Optional List was not updated with empty value even though \"UpdateAll\" option is used"
        )

        testCase "OptionalListType UpdateAllAppendLists" (fun () ->
            let eOld = {
                OptionalList = Some ["Value"]
            }
            let eNew = {
                OptionalList = Some ["NewValue"]
            }
            let eNewEmpty = {
                OptionalList = None         
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
            let updateEmpty = UpdateAllAppendLists.updateRecordType eOld eNewEmpty
            let appendedList = List.append eOld.OptionalList.Value eNew.OptionalList.Value

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.sequenceEqual update.OptionalList.Value appendedList "Optional Lists were not appended correctly"
            Expect.equal updateEmpty.OptionalList eOld.OptionalList "Optional List was updated with empty value even though \"UpdateAllAppendLists\" option is used"
        )

        testCase "OptionalRecordTypeType UpdateByExisting" (fun () ->
            let eOld = {
                OptionalRecordType = Some {StringField = "Value"}
            }
            let eNew = {
                OptionalRecordType = Some {StringField = "NewValue"}
            }
            let eNewEmpty = {
                OptionalRecordType = None         
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
            let updateEmpty = UpdateByExisting.updateRecordType eOld eNewEmpty

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalRecordType  eNew.OptionalRecordType "Optional Record Type was not updated correctly"
            Expect.equal updateEmpty.OptionalRecordType eOld.OptionalRecordType "Optional Record Type was updated with empty value even though \"UpdateByExisting\" option is used"
        )

        testCase "OptionalRecordTypeType UpdateAllAppendLists" (fun () ->
            let eOld = {
                OptionalRecordType = Some {StringField = "Value"}
            }
            let eNew = {
                OptionalRecordType = Some {StringField = "NewValue"}
            }
            let eNewEmpty = {
                OptionalRecordType = None         
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
            let updateEmpty = UpdateAllAppendLists.updateRecordType eOld eNewEmpty

            /// map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalRecordType  eNew.OptionalRecordType "Optional Record Type was not updated correctly"
            Expect.equal updateEmpty.OptionalRecordType eNewEmpty.OptionalRecordType "Optional Record Type was not updated with empty value even though \"UpdateAllAppendLists\" option is used"
        )
    ]
    