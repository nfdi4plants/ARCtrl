namespace ARCtrl

open Fable.Core
open ARCtrl.Contract

[<StringEnum>]
type LicenseContentType =
    | Fulltext

[<AttachMembers>]
type License( ``type``: LicenseContentType, content: string) =

    let mutable _type: LicenseContentType = ``type``
    let mutable _content: string = content
    let mutable _staticHash = 0

    member this.Type
        with get() = _type
        and set(h) = _type <- h

    member this.Content
        with get() = _content
        and set(h) = _content <- h

    member this.StaticHash
        with get() = _staticHash
        and set(h) = _staticHash <- h

    static member initFulltext(content: string) =
        License(LicenseContentType.Fulltext, content)

    member this.ToCreateContract () =
        let path = ArcPathHelper.LICENSEFileName
        match this.Type with
        | LicenseContentType.Fulltext ->
            Contract.createCreate(path, DTOType.PlainText, DTO.Text this.Content)

    member this.ToUpdateContract () =
        let path = ArcPathHelper.LICENSEFileName
        match this.Type with
        | LicenseContentType.Fulltext ->
            Contract.createUpdate(path, DTOType.PlainText, DTO.Text this.Content)

    member this.ToDeleteContract () =
        let path = ArcPathHelper.LICENSEFileName
        let c = Contract.createDelete(path)
        c

    static member toDeleteContract (run: ArcRun) : Contract =
        run.ToDeleteContract()

    static member toCreateContract (run: ArcRun,?WithFolder) : Contract [] =
        run.ToCreateContract(?WithFolder = WithFolder)

    static member toUpdateContract (run: ArcRun) : Contract =
        run.ToUpdateContract()

    static member tryFromReadContract (c:Contract) =
        match c with
        | {Operation = READ; Path = ArcPathHelper.LICENSEFileName; DTOType = Some DTOType.PlainText; DTO = Some (DTO.Text txt)} ->
            License.initFulltext txt |> Some
        | _ -> None

    static member GetDefaultLicense () =
        License.initFulltext ARCtrl.FileSystem.DefaultLicense.dl

    member this.Copy() = License(this.Type, this.Content)

    override this.Equals other =
        match other with
        | :? License as i -> 
            this.StructurallyEquals(i)
        | _ -> false

    override this.GetHashCode() = 
        [|
            box this.Type
            box this.Content
        |]
        |> Helper.HashCodes.boxHashArray 
        |> fun x -> x :?> int

     member this.StructurallyEquals (other: License) : bool =
        [|
            this.Type = other.Type
            this.Content = other.Content
        |] |> Seq.forall (fun x -> x = true)

    /// <summary>
    /// Use this function to check if this ArcInvestigation and the input ArcInvestigation refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcInvestigation to test for reference.</param>
    member this.ReferenceEquals (other: License) = System.Object.ReferenceEquals(this,other)
