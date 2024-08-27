namespace ARCtrl.ROCrate

module ROCrateObject =
    
    let tryAsDataset (roco: IROCrateObject) =
        match roco with 
        | :? Dataset as ds when ds.SchemaType = "schema.org/Dataset" -> Some ds
        | _ -> None

    let tryAsInvestigation (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Investigation", "schema.org/Dataset" -> 
            match roco with
            | :? Investigation as ids -> Some ids
            | _ -> None
        | _ -> 
            None

    let tryAsStudy (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Study", "schema.org/Dataset" -> 
            match roco with
            | :? Study as sds -> Some sds
            | _ -> None
        | _ -> 
            None

    let tryAsAssay (roco: IROCrateObject) =
        match roco.AdditionalType, roco.SchemaType with 
        | Some "Assay", "schema.org/Dataset" -> 
            match roco with
            | :? Assay as ads -> Some ads
            | _ -> None
        | _ -> 
            None