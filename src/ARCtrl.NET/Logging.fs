module ARCtrl.NET.Logging

open System
open NLog
open NLog.Config
open NLog.Targets
open NLog.Conditions


/// Creates a new logger with the given name. Configuration details are obtained from the generateConfig function.
let createLogger (loggerName : string) = 

    // new instance of "Logger" with activated config
    let logger = LogManager.GetLogger(loggerName)

    logger

/// Takes a logger and an exception and separates usage and error messages. Usage messages will be printed into the console while error messages will be logged.
let handleExceptionMessage (log : NLog.Logger) (exn : Exception) =
    // separate usage message (Argu) and error messages. Error messages shall be logged, usage messages shall not, empty error message shall not appear at all
    let isUsageMessage = exn.Message.Contains("USAGE") || exn.Message.Contains("SUBCOMMANDS")
    let isErrorMessage = exn.Message.Contains("ERROR")
    let isEmptyMessage = exn.Message = ""
    match isUsageMessage, isErrorMessage, isEmptyMessage with
    | true,true,false -> // exception message contains usage AND error messages
        let eMsg, uMsg = 
            exn.Message.Split(Array.singleton Environment.NewLine, StringSplitOptions.None) // '\n' leads to parsing problems
            |> fun arr ->
                arr |> Array.find (fun (t: string) -> t.Contains("ERROR")),
                arr |> Array.filter (fun t -> t.Contains("ERROR") |> not) |> String.concat "\n" // Argu usage instruction shall not be logged as error
        log.Error(eMsg)
        printfn "%s" uMsg
    | true,false,false -> printfn "%s" exn.Message // exception message contains usage message but NO error message
    | false,false,true -> () // empty error message
    | _ -> log.Error(exn) // everything else will be a non-empty error message
    
/// Checks if a message (string) is empty and if it is not, applies a logging function to it.
let checkNonLog s (logging : string -> unit) = if s <> "" then logging s
    