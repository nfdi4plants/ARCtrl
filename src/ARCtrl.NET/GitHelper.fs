namespace ARCtrl.NET

open System.Diagnostics
open System.Runtime.InteropServices
open System.IO

module GitHelper =

    /// Executes Git command and returns git output.
    let executeGitCommandWithResponse (repoDir : string) (command : string) =

        let log = Logging.createLogger "ExecuteGitCommandLog"

        log.Trace($"Run git {command}")

        let procStartInfo = 
            ProcessStartInfo(
                WorkingDirectory = repoDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = "git",
                Arguments = command
            )
        
        let outputs = System.Collections.Generic.List<string>()
        let outputHandler (_sender:obj) (args:DataReceivedEventArgs) = 
            if (args.Data = null |> not) then
                if args.Data.ToLower().Contains ("error") then
                    log.Error($"GIT: {args.Data}")    
                elif args.Data.ToLower().Contains ("trace") then
                    log.Trace($"GIT: {args.Data}")   
                else
                    outputs.Add(args.Data)
                    log.Info($"GIT: {args.Data}")
        
        let errorHandler (_sender:obj) (args:DataReceivedEventArgs) =  
            if (args.Data = null |> not) then
                let msg = args.Data.ToLower()
                if msg.Contains ("error") || msg.Contains ("fatal") then
                    log.Error($"GIT: {args.Data}")    
                elif msg.Contains ("trace") then
                    log.Trace($"GIT: {args.Data}")   
                else
                    outputs.Add(args.Data)
                    log.Info($"GIT: {args.Data}")
        
        let p = new Process(StartInfo = procStartInfo)

        p.OutputDataReceived.AddHandler(DataReceivedEventHandler outputHandler)
        p.ErrorDataReceived.AddHandler(DataReceivedEventHandler errorHandler)
        p.Start() |> ignore
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.WaitForExit()
        outputs

    /// Executes Git command.
    let executeGitCommand (repoDir : string) (command : string) =
        
        executeGitCommandWithResponse repoDir command |> ignore

    let formatRepoString username pass (url : string) = 
        let comb = username + ":" + pass + "@"
        url.Replace("https://","https://" + comb)

    let setLocalEmail (dir : string) (email : string) =
        executeGitCommand dir (sprintf "config user.email \"%s\"" email)

    let tryGetLocalEmail (dir : string) =
        let r = executeGitCommandWithResponse dir "config --local --get user.email"
        if r.Count = 0 then None
        else Some r.[0]

    let setGlobalEmail (email : string) =
        executeGitCommand "" (sprintf "config --global user.email \"%s\"" email)

    let tryGetGlobalEmail () =
        let r = executeGitCommandWithResponse "" "config --global --get user.email"
        if r.Count = 0 then None
        else Some r.[0]

    let setLocalName (dir : string) (name : string) =
        executeGitCommand dir (sprintf "config user.name \"%s\"" name)

    let tryGetLocalName (dir : string) =
        let r = executeGitCommandWithResponse dir "config --local --get user.name"
        if r.Count = 0 then None
        else Some r.[0]

    let setGlobalName (name : string) =
        executeGitCommand "" (sprintf "config --global user.name \"%s\"" name)

    let tryGetGlobalName () =
        let r = executeGitCommandWithResponse "" "config --global --get user.name"
        if r.Count = 0 then None
        else Some r.[0]

    let clone dir url =
        executeGitCommand dir (sprintf "clone %s" url)

    let noLFSConfig = "-c \"filter.lfs.smudge = git-lfs smudge --skip -- %f\" -c \"filter.lfs.process = git-lfs filter-process --skip\""

    let cloneNoLFS dir url =
        executeGitCommand dir (sprintf "clone %s %s" noLFSConfig url)        

    let add dir = 
        executeGitCommand dir "add ."

    let commit dir message =
        executeGitCommand dir (sprintf "commit -m \"%s\"" message)

    let push dir =
        executeGitCommand dir "push"
          
    /// Set containing all the paths and path rules that should be tracked using git lfs
    type LFSRuleSet = Set<string>
    
    /// Checks the .gitattributes file in the repo for all git lfs paths and path rules
    let retrieveLFSRules repoDir : LFSRuleSet =
        let gitAttributesFilePath = Path.Combine(repoDir,".gitattributes")
        let lfsRulePattern = ".*(?= filter=lfs diff=lfs merge=lfs -text)"
        if File.Exists gitAttributesFilePath then
            File.ReadAllLines(gitAttributesFilePath)
            |> Array.choose (fun l -> 
                let r = System.Text.RegularExpressions.Regex.Match(l,lfsRulePattern)
                if r.Success then
                    Some r.Value
                else None     
            )
            |> set
        else Set.empty

    let containsLFSRule (ruleSet : LFSRuleSet) (path : string) =
        ruleSet.Contains path
