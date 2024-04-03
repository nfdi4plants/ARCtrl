namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Investigation =

  type IContext = {
    sdo : string
    arc : string
    Investigation : string


    identifier: string
    title: string
    description: string
    submissionDate: string
    publicReleaseDate: string
    publications: string
    people: string
    studies: string
    ontologySourceReferences: string
    comments: string

    ``publications?``: string
    filename: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "Investigation", Encode.string "sdo:Dataset"

      "identifier", Encode.string "sdo:identifier"
      "title", Encode.string "sdo:headline"
      "additionalType", Encode.string "sdo:additionalType"
      "description", Encode.string "sdo:description"
      "submissionDate", Encode.string "sdo:dateCreated"
      "publicReleaseDate", Encode.string "sdo:datePublished"
      "publications", Encode.string "sdo:citation"
      "people", Encode.string "sdo:creator"
      "studies", Encode.string "sdo:hasPart"
      "ontologySourceReferences", Encode.string "sdo:mentions"
      "comments", Encode.string "sdo:comment"
      "filename", Encode.string "sdo:alternateName"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Investigation": "sdo:Dataset",

    "identifier" : "sdo:identifier",
    "title": "sdo:headline",
    "description": "sdo:description",
    "submissionDate": "sdo:dateCreated",
    "publicReleaseDate": "sdo:datePublished",
    "publications": "sdo:citation",
    "people": "sdo:creator",
    "studies": "sdo:hasPart",
    "ontologySourceReferences": "sdo:mentions",
    "comments": "sdo:disambiguatingDescription",

    "publications?": "sdo:subjectOf?",
    "filename": "sdo:alternateName"
  }
}
    """