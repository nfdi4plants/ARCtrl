namespace ARCtrl.WorkflowGraph

open ARCtrl.CWL

type ParsedSourceReference = {
    StepId: string option
    PortId: string
    Raw: string
}

[<RequireQualifiedAccess>]
module ReferenceParsing =

    let private trimHashPrefix (value: string) =
        let trimmed = value.Trim()
        if trimmed.StartsWith "#" then trimmed.Substring(1) else trimmed

    let parseSourceReference (source: string) : ParsedSourceReference =
        if System.String.IsNullOrWhiteSpace source then
            {
                StepId = None
                PortId = ""
                Raw = defaultArg (Option.ofObj source) ""
            }
        else
            let normalized = source |> trimHashPrefix
            let separatorIndex = normalized.IndexOf('/')
            if separatorIndex < 0 then
                {
                    StepId = None
                    PortId = normalized
                    Raw = source
                }
            else
                let stepId = normalized.Substring(0, separatorIndex).Trim()
                let portId = normalized.Substring(separatorIndex + 1).Trim()
                {
                    StepId =
                        if System.String.IsNullOrWhiteSpace stepId then
                            None
                        else
                            Some stepId
                    PortId = portId
                    Raw = source
                }

    let extractStepOutputId (output: StepOutput) : string =
        match output with
        | StepOutputString id -> id
        | StepOutputRecord record -> record.Id
