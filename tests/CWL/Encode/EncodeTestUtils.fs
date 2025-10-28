module EncodeTestUtils

open Fable.Pyxpecto
open ARCtrl.CWL
open TestingUtils.Expect

/// Assert that encoding then decoding an entity is deterministic across two cycles.
let assertDeterministic encode decode name (originalText:string) =
    let d1 = decode originalText
    let e1 = encode d1
    let d2 = decode e1
    let e2 = encode d2
    stringEqual e1 originalText ($"{d1}: encoding must be true to the original")
    Expect.equal e2 e1 ($"{name}: encoding must be deterministic across multiple encode/decode cycles")
    e1, d1, d2

/// Assert all outputs have outputSource present.
let assertAllOutputsHaveSource (wf: CWLWorkflowDescription) =
    wf.Outputs |> Seq.iter (fun o -> Expect.isSome o.OutputSource ($"{o.OutputSource.Value}"))

/// Quick utility to verify requirements rendered in extended form (heuristic search for 'class:').
let assertRequirementsExtended (text:string) =
    let ok = text.Contains("class:")
    Expect.isTrue ok "Requirements should be encoded in extended form including 'class:' lines"
