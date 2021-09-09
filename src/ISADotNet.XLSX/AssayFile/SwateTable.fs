namespace ISADotNet.XLSX.AssayFile

open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Packaging
open FSharpSpreadsheetML

open ISADotNet.SwateCustomXml
open ISADotNet.XLSX

/// Contains functions for accessing the SWATE written CustomXml
///
/// Swate saves some addtional information like protocol and validation templates in custom xml
module SwateTable =
    
    module SpannedBuildingBlock =
    
        let fromXmlElement (element:OpenXmlElement) =
            SpannedBuildingBlock.make
                (element.GetAttribute("Name","").Value)
                (element.GetAttribute("TermAccession","").Value)
        
    module Protocol =

        let fromXmlElement (element:OpenXmlElement) =
            Protocol.make
                (element.GetAttribute("Id","").Value)
                (element.GetAttribute("ProtocolVersion","").Value)
                (element.GetAttribute("SwateVersion","").Value)
                (element.GetAttribute("TableName","").Value)
                (element.GetAttribute("WorksheetName","").Value)
                (element.Elements() |> Seq.map SpannedBuildingBlock.fromXmlElement)
    
    module ProtocolGroup = 

        let fromXmlElement (element:OpenXmlElement) =
            ProtocolGroup.make
                (element.GetAttribute("SwateVersion","").Value)
                (element.GetAttribute("TableName","").Value)
                (element.GetAttribute("WorksheetName","").Value)
                (element.Elements() |> Seq.map Protocol.fromXmlElement)
    
    module ColumnValidation =

        let fromXmlElement (element:OpenXmlElement) =
            ColumnValidation.make
                (element.GetAttribute("ColumnAdress","").Value)
                (element.GetAttribute("ColumnHeader","").Value)
                (element.GetAttribute("Importance","").Value)
                (element.GetAttribute("Unit","").Value)
                (element.GetAttribute("ValidationFormat","").Value)
    
    module TableValidation =

        let fromXmlElement (element:OpenXmlElement) =
            TableValidation.make
                (element.GetAttribute("DateTime","").Value)
                (element.GetAttribute("SwateVersion","").Value)
                (element.GetAttribute("TableName","").Value)
                (element.GetAttribute("Userlist","").Value)
                (element.GetAttribute("WorksheetName","").Value)
                (element.Elements() |> Seq.map ColumnValidation.fromXmlElement)
    
    module SwateTable =

        let fromXmlElement (element:OpenXmlElement) =
            SwateTable.make
                (element.GetAttribute("Table","").Value)
                (element.GetAttribute("Worksheet","").Value)
                (element.Elements() |> Seq.tryPick (fun e -> try ProtocolGroup.fromXmlElement e |> Some with | _ -> None))
                (element.Elements() |> Seq.tryPick (fun e -> try TableValidation.fromXmlElement e |> Some with | _ -> None))
    
    
        let readSwateTables (wbp : WorkbookPart) =
            match wbp.GetPartsOfType<CustomXmlPart>() |> Seq.tryHead with
            | Some customXml ->

                let reader = DocumentFormat.OpenXml.OpenXmlReader.Create(customXml)    
        
                if reader.Read() then
                    reader.LoadCurrentElement().Elements()
                    |> Seq.map fromXmlElement
                else 
                    Seq.empty
            | None -> Seq.empty

        let readSwateTablesFromStream (stream:#System.IO.Stream) = 

            let doc = Spreadsheet.fromStream stream false
            try
                Spreadsheet.getWorkbookPart doc
                |> readSwateTables
            finally
                Spreadsheet.close doc


        let trySelectProtocolheaders (protocol:Protocol) (headers:seq<string>) =
            let nodes = AnnotationNode.splitIntoNodes headers
            let blocks = protocol.Blocks |> Seq.map (fun b -> b.Name) |> Set.ofSeq
            let blocksMatch =
                protocol.Blocks
                |> Seq.map (fun b -> Seq.exists (fun n -> Seq.contains b.Name n) nodes)
                |> Seq.reduce (&&)
            if blocksMatch then
                nodes 
                |> Seq.filter (Seq.exists blocks.Contains)
                |> Seq.concat
                |> Some
            else
                None
