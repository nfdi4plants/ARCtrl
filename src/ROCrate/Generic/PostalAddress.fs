namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.Helper

///
[<AttachMembers>]
type PostalAddress =

    static member schemaType = "http://schema.org/PostalAddress"

    static member addressCountry = "http://schema.org/addressCountry"

    static member postalCode = "http://schema.org/postalCode"

    static member streetAddress = "http://schema.org/streetAddress"

    static member tryGetAddressCountryAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(PostalAddress.addressCountry, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getAddressCountryAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(PostalAddress.addressCountry, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Value of property `addressCountry` of object with @id `{s.Id}` should have been a string"
        | None -> failwith $"Could not access property `addressCountry` of object with @id `{s.Id}`"

    static member setAddressCountryAsString(s : LDNode, n : string) =
        s.SetProperty(PostalAddress.addressCountry, n)

    static member tryGetPostalCodeAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(PostalAddress.postalCode, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getPostalCodeAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(PostalAddress.postalCode, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Value of property `postalCode` of object with @id `{s.Id}` should have been a string"
        | None -> failwith $"Could not access property `postalCode` of object with @id `{s.Id}`"

    static member setPostalCodeAsString(s : LDNode, n : string) =
        s.SetProperty(PostalAddress.postalCode, n)

    static member tryGetStreetAddressAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(PostalAddress.streetAddress, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getStreetAddressAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(PostalAddress.streetAddress, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Value of property `streetAddress` of object with @id `{s.Id}` should have been a string"
        | None -> failwith $"Could not access property `streetAddress` of object with @id `{s.Id}`"

    static member setStreetAddressAsString(s : LDNode, n : string) =
        s.SetProperty(PostalAddress.streetAddress, n)

    static member genID(?addressCountry : string, ?postalCode : string, ?streetAddress : string) =
        let items = 
            [
                if addressCountry.IsSome then yield "addressCountry"
                if postalCode.IsSome then yield "postalCode"
                if streetAddress.IsSome then yield "streetAddress"
            ]
        if items.IsEmpty then Identifier.createMissingIdentifier()
        else
            items
            |> List.reduce (fun acc x -> $"{acc}_{x}")
        
    static member validate(o : LDNode, ?context : LDContext) =
        o.HasType(PostalAddress.schemaType, ?context = context)

    static member create(?id : string, ?addressCountry : string, ?postalCode : string, ?streetAddress : string, ?context : LDContext) =
        let id = 
            match id with
            | Some x -> x
            | None -> PostalAddress.genID(?addressCountry = addressCountry, ?postalCode = postalCode, ?streetAddress = streetAddress)
        let s = LDNode(id, ResizeArray [PostalAddress.schemaType], ?context = context)
        s.SetOptionalProperty(PostalAddress.addressCountry, addressCountry)
        s.SetOptionalProperty(PostalAddress.postalCode, postalCode)
        s.SetOptionalProperty(PostalAddress.streetAddress, streetAddress)
        s