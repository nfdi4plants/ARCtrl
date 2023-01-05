namespace ISADotNet

open System.Text.Json
open System.Text.Json.Serialization
open FSharp.Reflection



/// Module containing additional Json Converters
module JsonExtensions =

    let private f2 i = 
        if i < 10 then sprintf "0%i" i
        else sprintf "%i" i 

    type System.DateTime with
        member this.ToJsonTimeString() = 
            $"{f2 this.Hour}:{f2 this.Minute}:{f2 this.Second}.{this.Millisecond}"

        member this.ToJsonDateString() = 
            $"{this.Year}-{f2 this.Month}-{f2 this.Day}"
        
        member this.ToJsonDateTimeString() = 
            $"{this.ToJsonDateString()}T{this.ToJsonTimeString()}Z"

    module Time =
    
        let fromInts hour minute = 
            let d = System.DateTime(1,1,1,hour,minute,0)
            d.ToJsonTimeString()

    module Date =
    
        let fromInts year month day = 
            let d = System.DateTime(year,month,day)
            d.ToJsonDateString()
      
    module DateTime =
    
        let fromInts year month day hour minute = 
            let d = System.DateTime(year,month,day,hour,minute,0)
            d.ToJsonDateTimeString()


    module private Serialization =

        /// Returns Some, if the input json string can be deserialized to the given type
        let tryDeserializeUnionCase (s: string) (returnType : System.Type) (opts: JsonSerializerOptions) =
            let s = if returnType = typeof<string> then sprintf "\"%s\"" s else s
            try
                JsonSerializer.Deserialize(s,returnType,opts)
                |> Some
            with
            | _ -> None


    module private Type =

        /// Returns true, if the type contains the given attribute
        let containsCustomAttribute<'T> (t : System.Type) =
            t.GetCustomAttributes(false)
            |> Seq.exists (fun a -> 
                try 
                    a :?> 'T |> ignore
                    true
                with
                | _ -> false           
            )

    module private RecordField = 

        /// Returns Some, if the union case contains the given attribute
        let tryGetCustomAttribute<'T> (f : System.Reflection.PropertyInfo) =
            f.GetCustomAttributes(false)
            |> Seq.tryPick (fun a -> 
                try 
                    a :?> 'T
                    |> Some
                with
                | _ -> None         
            ) 


    module private Case = 

        /// Returns Some, if the union case contains the given attribute
        let tryGetCustomAttribute<'T> (c : UnionCaseInfo) =
            c.GetCustomAttributes()
            |> Seq.tryPick (fun a -> 
                try 
                    a :?> 'T
                    |> Some
                with
                | _ -> None         
            ) 

        /// Gets the type associated with the union case
        let getType (c : UnionCaseInfo) =
            let fs = c.GetFields()
            if fs.Length = 1 then 
                fs.[0].PropertyType
            else 
                fs
                |> Array.map (fun f -> f.PropertyType)
                |> FSharpType.MakeTupleType

    type private TokenValue =
        | None
        | Number of float
        | String of string
        | Boolean of bool

    let private restoreEscape (s : string) = 
        s
            .Replace("\\","\\\\")
            .Replace("\n","\\n")
            .Replace("\b","\\b")
            .Replace("\f","\\f")
            .Replace("\r","\\r")
            .Replace("\t","\\t")
            .Replace("\u","\\u")
            .Replace("\"","\\\"")

    /// Serializes Json Tokens to a json string
    let private detokenizeJson (l : (JsonTokenType*TokenValue) list) = 
        let mutable elementCameBefore = false
        let rec loop s l =
        
            match l with
            | (t,v)::l when t = JsonTokenType.None ->                   loop s l            
            

            | (t,v)::l when t = JsonTokenType.StartArray && elementCameBefore ->             
                elementCameBefore <- false
                loop (s+",[") l

            | (t,v)::l when t = JsonTokenType.StartArray ->                             
                loop (s+"[") l

            | (t,v)::l when t = JsonTokenType.EndArray ->               
                elementCameBefore <- true
                loop (s+"]") l


            | (t,v)::l when t = JsonTokenType.StartObject && elementCameBefore ->            
                elementCameBefore <- false
                loop (s+",{") l
            
            | (t,v)::l when t = JsonTokenType.StartObject ->            
                loop (s+"{") l

            | (t,v)::l when t = JsonTokenType.EndObject ->          
                elementCameBefore <- true
                loop (s+"}") l


            | (t,String v)::l when t = JsonTokenType.PropertyName && elementCameBefore -> 
                elementCameBefore <- false
                loop (s + $",\"{v}\" : ") l
            | (t,String v)::l when t = JsonTokenType.PropertyName ->     
                elementCameBefore <- false
                loop (s + $"\"{v}\" : ") l

            | (t,String v)::l when t = JsonTokenType.Comment ->           loop (s) l

            | (t,String v)::l when t = JsonTokenType.String && elementCameBefore->  
                loop (s + $", \"{restoreEscape v}\"") l
            | (t,String v)::l when t = JsonTokenType.String ->  
                elementCameBefore <- true
                loop (s + $"\"{restoreEscape v}\"") l

            | (t,Boolean v)::l when (t = JsonTokenType.True || t = JsonTokenType.False)  && elementCameBefore ->      
                loop (s + sprintf ", %b" v) l
            | (t,Boolean v)::l when t = JsonTokenType.True  || t = JsonTokenType.False ->      
                elementCameBefore <- true
                loop (s + sprintf "%b" v) l

            | (t,Number v)::l when t = JsonTokenType.Number && elementCameBefore ->      
                loop (s + sprintf ", %f" v) l
            | (t,Number v)::l when t = JsonTokenType.Number ->      
                elementCameBefore <- true
                loop (s + sprintf "%f" v) l

            | [] -> s

            | (t,v)::l -> failwithf "Error while detokenizing. %O,%O is not a valid token combination"  t v

        loop "" l


    /// Converter to serialize and deserilize F# Union Cases to Json AnyOfs
    type AnyOfUnionBaseConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()


        override this.CanConvert(objectType) =       
            FSharp.Reflection.FSharpType.IsUnion objectType    
            &&
            Type.containsCustomAttribute<AnyOfAttribute> objectType       

        override __.Read(reader, t, opts) =    
            try
                let s = 
                    if reader.TokenType = JsonTokenType.String then 
                        reader.GetString() |> restoreEscape
                    elif reader.TokenType = JsonTokenType.Number then 
                        reader.GetDouble() |> string
                    else
                        let mutable l : (JsonTokenType*TokenValue) list = []
                        let mutable bracket = 0

                        if reader.TokenType = JsonTokenType.Number then 
                            l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                        elif reader.TokenType = JsonTokenType.PropertyName then 
                            l <- List.append l [reader.TokenType,reader.GetString() |> String]
                        elif reader.TokenType = JsonTokenType.False || reader.TokenType = JsonTokenType.True then 
                            l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                        elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                            bracket <- bracket + 1
                            l <- List.append l [reader.TokenType,None]
                        else l <- List.append l [reader.TokenType,None]

                        while bracket > 0 do
                            reader.Read() |> ignore

                            if reader.TokenType = JsonTokenType.Number then 
                                l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                            elif reader.TokenType = JsonTokenType.String then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.True || reader.TokenType = JsonTokenType.False then 
                                l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                            elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                                bracket <- bracket + 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.EndArray || reader.TokenType = JsonTokenType.EndObject then 
                                bracket <- bracket - 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            else l <- List.append l [reader.TokenType,None]
                        l
                        |> detokenizeJson

                FSharp.Reflection.FSharpType.GetUnionCases t
                // Sort union cases before trying to deseralize them one after one
                |> Array.sortBy (fun case ->
                    Case.tryGetCustomAttribute<SerializationOrderAttribute> case
                    |> Option.map (fun r -> r.Rank)
                    |> Option.defaultValue 0
                )
                // Returns the first union case value which could be deserialized from the input string
                |> Array.tryPick (fun case ->          
                    let caseType = Case.getType case
                    Serialization.tryDeserializeUnionCase s caseType opts
                    |> Option.map (fun value -> FSharpValue.MakeUnion(case,[|value|]) :?> 'T)   
                )
                |> function 
                    | Option.Some ds -> ds
                    | Option.None -> failwith $"Could not find a case to which the object matches: \n{s}"
                    
            with
            | err -> 
                failwith $"AnyOfUnionBaseConverterError: Could not deserialize object of type {t.Name}:\n{err.Message}"


        override __.Write(writer, value, opts) =
            let (case,value) = FSharp.Reflection.FSharpValue.GetUnionFields(value,value.GetType()) 
            if value.Length = 1 then 
                JsonSerializer.Serialize(writer,value.[0],opts)
            else 
                JsonSerializer.Serialize(writer,value,opts)

    /// Converter to serialize and deserilize F# Union Cases to Json AnyOfs
    type AnyOfUnionBaseConverter(fsOptions) =
        inherit JsonConverterFactory()
    
        override _.CanConvert(typeToConvert) =
            FSharp.Reflection.FSharpType.IsUnion typeToConvert    
            &&
            Type.containsCustomAttribute<AnyOfAttribute> typeToConvert       
        
        override _.CreateConverter(typeToConvert, _options) =
            typedefof<AnyOfUnionBaseConverter<_>>
                .MakeGenericType([|typeToConvert|])
                .GetConstructor([|typeof<JsonFSharpOptions>|])
                .Invoke([|fsOptions|])
                :?> JsonConverter
    

    
    /// Converter to serialize and deserilize F# Union Cases to Json String Enums
    type StringEnumConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()

        override this.CanConvert(objectType) =       
            FSharp.Reflection.FSharpType.IsUnion objectType    
            &&
            Type.containsCustomAttribute<StringEnumAttribute> objectType       

        override __.Read(reader, t, opts) =    
            try 
                let s = reader.GetString() |> restoreEscape
            
                FSharp.Reflection.FSharpType.GetUnionCases t
                |> Array.pick (fun case ->
                    let caseName = 
                        match Case.tryGetCustomAttribute<StringEnumValueAttribute> case with
                        | Some caseName -> caseName.Value
                        | Option.None -> case.Name
                    if s = caseName then 
                        Some (FSharpValue.MakeUnion(case,[||]) :?> 'T)
                    else
                        Option.None
                )
            with
            | err -> 
                failwith $"StringEnumConverterError: Could not deserialize object of type {t.Name}:\n{err.Message}"
              
        override __.Write(writer, value, opts) =
            let (case,value) = FSharp.Reflection.FSharpValue.GetUnionFields(value,value.GetType()) 
            let caseName = 
                match Case.tryGetCustomAttribute<StringEnumValueAttribute> case with
                | Some caseName -> caseName.Value
                | Option.None -> case.Name            
            JsonSerializer.Serialize(writer,caseName,opts)


    /// Converter to serialize and deserilize F# Union Cases to Json String Enums
    type StringEnumConverter(fsOptions) =
        inherit JsonConverterFactory()
    
        override _.CanConvert(typeToConvert) =
            FSharp.Reflection.FSharpType.IsUnion typeToConvert    
            &&
            Type.containsCustomAttribute<StringEnumAttribute> typeToConvert       
        
        override _.CreateConverter(typeToConvert, _options) =
            typedefof<StringEnumConverter<_>>
                .MakeGenericType([|typeToConvert|])
                .GetConstructor([|typeof<JsonFSharpOptions>|])
                .Invoke([|fsOptions|])
                :?> JsonConverter

    
    let private baseOptions =
        let options = 
            JsonSerializerOptions(
                IgnoreNullValues=true, 
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase, 
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )
        options.Converters.Add(AnyOfUnionBaseConverter())
        options.Converters.Add(StringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())
        options



    /// Converter to serialize and deserilize components
    type RecordConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()

        override this.CanConvert(objectType) =     
            FSharp.Reflection.FSharpType.IsRecord objectType

        override __.Read(reader, t, opts) =    
            try
                let fieldNames = 
                    FSharp.Reflection.FSharpType.GetRecordFields(t)
                    |> Array.collect (fun p -> 
                        match p |> RecordField.tryGetCustomAttribute<JsonPropertyNameAttribute> with
                        | Option.Some n -> [|n.Name|]
                        | Option.None ->   [|p.Name; p.Name.ToLower()|]
                    )
                    |> set

                let s = 
                    if reader.TokenType = JsonTokenType.String then 
                        reader.GetString() |> restoreEscape
                    elif reader.TokenType = JsonTokenType.Number then 
                        reader.GetDouble() |> string
                    else
                        let mutable l : (JsonTokenType*TokenValue) list = []
                        let mutable bracket = 0
                        let mutable objectStartCount = 0
                    
                        if reader.TokenType = JsonTokenType.Number then 
                            l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                        elif reader.TokenType = JsonTokenType.PropertyName then 
                            let token = reader.TokenType
                            let s = reader.GetString()
                            if fieldNames.Contains(s) |> not then failwithf "Could not read json object. Type %s does not contain property named %s" t.Name s
                            l <- List.append l [token, s |> String]
                        elif reader.TokenType = JsonTokenType.False || reader.TokenType = JsonTokenType.True then 
                            l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                        elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                            bracket <- bracket + 1
                            l <- List.append l [reader.TokenType,None]
                        else l <- List.append l [reader.TokenType,None]

                        while bracket > 0 do

                            reader.Read() |> ignore

                            if reader.TokenType = JsonTokenType.Number then 
                                l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                            elif reader.TokenType = JsonTokenType.String then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                let token = reader.TokenType
                                let s = reader.GetString()
                                if fieldNames.Contains(s) |> not && bracket < 2 then failwithf "Coult now read json object. Type %s does not contain property named %s" t.Name s
                                l <- List.append l [token, s |> String]
                            elif reader.TokenType = JsonTokenType.True || reader.TokenType = JsonTokenType.False then 
                                l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                            elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                                bracket <- bracket + 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.EndArray || reader.TokenType = JsonTokenType.EndObject then 
                                bracket <- bracket - 1
                                l <- List.append l [reader.TokenType,None]
                            else l <- List.append l [reader.TokenType,None]
                        l
                        |> detokenizeJson
                JsonSerializer.Deserialize<'T>(s,baseOptions)
            with
            | err -> 
                failwith $"RecordConverterError: Could not deserialize object of type {t.Name}:\n{err.Message}"

        override __.Write(writer, value, opts) =
     
            JsonSerializer.Serialize(writer,value,baseOptions)

    /// Converter to serialize and deserilize components
    type RecordConverter(fsOptions) =
        inherit JsonConverterFactory()
    
        override _.CanConvert(typeToConvert) =
            FSharp.Reflection.FSharpType.IsRecord typeToConvert  
        
        override _.CreateConverter(typeToConvert, _options) =
            typedefof<RecordConverter<_>>
                .MakeGenericType([|typeToConvert|])
                .GetConstructor([|typeof<JsonFSharpOptions>|])
                .Invoke([|fsOptions|])
                :?> JsonConverter

    
    /// Converter to serialize and deserilize components
    type ComponentConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()

        override this.CanConvert(objectType) =     
            typeof<Component> = objectType    


        override __.Read(reader, t, opts) =    
            try
                let s = 
                    if reader.TokenType = JsonTokenType.String then 
                        reader.GetString() |> restoreEscape
                    elif reader.TokenType = JsonTokenType.Number then 
                        reader.GetDouble() |> string
                    else
                        let mutable l : (JsonTokenType*TokenValue) list = []
                        let mutable bracket = 0

                        if reader.TokenType = JsonTokenType.Number then 
                            l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                        elif reader.TokenType = JsonTokenType.PropertyName then 
                            l <- List.append l [reader.TokenType,reader.GetString() |> String]
                        elif reader.TokenType = JsonTokenType.False || reader.TokenType = JsonTokenType.True then 
                            l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                        elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                            bracket <- bracket + 1
                            l <- List.append l [reader.TokenType,None]
                        else l <- List.append l [reader.TokenType,None]

                        while bracket > 0 do
                            reader.Read() |> ignore

                            if reader.TokenType = JsonTokenType.Number then 
                                l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                            elif reader.TokenType = JsonTokenType.String then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.True || reader.TokenType = JsonTokenType.False then 
                                l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                            elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                                bracket <- bracket + 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.EndArray || reader.TokenType = JsonTokenType.EndObject then 
                                bracket <- bracket - 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            else l <- List.append l [reader.TokenType,None]
                        l
                        |> detokenizeJson
                let c = JsonSerializer.Deserialize<Component>(s,baseOptions)
                let v, unit =  
                    match c.ComponentName with
                    | Some c -> Component.decomposeName c |> fun (a,b) -> Some a,b
                    | Option.None -> Option.None, Option.None
                box {c with ComponentValue = v; ComponentUnit = unit} :?> 'T
            with
            | err -> 
                failwith $"ComponentConverterError: Could not deserialize object of type {t.Name}:\n{err.Message}"

        override __.Write(writer, value, opts) =
     
            JsonSerializer.Serialize(writer,value,baseOptions)

    /// Converter to serialize and deserilize components
    type ComponentConverter(fsOptions) =
        inherit JsonConverterFactory()
    
        override _.CanConvert(typeToConvert) =
            typeof<Component> = typeToConvert   
        
        override _.CreateConverter(typeToConvert, _options) =
            typedefof<ComponentConverter<_>>
                .MakeGenericType([|typeToConvert|])
                .GetConstructor([|typeof<JsonFSharpOptions>|])
                .Invoke([|fsOptions|])
                :?> JsonConverter

    let private extendedOptions =
        let options = 
            JsonSerializerOptions(
                IgnoreNullValues=true, 
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase, 
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )
        options.Converters.Add(ComponentConverter())
        options.Converters.Add(AnyOfUnionBaseConverter())
        options.Converters.Add(StringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())
        options

    let private optionsForAnyOf =
        let options = 
            JsonSerializerOptions(
                IgnoreNullValues=true, 
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase, 
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )
        options.Converters.Add(RecordConverter())
        options.Converters.Add(ComponentConverter())
        options.Converters.Add(AnyOfUnionBaseConverter())
        options.Converters.Add(StringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())
        options

    /// Converter to serialize and deserilize F# Union Cases to Json AnyOfs
    type AnyOfUnionConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()


        override this.CanConvert(objectType) =       
            FSharp.Reflection.FSharpType.IsUnion objectType    
            &&
            Type.containsCustomAttribute<AnyOfAttribute> objectType       

        override __.Read(reader, t, opts) =    
            try
                let s = 
                    if reader.TokenType = JsonTokenType.String then 
                        let x = reader.GetString() |> restoreEscape
                        printfn "%s" x
                        x
                    elif reader.TokenType = JsonTokenType.Number then 
                        reader.GetDouble() |> string
                    else
                        let mutable l : (JsonTokenType*TokenValue) list = []
                        let mutable bracket = 0

                        if reader.TokenType = JsonTokenType.Number then 
                            l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                        elif reader.TokenType = JsonTokenType.PropertyName then 
                            l <- List.append l [reader.TokenType,reader.GetString() |> String]
                        elif reader.TokenType = JsonTokenType.False || reader.TokenType = JsonTokenType.True then 
                            l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                        elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                            bracket <- bracket + 1
                            l <- List.append l [reader.TokenType,None]
                        else l <- List.append l [reader.TokenType,None]

                        while bracket > 0 do
                            reader.Read() |> ignore

                            if reader.TokenType = JsonTokenType.Number then 
                                l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                            elif reader.TokenType = JsonTokenType.String then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            elif reader.TokenType = JsonTokenType.True || reader.TokenType = JsonTokenType.False then 
                                l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                            elif reader.TokenType = JsonTokenType.StartArray || reader.TokenType = JsonTokenType.StartObject then 
                                bracket <- bracket + 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.EndArray || reader.TokenType = JsonTokenType.EndObject then 
                                bracket <- bracket - 1
                                l <- List.append l [reader.TokenType,None]
                            elif reader.TokenType = JsonTokenType.PropertyName then 
                                l <- List.append l [reader.TokenType,reader.GetString() |> String]
                            else l <- List.append l [reader.TokenType,None]
                        l
                        |> detokenizeJson

                FSharp.Reflection.FSharpType.GetUnionCases t
                // Sort union cases before trying to deseralize them one after one
                |> Array.sortBy (fun case ->
                    Case.tryGetCustomAttribute<SerializationOrderAttribute> case
                    |> Option.map (fun r -> r.Rank)
                    |> Option.defaultValue 0
                )
                // Returns the first union case value which could be deserialized from the input string
                |> Array.tryPick (fun case ->          
                    let caseType = Case.getType case
                    Serialization.tryDeserializeUnionCase s caseType optionsForAnyOf
                    |> Option.map (fun value -> FSharpValue.MakeUnion(case,[|value|]) :?> 'T)   
                )
                |> function 
                    | Option.Some ds -> ds
                    | Option.None -> failwith $"Could not find a case to which the object matches: \n{s}"
                    
            with
            | err -> 
                failwith $"AnyOfUnionConverterError: Could not deserialize object of type {t.Name}:\n{err.Message}"  


        override __.Write(writer, value, opts) =
            let (case,value) = FSharp.Reflection.FSharpValue.GetUnionFields(value,value.GetType()) 
            if value.Length = 1 then 
                JsonSerializer.Serialize(writer,value.[0],optionsForAnyOf)
            else 
                JsonSerializer.Serialize(writer,value,optionsForAnyOf)

    /// Converter to serialize and deserilize F# Union Cases to Json AnyOfs
    type AnyOfUnionConverter(fsOptions) =
        inherit JsonConverterFactory()
    
        override _.CanConvert(typeToConvert) =
            FSharp.Reflection.FSharpType.IsUnion typeToConvert    
            &&
            Type.containsCustomAttribute<AnyOfAttribute> typeToConvert       
        
        override _.CreateConverter(typeToConvert, _options) =
            typedefof<AnyOfUnionConverter<_>>
                .MakeGenericType([|typeToConvert|])
                .GetConstructor([|typeof<JsonFSharpOptions>|])
                .Invoke([|fsOptions|])
                :?> JsonConverter

    let options =
        let options = 
            JsonSerializerOptions(
                IgnoreNullValues=true, 
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase, 
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )
        options.Converters.Add(ComponentConverter())
        options.Converters.Add(AnyOfUnionConverter())
        options.Converters.Add(StringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())
        options


    let fromString<'T> (s:string) = 
        JsonSerializer.Deserialize<'T>(s,options)

    let toString (i:'T) = 
        JsonSerializer.Serialize<'T>(i,options)

    let fromFile<'T> (path : string) = 
        System.IO.File.ReadAllText path 
        |> fromString<'T>

    let toFile (path : string) (i:'T) = 
        System.IO.File.WriteAllText(path,toString i)
