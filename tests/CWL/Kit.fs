/// The shared helpers of the CWL tests: builders for the text of a document, the check of a decode error, the round
/// trip and the variants of a fixture for equality and hash.
module Tests.Kit

open ARCtrl.CWL
open TestingUtils
open YAMLicious

/// The text of the lines, each ended by a line break.
let lines (body: string list) : string = String.concat "\n" body + "\n"

/// A document of the class at version v1.2, with the lines below the class.
let document (cls: string) (body: string list) : string =
    lines ([ "cwlVersion: v1.2"; "class: " + cls ] @ body)

/// A command line tool with the lines below the class. The lines give the ports.
let tool (body: string list) : string = document "CommandLineTool" body

/// A command line tool with the base command echo and no ports, then the lines.
let toolWith (body: string list) : string =
    tool ([ "baseCommand: echo"; "inputs: {}"; "outputs: {}" ] @ body)

/// A workflow with the lines below the class.
let workflow (body: string list) : string = document "Workflow" body

/// A workflow without ports whose steps field holds the lines, which the caller indents by two spaces.
let workflowWithSteps (stepLines: string list) : string =
    workflow ([ "inputs: {}"; "outputs: {}"; "steps:" ] @ stepLines)

/// A workflow without ports with the one step s, whose fields are the lines.
let step (fields: string list) : string =
    workflowWithSteps ("  s:" :: (fields |> List.map (fun line -> "    " + line)))

/// An expression tool with the lines below the class.
let expressionTool (body: string list) : string = document "ExpressionTool" body

/// An operation with the lines below the class.
let operation (body: string list) : string = document "Operation" body

/// A job file of the lines.
let jobFile (body: string list) : string = lines body

/// The decode error that the function raises, and none when it returns.
let decodeError (decode: unit -> 'T) : DecodeError option =
    try
        decode () |> ignore
        None
    with YamlDecodeException error -> Some error

/// Expect a decode error at the path that has a place in the text and whose message holds the keyword, and return the
/// message. The name tells the case in the messages of the expectations.
let decodeFailsWith (name: string) (decode: unit -> 'T) (path: string) (keyword: string) : string =
    match decodeError decode with
    | Some error ->
        Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
        Expect.stringContains error.Message keyword (sprintf "%s: the error should say what is wrong" name)
        Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        error.Message
    | None -> failwith (sprintf "%s must fail" name)

/// Expect a decode error at the path that has a place in the text and whose message holds the keyword.
let decodeFails (name: string) (decode: unit -> 'T) (path: string) (keyword: string) : unit =
    decodeFailsWith name decode path keyword |> ignore

/// decodeFails for the text of a command line tool.
let toolFails (name: string) (text: string) (path: string) (keyword: string) : unit =
    decodeFails name (fun () -> Decode.decodeCommandLineTool text) path keyword

/// decodeFails for the text of a workflow.
let workflowFails (name: string) (text: string) (path: string) (keyword: string) : unit =
    decodeFails name (fun () -> Decode.decodeWorkflow text) path keyword

/// decodeFails for the text of a process of any class.
let processFails (name: string) (text: string) (path: string) (keyword: string) : unit =
    decodeFails name (fun () -> Decode.decodeCWLProcessingUnit text) path keyword

/// Write the object, and expect the written text to decode to an equal object and to be written again as the same text.
/// Returns the object read back and the written text.
let rewrite (name: string) (decode: string -> 'T) (encode: 'T -> string) (value: 'T) : 'T * string =
    let written = encode value
    let again = decode written
    Expect.equal again value (sprintf "%s: the written text should decode to an equal object.\n%s" name written)
    Expect.equal (encode again) written (sprintf "%s: writing again should give the same text." name)
    again, written

/// Decode the text and rewrite the decoded object. Returns the decoded object and the written text.
let roundTrip (name: string) (decode: string -> 'T) (encode: 'T -> string) (text: string) : 'T * string =
    let decoded = decode text
    decoded, snd (rewrite name decode encode decoded)

/// roundTrip for the text of a command line tool. Returns the written text.
let toolRoundTrip (name: string) (text: string) : string =
    roundTrip name Decode.decodeCommandLineTool Encode.encodeToolDescription text |> snd

/// roundTrip for the text of a workflow. Returns the written text.
let workflowRoundTrip (name: string) (text: string) : string =
    roundTrip name Decode.decodeWorkflow Encode.encodeWorkflowDescription text |> snd

/// Expect the objects to be equal and to hash alike.
let expectSame (name: string) (left: obj) (right: obj) : unit =
    Expect.equal left right (sprintf "%s: the objects should be equal." name)
    Expect.equal (left.GetHashCode()) (right.GetHashCode()) (sprintf "%s: equal objects should have the same hash." name)

/// Edit the text of a fixture once for each variant (what, old text, new text) and expect the decoded object to differ
/// from the decoded original in equality and in hash. Two decodes of the original are equal and hash alike.
let variants (name: string) (fixture: string) (decode: string -> 'T) (edits: (string * string * string) list) : unit =
    let text = fixture.Replace("\r\n", "\n")
    let original = decode text
    expectSame (sprintf "%s: a second decode" name) (box original) (box (decode text))
    for (what, oldText, newText) in edits do
        Expect.equal (text.Split([| oldText |], System.StringSplitOptions.None).Length - 1) 1 (sprintf "%s: the fixture should hold %s exactly once" name oldText)
        let changed = decode (text.Replace(oldText, newText))
        Expect.notEqual (box changed) (box original) (sprintf "%s: another %s should make the objects unequal." name what)
        Expect.notEqual ((box changed).GetHashCode()) ((box original).GetHashCode()) (sprintf "%s: another %s should change the hash." name what)

/// The tree without its anchors, so that an alias is the only prohibited feature that a decoder meets.
let rec withoutAnchors (node: YamlNode) : YamlNode =
    let kind =
        match node.Kind with
        | YamlNodeKind.Mapping collection -> YamlNodeKind.Mapping { collection with Items = collection.Items |> List.map (fun entry -> { entry with Key = withoutAnchors entry.Key; Value = withoutAnchors entry.Value }) }
        | YamlNodeKind.Sequence collection -> YamlNodeKind.Sequence { collection with Items = collection.Items |> List.map (fun item -> { item with Node = withoutAnchors item.Node }) }
        | other -> other
    { node with Kind = kind; Properties = { node.Properties with Anchor = None } }
