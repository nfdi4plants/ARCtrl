namespace ARCtrl.ROCrate

/// Base interface implemented by all explicitly known objects in our ROCrate profiles.
type IROCrateObject =
    abstract member SchemaType : string
    abstract member Id: string
    abstract member AdditionalType: string option