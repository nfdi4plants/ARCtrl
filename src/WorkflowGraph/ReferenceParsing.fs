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
    /// Resolves an id against the id of the process that holds it. A document can write its ids in full, as cwltool
    /// writes them: the input msg of the step first of the workflow #main is #main/first/msg, and its source is
    /// #main/msg. An id that starts with # and with the id of the holder gives the part behind it, as in msg for
    /// #main/msg when the holder is #main. Any other id is relative already and stays as it is, so a step named like the
    /// workflow keeps its name.
    /// </summary>
    /// <param name="holderId">The id of the process or step that holds the id, if it has one.</param>
    /// <param name="id">The id or reference to resolve.</param>
    let relativeId (holderId: string option) (id: string) : string =
        match holderId with
        | Some holder when not (System.String.IsNullOrWhiteSpace holder) && not (isNull id) && id.Trim().StartsWith "#" ->
            let prefix = trimHashPrefix holder + "/"
            let full = trimHashPrefix id
            if full.StartsWith prefix && full.Length > prefix.Length then full.Substring prefix.Length else id
        | _ -> id

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
