#r "nuget: ARCtrl, 2.0.0-beta.1"
#r "nuget: Thoth.Json.Newtonsoft, 0.1.0"

open ARCtrl.Json.ROCrateContext

ARCtrl.Json.ROCrateContext.Assay.context_jsonvalue
ARCtrl.Json.ROCrateContext.Comment.context_jsonvalue
ARCtrl.Json.ROCrateContext.Component.context_jsonvalue
ARCtrl.Json.ROCrateContext.Factor.context_jsonvalue
ARCtrl.Json.ROCrateContext.FactorValue.context_jsonvalue
ARCtrl.Json.ROCrateContext.Investigation.context_jsonvalue
ARCtrl.Json.ROCrateContext.Material.context_jsonvalue
ARCtrl.Json.ROCrateContext.MaterialAttribute.context_jsonvalue
ARCtrl.Json.ROCrateContext.OntologyAnnotation.context_jsonvalue
ARCtrl.Json.ROCrateContext.MaterialAttributeValue.context_jsonvalue
ARCtrl.Json.ROCrateContext.OntologySourceReference.context_jsonvalue
ARCtrl.Json.ROCrateContext.Person.context_jsonvalue
ARCtrl.Json.ROCrateContext.Organization.context_jsonvalue
ARCtrl.Json.ROCrateContext.Study.context_jsonvalue
ARCtrl.Json.ROCrateContext.Process.context_jsonvalue
ARCtrl.Json.ROCrateContext.Protocol.context_jsonvalue
ARCtrl.Json.ROCrateContext.Source.context_jsonvalue
ARCtrl.Json.ROCrateContext.Sample.context_jsonvalue
ARCtrl.Json.ROCrateContext.Publication.context_jsonvalue
ARCtrl.Json.ROCrateContext.PropertyValue.context_jsonvalue
ARCtrl.Json.ROCrateContext.ProtocolParameter.context_jsonvalue
ARCtrl.Json.ROCrateContext.ProcessParameterValue.context_jsonvalue

let writeContextToFile name contextJson =
    let contextString = Thoth.Json.Newtonsoft.Encode.toString 2 contextJson
    let path = System.IO.Path.Combine(__SOURCE_DIRECTORY__, $"{name}_context.json")
    System.IO.File.WriteAllText(path, contextString)


writeContextToFile "data" Data.context_jsonvalue
writeContextToFile "assay" Assay.context_jsonvalue
writeContextToFile "comment" Comment.context_jsonvalue
writeContextToFile "component" Component.context_jsonvalue
writeContextToFile "factor" Factor.context_jsonvalue
writeContextToFile "factorValue" FactorValue.context_jsonvalue
writeContextToFile "investigation" Investigation.context_jsonvalue
writeContextToFile "material" Material.context_jsonvalue
writeContextToFile "materialAttribute" MaterialAttribute.context_jsonvalue
writeContextToFile "ontologyAnnotation" OntologyAnnotation.context_jsonvalue
writeContextToFile "materialAttributeValue" MaterialAttributeValue.context_jsonvalue
writeContextToFile "ontologySourceReference" OntologySourceReference.context_jsonvalue
writeContextToFile "person" Person.context_jsonvalue
writeContextToFile "organization" Organization.context_jsonvalue
writeContextToFile "study" Study.context_jsonvalue
writeContextToFile "process" Process.context_jsonvalue
writeContextToFile "protocol" Protocol.context_jsonvalue
writeContextToFile "source" Source.context_jsonvalue
writeContextToFile "sample" Sample.context_jsonvalue
writeContextToFile "publication" Publication.context_jsonvalue
writeContextToFile "propertyValue" PropertyValue.context_jsonvalue
writeContextToFile "protocolParameter" ProtocolParameter.context_jsonvalue
writeContextToFile "processParameterValue" ProcessParameterValue.context_jsonvalue
    
