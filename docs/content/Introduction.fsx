(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I @"../../bin/ISADotnet/netstandard2.0/"
#r @"../../packages/formatting/Newtonsoft.Json/lib/netstandard2.0/Newtonsoft.Json.dll"
#r @"../../packages/formatting/Plotly.NET/lib/netstandard2.0/Plotly.NET.dll"
open Plotly.NET

let inlineHTML = sprintf "<inlineHTML>%s</inlineHTML>"
(**
Introduction
============

This is just some placeholder text

*)
//Your path may differ
#r "ISADotnet.dll"

open ISADotnet

(**

Include Values
=======================

Little example for how to include values


*)

///string form of our hello world protein
let myList = [1;2;3]

(*** include-value:myList ***)

myList |> List.zip myList |> Chart.Point
|> GenericChart.toChartHTML
|> inlineHTML

(*** include-it ***)