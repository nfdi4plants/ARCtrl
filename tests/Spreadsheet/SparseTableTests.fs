module SparseTableTests

open ARCtrl.ISA.Spreadsheet

open TestingUtils

let main =

    testList "SparseTableTests" [
 
        testCase "Create" (fun () -> 
            
            let keys = ["A";"B"]
            let length = 2

            let sparseTable = SparseTable.Create(keys = keys,length = length)
            
            Expect.equal sparseTable.Matrix.Count 0 "Dictionary was not empty"
            Expect.equal sparseTable.Keys keys "Keys were not taken properly"
            Expect.equal sparseTable.CommentKeys [] "Comment keys should be empty"
            Expect.equal sparseTable.ColumnCount length "Length did not match"

        )

        testCase "AddRow" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]

            let sparseTableFirstRow = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow

            Expect.equal sparseTableFirstRow.Matrix.Count  2           "FirstRowAdded: Dictionary was not empty"
            Expect.equal sparseTableFirstRow.Keys          [firstKey]  "FirstRowAdded: Keys were not updated properly"
            Expect.equal sparseTableFirstRow.CommentKeys   []          "FirstRowAdded: Comment keys should be empty"
            Expect.equal sparseTableFirstRow.ColumnCount        3           "FirstRowAdded: Length did not update according to item count"
            
            let sparseTableSecondRow = 
                sparseTableFirstRow
                |> SparseTable.AddRow secondKey secondRow

            Expect.equal sparseTableSecondRow.Matrix.Count  3                       "SecondRowAdded: Dictionary was not empty"
            Expect.equal sparseTableSecondRow.Keys          [firstKey;secondKey]    "SecondRowAdded: Keys were not updated properly"
            Expect.equal sparseTableSecondRow.CommentKeys   []                      "SecondRowAdded: Comment keys should be empty"
            Expect.equal sparseTableSecondRow.ColumnCount        5                       "SecondRowAdded: Length did not update according to item count"            

        )

        testCase "AddComment" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]
            let firstComment,firstCommentRow = "CommentSameLength",[1,"Lel";2,"Lal";3,"Lul"]
            let secondComment,secondCommentRow = "CommentLonger",[2,"Lal";5,"Sho"]

            let sparseTableFirstComment = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow
                |> SparseTable.AddRow secondKey secondRow
                |> SparseTable.AddComment firstComment firstCommentRow

            Expect.equal sparseTableFirstComment.Matrix.Count  6                       "FirstCommentAdded: Dictionary was not empty"
            Expect.equal sparseTableFirstComment.Keys          [firstKey;secondKey]    "FirstCommentAdded: Keys were not updated properly"
            Expect.equal sparseTableFirstComment.CommentKeys   [firstComment]          "FirstCommentAdded: Comment keys should be empty"
            Expect.equal sparseTableFirstComment.ColumnCount        5                       "FirstCommentAdded: Length did not update according to item count"

            let sparseTableSecondComment = 
                sparseTableFirstComment
                |> SparseTable.AddComment secondComment secondCommentRow

            Expect.equal sparseTableSecondComment.Matrix.Count  8                              "SecondCommentAdded: Dictionary was not empty"
            Expect.equal sparseTableSecondComment.Keys          [firstKey;secondKey]           "SecondCommentAdded: Keys were not update properly"
            Expect.equal sparseTableSecondComment.CommentKeys   [firstComment;secondComment]   "SecondCommentAdded: Comment keys should be empty"
            Expect.equal sparseTableSecondComment.ColumnCount        6                              "SecondCommentAdded: Length did not update according to item count"                    
        )
        testCase "AddEmptyComment" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]
            let firstComment,firstCommentRow = "CommentSameLength",[1,"Lel";2,"Lal";3,"Lul"]
            let emptyComment = "CommentLonger"

            let sparseTableFirstComment = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow
                |> SparseTable.AddRow secondKey secondRow
                |> SparseTable.AddComment firstComment firstCommentRow

            let sparseTableEmptyComment = 
                sparseTableFirstComment
                |> SparseTable.AddEmptyComment emptyComment

            Expect.equal sparseTableEmptyComment.Matrix.Count  6                              "SecondCommentAdded: Dictionary was not empty"
            Expect.equal sparseTableEmptyComment.Keys          [firstKey;secondKey]           "SecondCommentAdded: Keys were not update properly"
            Expect.equal sparseTableEmptyComment.CommentKeys   [firstComment;emptyComment]   "SecondCommentAdded: Comment keys should be empty"
            Expect.equal sparseTableEmptyComment.ColumnCount        5                              "SecondCommentAdded: Length did not update according to item count"                    
        )

        testCase "ToRow" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]
            let firstComment,firstCommentRow = "CommentSameLength",[1,"Lel";2,"Lal";3,"Lul"]
            let secondComment,secondCommentRow = "CommentLonger",[2,"Lal";5,"Sho"]

            let sparseTable = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow
                |> SparseTable.AddRow secondKey secondRow
                |> SparseTable.AddComment firstComment firstCommentRow
                |> SparseTable.AddComment secondComment secondCommentRow


            let testRows = 
                [
                    firstKey ::     List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) firstRow with | Some (_,v) -> v | None -> "")
                    secondKey ::    List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) secondRow with | Some (_,v) -> v | None -> "")
                    Comment.wrapCommentKey firstComment ::  List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) firstCommentRow with | Some (_,v) -> v | None -> "")
                    Comment.wrapCommentKey secondComment :: List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) secondCommentRow with | Some (_,v) -> v | None -> "")               
                ]

            sparseTable
            |> SparseTable.ToRows
            |> Seq.iteri (fun i r ->               
                let testSeq = Seq.item i testRows
                Expect.sequenceEqual (SparseRow.getValues r) testSeq ""
            
            )

        )
    ]

//let main = 
//    testList "SparseTable" [
//    ]