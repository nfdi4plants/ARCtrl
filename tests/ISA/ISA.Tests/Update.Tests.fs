module Update.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ARCtrl.ISA.Aux
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

let genericFableTests = 
    testList "GenericFableTests" [
        //Test whether the fable generic is list function can detect a list
        testCase "IsList" (fun () -> 
            let l = [1;2;3]
            let isList = Update.isListType l
            Expect.isTrue isList "IsList failed with list"
        )
        //Test whether the fable generic is list function can detect an array
        testCase "IsArray" (fun () -> 
            let l = [|1;2;3|]
            let isList = Update.isListType l
            Expect.isFalse isList "IsList failed with array"
        )
        //Test whether the fable generic is list function can detect a seq
        testCase "IsSeq" (fun () -> 
            let l = seq {1;2;3}
            let isList = Update.isListType l
            Expect.isFalse isList "IsList failed with seq"
        )
        testCase "IsListEmpty" (fun () -> 
            let l = []
            let isList = Update.isListType l
            Expect.isTrue isList "IsList failed with empty list"
        )
        testCase "IsArrayEmpty" (fun () -> 
            let l = [||]
            let isList = Update.isListType l
            Expect.isFalse isList "IsList failed with empty array"
        )
        testCase "IsSeqEmpty" (fun () -> 
            let l = seq {()}
            let isList = Update.isListType l
            Expect.isFalse isList "IsList failed with empty seq"
        )
        testCase "AppendList" (fun () -> 
            let l1 = [1;2;3]
            let l2 = [4;5;6]
            let t = typeof<int>
            let l3 = Update.appendGenericListsByType l1 l2 t
            let expected = [1;2;3;4;5;6]
            Expect.equal l3 expected "AppendList failed"
        )
        testCase "AppendArray" (fun () -> 
            let l1 = [|1;2;3|]
            let l2 = [|4;5;6|]
            let t = typeof<int>
            let l3 = Update.appendGenericListsByType l1 l2 t
            let expected = [|1;2;3;4;5;6|]
            Expect.equal l3 expected "AppendArray failed"
        )
        testCase "AppendSeq" (fun () -> 
            let l1 = seq [1;2;3]
            let l2 = seq [4;5;6]
            let t = typeof<int>
            let l3 = Update.appendGenericListsByType l1 l2 t
            let expected = seq [1;2;3;4;5;6]
            Expect.equal l3 expected "AppendSeq failed"
        )
        testCase "AppendListEmpty" (fun () -> 
            let l1 : int list = []
            let l2 : int list = []
            let t = typeof<int>
            let l3 = Update.appendGenericListsByType l1 l2 t
            let expected : int list = []
            Expect.equal l3 expected "AppendListEmpty failed"
        )
        testCase "AppendArrayEmpty" (fun () -> 
            let l1 : int array = [||]
            let l2 : int array = [||]
            let t = typeof<int>
            let l3 = Update.appendGenericListsByType l1 l2 t
            let expected : int array = [||]
            Expect.equal l3 expected "AppendArrayEmpty failed"
        )
        testCase "AppendSeqEmpty" (fun () -> 
            let l1 : int seq = seq []
            let l2 : int seq = seq []
            let t = typeof<int>
            let l3 = Update.appendGenericListsByType l1 l2 t
            let expected : int seq = seq []
            Expect.equal l3 expected "AppendSeqEmpty failed"
        )
        testCase "DistinctList" (fun () -> 
            let l1 = [1;2;3;1;2;3]
            let t = typeof<int>
            let l2 = Update.distinctGenericList l1 t
            let expected = [1;2;3]
            Expect.equal l2 expected "DistinctList failed"
        )
        testCase "DistinctArray" (fun () -> 
            let l1 = [|1;2;3;1;2;3|]
            let t = typeof<int>
            let l2 = Update.distinctGenericList l1 t
            let expected = [|1;2;3|]
            Expect.equal l2 expected "DistinctArray failed"
        )
        testCase "DistinctSeq" (fun () -> 
            let l1 = seq [1;2;3;1;2;3]
            let t = typeof<int>
            let l2 = Update.distinctGenericList l1 t
            let expected = seq [1;2;3]
            Expect.equal l2 expected "DistinctSeq failed"
        )
        testCase "DistinctListEmpty" (fun () -> 
            let l1 : int list = []
            let t = typeof<int>
            let l2 = Update.distinctGenericList l1 t
            let expected : int list = []
            Expect.equal l2 expected "DistinctListEmpty failed"
        )
        testCase "DistinctArrayEmpty" (fun () -> 
            let l1 : int array = [||]
            let t = typeof<int>
            let l2 = Update.distinctGenericList l1 t
            let expected : int array = [||]
            Expect.equal l2 expected "DistinctArrayEmpty failed"
        )
        testCase "DistinctSeqEmpty" (fun () -> 
            let l1 : int seq = seq []
            let t = typeof<int>
            let l2 = Update.distinctGenericList l1 t
            let expected : int seq = seq []
            Expect.equal l2 expected "DistinctSeqEmpty failed"
        )
        // Test whether the Update.isMapType correctly detects a that a list is not a map
        testCase "IsMapList" (fun () -> 
            let l = [1;2;3]
            let isMap = Update.isMapType l
            Expect.isFalse isMap "IsMap failed with list"
        )
        // Test whether the Update.isMapType correctly detects a that a array is not a map
        testCase "IsMapArray" (fun () -> 
            let l = [|1;2;3|]
            let isMap = Update.isMapType l
            Expect.isFalse isMap "IsMap failed with array"
        )
        // Test whether the Update.isMapType correctly detects a that a seq is not a map
        testCase "IsMapSeq" (fun () -> 
            let l = seq [1;2;3]
            let isMap = Update.isMapType l
            Expect.isFalse isMap "IsMap failed with seq"
        )
        // Test whether the Update.isMapType correctly detects a that a map is  a map
        testCase "IsMapMap" (fun () -> 
            let l = Map.ofList [1,2;3,4]
            let isMap = Update.isMapType l
            Expect.isTrue isMap "IsMap failed with map"
        )
        // Test whether the Update.isMapType correctly detects that an empty map is  a map
        testCase "IsMapMapEmpty" (fun () -> 
            let l = Map.empty
            let isMap = Update.isMapType l
            Expect.isTrue isMap "IsMap failed with map"
        )
        

    ]

let updateTests = 
    testList "UpdateTests" [

        testCase "appendGenericArray" (fun () ->
            let a1 = [|1;2;3|] |> box
            let a2 = [|4;5;6|] |> box

            let a3 = appendGenericListsByType a1 a2 typeof<int>
            let expected = [|1;2;3;4;5;6|] |> box

            let m = $"{a1}"
            Expect.equal a3 expected m //"appendGenericArray failed"          
        )
        testCase "appendGenericArrayEmpty" (fun () ->
            let a1 = [|1;2;3|] |> box
            let a2 = ([||] : int array) |> box

            let a3 = appendGenericListsByType a1 a2 typeof<int>
            let expected = [|1;2;3|] |> box

            let m = $"{a1}"
            Expect.equal a3 expected m //"appendGenericArray failed"          
        )
        testCase "appendGenericList" (fun () ->
            let l1 = [1;2;3] |> box
            let l2 = [4;5;6] |> box
            let l3 = appendGenericListsByType l1 l2 typeof<int>
            let expected = [1;2;3;4;5;6] |> box
            let m = $"{l1}"
            Expect.equal l3 expected m //"appendGenericList failed"          
        )
        testCase "appendGenericListEmpty" (fun () ->
            let l1 = [1;2;3] |> box
            let l2 = ([] : int list) |> box
            let l3 = appendGenericListsByType l1 l2 typeof<int>
            let expected = [1;2;3] |> box
            let m = $"{l1}"
            Expect.equal l3 expected m //"appendGenericList failed"          
        )
        testCase "appendGenericSeq" (fun () ->
            let l1 = seq [1;2;3] |> box
            let l2 = seq [4;5;6] |> box
            let l3 = appendGenericListsByType l1 l2 typeof<int>
            let expected = seq [1;2;3;4;5;6] |> box
            let m = $"{l1}"
            Expect.equal l3 expected m //"appendGenericList failed"          
        )


        testCase "StringType UpdateByExisting" (fun () ->
            let eOld = {StringField = "Hello"}
            let eExisting = {StringField = "New Hello"}
            let eEmpty = {StringField = ""}
    
            let shouldUpate = UpdateByExisting.updateRecordType eOld eExisting
            let shouldNotUpdate = UpdateByExisting.updateRecordType eOld eEmpty
    
             //eExisting is not empty and should update eOld
            Expect.equal shouldUpate eExisting "Record Type with string failed with 'UpdateByExisting' by not updating old record type."
             //eEmpty is empty and should not update eOld
            Expect.equal shouldNotUpdate eOld "Record Type with string failed with 'UpdateByExisting' by updating with new empty record type."
        )
    
        testCase "StringType UpdateAllAppendLists" (fun () ->
            let eOld = {StringField = "Hello"}
            let eExisting = {StringField = "New Hello"}
    
            let shouldUpate = UpdateAllAppendLists.updateRecordType eOld eExisting
    
             //eExisting is not a list and should update by replacing eOld    
            Expect.equal shouldUpate eExisting "Record Type with string failed with 'UpdateAllAppendLists'"
        )

        testCase "StringType UpdateByExistingAppendLists" (fun () ->
            let eOld = {StringField = "Hello"}
            let eExisting = {StringField = "New Hello"}
            let eEmpty = {StringField = ""}
            
            let shouldUpate = UpdateByExistingAppendLists.updateRecordType eOld eExisting
            let shouldNotUpdate = UpdateByExistingAppendLists.updateRecordType eOld eEmpty
            
            // eExisting is not empty and should update eOld
            Expect.equal shouldUpate eExisting "Record Type with string failed with 'UpdateByExistingAppendLists' by not updating old record type."
            // eEmpty is empty and should not update eOld
            Expect.equal shouldNotUpdate eOld "Record Type with string failed with 'UpdateByExistingAppendLists' by updating with new empty record type."
        )
    
        testCase "IntType UpdateByExisting" (fun () ->
            let eOld = {IntField = 5}
            let eExisting = {IntField = 12}
    
            let shouldUpate = UpdateByExisting.updateRecordType eOld eExisting

            // eExisting is not empty and should update eOld
            Expect.equal shouldUpate  eExisting "IntType failed with 'UpdateByExisting'"
        )
    
        testCase "IntType UpdateAllAppendLists" (fun () ->
            let eOld = {IntField = 2}
            let eExisting = {IntField = 5}
    
            let shouldUpate = UpdateAllAppendLists.updateRecordType eOld eExisting
    
            // eExisting is not a list and should update by replacing eOld
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
    
            // new IntList is empty and should not update
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
            // new IntList is empty and should not update
            TestingUtils.Expect.mySequenceEqual update.IntList (List.append eOld.IntList eNew.IntList) "ListType UpdateAllAppendLists; failed by not correctly appending IntList"
            TestingUtils.Expect.mySequenceEqual update.StringList (List.append eOld.StringList eNew.StringList) "ListType UpdateAllAppendLists; failed by not correctly appending StringList"
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
    
            // new IntList is empty and should not update
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
    
            // new IntList is empty and should not update
            TestingUtils.Expect.mySequenceEqual update.FloatArray (Array.append eOld.FloatArray eNew.FloatArray) "ArrayType UpdateAllAppendLists; failed by not correctly appending IntList"
            TestingUtils.Expect.mySequenceEqual update.StringArray (Array.append eOld.StringArray eNew.StringArray) "ArrayType UpdateAllAppendLists; failed by not correctly appending StringList"
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
    
            // new IntList is empty and should not update
            Expect.equal update.StringOptSeq eOld.StringOptSeq "SeqType UpdateByExisting; failed by updating by empty list"
            Expect.equal update.StringSeq eNew.StringSeq "SeqType UpdateByExisting; failed by not updating StringList"
        )
    
        testCase "SeqType UpdateAllAppendLists" (fun () ->
            let eOld = {
                StringSeq = seq ["This"; "Is"; "a"; "Test"]
                StringOptSeq = seq [Some "Input"; None; Some "noNone"]
            }
            let eNew = {
                StringSeq = seq ["A"; "New"; "Varient"]
                StringOptSeq = seq []
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
    
            // Here the values are passed hard coded as Expect seems to interact strangely when comparing seqs.
            TestingUtils.Expect.mySequenceEqual update.StringSeq       (Seq.append eOld.StringSeq eNew.StringSeq) "ArrayType UpdateAllAppendLists; failed by not correctly appending list1"
            TestingUtils.Expect.mySequenceEqual update.StringOptSeq    (Seq.append eOld.StringOptSeq eNew.StringOptSeq) "ArrayType UpdateAllAppendLists; failed by not correctly appending list2"
        )
    
        testCase "SeqType UpdateByExistingAppendLists" (fun () ->
            let eOld = {
                StringSeq = seq ["This"; "Is"; "a"; "Test"]
                StringOptSeq = seq [Some "Input"; None; Some "noNone"]
            }
            let eNew = {
                StringSeq = seq ["A"; "New"; "Varient"]
                StringOptSeq = seq []
            }
    
            let update = UpdateByExistingAppendLists.updateRecordType eOld eNew
    
            // Here the values are passed hard coded as Expect seems to interact strangely when comparing seqs.
            TestingUtils.Expect.mySequenceEqual update.StringSeq       (Seq.append eOld.StringSeq eNew.StringSeq) "ArrayType UpdateAllAppendLists; failed by not correctly appending list1"
            TestingUtils.Expect.mySequenceEqual update.StringOptSeq    (Seq.append eOld.StringOptSeq eNew.StringOptSeq) "ArrayType UpdateAllAppendLists; failed by not correctly appending list2"
        )

        testCase "RecordTypeType UpdateByExisting" (fun () ->
            let eOld = {
                StringType = {StringField = "This is a string"} 
            }
            let eNew = {
                StringType = {StringField = "This is a new string"} 
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
    
            // record types will never be checked if they are empty or not, so they will always be replaced
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
    
            // record types will never be checked if they have lists to append or not, so they will always be replaced
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
    
            // map types will never be checked if they are empty or not, so they will always be replaced
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
    
            // record types will never be checked if they have lists to append or not, so they will always be replaced
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

            // map types will never be checked if they are empty or not, so they will always be replaced
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

            // map types will never be checked if they are empty or not, so they will always be replaced
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

            // map types will never be checked if they are empty or not, so they will always be replaced
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

            // map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalList  eNew.OptionalList "Optional List was not updated correctly"
            Expect.equal updateEmpty.OptionalList eNewEmpty.OptionalList "Optional List was not updated with empty value even though \"UpdateAll\" option is used"
        )

        testCase "OptionalListType UpdateAllAppendLists" (fun () ->
            let eOld = {
                OptionalList = Some ["MyValue"]
            }
            let eNew = {
                OptionalList = Some ["NewValue"]
            }
    
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
            let appendedList = List.append eOld.OptionalList.Value eNew.OptionalList.Value

            // map types will never be checked if they are empty or not, so they will always be replaced
            TestingUtils.Expect.mySequenceEqual update.OptionalList.Value appendedList "Optional Lists were not appended correctly"
        )

        testCase "OptionalListType UpdateAllAppendListsEmpty" (fun () ->
            // Object type is ignored in javascript so eOld passed as an IEnumerable in the update function
            // There the append method does not work, as eNewEmpty is an undefined object (= null) and not an empty list
            // Therefore we need to check if the new List is a null and if so, return the old list
            let eOld = {
                OptionalList = Some ["MyValue"]
            }
            let eNewEmpty = {
                OptionalList = None         
            }
    
            let updateEmpty = UpdateAllAppendLists.updateRecordType eOld eNewEmpty

            Expect.equal updateEmpty.OptionalList eOld.OptionalList "Optional List was updated with empty value even though \"UpdateAllAppendLists\" option is used"
        )

        testCase "OptionalListType UpdateByExistingAppendLists" (fun () ->
            let eOld = {
                OptionalList = Some ["MyValue"]
            }
            let eNew = {
                OptionalList = Some ["NewValue"]
            }

            let update = UpdateByExistingAppendLists.updateRecordType eOld eNew
            let appendedList = List.append eOld.OptionalList.Value eNew.OptionalList.Value

            // map types will never be checked if they are empty or not, so they will always be replaced
            TestingUtils.Expect.mySequenceEqual update.OptionalList.Value appendedList "Optional Lists were not appended correctly"
        )

        testCase "OptionalListType UpdateByExistingAppendListsEmpty" (fun () ->
            let eOld = {
                OptionalList = Some ["MyValue"]
            }
            let eNewEmpty = {
                OptionalList = None         
            }
    
            let updateEmpty = UpdateByExistingAppendLists.updateRecordType eOld eNewEmpty

            Expect.equal updateEmpty.OptionalList eOld.OptionalList "Optional List was updated with empty value even though \"UpdateByExistingAppendLists\" option is used"
        )


        testCase "OptionalRecordTypeType UpdateByExisting" (fun () ->
            let eOld = {
                OptionalRecordType = Some {StringField = "MyValue"}
            }
            let eNew = {
                OptionalRecordType = Some {StringField = "NewValue"}
            }
            let eNewEmpty = {
                OptionalRecordType = None         
            }
    
            let update = UpdateByExisting.updateRecordType eOld eNew
            let updateEmpty = UpdateByExisting.updateRecordType eOld eNewEmpty

            // map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalRecordType  eNew.OptionalRecordType "Optional Record Type was not updated correctly"
            Expect.equal updateEmpty.OptionalRecordType eOld.OptionalRecordType "Optional Record Type was updated with empty value even though \"UpdateByExisting\" option is used"
        )

        testCase "OptionalRecordTypeType UpdateAllAppendLists" (fun () ->
            let eOld = {
                OptionalRecordType = Some {StringField = "MyValue"}
            }
            let eNew = {
                OptionalRecordType = Some {StringField = "NewValue"}
            }
            let eNewEmpty = {
                OptionalRecordType = None         
            }
            let update = UpdateAllAppendLists.updateRecordType eOld eNew
            let updateEmpty = UpdateAllAppendLists.updateRecordType eOld eNewEmpty

            // map types will never be checked if they are empty or not, so they will always be replaced
            Expect.equal update.OptionalRecordType  eNew.OptionalRecordType "Optional Record Type was not updated correctly"
            Expect.equal updateEmpty.OptionalRecordType eNewEmpty.OptionalRecordType "Optional Record Type was not updated with empty value even though \"UpdateAllAppendLists\" option is used"
        )
    ]

let main = 
    testList "APITests" [
        genericFableTests
        updateTests

        
    ]