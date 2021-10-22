namespace ISADotNet.Viz

type Schema = 

    {
        VertexColor : string
        VertexLabelColor : string
        EdgeColor : string
        EdgeLabelColor : string
    }

    static member make vc vlc ec elc = {VertexColor = vc;VertexLabelColor = vlc; EdgeColor = ec; EdgeLabelColor = elc}

    static member create (?VertexColor,?VertexLabelColor,?EdgeColor,?EdgeLabelColor) =
        let vc = Option.defaultValue "DDDDDD" VertexColor
        let vlc = Option.defaultValue "DDDDDD" VertexLabelColor
        let ec = Option.defaultValue "999999" EdgeColor
        let elc = Option.defaultValue "BBBBBB" EdgeLabelColor
        Schema.make vc vlc ec elc 

    static member DefaultGrey =

        Schema.create ()

    static member NFDIBlue =

        {
            VertexColor         = "#2D3E50"
            VertexLabelColor    = "#2D3E50"
            EdgeColor           = "#4FB3D9"
            EdgeLabelColor      = "#425162"
        }

    static member SwateGreen =

        {
            VertexColor         = "#252423"
            VertexLabelColor    = "#252423"
            EdgeColor           = "#1FC2A7"
            EdgeLabelColor      = "#2f6b82"
        }
