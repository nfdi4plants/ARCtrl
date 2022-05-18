namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization

open System.Collections.Generic
open System.Collections

type QColumn = 
    {
        [<JsonPropertyName(@"category")>]
        Header : ISACategory
        [<JsonPropertyName(@"values")>]
        Values : KeyValuePair<string*string,ISAValue> list
    }

