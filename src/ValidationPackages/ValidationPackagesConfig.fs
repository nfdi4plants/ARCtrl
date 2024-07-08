namespace ARCtrl.ValidationPackages

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type ValidationPackagesConfig(validation_packages, ?arc_specification) =
    
    let mutable _validation_packages : ResizeArray<ValidationPackage> = validation_packages
    let mutable _arc_specification : string option = arc_specification

    member this.ValidationPackages 
        with get() = _validation_packages
        and set(validation_packages) = _validation_packages <- validation_packages

    member this.ARCSpecification
        with get() = _arc_specification
        and set(arc_specification) = _arc_specification <- arc_specification

    static member make validation_packages arc_specification = ValidationPackagesConfig(validation_packages=validation_packages, ?arc_specification=arc_specification)

    member this.Copy() =
        ValidationPackagesConfig.make this.ValidationPackages this.ARCSpecification

    override this.Equals(obj) =
        match obj with
        | :? ValidationPackagesConfig as other_vpc -> other_vpc.ValidationPackages = this.ValidationPackages && other_vpc.ARCSpecification = this.ARCSpecification
        | _ -> false

    override this.GetHashCode() =        
        [|
            this.ValidationPackages.GetHashCode() |> box
            HashCodes.boxHashOption this.ARCSpecification
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int