namespace ISA

type ArcTable = 
    {
        Name : string
        InputHeader : IOType
        Inputs : string []
        OutputHeader : IOType
        Outputs : string []
        ValueHeaders : CompositeHeader []
        Values : System.Collections.Generic.Dictionary<int*int,CompositeCell>  
    }

    static member insertParameterValue (t : ArcTable) (p : ProcessParameterValue) : ArcTable = 
        raise (System.NotImplementedException())

    static member getParameterValues (t : ArcTable) : ProcessParameterValue [] = 
        raise (System.NotImplementedException())

    // no 
    static member addProcess = 
        raise (System.NotImplementedException())

    static member addRow input output values = //yes
        raise (System.NotImplementedException())

    static member insertColumn index values = //yes
        raise (System.NotImplementedException())

    //add Param?
    static member appendColumn index values = //yes
        raise (System.NotImplementedException())

    static member getProtocols (t : ArcTable) : Protocol [] = 
        raise (System.NotImplementedException())

    static member getProcesses (t : ArcTable) : Process [] = 
        raise (System.NotImplementedException())

    static member fromProcesses (ps : Process array) : ArcTable = 
        raise (System.NotImplementedException())
