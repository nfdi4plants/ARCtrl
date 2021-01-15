namespace ISADotNet

open System.Text.Json
open System.Text.Json.Serialization
open FSharp.Reflection

type AnyOfAttribute() =
    inherit System.Attribute()

type SerializationOrderAttribute(i:int) =
    inherit System.Attribute()
    member this.Rank = i

module JsonAnyOf =

    module Serialization =
        let tryDeserializeUnionCase (s: string) (returnType : System.Type) (opts: JsonSerializerOptions) =
            let s = if returnType = typeof<string> then sprintf "\"%s\"" s else s
            try
                JsonSerializer.Deserialize(s,returnType,opts)
                |> Some
            with
            | _ -> None


    module Type =

        let containsCustomAttribute<'T> (t : System.Type) =
            t.GetCustomAttributes(false)
            |> Seq.exists (fun a -> 
                try 
                    a :?> 'T |> ignore
                    true
                with
                | _ -> false           
            )


    module Case = 

        let tryGetCustomAttribute<'T> (c : UnionCaseInfo) =
            c.GetCustomAttributes()
            |> Seq.tryPick (fun a -> 
                try 
                    a :?> 'T
                    |> Some
                with
                | _ -> None    
       
            ) 

        let getType (c : UnionCaseInfo) =
            let fs = c.GetFields()
            if fs.Length = 1 then 
                fs.[0].PropertyType
            else 
                fs
                |> Array.map (fun f -> f.PropertyType)
                |> FSharpType.MakeTupleType

    type TokenValue =
        | None
        | Number of float
        | String of string
        | Boolean of bool

    let detokenizeJson (l : (JsonTokenType*TokenValue) list) = 
        let mutable elementCameBefore = false
        let rec loop s l =
        
            match l with
            | (t,v)::l when t = JsonTokenType.None ->                   loop s l
            | (t,v)::l when t = JsonTokenType.StartObject ->            loop (s+"{") l
            | (t,v)::l when t = JsonTokenType.EndObject ->              loop (s+"}") l
            | (t,v)::l when t = JsonTokenType.StartArray ->             loop (s+"[") l
            | (t,v)::l when t = JsonTokenType.EndArray ->               loop (s+"]") l

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


    /// Reihenfolge Attribut
    ///Verschwindibus Attribut
    type AnyOfUnionConverter<'T>() =
        inherit JsonConverter<'T>()

        override this.CanConvert(objectType) =       
            FSharp.Reflection.FSharpType.IsUnion objectType    
            &&
            Type.containsCustomAttribute<AnyOfAttribute> objectType       

        override __.Read(reader, t, opts) =    
        
            printfn "Reading %s" t.Name
        

            let s = 
                if reader.TokenType = JsonTokenType.String then 
                   reader.GetString() 
                else
                    let mutable l : (JsonTokenType*TokenValue) list = []

            
                    if reader.TokenType = JsonTokenType.Number then 
                        l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                    elif reader.TokenType = JsonTokenType.PropertyName then 
                        l <- List.append l [reader.TokenType,reader.GetString() |> String]
                    elif reader.TokenType = JsonTokenType.True then 
                        l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean ]
                    elif reader.TokenType = JsonTokenType.False then 
                        l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                    elif reader.TokenType = JsonTokenType.PropertyName then 
                        l <- List.append l [reader.TokenType,reader.GetString() |> String]
                    else l <- List.append l [reader.TokenType,None]
          
                    while reader.Read() do
                        if reader.TokenType = JsonTokenType.Number then 
                            l <- List.append l [reader.TokenType,reader.GetDouble() |> Number]
                        elif reader.TokenType = JsonTokenType.String then 
                            l <- List.append l [reader.TokenType,reader.GetString() |> String]
                        elif reader.TokenType = JsonTokenType.PropertyName then 
                            l <- List.append l [reader.TokenType,reader.GetString() |> String]
                        elif reader.TokenType = JsonTokenType.True then 
                            l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                        elif reader.TokenType = JsonTokenType.False then 
                            l <- List.append l [reader.TokenType,reader.GetBoolean() |> Boolean]
                        elif reader.TokenType = JsonTokenType.PropertyName then 
                            l <- List.append l [reader.TokenType,reader.GetString() |> String]
                        else l <- List.append l [reader.TokenType,None]
                    l
                    |> List.iter (fun (t,v) -> printfn "%A,%A" t v)

                    l
                    |> detokenizeJson
        
            printfn "%s" s

            FSharp.Reflection.FSharpType.GetUnionCases t
            |> Array.sortBy (fun case ->
                Case.tryGetCustomAttribute<SerializationOrderAttribute> case
                |> Option.map (fun r -> r.Rank)
                |> Option.defaultValue 0
            )
            |> Array.pick (fun case -> 
            
                let caseType = Case.getType case
                Serialization.tryDeserializeUnionCase s caseType opts
                |> Option.map (fun value -> FSharpValue.MakeUnion(case,[|value|]) :?> 'T)   
            )

        override __.Write(writer, value, opts) =
            printfn "Writing %s" (typeof<'T>).Name
            let (case,value) = FSharp.Reflection.FSharpValue.GetUnionFields(value,value.GetType()) 
            if value.Length = 1 then 
                JsonSerializer.Serialize(writer,value.[0],opts)
            else 
                JsonSerializer.Serialize(writer,value,opts)


    let options =
        let options = JsonSerializerOptions(IgnoreNullValues=true, PropertyNamingPolicy=JsonNamingPolicy.CamelCase)
        options.Converters.Add(AnyOfUnionConverter())
        options.Converters.Add(JsonStringEnumConverter())
        options.Converters.Add(JsonFSharpConverter())
        options

