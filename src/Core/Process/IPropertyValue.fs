namespace ARCtrl.Process

open ARCtrl

type IPropertyValue<'PropertyValue> =

    //static abstract member create :  OntologyAnnotation option -> Value option -> OntologyAnnotation option -> 'PropertyValue

    abstract member GetCategory : unit -> OntologyAnnotation option

    abstract member GetValue : unit -> Value option

    abstract member GetUnit : unit -> OntologyAnnotation option

    abstract member GetAdditionalType : unit -> string

type createPVFunction<'T> = OntologyAnnotation option -> Value option -> OntologyAnnotation option -> 'T
