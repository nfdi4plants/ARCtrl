namespace ISADotNet

open System.Text.Json
open System.Text.Json.Serialization
open FSharp.Reflection

/// Unions marked with this attribute will be parsed like Json Enums by the JsonSerilaizer#
///
/// Use StringEnumValueAttribute to give names to the enum values. 
type StringEnumAttribute() =
    inherit System.Attribute()

/// Used on union cases of union types marked with the StringEnumAttribute
///
/// Sets the name of the enum value
type StringEnumValueAttribute(s:string) =
    inherit System.Attribute()
    member this.Value = s

/// Unions marked witht this attribute will be parsed like Json AnyOfs by the JsonSerilaizer
///
/// Use SerializationOrderAttribute to determine the order in which the cases should be tried to deserialize.
type AnyOfAttribute() =
    inherit System.Attribute()

/// Used on union cases of union types marked with the AnyOfAttribute
///
/// As in Json AnyOfs, the case name is not given. When deserializing such a value the type has to be inferred just by parsability.
///
/// The serialization order attribute arranges the order, in which the cases should be deserialized. Cases with harder parsing criteria should be given lower numbers. E.g int < string
type SerializationOrderAttribute(i:int) =
    inherit System.Attribute()
    member this.Rank = i

/// Module containing additional Json Converters
module JsonExtensions =

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
                loop (s + sprintf ",\"%s\" : " v) l
            | (t,String v)::l when t = JsonTokenType.PropertyName ->     
                elementCameBefore <- false
                loop (s + sprintf "\"%s\" : " v) l

            | (t,String v)::l when t = JsonTokenType.Comment ->           loop (s) l

            | (t,String v)::l when t = JsonTokenType.String && elementCameBefore->  
                loop (s + sprintf ", \"%s\"" v) l
            | (t,String v)::l when t = JsonTokenType.String ->  
                elementCameBefore <- true
                loop (s + sprintf "\"%s\"" v) l

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
    type AnyOfUnionConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()

        override this.CanConvert(objectType) =       
            FSharp.Reflection.FSharpType.IsUnion objectType    
            &&
            Type.containsCustomAttribute<AnyOfAttribute> objectType       

        override __.Read(reader, t, opts) =    
        
            let s = 
                if reader.TokenType = JsonTokenType.String then 
                    reader.GetString() 
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
                    elif reader.TokenType = JsonTokenType.PropertyName then 
                        l <- List.append l [reader.TokenType,reader.GetString() |> String]
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
            |> Array.pick (fun case ->          
                let caseType = Case.getType case
                Serialization.tryDeserializeUnionCase s caseType opts
                |> Option.map (fun value -> FSharpValue.MakeUnion(case,[|value|]) :?> 'T)   
            )
                 


        override __.Write(writer, value, opts) =
            let (case,value) = FSharp.Reflection.FSharpValue.GetUnionFields(value,value.GetType()) 
            if value.Length = 1 then 
                JsonSerializer.Serialize(writer,value.[0],opts)
            else 
                JsonSerializer.Serialize(writer,value,opts)

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
    

    
    /// Converter to serialize and deserilize F# Union Cases to Json String Enums
    type StringEnumConverter<'T>(fsOptions) =
        inherit JsonConverter<'T>()

        override this.CanConvert(objectType) =       
            FSharp.Reflection.FSharpType.IsUnion objectType    
            &&
            Type.containsCustomAttribute<StringEnumAttribute> objectType       

        override __.Read(reader, t, opts) =    
        
            let s = reader.GetString()
            
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

    
    let options =
        let options = 
            JsonSerializerOptions(
                IgnoreNullValues=true, 
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase, 
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )
        options.Converters.Add(AnyOfUnionConverter())
        options.Converters.Add(StringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())
        options
