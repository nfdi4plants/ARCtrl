namespace ISA

type Process = 
    {
        ID : URI option
        Name : string option
        ExecutesProtocol : Protocol option
        ParameterValues : ProcessParameterValue list option
        Performer : string option
        Date : string option
        PreviousProcess : Process  option
        NextProcess : Process option
        Inputs : ProcessInput list option
        Outputs : ProcessOutput list option
        Comments : Comment list option
    }

    static member make id name executesProtocol parameterValues performer date previousProcess nextProcess inputs outputs comments : Process= 
        {       
            ID                  = id
            Name                = name
            ExecutesProtocol    = executesProtocol
            ParameterValues     = parameterValues
            Performer           = performer
            Date                = date
            PreviousProcess     = previousProcess
            NextProcess         = nextProcess
            Inputs              = inputs
            Outputs             = outputs
            Comments            = comments       
        }

    static member create (?Id,?Name,?ExecutesProtocol,?ParameterValues,?Performer,?Date,?PreviousProcess,?NextProcess,?Inputs,?Outputs,?Comments) : Process= 
        Process.make Id Name ExecutesProtocol ParameterValues Performer Date PreviousProcess NextProcess Inputs Outputs Comments

    static member empty =
        Process.create()

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let inputCount = this.Inputs |> Option.defaultValue [] |> List.length
            let outputCount = this.Outputs |> Option.defaultValue [] |> List.length
            let paramCount = this.ParameterValues |> Option.defaultValue [] |> List.length

            let name = this.Name |> Option.defaultValue "Unnamed Process"

            sprintf "%s [%i Inputs -> %i Params -> %i Outputs]" name inputCount paramCount outputCount
            
    static member composeName (processNameRoot : string) (i : int) =
        $"{processNameRoot}_{i}"

    static member decomposeName (name : string) =
        let pattern = """(?<name>.+)_(?<num>\d+)"""
        let r = System.Text.RegularExpressions.Regex.Match(name,pattern)

        if r.Success then
            (r.Groups.Item "name").Value, Some ((r.Groups.Item "num").Value |> int)
        else 
            name, None
