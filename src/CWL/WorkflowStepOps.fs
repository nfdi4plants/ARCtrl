namespace ARCtrl.CWL

[<RequireQualifiedAccess>]
module StepInputOps =

    /// Updates a StepInput at the given index.
    let updateAt (index: int) (f: StepInput -> StepInput) (inputs: ResizeArray<StepInput>) =
        if index < 0 || index >= inputs.Count then
            invalidArg (nameof index) $"StepInput index {index} is out of range."
        inputs.[index] <- f inputs.[index]

[<RequireQualifiedAccess>]
module WorkflowStepOps =

    /// Updates a workflow step input by index.
    let updateInputAt (index: int) (f: StepInput -> StepInput) (step: WorkflowStep) =
        StepInputOps.updateAt index f step.In

    /// Adds a new StepInput to a workflow step.
    let addInput (input: StepInput) (step: WorkflowStep) =
        step.In.Add input

    /// Removes all StepInputs matching the provided id.
    let removeInputsById (id: string) (step: WorkflowStep) =
        step.In
        |> Seq.filter (fun i -> i.Id <> id)
        |> ResizeArray
        |> fun remaining -> step.In <- remaining

    /// Updates the first StepInput matching the provided id.
    let updateInputById (id: string) (f: StepInput -> StepInput) (step: WorkflowStep) =
        step.In
        |> Seq.tryFindIndex (fun i -> i.Id = id)
        |> Option.iter (fun i -> StepInputOps.updateAt i f step.In)
