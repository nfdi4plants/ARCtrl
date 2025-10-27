module ARCtrl.FileSystem.DefaultGitattributes

let [<Literal>] dga= """**/dataset/** filter=lfs diff=lfs merge=lfs -text"""
