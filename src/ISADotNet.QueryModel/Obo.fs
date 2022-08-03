namespace ISADotNet.QueryModel

open ISADotNet

/// Functions for parsing and querying an OBO ontology
module Obo =

    let trimComment (line : string) = 
        line.Split('!').[0].Trim()

    open System

    //Dbxref definitions take the following form:

    //<dbxref name> {optional-trailing-modifier}

    //or

    //<dbxref name> "<dbxref description>" {optional-trailing-modifier}

    //The dbxref is a colon separated key-value pair. The key should be taken from GO.xrf_abbs but this is not a requirement. 
    //If provided, the dbxref description is a string of zero or more characters describing the dbxref. 
    //DBXref descriptions are rarely used and as of obof1.4 are discouraged.

    //Dbxref lists are used when a tag value must contain several dbxrefs. Dbxref lists take the following form:

    //[<dbxref definition>, <dbxref definition>, ...]

    //The brackets may contain zero or more comma separated dbxref definitions. An example of a dbxref list can be seen in the GO def for "ribonuclease MRP complex":

    //def: "A ribonucleoprotein complex that contains an RNA molecule of the snoRNA family, and cleaves the rRNA precursor as part of rRNA transcript processing. It also has other roles: In S. cerevisiae it is involved in cell cycle-regulated degradation of daughter cell-specific mRNAs, while in mammalian cells it also enters the mitochondria and processes RNAs to create RNA primers for DNA replication." [GOC:sgd_curators, PMID:10690410, Add to Citavi project by Pubmed ID PMID:14729943, Add to Citavi project by Pubmed ID PMID:7510714] Add to Citavi project by Pubmed ID

    //Note that the trailing modifiers (like all trailing modifiers) do not need to be decoded or round-tripped by parsers; trailing modifiers can always be optionally ignored. However, all parsers must be able to gracefully ignore trailing modifiers. It is important to recognize that lines which accept a dbxref list may have a trailing modifier for each dbxref in the list, and another trailing modifier for the line itself.
    type DBXref = {
        Name        : string
        Description : string
        Modifiers   : string
    }

    let private xrefRegex = 
        System.Text.RegularExpressions.Regex("""(?<xrefName>^([^"{])*)(\s?)(?<xrefDescription>\"(.*?)\")?(\s?)(?<xrefModifiers>\{(.*?)}$)?""")

    let parseDBXref (v:string) =
        let matches = xrefRegex.Match(v.Trim()).Groups

        {
            Name = matches.Item("xrefName").Value
            Description = matches.Item("xrefDescription").Value
            Modifiers = matches.Item("xrefModifiers").Value
        }

    //The value consists of a quote enclosed synonym text, a scope identifier, an optional synonym type name, and an optional dbxref list, like this:
    //synonym: "The other white meat" EXACT MARKETING_SLOGAN [MEAT:00324, BACONBASE:03021]

    //The synonym scope may be one of four values: EXACT, BROAD, NARROW, RELATED. If the first form is used to specify a synonym, the scope is assumed to be RELATED.

    //The synonym type must be the id of a synonym type defined by a synonymtypedef line in the header. If the synonym type has a default scope, that scope is used regardless of any scope declaration given by a synonym tag.

    //The dbxref list is formatted as specified in dbxref formatting. A term may have any number of synonyms.
    type TermSynonymScope =
        | Exact   
        | Broad   
        | Narrow  
        | Related 

        static member ofString (line:int) (s:string) = 
            match s with
            | "EXACT"   -> Exact
            | "BROAD"   -> Broad
            | "NARROW"  -> Narrow
            | "RELATED" -> Related
            | _         ->  printfn "[WARNING@L %i]unable to recognize %s as synonym scope" line s
                            Related

    let private synonymRegex = 
        System.Text.RegularExpressions.Regex("""(?<synonymText>^\"(.*?)\"){1}(\s?)(?<synonymScope>(EXACT|BROAD|NARROW|RELATED))?(\s?)(?<synonymDescription>\w*)(\s?)(?<dbxreflist>\[(.*?)\])?""")

    type TermSynonym = {
        Text        : string
        Scope       : TermSynonymScope
        TypeName    : string
        DBXrefs     : DBXref list
    }

    let parseSynonym (scopeFromDeprecatedTag:TermSynonymScope option) (line:int) (v:string) =
        let matches = synonymRegex.Match(v.Trim()).Groups
        {
            Text = matches.Item("synonymText").Value
            Scope =
                match scopeFromDeprecatedTag with
                |Some scope -> scope
                |_ ->   matches.Item("synonymScope").Value
                        |> TermSynonymScope.ofString line
            TypeName = matches.Item("synonymDescription").Value
            DBXrefs =
                let tmp = matches.Item("dbxreflist").Value
                match tmp.Replace("[","").Replace("]","") with
                | "" -> []
                | dbxrefs ->
                    dbxrefs.Split(',')
                    |> Array.map parseDBXref
                    |> Array.toList
        }

    /// Models the entities in an Obo Ontology
    type OboTerm = 
        {

        ///The unique id of the current term. 
        //Cardinality: exactly one.
        Id : string 
        
        ///The term name. 
        //Any term may have only zero or one name defined. 
        //Cardinality: zero or one - If multiple term names are defined, it is a parse error. In 1.2 name was required. This has been relaxed in 1.4. This helps with OWL interoperability, as labels are optional in OWL
        Name : string 
        
        ///Whether or not the current object has an anonymous id. 
        //Cardinality: zero or one. The semantics are the same as B-Nodes in RDF.
        IsAnonymous: bool
        
        ///Defines an alternate id for this term. 
        //Cardinality: any. A term may have any number of alternate ids.
        AltIds : string list
        
        ///The definition of the current term. 
        //Cardinality: zero or one. More than one definition for a term generates a parse error. The value of this tag should be the quote enclosed definition text, followed by a dbxref list containing dbxrefs that describe the origin of this definition (see dbxref formatting for information on how dbxref lists are encoded). An example of this tag would look like this:
        //definition: "The breakdown into simpler components of (+)-camphor, a bicyclic monoterpene ketone." [UM-BBD:pathway "", http://umbbd.ahc.umn.edu/cam/cam_map.html ""]
        Definition : string 
        
        ///A comment for this term. 
        //Cardinality: zero or one. There must be zero or one instances of this tag per term description. More than one comment for a term generates a parse error.
        Comment : string
        
        ///This tag indicates a term subset to which this term belongs. 
        //The value of this tag must be a subset name as defined in a subsetdef tag in the file header. 
        //If the value of this tag is not mentioned in a subsetdef tag, a parse error will be generated. 
        //Cardinality: any. A term may belong to any number of subsets.
        Subsets : string list

        ///This tag gives a synonym for this term, some xrefs to describe the origins of the synonym, and may indicate a synonym category or scope information. 
        //Cardinality: any.
        //A term may have any number of synonyms.
        Synonyms : TermSynonym list

        ///Cross references that describe analagous terms in another vocabularies. 
        //Cardinality: any. A term may have any number of xrefs.
        Xrefs : DBXref list

        //Describes a subclassing relationship between one term and another. The value is the id of the term of which this term is a subclass. 
        //A term may have any number of is_a relationships. This is equivalent to a SubClassOf axiom in OWL. Cardinality: any.
        //Parsers which support trailing modifiers may optionally parse the following trailing modifier tags for is_a:
        IsA  : string list // new

        //namespace <any namespace id>
        //derived true OR false

        //The namespace modifier allows the is_a relationship to be assigned its own namespace (independent of the namespace of the superclass or subclass of this is_a relationship).

        //The derived modifier indicates that the is_a relationship was not explicitly defined by a human ontology designer, but was created automatically by a reasoner, and could be re-derived using the non-derived relationships in the ontology.

        //This tag previously supported the completes trailing modifier. This modifier is now deprecated. Use the intersection_of tag instead.

        //Cardinality: EITHER zero OR two or more. This tag indicates that this term is equivalent to the intersection of several other terms. The value is either a term id, or a relationship type id, a space, and a term id. For example:
        //id: GO:0000085
        //name: G2 phase of mitotic cell cycle
        //intersection_of: GO:0051319 ! G2 phase
        //intersection_of: part_of GO:0000278 ! mitotic cell cycle

        //This means that GO:0000085 is equivalent to any term that is both a subtype of 'G2 phase' and has a part_of relationship to 'mitotic cell cycle' (i.e. the G2 phase of the mitotic cell cycle). Note that whilst relationship tags specify necessary conditions, intersection_of tags specify necessary and sufficient conditions.

        //A collection of intersection_of tags appearing in a term is also known as a cross-product definition (this is the same as what OWL users know as a defined class, employing intersectionOf constructs).

        //It is strongly recommended that all intersection_of tags follow a genus-differentia pattern. In this pattern, one of the tags is directly to a term id (the genus) and the other tags are relation term pairs. For example:

        //[Term]
        //id: GO:0045495 name: pole plasm
        //intersection_of: GO:0005737 ! cytoplasm
        //intersection_of: part_of CL:0000023 ! oocyte

        //These definitions can be read as sentences, such as a pole plasm is equivalent to a cytoplasm that is part_of an oocyte

        //If any intersection_of tags are specified for a term, at least two intersection_of tags need to be present or it is a parse error. The full intersection for the term is the set of all ids specified by all intersection_of tags for that term.

        //As of OBO 1.4, this tag may be applied in Typedef stanzas
        IntersectionOf : string list

        ///indicates that this term represents the union of several other terms. 
        //Cardinality: EITHER zero OR two or more.
        //The value is the id of one of the other terms of which this term is a union.
        //If any union_of tags are specified for a term, at least 2 union_of tags need to be present or it is a parse error. The full union for the term is the set of all ids specified by all union_of tags for that term.

        //This tag may not be applied to relationship types.

        //Parsers which support trailing modifiers may optionally parse the following trailing modifier tag for disjoint_from:

        //namespace <any namespace id>
        UnionOf: string list

        ///indicates that a term is disjoint from another, meaning that the two terms have no instances or subclasses in common. 
        //The value is the id of the term from which the current term is disjoint. This tag may not be applied to relationship types.
        //Cardinality: any.
        //Parsers which support trailing modifiers may optionally parse the following trailing modifier tag for disjoint_from:

        //namespace <any namespace id>
        //derived true OR false

        //The namespace modifier allows the disjoint_from relationship to be assigned its own namespace.

        //The derived modifier indicates that the disjoint_from relationship was not explicitly defined by a human ontology designer, but was created automatically by a reasoner, and could be re-derived using the non-derived relationships in the ontology.
        DisjointFrom : string list

        ///describes a typed relationship between this term and another term or terms. 
        //relationship
        //Cardinality: any.
        ///The value of this tag should be the relationship type id, and then the id of the target term, plus, optionally, other target terms. The relationship type 
        //name must be a relationship type name as defined in a typedef tag stanza. The [Typedef] must either occur in a document in the current parse batch, or in a 
        //file imported via an import header tag. If the relationship type name is undefined, a parse error will be generated. If the id of the target term cannot be 
        //resolved by the end of parsing the current batch of files, this tag describes a "dangling reference"; see the parser requirements section for information 
        //about how a parser may handle dangling references. If a relationship is specified for a term with an is_obsolete value of true, a parse error will be generated.
        //Parsers which support trailing modifiers may optionally parse the following trailing modifier tags for relationships:

        //namespace <any namespace id>
        //inferred true OR false
        //cardinality any non-negative integer
        //maxCardinality any non-negative integer
        //minCardinality any non-negative integer

        //The namespace modifier allows the relationship to be assigned its own namespace (independant of the namespace of the parent, child, or type of the relationship).

        //The inferred modifier indicates that the relationship was not explicitly defined by a human ontology designer, but was created automatically by a reasoner, and could be re-derived using the non-derived relationships in the ontology.

        //Cardinality qualifiers can be used to specify constraints on the number of relations of the specified type any given instance can have. For example, in the stanza declaring a id: SO:0000634 ! polycistronic mRNA, we can say: relationship: has_part SO:0000316 {minCardinality=2} ! CDS which means that every instance of a transcript of this type has two or more CDS features such that they stand in a has_part relationship from the transcript.

        //The semantics of a relationship tag is by default "all-some". Formally, in OWL this corresponds to an existential restriction - see the OWL section.
        Relationships : string list

        ///Whether or not this term is obsolete. 
        //Cardinality: zero or one. 
        //Allowable values are "true" and "false" (false is assumed if this tag is not present). Obsolete terms must have no relationships, and no defined is_a, inverse_of, disjoint_from, union_of, or intersection_of tags.
        IsObsolete : bool


        ///Gives a term which replaces an obsolete term. The value is the id of the replacement term. 
        //The value of this tag can safely be used to automatically reassign instances whose instance_of property points to an obsolete term.
        //Cardinality: any. 
        //The replaced_by tag may only be specified for obsolete terms. A single obsolete term may have more than one replaced_by tag. This tag can be used in conjunction with the consider tag.
        Replacedby : string list
        
        //A term which may be an appropriate substitute for an obsolete term, but needs to be looked at carefully by a human expert before the replacement is done.
        //Cardinality: any. 
        //This tag may only be specified for obsolete terms. A single obsolete term may have many consider tags. This tag can be used in conjunction with replaced_by.
        Consider : string list

        PropertyValues : string list

        ///Whether or not this term or relation is built in to the OBO format. 
        //Allowable values are "true" and "false" (false assumed as default). Rarely used. One example of where this is used is the OBO relations ontology, which provides a stanza for the is_a relation, even though this relation is axiomatic to the language.
        //builtin
        //Cardinality: zero or one. 
        BuiltIn : bool 


        ///Name of the creator of the term. May be a short username, initials or ID. 
        //Cardinality: zero or one. 
        //Example: dph
        //Note that although this tag is defined in obof1.4, it can be used in obof1.2 harmlessly
        CreatedBy : string


        ///Date of creation of the term specified in ISO 8601 format. Example: 2009-04-13T01:32:36Z
        //Cardinality: zero or one. 
        //Note that although this tag is defined in obof1.4, it can be used in obof1.2 harmlessly
        CreationDate : string

        }

        /// Create an Obo Term from its field values
        static member make id name isAnonymous altIds definition comment subsets synonyms xrefs isA         
            intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider propertyValues builtIn    
            createdBy creationDate = {  

            Id              = id          
            Name            = name        
            IsAnonymous     = isAnonymous 
            AltIds          = altIds      
            Definition      = definition  
            Comment         = comment     
            Subsets         = subsets     
            Synonyms        = synonyms    
            Xrefs           = xrefs     
            IsA             = isA         
            IntersectionOf  = intersectionOf
            UnionOf         = unionOf       
            DisjointFrom    = disjointFrom  
            Relationships   = relationships  
            IsObsolete      = isObsolete    
            Replacedby      = replacedby    
            Consider        = consider      
            PropertyValues  = propertyValues
            BuiltIn         = builtIn       
            CreatedBy       = createdBy     
            CreationDate    = creationDate  
            
        }

        /// Create an Obo Term from its field values
        static member Create (id,?Name,?IsAnonymous,?AltIds,?Definition,?Comment,?Subsets,?Synonyms,?Xrefs,?IsA,         
            ?IntersectionOf,?UnionOf,?DisjointFrom,?Relationships,?IsObsolete,?Replacedby,?Consider,?PropertyValues,?BuiltIn,
            ?CreatedBy,?CreationDate) =

            {              
                    Id              = id          
                    Name            = Option.defaultValue "" Name        
                    IsAnonymous     = Option.defaultValue false IsAnonymous 
                    AltIds          = Option.defaultValue [] AltIds      
                    Definition      = Option.defaultValue "" Definition  
                    Comment         = Option.defaultValue "" Comment     
                    Subsets         = Option.defaultValue [] Subsets     
                    Synonyms        = Option.defaultValue [] Synonyms    
                    Xrefs           = Option.defaultValue [] Xrefs     
                    IsA             = Option.defaultValue [] IsA         
                    IntersectionOf  = Option.defaultValue [] IntersectionOf
                    UnionOf         = Option.defaultValue [] UnionOf       
                    DisjointFrom    = Option.defaultValue [] DisjointFrom  
                    Relationships   = Option.defaultValue [] Relationships  
                    IsObsolete      = Option.defaultValue false IsObsolete    
                    Replacedby      = Option.defaultValue [] Replacedby    
                    Consider        = Option.defaultValue [] Consider      
                    PropertyValues  = Option.defaultValue [] PropertyValues
                    BuiltIn         = Option.defaultValue false BuiltIn       
                    CreatedBy       = Option.defaultValue "" CreatedBy     
                    CreationDate    = Option.defaultValue "" CreationDate  
                    
                }

        /// Read an Obo Term from lines in "key:value" style
        static member fromLines verbose (en:Collections.Generic.IEnumerator<string>) lineNumber 
            id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
            intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
            propertyValues builtIn createdBy creationDate =   

            if en.MoveNext() then                
                let split = (en.Current |> trimComment).Split([|": "|], System.StringSplitOptions.None)
                match split.[0] with
                | "id"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTerm.fromLines verbose  en (lineNumber + 1)
                        v name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate            
        
                | "name"            -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id v isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "is_anonymous"    ->
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name true altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "alt_id"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous (v::altIds) definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "def"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds v comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "comment"             -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition v subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "subset"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment (v::subsets) synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | synonymTag when synonymTag.Contains("synonym")              -> 
                    let scope =
                        match synonymTag with
                        | "exact_synonym"   -> Some Exact
                        | "narrow_synonym"  -> Some Narrow
                        | "broad_synonym"   -> Some Broad
                        | _                 -> None
                    let v = parseSynonym scope lineNumber (split.[1..] |> String.concat ": ")
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets (v::synonyms) xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "xref" | "xref_analog" | "xref_unk" -> 
                    let v = (split.[1..] |> String.concat ": ") |> parseDBXref
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms (v::xrefs) isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "is_a"              -> 
                    let v = (split.[1..] |> String.concat ": ")
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs (v::isA)
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "intersection_of"              -> 
                    let v = (split.[1..] |> String.concat ": ")
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        (v::intersectionOf) unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "union_of"              -> 
                    let v = (split.[1..] |> String.concat ": ")
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf (v::unionOf) disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate
                    
                | "disjoint_from"              -> 
                    let v = (split.[1..] |> String.concat ": ")
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf (v::disjointFrom) relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate
                    
                | "relationship"              -> 
                    let v = (split.[1..] |> String.concat ": ")
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom (v::relationships) isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate

                | "is_obsolete"             -> 
                    let v = ((split.[1..] |> String.concat ": ").Trim()) 
                    let v' = v = "true"

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships v' replacedby consider 
                        propertyValues builtIn createdBy creationDate            

                | "replaced_by"             -> 
                    let v = (split.[1..] |> String.concat ": ")

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete (v::replacedby) consider 
                        propertyValues builtIn createdBy creationDate


                | "consider" | "use_term"            -> 
                    let v = (split.[1..] |> String.concat ": ")

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby (v::consider)
                        propertyValues builtIn createdBy creationDate


                | "builtin"             -> 
                    let v = ((split.[1..] |> String.concat ": ").Trim()) 
                    let v' = v = "true"

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues v' createdBy creationDate

                | "property_value"             -> 
                    let v = (split.[1..] |> String.concat ": ")

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        (v::propertyValues) builtIn createdBy creationDate

                | "created_by"             -> 
                    let v = (split.[1..] |> String.concat ": ")

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn v creationDate


                | "creation_date"             -> 
                    let v = (split.[1..] |> String.concat ": ")

                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy v

                | "" -> 
                    lineNumber,
                    OboTerm.make id name isAnonymous 
                        (altIds |> List.rev) 
                        definition comment 
                        (subsets        |> List.rev)
                        (synonyms       |> List.rev)
                        (xrefs          |> List.rev)
                        (isA            |> List.rev)
                        (intersectionOf |> List.rev)
                        (unionOf        |> List.rev)
                        (disjointFrom   |> List.rev)
                        (relationships  |> List.rev)
                        isObsolete 
                        (replacedby     |> List.rev)
                        (consider       |> List.rev)
                        (propertyValues |> List.rev)
                        builtIn 
                        createdBy creationDate

                | unknownTag -> 
                    if verbose then printfn "[WARNING@L %i]: Found term tag <%s> that does not fit OBO flat file specifications 1.4. Skipping it..." lineNumber unknownTag
                    OboTerm.fromLines verbose en (lineNumber + 1)
                        id name isAnonymous altIds definition comment subsets synonyms xrefs isA 
                        intersectionOf unionOf disjointFrom relationships isObsolete replacedby consider 
                        propertyValues builtIn createdBy creationDate
                                   
            else
                // Maybe check if id is empty
                lineNumber,
                OboTerm.make id name isAnonymous 
                    (altIds |> List.rev) 
                    definition comment 
                    (subsets        |> List.rev)
                    (synonyms       |> List.rev)
                    (xrefs          |> List.rev)
                    (isA            |> List.rev)
                    (intersectionOf |> List.rev)
                    (unionOf        |> List.rev)
                    (disjointFrom   |> List.rev)
                    (relationships  |> List.rev)
                    isObsolete 
                    (replacedby     |> List.rev)
                    (consider       |> List.rev)
                    (propertyValues |> List.rev)
                    builtIn 
                    createdBy creationDate
                //failwithf "Unexcpected end of file."

        /// Write an Obo Term to lines in "key:value" style
        static member toLines (term : OboTerm) =
            seq {
                yield "id: " + term.Id
                if term.IsAnonymous then yield "is_anonymous"
                yield "name: " + term.Name
                for altid in term.AltIds do yield $"alt_id: {altid}"
                if term.Definition = "" |> not then yield $"def: {term.Definition}"
                for comment in term.Comment do yield $"comment: {comment}"
                for subset in term.Subsets do yield $"subset: {subset}"
                for synonym in term.Synonyms do yield $"synonym: {synonym}"
                for xref in term.Xrefs do yield $"xref: {xref}"
                if term.BuiltIn then yield "builtin"
                for property_value in term.PropertyValues do yield $"property_value: {property_value}"
                for is_a in term.IsA do yield $"is_a: {is_a}"
                for intersection in term.IntersectionOf do yield $"intersection_of: {intersection}"
                for union in term.UnionOf do yield $"union_of: {union}"
                //for equivalent in term.equ do yield $"equivalent_to: {equivalent}"
                for disjoint in term.DisjointFrom do yield $"disjoint_from: {disjoint}"
                for relationship in term.Relationships do yield $"relationship: {relationship}"
                for created_by in term.CreatedBy do yield $"created_by: {created_by}"
                if term.CreationDate = "" |> not then yield $"creation_date: {term.CreationDate}"
                if term.IsObsolete then yield "is_obsolete"
                for replaced_by in term.Replacedby do yield $"replaced_by: {replaced_by}"
                for consider in term.Consider do yield $"consider: {consider}"
            }
         
        /// Translates a OBO `term` into an ISADotNet `OntologyAnnotation`
        static member toOntologyAnnotation (term : OboTerm) =
            let ref,num = ISADotNet.OntologyAnnotation.splitAnnotation term.Id
            ISADotNet.OntologyAnnotation.fromString term.Name ref num

        /// Translates an ISADotNet `OntologyAnnotation` into a OBO `term`
        static member ofOntologyAnnotation (term : ISADotNet.OntologyAnnotation) =
            OboTerm.Create(term.ShortAnnotationString,term.NameText)

    /// Models the relationship between OBO Terms 
    type OboTypeDef = 
        {
            ///The unique id of the current term. 
            //Cardinality: exactly one.
            Id : string                        
            // The id of a term, or a special reserved identifier, which indicates the domain for this relationship type. 
            // If a property P has domain D, then any term T that has a relationship of type P to another term is a subclass of D.
            // Note that this does not mean that the domain restricts which classes of terms can have a relationship of type P to another term. 
            // Rather, it means that any term that has a relationship of type P to another term is by definition a subclass of D. 
            // Cardinality: zero or one. If the intent is to declare a disjunctive domain, then a new class must be declared and defined using the union_of construct.
            Domain : string
            // The id of a term, or a special reserved identifier, which indicates acceptable range for this relationship type. 
            // If a property P has range R, then any term T that is the target of a relationship of type P is a subclass of R. 
            // Note that this does not mean that the range restricts which classes of terms can be the target of relationships of type P. 
            // Rather, it means that any term that is the target of a relationship of type P is by definition a subclass of R. 
            // Cardinality: zero or one. If the intent is to declare a disjunctive range, then a new class must be declared and defined using the union_of construct.
            Range : string 
            ///The term name. 
            //Any term may have only zero or one name defined. 
            //Cardinality: zero or one - If multiple term names are defined, it is a parse error. In 1.2 name was required. This has been relaxed in 1.4. This helps with OWL interoperability, as labels are optional in OWL
            Name : string 
            // The id of another relationship type that is the inverse of this relationship type.
            // If relation A is the inverse_of type B, and X has relationship A to Y, then it is implied that Y has relation B to X. 
            // In obof1.2 the semantics of inverse_of were unclear, as obof1.2 unofficially allowed type-level relations. 
            // In obof1.4, the semantics are identical to OWL. Cardinality: any.
            Inverse_of : string list
            // The id of another relationship type that this relationship type is transitive over. 
            // If P is transitive over Q, and the ontology has X P Y and Y Q Z then it follows that X P Z (term/type level). 
            // Equivalent to property chains in OWL2. Cardinality: any.
            Transitive_over : string list
            // Whether or not a cycle can be made from this relationship type. 
            // If a relationship type is non-cyclic, it is illegal for an ontology to contain a cycle made from user-defined or implied relationships of this type. 
            // Allowed values: true or false. Cardinality: zero or one.
            Is_cyclic : bool
            // Whether this relationship is reflexive. All reflexive relationships are also cyclic. 
            // Allowed values: true or false. Term/type level. Cardinality: zero or one.
            Is_reflexive : bool
            // Whether this relationship is symmetric. All symmetric relationships are also cyclic. 
            // Allowed values: true or false. Term/type level. Cardinality: zero or one.
            Is_symmetric : bool
            // Whether this relationship is anti-symmetric. 
            // Allowed values: true or false. Term/type level. Cardinality: zero or one.
            Is_anti_symmetric : bool
            // Whether this relationship is transitive.
            // Allowed values: true or false. Term/type level. Cardinality: zero or one.
            Is_transitive : bool
            // Whether this relationship is a metadata tag. 
            // Properties that are marked as metadata tags are used to record object metadata. 
            // Object metadata is additional information about an object that is useful to track, 
            // but does not impact the definition of the object or how it should be treated by a reasoner. 
            // Metadata tags might be used to record special term synonyms or structured notes about a term, for example. 
            // Cardinality: zero or one.
            Is_metadata_tag : bool
            // Whether this relation is a class-level relation. 
            // In OBO-Format, all relationship tags are taken by default to mean an all-some relationship over an instance level relation.
            // This tag is used for other cases, e.g. lacks_part. In OWL this is translated to a hasValue restriction. 
            // Cardinality: zero or one.
            Is_class_level : bool
        }
        
        /// Create an Obo Type Def from its field values
        static member make id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
            is_anti_symmetric is_transitive is_metadata_tag is_class_level =

            {              
                    Id                  = id
                    Domain              = domain          
                    Range               = range 
                    Name                = name
                    Inverse_of          = inverse_of 
                    Transitive_over     = transitive_over      
                    Is_cyclic           = is_cyclic  
                    Is_reflexive        = is_reflexive  
                    Is_symmetric        = is_symmetric     
                    Is_anti_symmetric   = is_anti_symmetric     
                    Is_transitive       = is_transitive    
                    Is_metadata_tag     = is_metadata_tag     
                    Is_class_level      = is_class_level         
            }

        /// Create an Obo Type Def from its field values
        static member Create (id,domain,range,?Name,?Inverse_of,?Transitive_over,?Is_cyclic,?Is_reflexive,?Is_symmetric,
            ?Is_anti_symmetric,?Is_transitive,?Is_metadata_tag,?Is_class_level) =

            {              
                    Id                  = id
                    Domain              = domain          
                    Range               = range   
                    Name                = Option.defaultValue "" Name
                    Inverse_of          = Option.defaultValue [] Inverse_of 
                    Transitive_over     = Option.defaultValue [] Transitive_over      
                    Is_cyclic           = Option.defaultValue false Is_cyclic  
                    Is_reflexive        = Option.defaultValue false Is_reflexive  
                    Is_symmetric        = Option.defaultValue false Is_symmetric     
                    Is_anti_symmetric   = Option.defaultValue false Is_anti_symmetric     
                    Is_transitive       = Option.defaultValue false Is_transitive    
                    Is_metadata_tag     = Option.defaultValue false Is_metadata_tag     
                    Is_class_level      = Option.defaultValue false Is_class_level         
                }

        /// Read an Obo Type Def from lines in "key:value" style
        static member fromLines verbose (en:Collections.Generic.IEnumerator<string>) lineNumber 
            id domain range name (inverse_of:string list) transitive_over is_cyclic is_reflexive is_symmetric 
            is_anti_symmetric is_transitive is_metadata_tag is_class_level =   

            if en.MoveNext() then                
                let split = (en.Current |> trimComment).Split([|": "|], System.StringSplitOptions.None)
                match split.[0] with
                | "id"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        v domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level  

                | "domain"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id v range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level           
        
                | "range"            -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain v name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level
    
                | "name"            -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range v inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | "inverse_of"    ->
                    let v = split.[1..] |> String.concat ": "
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name (v::inverse_of) transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | "transitive_over"              -> 
                    let v = split.[1..] |> String.concat ": "
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of (v::transitive_over) is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | "is_cyclic"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over true is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | "is_reflexive"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic true is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | "is_symmetric"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic is_reflexive true 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | "is_anti_symmetric"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        true is_transitive is_metadata_tag is_class_level

                | "is_transitive"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric true is_metadata_tag is_class_level

                | "is_metadata_tag"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive true is_class_level

                | "is_class_level"              -> 
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag true


                | "" -> 
                    lineNumber,
                    OboTypeDef.make id domain range name
                        (inverse_of |> List.rev)
                        (transitive_over |> List.rev)
                        is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level

                | unknownTag -> 
                    if verbose then printfn "[WARNING@L %i]: Found typedef tag <%s> that does not fit OBO flat file specifications 1.4. Skipping it..." lineNumber unknownTag
                    OboTypeDef.fromLines verbose en (lineNumber + 1)
                        id domain range name inverse_of transitive_over is_cyclic is_reflexive is_symmetric 
                        is_anti_symmetric is_transitive is_metadata_tag is_class_level
                           
            else
                // Maybe check if id is empty
                lineNumber,
                OboTypeDef.make id domain range name
                    (inverse_of |> List.rev)
                    (transitive_over |> List.rev)
                    is_cyclic is_reflexive is_symmetric 
                    is_anti_symmetric is_transitive is_metadata_tag is_class_level
                //failwithf "Unexcpected end of file."

        /// Write an Obo Type Def to lines in "key:value" style
        static member toLines (typedef : OboTypeDef) =
            seq {
                "id: " + typedef.Id
                "domain: " + typedef.Domain
                "range: " + typedef.Range
                "name: " + typedef.Name
            }


    /// Ontology containing Obo Terms and Obo Type Defs (OBO 1.2)
    type OboOntology =

        {
            Terms : OboTerm list
            TypeDefs : OboTypeDef list
        }


        static member create terms typedefs =
            {
                Terms = terms
                TypeDefs = typedefs
            }

        /// Read an Obo Ontology containing term and type def stanzas from lines
        static member fromLines verbose (input:seq<string>) =         
                
            let en = input.GetEnumerator()
            let rec loop (en:System.Collections.Generic.IEnumerator<string>) terms typedefs lineNumber =
                
                match en.MoveNext() with
                | true ->             
                    match (en.Current |> trimComment) with
                    | "[Term]"    -> let lineNumber,parsedTerm = (OboTerm.fromLines verbose en lineNumber "" "" false [] "" "" [] [] [] [] [] [] [] [] false [] [] [] false "" "")
                                     loop en (parsedTerm :: terms) typedefs lineNumber
                    | "[Typedef]" -> let lineNumber,parsedTypeDef = (OboTypeDef.fromLines verbose en lineNumber "" "" "" "" [] [] false false false false false false false)
                                     loop en terms (parsedTypeDef :: typedefs) lineNumber
                    | _ -> loop en terms typedefs (lineNumber+1)
                | false -> OboOntology.create (List.rev terms) (List.rev typedefs)
                
            loop en [] [] 1

        /// Read an Obo Ontology containing term and type def stanzas from a file with the given path
        static member fromFile verbose (path : string) =
            System.IO.File.ReadAllLines path
            |> OboOntology.fromLines verbose

        /// Write an Obo Ontology to term and type def stanzas in line form
        static member toLines (oboOntology:OboOntology) =         
            seq {
                for term in oboOntology.Terms do
                    yield "[Term]"
                    yield! OboTerm.toLines term
                    yield ""

                for typedef in oboOntology.TypeDefs do                   
                    yield "[Typedef]"
                    yield! OboTypeDef.toLines typedef
                    yield ""
            }
            
        /// Write an Obo Ontology to term and type def stanzas to a file in the given path
        static member toFile (path : string) (oboOntology:OboOntology) =         
            System.IO.File.WriteAllLines(path,OboOntology.toLines oboOntology)

        /// Write an Obo Ontology to term and type def stanzas in line form
        member this.ToLines() = 
            OboOntology.toLines this

        /// Write an Obo Ontology to term and type def stanzas to a file in the given path
        member this.ToFile(path : string) =
            OboOntology.toFile path this

        /// Find obo term by "TermSourceRef:TermAccessionNumber" style id
        member this.TryGetTerm(id : string) = 
            this.Terms
            |> List.tryFind (fun t ->
                t.Id = id
            )

        /// Find obo term by "TermSourceRef:TermAccessionNumber" style id
        member this.GetTerm(id : string) = 
            this.Terms
            |> List.find (fun t ->
                t.Id = id
            )

        /// Find obo term by "TermSourceRef:TermAccessionNumber" style id and return it as ISA OntologyAnnotation type
        member this.TryGetOntologyAnnotation(id : string) = 
            this.Terms
            |> List.tryPick (fun t ->
                if t.Id = id then Some (OboTerm.toOntologyAnnotation t) else None
            )

        /// Find obo term by "TermSourceRef:TermAccessionNumber" style id and return it as ISA OntologyAnnotation type
        member this.GetOntologyAnnotation(id : string) = 
            this.Terms
            |> List.pick (fun t ->
                if t.Id = id then Some (OboTerm.toOntologyAnnotation t) else None
            )

        /// Find obo term by it's free text name
        member this.TryGetTermByName(name : string) = 
            this.Terms
            |> List.tryFind (fun t ->
                t.Name = name
            )

        /// Find obo term by it's free text name
        member this.GetTermByName(name : string) = 
            this.Terms
            |> List.find (fun t ->
                t.Name = name
            )

        /// Find obo term by it's free text name and return it as ISA OntologyAnnotation type
        member this.TryGetOntologyAnnotationByName(name : string) = 
            this.Terms
            |> List.tryPick (fun t ->
                if t.Name = name then Some (OboTerm.toOntologyAnnotation t) else None
            )

        /// Find obo term by it's free text name and return it as ISA OntologyAnnotation type
        member this.GetOntologyAnnotationByName(name : string) = 
            this.Terms
            |> List.pick (fun t ->
                if t.Name = name then Some (OboTerm.toOntologyAnnotation t) else None
            )

        /// For a given ontology term, find all equivalent terms that are connected via XRefs
        ///
        /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked
        member this.GetEquivalentOntologyAnnotations(term : ISADotNet.OntologyAnnotation, ?Depth : int) =
            
            let rec loop depth (equivalents : ISADotNet.OntologyAnnotation list) (lastLoop : ISADotNet.OntologyAnnotation list) =
                if equivalents.Length = lastLoop.Length then equivalents
                elif Depth.IsSome && Depth.Value < depth then equivalents
                else
                    let newEquivalents = 
                        equivalents
                        |> List.collect (fun t ->
                            match this.TryGetTerm t.ShortAnnotationString with
                            | Some term ->
                                term.Xrefs
                                |> List.map (fun xref ->
                                    let id = OntologyAnnotation.createShortAnnotation "" xref.Name
                                    match this.TryGetOntologyAnnotation id with
                                    | Some oa ->
                                        oa
                                    | None -> 
                                        OntologyAnnotation.fromString "" "" xref.Name
                                )
                            | None ->
                                []
                        )
                    loop (depth + 1) (equivalents @ newEquivalents |> List.distinct) equivalents
            loop 1 [term] []
            |> List.filter ((<>) term)

        /// For a given ontology term, find all equivalent terms that are connected via XRefs
        ///
        /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked
        member this.GetEquivalentOntologyAnnotations(termId : string, ?Depth) =
            match Depth with 
            | Some d ->
                OntologyAnnotation.fromAnnotationId termId         
                |> fun oa -> this.GetEquivalentOntologyAnnotations(oa, d)
            | None -> 
                OntologyAnnotation.fromAnnotationId termId    
                |> this.GetEquivalentOntologyAnnotations

        /// For a given ontology term, find all terms to which this term points in a "isA" relationship
        ///
        /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked
        member this.GetParentOntologyAnnotations(term : ISADotNet.OntologyAnnotation, ?Depth) =
            let rec loop depth (equivalents : ISADotNet.OntologyAnnotation list) (lastLoop : ISADotNet.OntologyAnnotation list) =
                if equivalents.Length = lastLoop.Length then equivalents
                elif Depth.IsSome && Depth.Value < depth then equivalents
                else
                    let newEquivalents = 
                        equivalents
                        |> List.collect (fun t ->
                            match this.TryGetTerm t.ShortAnnotationString with
                            | Some term ->
                                term.IsA
                                |> List.map (fun isA ->
                                    match this.TryGetOntologyAnnotation isA with
                                    | Some oa ->
                                        oa
                                    | None -> 
                                        OntologyAnnotation.fromString "" "" isA
                                )
                            | None ->
                                []
                        )
                    loop (depth + 1) (equivalents @ newEquivalents |> List.distinct) equivalents
            loop 1 [term] []
            |> List.filter ((<>) term)

        /// For a given ontology term, find all terms to which this term points in a "isA" relationship
        ///
        /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked
        member this.GetParentOntologyAnnotations(termId : string, ?Depth) =
            match Depth with 
            | Some d ->
                OntologyAnnotation.fromAnnotationId termId         
                |> fun oa -> this.GetParentOntologyAnnotations(oa, d)
            | None -> 
                OntologyAnnotation.fromAnnotationId termId    
                |> this.GetParentOntologyAnnotations

        /// For a given ontology term, find all terms which point to this term "isA" relationship
        ///
        /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked
        member this.GetChildOntologyAnnotations(term : ISADotNet.OntologyAnnotation, ?Depth) =
            let rec loop depth (equivalents : ISADotNet.OntologyAnnotation list) (lastLoop : ISADotNet.OntologyAnnotation list) =
                if equivalents.Length = lastLoop.Length then equivalents
                elif Depth.IsSome && Depth.Value < depth then equivalents
                else
                    let newEquivalents = 
                        equivalents
                        |> List.collect (fun t ->
                            this.Terms
                            |> List.choose (fun pt -> 
                                let isChild = 
                                    pt.IsA
                                    |> List.exists (fun isA -> t.ShortAnnotationString = isA)
                                if isChild then
                                    Some (OboTerm.toOntologyAnnotation(pt))
                                else
                                    None

                            )
                        )
                    loop (depth + 1) (equivalents @ newEquivalents |> List.distinct) equivalents
            loop 1 [term] []
            |> List.filter ((<>) term)

        /// For a given ontology term, find all terms which point to this term "isA" relationship
        ///
        /// Depth can be used to restrict the number of iterations by which neighbours of neighbours are checked
        member this.GetChildOntologyAnnotations(termId : string, ?Depth) =
            match Depth with 
            | Some d ->
                OntologyAnnotation.fromAnnotationId termId         
                |> fun oa -> this.GetChildOntologyAnnotations(oa, d)
            | None -> 
                OntologyAnnotation.fromAnnotationId termId    
                |> this.GetChildOntologyAnnotations


    type OboTermDef = 
        {
            Id           : string
            Name         : string
            IsTransitive : string
            IsCyclic     : string
        }

        static member make id name  isTransitive isCyclic =
            {Id = id; Name = name; IsTransitive = isTransitive; IsCyclic = isCyclic}

        //parseTermDef
        static member fromLines (en:Collections.Generic.IEnumerator<string>) id name isTransitive isCyclic =     
            if en.MoveNext() then                
                let split = (en.Current |> trimComment).Split([|": "|], System.StringSplitOptions.None)
                match split.[0] with
                | "id"            -> OboTermDef.fromLines en (split.[1..] |> String.concat ": ") name isTransitive isCyclic
                | "name"          -> OboTermDef.fromLines en id (split.[1..] |> String.concat ": ") isTransitive isCyclic 
                | "is_transitive" -> OboTermDef.fromLines en id name (split.[1..] |> String.concat ": ") isCyclic
                | "is_cyclic"     -> OboTermDef.fromLines en id name isTransitive (split.[1..] |> String.concat ": ")
                | ""              -> OboTermDef.make id name isTransitive isCyclic
                  
                | _               -> OboTermDef.fromLines en id name isTransitive isCyclic
            else
                // Maybe check if id is empty
                OboTermDef.make id name isTransitive isCyclic
                //failwithf "Unexcpected end of file."

    

    /// Parse Obo Terms [Term] from seq<string>
    [<Obsolete("Use fromLines function instead")>]
    let parseOboTerms verbose (input:seq<string>)  =         
        
        let en = input.GetEnumerator()
        let rec loop (en:System.Collections.Generic.IEnumerator<string>) lineNumber =
            seq {
                match en.MoveNext() with
                | true ->             
                    match (en.Current |> trimComment) with
                    | "[Term]"    -> let lineNumber,parsedTerm = (OboTerm.fromLines verbose en lineNumber "" "" false [] "" "" [] [] [] [] [] [] [] [] false [] [] [] false "" "")
                                     yield parsedTerm
                                     yield! loop en lineNumber
                    | _ -> yield! loop en (lineNumber+1)
                | false -> ()
            }
        loop en 1



//    //########################################
//    // Definition of OboGraph
//
//
//    module FastOboGraph =
//
//        /// Obo Term as node
//        [<StructuredFormatDisplay("{PrettyString}")>]
//        type OboNode = { 
//            Id : int
//            Name : string
//            NameSpace : string
//            OntologyId : string // GO:...
//            }
//            with
//            member this.PrettyString = sprintf "%s:%07i | %s {%s}" this.OntologyId this.Id this.Name this.NameSpace
//            interface INode<int>
//                with member this.Id = this.Id                
//
//
//        /// Creates OboNode
//        let createOboNode id name nameSpace ontologyId =
//            {Id = id; Name = name; NameSpace = nameSpace; OntologyId = ontologyId; }
//
//
//
//        type OboEdgeType =
//            | Is_A
//            | Part_Of
//
//        [<StructuredFormatDisplay("{PrettyString}")>]
//        type OboEdge = { 
//            Id : int
//            SourceId :int
//            TargetId :int } 
//            with
//            member this.PrettyString =  if this.Id = this.SourceId then
//                                            sprintf "o---> %07i | (%i)" this.Id this.TargetId
//                                        else 
//                                            sprintf "%07i <---o | (%i)" this.Id this.TargetId
//            interface IEdge<int> with
//                member this.Id = this.Id
//                member this.SourceId = this.SourceId
//                member this.TargetId = this.TargetId
//            
//
//        /// Creates OboEdge
//        let createOboEdge id sourceId targetId =
//            {Id = id; SourceId = sourceId; TargetId = targetId}
//
//
//        type oboAdjacencyNode = AdjacencyNode<OboNode,OboEdge,int>
//
//
//
//        /// Splits String s at ":", returns sa.[1]
//        let tryIdToInt str =
//            match str with
//            | Regex.RegexValue @"GO:(?<goId>[\d]+)" [ goId; ] -> Some( int goId )
//            | _ -> None
//
//        let idToInt str =
//            match tryIdToInt str with
//            | Some v -> v
//            | None   -> failwithf "%s invaild GO id" str
//
//        let private oboIdStringToInt s =
//            let sa = String.split ':' s
//            if sa.Length > 1 then
//                sa.[1] |> int
//            else
//                -1
//
//        /// Creates fromOboTerm from oboTerm startIndex
//        let fromOboTerm (obo: OboTerm) (startIndex: int) =
//            let nodeId = oboIdStringToInt obo.Id
//            let node   = createOboNode nodeId obo.Name obo.Namespace
//            let edges = 
//                obo.IsA
//                |> List.mapi (fun i edId -> let edgeTargetId = oboIdStringToInt edId
//                                            createOboEdge (i+startIndex) nodeId edgeTargetId
//                              )
//            (node,edges,(startIndex + obo.IsA.Length))
//
//
//        /// Creates OboEnumerator from oboNode oboEdge
//        let oboTermToOboGraph (input: seq<OboTerm>) = //: seq<oboAdjacencyNode> =
//            let en = input.GetEnumerator()
//            let rec loop (en:System.Collections.Generic.IEnumerator<OboTerm>) acc  =
//                seq { 
//                    match en.MoveNext() with
//                    | true -> let cNode,cEdges,cIndex = fromOboTerm en.Current acc
//                      
//                              yield (cNode,cEdges)
//                              yield! loop en cIndex
//                    | false -> ()
//                    }
//            loop en 0
//
//
//        /// Reads obo file 
//        let readFile path =
//            FileIO.readFile path
//            |> parseOboTerms
//            |> oboTermToOboGraph
//            |> Seq.toList


