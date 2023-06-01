namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

type QCommentCollection(comments : Comment list) =

    new(comments : Comment list option) = QCommentCollection(Option.defaultValue [] comments) 

    member this.Comments = comments

    interface IEnumerable<Comment> with
        member this.GetEnumerator() = (Seq.ofList this.Comments).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<Comment>).GetEnumerator() :> IEnumerator

    member this.GetValueByName(key : string) =
        API.CommentList.item key this.Comments

    member this.TryGetValueByName(key : string) =
        API.CommentList.tryItem key this.Comments
