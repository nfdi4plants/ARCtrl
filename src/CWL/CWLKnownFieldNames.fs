namespace ARCtrl.CWL

/// Backing values for KnownFieldNames static properties.
///
/// Fable's Python backend cannot initialize non-trivial static property getters
/// on AttachMembers types and otherwise emits None. Plain references to these
/// module-level values compile consistently across .NET, JavaScript, and Python.
module internal CWLKnownFieldNames =

    let fileInstance =
        Set [
            "class"
            "type"
            "location"
            "path"
            "basename"
            "dirname"
            "nameroot"
            "nameext"
            "checksum"
            "size"
            "secondaryFiles"
            "format"
            "contents"
        ]

    let directoryInstance =
        Set [ "class"; "type"; "location"; "path"; "basename"; "listing" ]

    let direntInstance =
        Set [ "entry"; "entryname"; "writable" ]

    let inputEnumSchema =
        Set [ "type"; "symbols"; "label"; "doc"; "name" ]

    let inputRecordField =
        Set [ "name"; "type"; "doc"; "label" ]

    let inputRecordSchema =
        Set [ "type"; "fields"; "label"; "doc"; "name" ]

    let inputArraySchema =
        Set [ "type"; "items"; "label"; "doc"; "name" ]

    let schemaDefRequirementType =
        Set [ "name"; "type" ]

    let softwarePackage =
        Set [ "package"; "version"; "specs" ]

    let inputBinding =
        Set [ "loadContents"; "position"; "prefix"; "separate"; "itemSeparator"; "valueFrom"; "shellQuote" ]

    let cwlInput =
        Set [ "id"; "type"; "label"; "secondaryFiles"; "streamable"; "doc"; "format"; "loadContents"; "loadListing"; "default"; "inputBinding" ]

    let outputBinding =
        Set [ "loadContents"; "loadListing"; "glob"; "outputEval" ]

    let cwlOutput =
        Set [ "id"; "type"; "label"; "secondaryFiles"; "streamable"; "doc"; "format"; "outputBinding"; "outputSource" ]

    let dockerRequirement =
        Set [ "class"; "dockerPull"; "dockerFile"; "dockerImageId"; "dockerLoad"; "dockerImport"; "dockerOutputDirectory"; "cwltool:dockerRunOptions" ]

    let environmentDef =
        Set [ "envName"; "envValue" ]

    let loadListingRequirementValue =
        Set [ "class"; "loadListing" ]

    let workReuseRequirementValue =
        Set [ "class"; "enableReuse" ]

    let networkAccessRequirementValue =
        Set [ "class"; "networkAccess" ]

    let inplaceUpdateRequirementValue =
        Set [ "class"; "inplaceUpdate" ]

    let resourceRequirementInstance =
        Set [ "class"; "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" ]

    let inlineJavascriptRequirementValue =
        Set [ "class"; "expressionLib" ]

    let hintUnknownValue =
        Set [ "class"; "raw" ]

    let toolDescription =
        Set [
            "inputs"
            "outputs"
            "class"
            "id"
            "label"
            "doc"
            "intent"
            "requirements"
            "hints"
            "cwlVersion"
            "baseCommand"
            "arguments"
            "stdin"
            "stderr"
            "stdout"
            "successCodes"
            "temporaryFailCodes"
            "permanentFailCodes"
        ]

    let expressionToolDescription =
        Set [
            "inputs"
            "outputs"
            "class"
            "id"
            "label"
            "doc"
            "intent"
            "requirements"
            "hints"
            "cwlVersion"
            "expression"
        ]

    let operationDescription =
        Set [ "inputs"; "outputs"; "label"; "doc"; "intent"; "class"; "id"; "requirements"; "hints"; "cwlVersion" ]

    let workflowDescription =
        Set [ "inputs"; "outputs"; "label"; "doc"; "intent"; "class"; "steps"; "id"; "requirements"; "hints"; "cwlVersion" ]

    let stepInput =
        Set [ "id"; "source"; "default"; "valueFrom"; "linkMerge"; "pickValue"; "doc"; "loadContents"; "loadListing"; "label" ]

    let stepOutputParameter =
        Set [ "id" ]

    let workflowStep =
        Set [ "id"; "run"; "in"; "out"; "requirements"; "hints"; "label"; "doc"; "scatter"; "scatterMethod"; "when" ]

    let parameterReference =
        Set [ "class"; "path"; "location"; "type"; "value" ]
