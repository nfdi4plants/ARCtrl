namespace ISA.Spreadsheet

open ISA
open ISA.API
open Comment
open Remark
open System.Collections.Generic

module Contacts = 

    let lastNameLabel = "Last Name"
    let firstNameLabel = "First Name"
    let midInitialsLabel = "Mid Initials"
    let emailLabel = "Email"
    let phoneLabel = "Phone"
    let faxLabel = "Fax"
    let addressLabel = "Address"
    let affiliationLabel = "Affiliation"
    let rolesLabel = "Roles"
    let rolesTermAccessionNumberLabel = "Roles Term Accession Number"
    let rolesTermSourceREFLabel = "Roles Term Source REF"

    let labels = [lastNameLabel;firstNameLabel;midInitialsLabel;emailLabel;phoneLabel;faxLabel;addressLabel;affiliationLabel;rolesLabel;rolesTermAccessionNumberLabel;rolesTermSourceREFLabel]

    let fromString lastName firstName midInitials email phone fax address affiliation role rolesTermAccessionNumber rolesTermSourceREF comments =
        let roles = OntologyAnnotation.fromAggregatedStrings ';' role rolesTermSourceREF rolesTermAccessionNumber
        Person.make 
            None 
            (Option.fromValueWithDefault "" lastName   ) 
            (Option.fromValueWithDefault "" firstName  )
            (Option.fromValueWithDefault "" midInitials) 
            (Option.fromValueWithDefault "" email      )
            (Option.fromValueWithDefault "" phone      )
            (Option.fromValueWithDefault "" fax        )
            (Option.fromValueWithDefault "" address    )
            (Option.fromValueWithDefault "" affiliation) 
            (Option.fromValueWithDefault [] roles      )
            (Option.fromValueWithDefault [] comments   )

    let fromSparseTable (matrix : SparseTable) =
        
        List.init matrix.Length (fun i -> 
            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))
            fromString
                (matrix.TryGetValueDefault("",(lastNameLabel,i)))
                (matrix.TryGetValueDefault("",(firstNameLabel,i)))
                (matrix.TryGetValueDefault("",(midInitialsLabel,i)))
                (matrix.TryGetValueDefault("",(emailLabel,i)))
                (matrix.TryGetValueDefault("",(phoneLabel,i)))
                (matrix.TryGetValueDefault("",(faxLabel,i)))
                (matrix.TryGetValueDefault("",(addressLabel,i)))
                (matrix.TryGetValueDefault("",(affiliationLabel,i)))
                (matrix.TryGetValueDefault("",(rolesLabel,i)))
                (matrix.TryGetValueDefault("",(rolesTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(rolesTermSourceREFLabel,i)))
                comments
        )

    let toSparseTable (persons:Person list) =
        let matrix = SparseTable.Create (keys = labels,length=persons.Length + 1)
        let mutable commentKeys = []
        persons
        |> List.iteri (fun i p ->
            let i = i + 1
            let role,rolesTermSourceREF,rolesTermAccessionNumber = Option.defaultValue [] p.Roles |> OntologyAnnotation.toAggregatedStrings ';'
            do matrix.Matrix.Add ((lastNameLabel,i),                    (Option.defaultValue ""  p.LastName     ))
            do matrix.Matrix.Add ((firstNameLabel,i),                   (Option.defaultValue ""  p.FirstName    ))
            do matrix.Matrix.Add ((midInitialsLabel,i),                 (Option.defaultValue ""  p.MidInitials  ))
            do matrix.Matrix.Add ((emailLabel,i),                       (Option.defaultValue ""  p.EMail        ))
            do matrix.Matrix.Add ((phoneLabel,i),                       (Option.defaultValue ""  p.Phone        ))
            do matrix.Matrix.Add ((faxLabel,i),                         (Option.defaultValue ""  p.Fax          ))
            do matrix.Matrix.Add ((addressLabel,i),                     (Option.defaultValue ""  p.Address      ))
            do matrix.Matrix.Add ((affiliationLabel,i),                 (Option.defaultValue ""  p.Affiliation  ))
            do matrix.Matrix.Add ((rolesLabel,i),                       role)  
            do matrix.Matrix.Add ((rolesTermAccessionNumberLabel,i),    rolesTermAccessionNumber)
            do matrix.Matrix.Add ((rolesTermSourceREFLabel,i),          rolesTermSourceREF)

            match p.Comments with 
            | None -> ()
            | Some c ->
                c
                |> List.iter (fun comment -> 
                    let n,v = comment |> Comment.toString
                    commentKeys <- n :: commentKeys
                    matrix.Matrix.Add((n,i),v)
                )
        )
        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev} 


    let fromRows (prefix : string option) lineNumber (rows : IEnumerator<SparseRow>) =
        match prefix with
        | Some p -> SparseTable.FromRows(rows,labels,lineNumber,p)
        | None -> SparseTable.FromRows(rows,labels,lineNumber)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)

    let toRows (prefix : string option) (persons : Person list) =
        persons
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)