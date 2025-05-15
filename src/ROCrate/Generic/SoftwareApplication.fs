namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type SoftwareApplication(
    id: string,
    name: string,
    ?description: string,
    ?version: string,
    ?url: string,
    ?license: string,
    ?softwareRequirements: string list
) as this =
    inherit LDNode(id = id, schemaType = ResizeArray ["SoftwareApplication"])
    do
        DynObj.setProperty (nameof name) name this
        DynObj.setOptionalProperty (nameof description) description this
        DynObj.setOptionalProperty (nameof version) version this
        DynObj.setOptionalProperty (nameof url) url this
        DynObj.setOptionalProperty (nameof license) license this
        DynObj.setOptionalProperty (nameof softwareRequirements) softwareRequirements this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "SoftwareApplication" (nameof name) this
    static member getName = fun (sa: SoftwareApplication) -> sa.GetName()

    member this.GetDescription() = 
        match DynObj.tryGetPropertyValue "description" this with
        | Some desc -> Some desc
        | None -> None
    static member getDescription = fun (sa: SoftwareApplication) -> sa.GetDescription()

    member this.GetVersion() = 
        match DynObj.tryGetPropertyValue "version" this with
        | Some version -> Some version
        | None -> None
    static member getVersion = fun (sa: SoftwareApplication) -> sa.GetVersion()

    member this.GetUrl() = 
        match DynObj.tryGetPropertyValue "url" this with
        | Some url -> Some url
        | None -> None
    static member getUrl = fun (sa: SoftwareApplication) -> sa.GetUrl()

    member this.GetLicense() = 
        match DynObj.tryGetPropertyValue "license" this with
        | Some license -> Some license
        | None -> None
    static member getLicense = fun (sa: SoftwareApplication) -> sa.GetLicense()

    member this.GetSoftwareRequirements() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<string> "softwareRequirements" this with
        | Some reqs -> Some reqs
        | None -> None
    static member getSoftwareRequirements = fun (sa: SoftwareApplication) -> sa.GetSoftwareRequirements()
