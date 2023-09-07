module ARCtrl.Contract.Git

open ARCtrl.Contract

let [<Literal>] git = @"git"  
let [<Literal>] defaultBranch = @"main" 
let [<Literal>] gitignoreFileName = @".gitignore" 

let gitWithArgs(arguments : string []) = CLITool.create(git,arguments)

let createGitContractAt path arguments = Contract.createExecute(gitWithArgs(arguments),path)

let createGitContract(arguments) = Contract.createExecute(gitWithArgs(arguments))

let gitIgnoreContract = Contract.createCreate(gitignoreFileName,DTOType.PlainText,DTO.Text ARCtrl.FileSystem.DefaultGitignore.dgi)

module Init = 

    let [<Literal>] init = "init" 
    let [<Literal>] branchFlag = "-b"

    let [<Literal>] remote = @"remote"
    let [<Literal>] add = @"add"
    let [<Literal>] origin = @"origin"

    let createInitContract(branch : string option) =
        let branch = Option.defaultValue defaultBranch branch
        createGitContract([|init;branchFlag;branch|])

    let createAddRemoteContract(remoteUrl : string) =
        createGitContract([|remote;add;origin;remoteUrl|])
