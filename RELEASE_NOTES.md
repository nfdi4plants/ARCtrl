### 0.0.4+fa4d1c5 (Released 2021-2-8)
* Additions:
    * latest commit #fa4d1c5
    * [[#23a6d87](https://github.com/nfdi4plants/Swate/commit/23a6d876759e16adbbe18a1791a175438e2a7940)] Add --noapidocs flag to docs tasks
    * [[#a05b17e](https://github.com/nfdi4plants/Swate/commit/a05b17e20f5bf1bf24faf61b6b7cda040259a27c)] Update global.json to "latestMajor"
    * [[#7f126ed](https://github.com/nfdi4plants/Swate/commit/7f126ed2649a68f489d13d00e91ea2d0133ab485)] Modernize library structure
    * [[#91327aa](https://github.com/nfdi4plants/Swate/commit/91327aaed2af5cff83b402ec55ef30ec6da56c8f)] Merge pull request #7 from nfdi4plants/OptionalValueTransition
    * [[#07a9799](https://github.com/nfdi4plants/Swate/commit/07a97999b2f25febf2df25185e2da3ffa17b5391)] add tests for update function of optional values
    * [[#00d2c1e](https://github.com/nfdi4plants/Swate/commit/00d2c1e65cd626e6e38c3df8a3bc7dae413a8ebe)] add Optional list appendage to UpdateAllAppendLists
    * [[#a399c9a](https://github.com/nfdi4plants/Swate/commit/a399c9a9665f0d724b7b126a227437b434072475)] set FSharpSpreadsheetML dependancy to 0.0.2 upwards
* Deletions:
    * [[#d3586ff](https://github.com/nfdi4plants/Swate/commit/d3586ffa889a6e8a9ce3729f15a6d38c66ee3c60)] remove codeCoverage
* Bugfixes:
    * [[#fa4d1c5](https://github.com/nfdi4plants/Swate/commit/fa4d1c50975765e3838fcebe2aa642220938a77e)] fix remove functions fixes #8
    * [[#1216fcc](https://github.com/nfdi4plants/Swate/commit/1216fccd1f1062b12823bb3edeb3c3befc0799dc)] fix parsing of ISATab Aggregated Strings references #5

#### 0.0.3 - Sunday, January 31, 2021

* Switched all ISA Datamodel field to optional to better reflect the json files
* Update API to these datamodel changes
* Update XLSX reader/writer to these datamodel changes
* Fixed bug where aggregated strings in Excel files were not parsed correctly

#### 0.0.2 - Thursday, January 21, 2021

* Increased the minimum version of the System.Json.Text dependancy
* Minor additions to the API functions

#### 0.0.1 - Monday, January 11, 2021

* Moved Project from nfdi4Plants/ArcCommander
* Full ISA Datamodel
* Reworked API
* ISA XLSX Investigation file parser
* Added ISA Json IO
