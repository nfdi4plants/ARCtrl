namespace ISADotNet

/// Contains Types for accessing the SWATE written CustomXml
///
/// Swate saves some addtional information like protocol and validation templates in custom xml
module SwateCustomXml =
    
    type SpannedBuildingBlock =
    
        {
            Name: string
            TermAccession: string
        }
    
        static member make name termAccession =
            {Name = name; TermAccession = termAccession}
    
        
    type Protocol =
        {
            Id              : string
            ProtocolVersion : string
            SwateVersion    : string
            TableName       : string
            WorksheetName   : string
            Blocks: SpannedBuildingBlock seq       
        }
    
        static member make id protocolVersion swateVersion tableName worksheetName blocks =
            {Id = id; ProtocolVersion = protocolVersion; SwateVersion = swateVersion; Blocks = blocks; TableName = tableName; WorksheetName = worksheetName}
    
    type ProtocolGroup = 
        {
            SwateVersion : string
            TableName : string
            WorksheetName : string
            Protocols : Protocol seq
        }
    
        static member make swateVersion tableName worksheetName protocols =
            {SwateVersion = swateVersion; TableName = tableName; WorksheetName = worksheetName; Protocols = protocols}
    
    type ColumnValidation =
        {
            ColumnAdress    : string
            ColumnHeader    : string
            Importance      : string
            Unit            : string
            ValidationFormat: string
        }
    
        static member make adress header importance unit validationFormat =
            {ColumnAdress = adress; ColumnHeader = header; Importance = importance; Unit = unit; ValidationFormat = validationFormat}
    
    type TableValidation =
        {
            DateTime        : string
            SwateVersion    : string
            TableName       : string
            Userlist        : string
            WorksheetName   : string
            Columns         : ColumnValidation seq
        }
    
        static member make dateTime swateVersion tableName userlist worksheetName columns=
            {DateTime = dateTime; SwateVersion = swateVersion; TableName = tableName; Userlist = userlist; WorksheetName = worksheetName; Columns = columns}
    
    type SwateTable =
        {
            Table : string
            Worksheet : string
            ProtocolGroup: ProtocolGroup option
            TableValidation: TableValidation option
        }
    
        static member make table worksheet protocolGroup tableValidation =
            {Table = table; Worksheet = worksheet; ProtocolGroup = protocolGroup; TableValidation = tableValidation}
    