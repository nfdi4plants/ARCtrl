namespace ARCtrl.WorkflowGraph

open ARCtrl.CWL

type ParsedSourceReference = {
    StepId: string option
    PortId: string
    Raw: string
}

[<RequireQualifiedAccess>]
module ReferenceParsing =

    /// Strips a leading '#' prefix from a CWL reference string.
    let trimHashPrefix (value: string) =
        let trimmed = value.Trim()
        if trimmed.StartsWith "#" then trimmed.Substring(1) else trimmed

    /// <summary>
    /// Parses a CWL source reference string into its step and port components.
    /// A reference like "step1/output_file" is parsed into StepId=Some "step1", PortId="output_file".
    /// A simple reference like "input_file" is parsed into StepId=None, PortId="input_file".
    /// </summary>
    /// <param name="source">The CWL source reference string to parse.</param>
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

    /// <summary>
    /// Extracts the output ID string from a StepOutput discriminated union value.
    /// </summary>
    /// <param name="output">The StepOutput to extract the ID from.</param>
    let extractStepOutputId (output: StepOutput) : string =
        match output with
        | StepOutputString id -> id
        | StepOutputRecord record -> record.Id
