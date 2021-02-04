(**
# Inline package references and charting

With fsdocs 8.0, the tool can roll forward to .net 5, meaning you can use inline package references in the docs scripts:
*)

#r "nuget: Plotly.NET, 2.0.0-alpha5"

open Plotly.NET

let myChart = 
    Chart.Line(
        [
            1.,1.
            5.,6.
            23.,9.
        ]
    )
    |> Chart.withTitle "Hello fsdocs!"

(**
You can now also include raw html in your docs scripts with the new `include-it-raw`.
To incude the chart html of a Plotly.NET chart and and render it on the docs page, use the `GenericChart.toChartHTML`
and include the raw output. 

the actual codeblock looks like this:

*)
(***hide***)
"""
<pre>
(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)
</pre>
</pre>
"""
(***include-it-raw***)

(**
Here is the rendered chart:
*)

(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)