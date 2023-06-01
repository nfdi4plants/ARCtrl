### FileSystem.DataModel

#### FilesystemTree

```fsharp
type FileSystem =
    | File of string
    | Folder of FileSystem list
```

#### History

```mermaid
classDiagram

class commit ["Commit"] {
    Hash
    UserName
    UserEmail
    Date
    Message
}
```

- commit history metadata (literally `git log`)
  - git log --date=local --pretty=format:'%H, %an, %ae, %ad, "%s"'
