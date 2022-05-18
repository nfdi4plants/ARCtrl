namespace ISADotNet

module ValueIndex = 

    let private tryInt (str:string) =
        match System.Int32.TryParse str with
        | true,int -> Some int
        | _ -> None

    let orderName = "ValueIndex"

    let createOrderComment (index : int) =
        Comment.fromString orderName (string index)

    let tryGetIndex (comments : Comment list) =
        comments 
        |> API.CommentList.tryItem orderName 
        |> Option.bind tryInt

    let tryGetParameterIndex (param : ProtocolParameter) =
        param.ParameterName 
        |> Option.bind (fun oa -> 
            oa.Comments |> Option.bind tryGetIndex
        )

    let tryGetParameterValueIndex (paramValue : ProcessParameterValue) =
        paramValue.Category 
        |> Option.bind tryGetParameterIndex

    let tryGetFactorIndex (factor : Factor) =
        factor.FactorType 
        |> Option.bind (fun oa -> 
            oa.Comments |> Option.bind tryGetIndex
        )
      
    let tryGetFactorValueIndex (factorValue : FactorValue) =
        factorValue.Category 
        |> Option.bind tryGetFactorIndex

    let tryGetCharacteristicIndex (characteristic : MaterialAttribute) =
        characteristic.CharacteristicType 
        |> Option.bind (fun oa -> 
            oa.Comments |> Option.bind tryGetIndex
        )
      
    let tryGetCharacteristicValueIndex (characteristicValue : MaterialAttributeValue) =
        characteristicValue.Category 
        |> Option.bind tryGetCharacteristicIndex


    
[<AutoOpen>]
module ValueIndexExtensions = 
    
    type Factor with
        
        /// Create a ISAJson Factor from ISATab string entries
        static member fromStringWithValueIndex (name:string) (term:string) (source:string) (accession:string) valueIndex =
            Factor.fromStringWithComments name term source accession [ValueIndex.createOrderComment valueIndex]

        /// Create a ISAJson Factor from ISATab string entries
        static member fromStringWithNumberValueIndex (name:string) (term:string) (source:string) (accession:string) valueIndex =
            Factor.fromStringWithNumberAndComments name term source accession [ValueIndex.createOrderComment valueIndex]

        static member getValueIndex(f) = ValueIndex.tryGetFactorIndex f |> Option.get

        member this.GetValueIndex() = ValueIndex.tryGetFactorIndex this |> Option.get

    type FactorValue with

        static member getValueIndex(f) = ValueIndex.tryGetFactorValueIndex f |> Option.get

        member this.GetValueIndex() = ValueIndex.tryGetFactorValueIndex this |> Option.get

    type MaterialAttribute with
    
        /// Create a ISAJson characteristic from ISATab string entries
        static member fromStringWithValueIndex (term:string) (source:string) (accession:string) valueIndex =
            MaterialAttribute.fromStringWithComments term source accession [ValueIndex.createOrderComment valueIndex]

        /// Create a ISAJson characteristic from ISATab string entries
        static member fromStringWithNumberValueIndex (term:string) (source:string) (accession:string) valueIndex =
            MaterialAttribute.fromStringWithNumberAndComments term source accession [ValueIndex.createOrderComment valueIndex]

        static member getValueIndex(m) = ValueIndex.tryGetCharacteristicIndex m |> Option.get

        member this.GetValueIndex() = ValueIndex.tryGetCharacteristicIndex this |> Option.get

    type MaterialAttributeValue with
            
            static member getValueIndex(m) = ValueIndex.tryGetCharacteristicValueIndex m |> Option.get

            member this.GetValueIndex() = ValueIndex.tryGetCharacteristicValueIndex this |> Option.get

    type ProtocolParameter with
    
        /// Create a ISAJson parameter from ISATab string entries
        static member fromStringWithValueIndex (term:string) (source:string) (accession:string) valueIndex =
            ProtocolParameter.fromStringWithComments term source accession [ValueIndex.createOrderComment valueIndex]

        /// Create a ISAJson parameter from ISATab string entries
        static member fromStringWithNumberValueIndex (term:string) (source:string) (accession:string) valueIndex =
            ProtocolParameter.fromStringWithNumberAndComments term source accession [ValueIndex.createOrderComment valueIndex]

        static member getValueIndex(p) = ValueIndex.tryGetParameterIndex p |> Option.get

        member this.GetValueIndex() = ValueIndex.tryGetParameterIndex this |> Option.get

    type ProcessParameterValue with
    
        static member getValueIndex(p) = ValueIndex.tryGetParameterValueIndex p |> Option.get

        member this.GetValueIndex() = ValueIndex.tryGetParameterValueIndex this |> Option.get
