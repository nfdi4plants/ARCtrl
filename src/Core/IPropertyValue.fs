namespace ARCtrl

open ARCtrl

type IPropertyValue =

    //static abstract member create :  OntologyAnnotation option -> Value option -> OntologyAnnotation option -> 'PropertyValue

    abstract member AlternateName : unit -> string option

    abstract member MeasurementMethod: unit -> string option

    abstract member Description: unit -> string option

    abstract member GetCategory : unit -> OntologyAnnotation option

    abstract member GetValue : unit -> Value option
    abstract member GetUnit : unit -> OntologyAnnotation option

    abstract member GetAdditionalType : unit -> string

type createPVFunction<'T> = string option -> string option -> string option -> OntologyAnnotation option -> Value option -> OntologyAnnotation option -> 'T