module ARCtrl.Helper.DateTime

let tryParse (s : string) =
    match System.DateTime.TryParse(s) with
    | true, datetime -> Some datetime 
    | _ -> None
