module TestObjects.ROCrate.LabProcess

open ARCtrl.ROCrate

let mandatory_properties() =

    let lp = LabProcess("lab_process_id_1")

    lp.SetValue("name","name")
    lp.SetValue("agent","name")
    lp.SetValue("object", Sample.mandatory_properties())
    lp.SetValue("result","name")


    lp

let all_properties() =

    let lp = LabProcess("lab_process_id_2", "additionalType")

    lp