namespace ARCtrl.ValidationPackages

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type ValidationPackage(name, ?version) =
    
    let mutable _name : string = name
    let mutable _version : string option = version

    member this.Name 
        with get() = _name
        and set(name) = _name <- name

    member this.Version
        with get() = _version
        and set(version) = _version <- version

    static member make name version = ValidationPackage(name=name, ?version=version)

    member this.Copy() =
        ValidationPackage.make this.Name this.Version

    /// Pretty printer 
    override this.ToString() =
        [
            "{"
            $" Name = {this.Name}"
            if version.IsSome then $" Version = {this.Version.Value}"
            "}"
        ]
        |> String.concat System.Environment.NewLine

    override this.Equals(obj) =
        match obj with
        | :? ValidationPackage as other_vp -> other_vp.Name = this.Name && other_vp.Version = this.Version
        | _ -> false

    override this.GetHashCode() =        
        [|
            this.Name.GetHashCode() |> box
            HashCodes.boxHashOption this.Version
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int