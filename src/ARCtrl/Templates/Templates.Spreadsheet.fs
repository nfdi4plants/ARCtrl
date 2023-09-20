module ARCtrl.Templates.Spreadsheet

open FsSpreadsheet
open ARCtrl.ISA.Aux
open ARCtrl.ISA.Spreadsheet
open ARCtrl.ISA
open System.Collections.Generic

module Metadata = 
    
    
    module ER = 

        let [<Literal>] erHeaderLabel = "#ER list"

        let [<Literal>] erLabel = "ER"
        let [<Literal>] erTermAccessionNumberLabel = "ER Term Accession Number"
        let [<Literal>] erTermSourceREFLabel = "ER Term Source REF"

        let labels = [erLabel;erTermAccessionNumberLabel;erTermSourceREFLabel]

        let fromSparseTable (matrix : SparseTable) =
            OntologyAnnotationSection.fromSparseTable erLabel erTermAccessionNumberLabel erTermSourceREFLabel matrix

        let toSparseTable (designs: OntologyAnnotation list) =
            OntologyAnnotationSection.toSparseTable erLabel erTermAccessionNumberLabel erTermSourceREFLabel designs

        let fromRows (prefix : string option) (rows : IEnumerator<SparseRow>) =
            let nextHeader, _, _, ers = OntologyAnnotationSection.fromRows prefix erLabel erTermAccessionNumberLabel erTermSourceREFLabel 0 rows
            nextHeader,ers
    
        let toRows (prefix : string option) (designs : OntologyAnnotation list) =
            OntologyAnnotationSection.toRows prefix erLabel erTermAccessionNumberLabel erTermSourceREFLabel designs

    module Tags = 

        let [<Literal>] tagsHeaderLabel = "#TAGS list"

        let [<Literal>] tagsLabel = "Tags"
        let [<Literal>] tagsTermAccessionNumberLabel = "Tags Term Accession Number"
        let [<Literal>] tagsTermSourceREFLabel = "Tags Term Source REF"

        let labels = [tagsLabel;tagsTermAccessionNumberLabel;tagsTermSourceREFLabel]

        let fromSparseTable (matrix : SparseTable) =
            OntologyAnnotationSection.fromSparseTable tagsLabel tagsTermAccessionNumberLabel tagsTermSourceREFLabel matrix

        let toSparseTable (designs: OntologyAnnotation list) =
            OntologyAnnotationSection.toSparseTable tagsLabel tagsTermAccessionNumberLabel tagsTermSourceREFLabel designs

        let fromRows (prefix : string option) (rows : IEnumerator<SparseRow>) =
            let nextHeader, _, _, tags = OntologyAnnotationSection.fromRows prefix tagsLabel tagsTermAccessionNumberLabel tagsTermSourceREFLabel 0 rows
            nextHeader, tags

        let toRows (prefix : string option) (designs : OntologyAnnotation list) =
            OntologyAnnotationSection.toRows prefix tagsLabel tagsTermAccessionNumberLabel tagsTermSourceREFLabel designs

    module Authors =       

        let [<Literal>] lastNameLabel = "Last Name"
        let [<Literal>] firstNameLabel = "First Name"
        let [<Literal>] midInitialsLabel = "Mid Initials"
        let [<Literal>] emailLabel = "Email"
        let [<Literal>] phoneLabel = "Phone"
        let [<Literal>] faxLabel = "Fax"
        let [<Literal>] addressLabel = "Address"
        let [<Literal>] affiliationLabel = "Affiliation"
        let [<Literal>] orcidLabel = "ORCID"
        let [<Literal>] rolesLabel = "Roles"
        let [<Literal>] rolesTermAccessionNumberLabel = "Roles Term Accession Number"
        let [<Literal>] rolesTermSourceREFLabel = "Roles Term Source REF"

        let labels = [lastNameLabel;firstNameLabel;midInitialsLabel;emailLabel;phoneLabel;faxLabel;addressLabel;affiliationLabel;rolesLabel;rolesTermAccessionNumberLabel;rolesTermSourceREFLabel]

        let fromString lastName firstName midInitials email phone fax address affiliation orcid role rolesTermAccessionNumber rolesTermSourceREF comments =
            let roles = OntologyAnnotation.fromAggregatedStrings ';' role rolesTermSourceREF rolesTermAccessionNumber
            Person.make 
                None 
                orcid
                (lastName   ) 
                (firstName  )
                (midInitials) 
                (email      )
                (phone      )
                (fax        )
                (address    )
                (affiliation) 
                (Option.fromValueWithDefault [||] roles    )
                (Option.fromValueWithDefault [||] comments )
            |> Person.setOrcidFromComments

        let fromSparseTable (matrix : SparseTable) =
            if matrix.ColumnCount = 0 && matrix.CommentKeys.Length <> 0 then
                let comments = SparseTable.GetEmptyComments matrix
                Person.create(Comments = comments)
                |> List.singleton
            else
                List.init matrix.ColumnCount (fun i -> 
                    let comments = 
                        matrix.CommentKeys 
                        |> List.map (fun k -> 
                            Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))
                        |> Array.ofList
                    fromString
                        (matrix.TryGetValue(lastNameLabel,i))
                        (matrix.TryGetValue(firstNameLabel,i))
                        (matrix.TryGetValue(midInitialsLabel,i))
                        (matrix.TryGetValue(emailLabel,i))
                        (matrix.TryGetValue(phoneLabel,i))
                        (matrix.TryGetValue(faxLabel,i))
                        (matrix.TryGetValue(addressLabel,i))
                        (matrix.TryGetValue(affiliationLabel,i))
                        (matrix.TryGetValue(orcidLabel,i))
                        (matrix.TryGetValueDefault("",(rolesLabel,i)))
                        (matrix.TryGetValueDefault("",(rolesTermAccessionNumberLabel,i)))
                        (matrix.TryGetValueDefault("",(rolesTermSourceREFLabel,i)))
                        comments
                )

        let toSparseTable (persons:Person list) =
            let matrix = SparseTable.Create (keys = labels,length=persons.Length + 1)
            let mutable commentKeys = []
            persons
            |> List.map Person.setCommentFromORCID
            |> List.iteri (fun i p ->
                let i = i + 1
                let rAgg = Option.defaultValue [||] p.Roles |> OntologyAnnotation.toAggregatedStrings ';'
                do matrix.Matrix.Add ((lastNameLabel,i),                    (Option.defaultValue ""  p.LastName     ))
                do matrix.Matrix.Add ((firstNameLabel,i),                   (Option.defaultValue ""  p.FirstName    ))
                do matrix.Matrix.Add ((midInitialsLabel,i),                 (Option.defaultValue ""  p.MidInitials  ))
                do matrix.Matrix.Add ((emailLabel,i),                       (Option.defaultValue ""  p.EMail        ))
                do matrix.Matrix.Add ((phoneLabel,i),                       (Option.defaultValue ""  p.Phone        ))
                do matrix.Matrix.Add ((faxLabel,i),                         (Option.defaultValue ""  p.Fax          ))
                do matrix.Matrix.Add ((addressLabel,i),                     (Option.defaultValue ""  p.Address      ))
                do matrix.Matrix.Add ((affiliationLabel,i),                 (Option.defaultValue ""  p.Affiliation  ))
                do matrix.Matrix.Add ((rolesLabel,i),                       rAgg.TermNameAgg)  
                do matrix.Matrix.Add ((rolesTermAccessionNumberLabel,i),    rAgg.TermAccessionNumberAgg)
                do matrix.Matrix.Add ((rolesTermSourceREFLabel,i),          rAgg.TermSourceREFAgg)

                match p.Comments with 
                | None -> ()
                | Some c ->
                    c
                    |> Array.iter (fun comment -> 
                        let n,v = comment |> Comment.toString
                        commentKeys <- n :: commentKeys
                        matrix.Matrix.Add((n,i),v)
                    )
            )
            {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev} 


        let fromRows (prefix : string option) (rows : IEnumerator<SparseRow>) =
            SparseTable.FromRows(rows,labels,0,?prefix = prefix)
            |> fun (s,_,_,sm) -> (s, fromSparseTable sm)

        let toRows (prefix : string option) (persons : Person list) =
            persons
            |> toSparseTable
            |> fun m -> 
                match prefix with 
                | Some prefix -> SparseTable.ToRows(m,prefix)
                | None -> SparseTable.ToRows(m)

    module Template = 
  
        let [<Literal>] identifierLabel = "Id"
        let [<Literal>] nameLabel = "Name"
        let [<Literal>] versionLabel = "Version"
        let [<Literal>] descriptionLabel = "Description"
        let [<Literal>] organisationLabel = "Organisation"
        let [<Literal>] tableLabel = "Table"

        let [<Literal>] authorsLabelPrefix = "Authors"

        let [<Literal>] authorsLabel = "#AUTHORS list"
        let [<Literal>] erLabel = "#ER list"
        let [<Literal>] tagsLabel = "#TAGS list"


        type TemplateInfo =
            {
            Id : string
            Name : string
            Version : string
            Description : string
            Organisation : string
            Table : string
            Comments : Comment list
            }

            static member create id name version description organisation table comments =
                {Id = id;Name = name;Version = version;Description = description;Organisation = organisation;Table = table;Comments = comments}
  
            static member empty = 
                TemplateInfo.create "" "" "" "" "" "" []

            static member Labels = 
                [identifierLabel;nameLabel;versionLabel;descriptionLabel;organisationLabel;tableLabel]

            static member FromSparseTable (matrix : SparseTable) =
        
                let i = 0

                let comments = 
                    matrix.CommentKeys 
                    |> List.map (fun k -> 
                        Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

                TemplateInfo.create
                    (matrix.TryGetValueDefault(Identifier.createMissingIdentifier(),(identifierLabel,i)))  
                    (matrix.TryGetValueDefault("",(nameLabel,i)))  
                    (matrix.TryGetValueDefault("",(versionLabel,i)))  
                    (matrix.TryGetValueDefault("",(descriptionLabel,i)))  
                    (matrix.TryGetValueDefault("",(organisationLabel,i)))  
                    (matrix.TryGetValueDefault("",(tableLabel,i)))                    
                    comments


            static member ToSparseTable (template: Template) =
                let i = 1
                let matrix = SparseTable.Create (keys = TemplateInfo.Labels,length = 2)
                let mutable commentKeys = []
                let processedIdentifier =
                    if template.Id.ToString().StartsWith(Identifier.MISSING_IDENTIFIER) then "" else 
                        template.Id.ToString()

                do matrix.Matrix.Add ((identifierLabel,i),          processedIdentifier)
                do matrix.Matrix.Add ((nameLabel,i),               (template.Name))
                do matrix.Matrix.Add ((versionLabel,i),      (template.Version))
                //do matrix.Matrix.Add ((descriptionLabel,i),         (Option.defaultValue "" template.Description))
                do matrix.Matrix.Add ((organisationLabel,i),   (template.Organisation.ToString()))
                do matrix.Matrix.Add ((tableLabel,i),            template.Table.Name)

                //if Array.isEmpty template.Comments |> not then
                //    template.Comments
                //    |> Array.iter (fun comment -> 
                //        let n,v = comment |> Comment.toString
                //        commentKeys <- n :: commentKeys
                //        matrix.Matrix.Add((n,i),v)
                //    )    

                {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

            static member fromRows (rows : IEnumerator<SparseRow>) =
                SparseTable.FromRows(rows,TemplateInfo.Labels,0)
                |> fun (s,ln,rs,sm) -> (s,TemplateInfo.FromSparseTable sm)
    
            static member toRows (template : Template) =  
                template
                |> TemplateInfo.ToSparseTable
                |> SparseTable.ToRows
    
        


        let fromRows (rows : seq<SparseRow>) = 

            let rec loop en lastLine (templateInfo : TemplateInfo) ers tags authors  =
           
                match lastLine with

                | Some k when k = erLabel -> 
                    let currentLine,newERs = ER.fromRows None en
                    loop en currentLine templateInfo (List.append ers newERs) tags authors 

                | Some k when k = tagsLabel -> 
                    let currentLine,newTags = Tags.fromRows None en
                    loop en currentLine templateInfo ers (List.append tags newTags) authors 

                | Some k when k = authorsLabel -> 
                    let currentLine,newAuthors = Authors.fromRows (Some authorsLabelPrefix) en
                    loop en currentLine templateInfo ers tags (List.append authors newAuthors)

                | k -> 
                    templateInfo,ers,tags,authors
            let en = rows.GetEnumerator()
            let currentLine,item = TemplateInfo.fromRows en  
            loop en currentLine item [] [] []

    
        let toRows (template : Template) =
            seq {          
                yield! TemplateInfo.toRows template

                yield  SparseRow.fromValues [erLabel]
                yield! ER.toRows (None) (template.EndpointRepositories |> Array.toList)

                yield  SparseRow.fromValues [tagsLabel]
                yield! Tags.toRows (None) (template.Tags |> Array.toList)

                yield  SparseRow.fromValues [authorsLabel]
                yield! Authors.toRows (Some authorsLabelPrefix) (List.ofArray template.Authors)
            }

    
module Template = 
    
    open Metadata
    open Template

    let [<Literal>] metaDataSheetName = "SwateTemplateMetadata"

    let fromParts (templateInfo:TemplateInfo) (ers:OntologyAnnotation list) (tags: OntologyAnnotation list) (authors : Person list) (table : ArcTable) (lastUpdated : System.DateTime)=
            Template.make 
                (System.Guid templateInfo.Id)
                table
                (templateInfo.Name)
                (Organisation.ofString templateInfo.Organisation) 
                (templateInfo.Version)
                (Array.ofList authors)
                (Array.ofList ers)
                (Array.ofList tags)  
                (lastUpdated)

    let toMetadataSheet (template : Template) : FsWorksheet =
        
        let sheet = FsWorksheet(metaDataSheetName)
        Template.toRows template
        |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
        sheet

    let fromMetadataSheet (sheet : FsWorksheet)  =
        sheet.Rows 
        |> Seq.map SparseRow.fromFsRow
        |> Template.fromRows

[<AutoOpen>]
module Extensions =

    type Template with
    
        /// Reads an assay from a spreadsheet
        static member fromFsWorkbook (doc:FsWorkbook) = 
            // Reading the "Assay" metadata sheet. Here metadata 
            let templateInfo,ers,tags,authors = 
        
                match doc.TryGetWorksheetByName Template.metaDataSheetName with 
                | Option.Some sheet ->
                    Template.fromMetadataSheet sheet
                | None ->  
                    Metadata.Template.TemplateInfo.empty,[],[],[]
            
            let tryTableNameMatches (ws : FsWorksheet) = 
                if ws.Tables |> Seq.exists (fun t -> t.Name = templateInfo.Table) then Some ws else None

            let tryWSNameMatches (ws : FsWorksheet) = 
                if ws.Name = templateInfo.Table then Some ws else None

            let sheets = doc.GetWorksheets()
                
            let table = 
                match sheets |> Seq.tryPick tryTableNameMatches with
                | Some ws -> 
                    match ArcTable.tryFromFsWorksheet ws with
                    | Some t -> t
                    | None -> failwithf "Ws with name %s could not be converted to a table" ws.Name
                | None ->
                    match sheets |> Seq.tryPick tryWSNameMatches with
                      | Some ws -> 
                            match ArcTable.tryFromFsWorksheet ws with
                            | Some t -> t
                            | None -> failwithf "Ws with name %s could not be converted to a table" ws.Name
                      | None -> failwithf "No worksheet with name %s found" templateInfo.Table
            
            Template.fromParts templateInfo ers tags authors table (System.DateTime.Now)

        static member toFsWorkbook (template : Template) =
            let doc = new FsWorkbook()
            let metaDataSheet = Template.toMetadataSheet template
            doc.AddWorksheet metaDataSheet

            template.Table
            |> ArcTable.toFsWorksheet 
            |> doc.AddWorksheet

            doc


