namespace ARCtrl.ValidationPackages

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type ValidationPackagesConfig(validation_packages, ?arc_specification) =
    
    let mutable _arc_specification : string option = arc_specification
    let mutable _validation_packages : ResizeArray<ValidationPackage> = validation_packages

    member this.ValidationPackages 
        with get() = _validation_packages
        and set(validation_packages) = _validation_packages <- validation_packages

    member this.ARCSpecification
        with get() = _arc_specification
        and set(arc_specification) = _arc_specification <- arc_specification

    static member make validation_packages arc_specification = ValidationPackagesConfig(validation_packages=validation_packages, ?arc_specification=arc_specification)

    member this.Copy() =
        ValidationPackagesConfig.make this.ValidationPackages this.ARCSpecification


    /// Pretty printer 
    override this.ToString() =
        [
            "{"
            if this.ARCSpecification.IsSome then $" ARCSpecification = {this.ARCSpecification.Value}"
            " ValidationPackages = ["
            this.ValidationPackages
            |> Seq.map (fun vp -> vp.ToString())
            |> String.concat $";{System.Environment.NewLine}"
            "]"
            "}"
        ]
        |> String.concat System.Environment.NewLine

    member this.StructurallyEquals (other: ValidationPackagesConfig) =
        let sort = Array.ofSeq >> Array.sortBy (fun (vp: ValidationPackage) -> vp.Name, vp.Version)
        let specs = this.ARCSpecification = other.ARCSpecification
        let packages = Seq.compare (sort this.ValidationPackages) (sort other.ValidationPackages)
        specs && packages

    member this.ReferenceEquals (other: ValidationPackagesConfig) = System.Object.ReferenceEquals(this,other)

    override this.Equals other =
        match other with
        | :? ValidationPackagesConfig as other_vp -> 
            this.StructurallyEquals(other_vp)
        | _ -> false

    override this.GetHashCode() =        
        [|
            HashCodes.boxHashOption this.ARCSpecification
            this.ValidationPackages |> HashCodes.boxHashSeq 
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int