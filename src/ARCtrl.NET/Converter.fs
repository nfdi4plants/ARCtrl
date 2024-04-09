namespace ARCtrl.NET.Converter

//open ISADotNet
//open ISADotNet.QueryModel
//open FsSpreadsheet.DSL
//open LitXml
//open JsonDSL
//open System.Text.Json

//type ARCconverter =
//| ARCtoCSV      of (QInvestigation -> QStudy -> QAssay -> SheetEntity<Workbook>)
//| ARCtoTSV      of (QInvestigation -> QStudy -> QAssay -> SheetEntity<Workbook>)
//| ARCtoXLSX     of (QInvestigation -> QStudy -> QAssay -> SheetEntity<Workbook>)
//| ARCtoXML      of (QInvestigation -> QStudy -> QAssay -> LitXml.XmlPart)
//| ARCtoJSON     of (QInvestigation -> QStudy -> QAssay -> JEntity<Nodes.JsonNode>)

//    member this.ConvertCSV(i,s,a) = 
//        match this with
//        | ARCtoCSV f -> f i s a
//        | _ -> failwith "could not convert to csv"

//    member this.ConvertTSV(i,s,a) = 
//        match this with
//        | ARCtoTSV f -> f i s a
//        | _ -> failwith "could not convert to tsv"

//    member this.ConvertXLSX(i,s,a) = 
//        match this with
//        | ARCtoXLSX f -> f i s a
//        | _ -> failwith "could not convert to xlsx"

//    member this.ConvertXML(i,s,a) = 
//        match this with
//        | ARCtoXML f -> f i s a
//        | _ -> failwith "could not convert to xml"

//    member this.ConvertJsn(i,s,a) = 
//        match this with
//        | ARCtoJSON f -> f i s a
//        | _ -> failwith "could not convert to xml"
