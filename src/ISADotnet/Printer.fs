namespace ISADotNet

type IISAPrintable =
    abstract member Print : unit -> string

    abstract member PrintCompact : unit -> string
