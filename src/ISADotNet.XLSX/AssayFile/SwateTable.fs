namespace ISADotNet.XLSX.AssayFile

open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Packaging

open ISADotNet.XLSX

/// Contains Types and functions for accessing the SWATE written CustomXml
///
/// Swate saves some addtional information like protocol and validation templates in custom xml
module SwateTable =
    
    type SpannedBuildingBlock =
    
        {
            Name: string
            TermAccession: string
        }
    
        static member create name termAccession =
            {Name = name; TermAccession = termAccession}
    
        static member fromXmlElement (element:OpenXmlElement) =
            SpannedBuildingBlock.create
                (element.GetAttribute("Name","").Value)
                (element.GetAttribute("TermAccession","").Value)
        
    type Protocol =
        {
            Id              : string
            ProtocolVersion : string
            SwateVersion    : string
            TableName       : string
            WorksheetName   : string
            Blocks: SpannedBuildingBlock seq       
        }
    
        static member create id protocolVersion swateVersion tableName worksheetName blocks =
            {Id = id; ProtocolVersion = protocolVersion; SwateVersion = swateVersion; Blocks = blocks; TableName = tableName; WorksheetName = worksheetName}
    
        static member fromXmlElement (element:OpenXmlElement) =
            Protocol.create
                (element.GetAttribute("Id","").Value)
                (element.GetAttribute("ProtocolVersion","").Value)
                (element.GetAttribute("SwateVersion","").Value)
                (element.GetAttribute("TableName","").Value)
                (element.GetAttribute("WorksheetName","").Value)
                (element.Elements() |> Seq.map SpannedBuildingBlock.fromXmlElement)
    
    type ProtocolGroup = 
        {
            SwateVersion : string
            TableName : string
            WorksheetName : string
            Protocols : Protocol seq
        }
    
        static member create swateVersion tableName worksheetName protocols =
            {SwateVersion = swateVersion; TableName = tableName; WorksheetName = worksheetName; Protocols = protocols}
    
        static member fromXmlElement (element:OpenXmlElement) =
            ProtocolGroup.create
                (element.GetAttribute("SwateVersion","").Value)
                (element.GetAttribute("TableName","").Value)
                (element.GetAttribute("WorksheetName","").Value)
                (element.Elements() |> Seq.map Protocol.fromXmlElement)
    
    type ColumnValidation =
        {
            ColumnAdress    : string
            ColumnHeader    : string
            Importance      : string
            Unit            : string
            ValidationFormat: string
        }
    
        static member create adress header importance unit validationFormat =
            {ColumnAdress = adress; ColumnHeader = header; Importance = importance; Unit = unit; ValidationFormat = validationFormat}
    
        static member fromXmlElement (element:OpenXmlElement) =
            ColumnValidation.create
                (element.GetAttribute("ColumnAdress","").Value)
                (element.GetAttribute("ColumnHeader","").Value)
                (element.GetAttribute("Importance","").Value)
                (element.GetAttribute("Unit","").Value)
                (element.GetAttribute("ValidationFormat","").Value)
    
    type TableValidation =
        {
            DateTime        : string
            SwateVersion    : string
            TableName       : string
            Userlist        : string
            WorksheetName   : string
            Columns         : ColumnValidation seq
        }
    
        static member create dateTime swateVersion tableName userlist worksheetName columns=
            {DateTime = dateTime; SwateVersion = swateVersion; TableName = tableName; Userlist = userlist; WorksheetName = worksheetName; Columns = columns}
    
        static member fromXmlElement (element:OpenXmlElement) =
            TableValidation.create
                (element.GetAttribute("DateTime","").Value)
                (element.GetAttribute("SwateVersion","").Value)
                (element.GetAttribute("TableName","").Value)
                (element.GetAttribute("Userlist","").Value)
                (element.GetAttribute("WorksheetName","").Value)
                (element.Elements() |> Seq.map ColumnValidation.fromXmlElement)
    
    type SwateTable =
        {
            Table : string
            Worksheet : string
            ProtocolGroup: ProtocolGroup option
            TableValidation: TableValidation option
        }
    
        static member create table worksheet protocolGroup tableValidation =
            {Table = table; Worksheet = worksheet; ProtocolGroup = protocolGroup; TableValidation = tableValidation}
    
        static member fromXmlElement (element:OpenXmlElement) =
            SwateTable.create
                (element.GetAttribute("Table","").Value)
                (element.GetAttribute("Worksheet","").Value)
                (element.Elements() |> Seq.tryPick (fun e -> try ProtocolGroup.fromXmlElement e |> Some with | _ -> None))
                (element.Elements() |> Seq.tryPick (fun e -> try TableValidation.fromXmlElement e |> Some with | _ -> None))
    
    let readSwateTables (wbp : WorkbookPart) =
        let customXml = wbp.GetPartsOfType<CustomXmlPart>() |> Seq.head
        
        let reader = DocumentFormat.OpenXml.OpenXmlReader.Create(customXml)    
        
        if reader.Read() then
            reader.LoadCurrentElement().Elements()
            |> Seq.map SwateTable.fromXmlElement
        else 
            Seq.empty

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
