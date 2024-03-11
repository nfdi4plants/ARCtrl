namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Publication =

  type IContext = {
    sdo : string
    arc : string

    Publication: string
    
    pubMedID: string
    doi: string
    title: string
    status: string
    authorList: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "Publication", Encode.string "sdo:ScholarlyArticle"
      
      "pubMedID", Encode.string "sdo:url"
      "doi", Encode.string "sdo:sameAs"
      "title", Encode.string "sdo:headline"
      "status", Encode.string "sdo:creativeWorkStatus"
      "authorList", Encode.string "sdo:author"
      "comments", Encode.string "sdo:disambiguatingDescription"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Publication": "sdo:ScholarlyArticle",
    
    "pubMedID": "sdo:url",
    "doi": "sdo:sameAs",
    "title": "sdo:headline",
    "status": "sdo:creativeWorkStatus",
    "authorList": "sdo:author",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """