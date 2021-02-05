(*** hide ***)

(*** condition: prepare ***)
//#r @"..\packages\Newtonsoft.Json\lib\netstandard2.0\Newtonsoft.Json.dll"
#r @"..\bin\ISADotNet\netstandard2.0\ISADotNet.dll"
(*** condition: ipynb ***)
#if IPYNB
#r "nuget: Plotly.NET, {{fsdocs-package-version}}"
#r "nuget: Plotly.NET.Interactive, {{fsdocs-package-version}}"
#endif // IPYNB
(**
# Home
[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=Home.ipynb)
*)

open ISADotNet

let emptyProcess = ISADotNet.Process.empty

