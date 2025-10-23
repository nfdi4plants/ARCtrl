namespace ARCtrl

open Fable.Core
open ARCtrl.Contract

[<StringEnum>]
type LicenseContentType =
    | Fulltext

[<AttachMembers>]
type License( contentType: LicenseContentType, content: string, ?path : string) =

    let mutable _type: LicenseContentType = contentType
    let mutable _content: string = content
    let mutable _staticHash = 0
    let mutable _path =
        match path with
        | Some p -> p
        | None -> ArcPathHelper.LICENSEFileName

    member this.Type
        with get() = _type
        and set(h) = _type <- h

    member this.Content
        with get() = _content
        and set(h) = _content <- h

    member this.StaticHash
        with get() = _staticHash
        and set(h) = _staticHash <- h

    member this.Path
        with get() = _path
        and internal set(p) = _path <- p

    static member initFulltext(content: string) =
        License(LicenseContentType.Fulltext, content)

    member this.ToCreateContract () =
        match this.Type with
        | LicenseContentType.Fulltext ->
            Contract.createCreate(_path, DTOType.PlainText, DTO.Text this.Content)

    member this.ToUpdateContract () =
        match this.Type with
        | LicenseContentType.Fulltext ->
            Contract.createUpdate(_path, DTOType.PlainText, DTO.Text this.Content)

    member this.ToDeleteContract () =
        let c = Contract.createDelete(_path)
        c

    static member toDeleteContract (run: ArcRun) : Contract =
        run.ToDeleteContract()

    static member toCreateContract (run: ArcRun,?WithFolder) : Contract [] =
        run.ToCreateContract(?WithFolder = WithFolder)

    static member toUpdateContract (run: ArcRun) : Contract =
        run.ToUpdateContract()

    static member tryFromReadContract (c:Contract) =
        match c with
        | {Operation = READ; DTOType = Some DTOType.PlainText; DTO = Some (DTO.Text txt)} when
            c.Path = ArcPathHelper.LICENSEFileName || Seq.contains c.Path ArcPathHelper.alternativeLICENSEFileNames
            ->
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
