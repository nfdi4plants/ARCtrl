module TestObjects.ROCrate.PropertyValue

open ARCtrl.ROCrate
open DynamicObj

let mandatory_properties() =

    let pv = PropertyValue("property_value_id_1")

    pv.SetValue("name","name")
    pv.SetValue("value","value")

    pv

let all_properties() =

    let pv = PropertyValue("property_value_id_2", "additionalType")

    pv.SetValue("name","name")
    pv.SetValue("value","value")

    pv.SetValue("propertyID","propertyID")
    pv.SetValue("unitCode","unitCode")
    pv.SetValue("unitText","unitText")
    pv.SetValue("valueReference","valueReference")

    pv