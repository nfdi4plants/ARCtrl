module TestObjects.ROCrate.Sample

open ARCtrl.ROCrate

let mandatory_properties() =

    let s = Sample("sample_id_1")

    s

let all_properties() =

    let s = Sample("sample_id_2", "additionalType")

    s