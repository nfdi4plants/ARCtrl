namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Person =

  type IContext = {
    sdo : string
    arc : string

    Person: string
    
    firstName: string
    lastName: string
    midInitials: string
    email: string
    address: string
    phone: string
    fax: string
    comments: string
    roles: string
    affiliation: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "Person", Encode.string "sdo:Person"
      "name", Encode.string "sdo:name"
      "firstName", Encode.string "sdo:givenName"
      "lastName", Encode.string "sdo:familyName"
      "midInitials", Encode.string "sdo:additionalName"
      "email", Encode.string "sdo:email"
      "address", Encode.string "sdo:address"
      "phone", Encode.string "sdo:telephone"
      "fax", Encode.string "sdo:faxNumber"
      "comments", Encode.string "sdo:disambiguatingDescription"
      "roles", Encode.string "sdo:jobTitle"
      "affiliation", Encode.string "sdo:affiliation"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Person": "sdo:Person",
    "name": "sdo:name",
    "firstName": "sdo:givenName",
    "lastName": "sdo:familyName",
    "midInitials": "sdo:additionalName",
    "email": "sdo:email",
    "address": "sdo:address",
    "phone": "sdo:telephone",
    "fax": "sdo:faxNumber",
    "comments": "sdo:disambiguatingDescription",
    "roles": "sdo:jobTitle",
    "affiliation": "sdo:affiliation"
  }
}
    """