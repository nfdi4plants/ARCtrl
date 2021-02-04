(***hide***)

(***condition:prepare***)
#r "nuget: Plotly.NET, 2.0.0-beta5"

(***condition:ipynb***)
#if IPYNB
#r "nuget: Plotly.NET, 2.0.0-beta5"
#r "nuget: Plotly.NET.Interactive, 2.0.0-beta5"
#endif // IPYNB

(**
# Including notebooks 

To include dotnet interactive notebooks in the ipynb format, it is enough for the `_template.ipynb` file to simply exist.

There are however some customization options with fsdocs that move your documentation to the next level:

## Conditional package references

use the IPYNB compiler directive in conjuntion with `condition:ipynb` to include blocks only in the rendered notebook file. 

This is especially usefull for referencing packages that otherwise be referenced locally during yopur buildchain:

*)

(***hide***)
"""
<pre>
#r "/path/to/your/binaries/during/local/build"

(***condition:ipynb***)
#if IPYNB
#r "nuget: yourProjectOnNuget, 1.3.3.7"
#endif // IPYNB
</pre>
"""
(***include-it-raw***)

(**

## Conditional value inclusion

Sometimes the content you want to include might differ aswell. An example is Plotly.NET charts. 
While you want to dump the chart html directly into the html docs via (`include-it-raw`), 
you want to end cells in notebooks with the chart value itself to include the chart in the output cell with Plotly.NET.Interactive. 

Here is an example for such an conditional block:

*)

(***hide***)
"""
<pre>
open Plotly.NET

let myChart = Chart.Point([1.,2.])

(***condition:ipynb***)
#if IPYNB
myChart
#endif // IPYNB

(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)
</pre>
"""
(***include-it-raw***)

open Plotly.NET

let myChart = Chart.Point([1.,2.])

(***condition:ipynb***)
#if IPYNB
myChart
#endif // IPYNB

(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**
## Including binder links

[Binder]() is an awesome project that launches an instance of your notebook given the correct `Dockerfile` and `nuget.config`, which will be added automatically by the `fsdocs` tool when you build the docs.

you can include a binder link like this (supposed you use gh-pages to host your docs):

```
[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/<YOUR-ORG>/<YOUR-PROJECT>/gh-pages?filepath=<YOUR-DOCS-FILENAME>.ipynb)
```

In fact, you can use this link here to check the conditionals of this very page in a notebook:

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/fslaborg/docs-template/gh-pages?filepath=3_notebooks.ipynb.ipynb)

*)