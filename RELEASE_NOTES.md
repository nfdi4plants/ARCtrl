### 2.4.0+4a309c7 (Released 2025-3-7)


* Additions:
    * [[#d2e1f99](https://github.com/nfdi4plants/ARCtrl/commit/d2e1f99e2652583396491664921f3610e36830c5)] add ArcTable related helper functions
    * [[#c1a34d6](https://github.com/nfdi4plants/ARCtrl/commit/c1a34d64566ada818ed7ba1d8a9473835eaeb203)] Update documentation link in README.md
    * [[#921594e](https://github.com/nfdi4plants/ARCtrl/commit/921594e637cd540943550f19a0618c24dd1d8b8a)] make xlsx controller use async calls in javascript
    * [[#8e78499](https://github.com/nfdi4plants/ARCtrl/commit/8e784996f6d9554c6a232e59db3b1aef0d18b76c)] add some missing XlsxHelper functions
    * [[#5ce39f9](https://github.com/nfdi4plants/ARCtrl/commit/5ce39f9565bb60dc6d52bb6ab82b170b8d565fba)] RO-Crate export: added default publication date
    * [[#5d498cb](https://github.com/nfdi4plants/ARCtrl/commit/5d498cb3dc1850bc36d83ac567eae9fb277fc570)] RO-Crate context: changed headline to name
    * [[#b047349](https://github.com/nfdi4plants/ARCtrl/commit/b0473491018c6b78ff6cfc5101df97f413755ba1)] Added dataFiles to Study RO-Crate export
    * [[#7cfcece](https://github.com/nfdi4plants/ARCtrl/commit/7cfceceea9b05eeaa98fdd8bb7c032d2492c12e6)] add logo references to ReadMe
    * [[#5369bad](https://github.com/nfdi4plants/ARCtrl/commit/5369badb0aaa3aee31ceee8d529e26530c7afa59)] small changes to logos - make hand lighter - higher quality
    * [[#d95455d](https://github.com/nfdi4plants/ARCtrl/commit/d95455dd63d8ecb7bb07f76840f4c39c4af676b0)] added ARCtrl logos
* Bugfixes:
    * [[#e4f9b91](https://github.com/nfdi4plants/ARCtrl/commit/e4f9b911f0a2d6e7583e378db07db8856910e0ef)] fix datamap reader failing for empty datamap files
    * [[#651fd8e](https://github.com/nfdi4plants/ARCtrl/commit/651fd8ed0ef0571760fa89516ff28f3d3cfc67a2)] fix filesystem access related tests failing in linux CI
    * [[#134caf8](https://github.com/nfdi4plants/ARCtrl/commit/134caf89aa544223a3d10948b085d2344f194284)] RO-Crate export: fixed default publication date
    * [[#d353a43](https://github.com/nfdi4plants/ARCtrl/commit/d353a43cdd1f4b1b7ae79757189c4197421dd21c)] RO-Crate export: fixed paths/ids for directories
    * [[#4813991](https://github.com/nfdi4plants/ARCtrl/commit/4813991595a013f0e5c52e7b22c7edb5fd4f249b)] RO-Crate context: fixed conformsTo mapping

### 2.3.1+56da453 (Released 2024-12-5)
* Additions:
    * [[#51eb019](https://github.com/nfdi4plants/ARCtrl/commit/51eb019ae0dc8b3dc7adb1a9e7145b884cca74c1)] rename top-level ARC IO functions

### 2.3.0+13842ba (Released 2024-12-4)
    * Cross-language file access functionality is here!
* Additions:
    * [[#94714ba](https://github.com/nfdi4plants/ARCtrl/commit/94714ba4e9c0d19ddb7524d9bdc6c56b804a60a4)] add level synchronous IO functions for dotnet and python
    * [[#5c1f99e](https://github.com/nfdi4plants/ARCtrl/commit/5c1f99ea046ee6634da97423ba35356e5dc92b23)] finish up basic cross language IO and tests
    * [[#8c6d20d](https://github.com/nfdi4plants/ARCtrl/commit/8c6d20d3a84775caf7b4b5411a8faa7b0fd53f21)] finish up python io
    * [[#ac29af6](https://github.com/nfdi4plants/ARCtrl/commit/ac29af603dc48be54c1e3e62f720a3e574cc1e2e)] finish up first version python arc io
    * [[#a6438eb](https://github.com/nfdi4plants/ARCtrl/commit/a6438eb834fee7724a364a2cf46d5361fbb23ce3)] start working on python arc IO
    * [[#2b754f5](https://github.com/nfdi4plants/ARCtrl/commit/2b754f5ad5494bd3a3e052503f882bc96e5ad58f)] finish up first working version of javascript arc io
    * [[#c20e8c1](https://github.com/nfdi4plants/ARCtrl/commit/c20e8c1cc99ace2c8730fc803aa33b96b011d591)] add preliminary promise catch mechanic for js io tests
    * [[#a5c195b](https://github.com/nfdi4plants/ARCtrl/commit/a5c195bbac29df77ad7d0a5bba17a81108a8b3ba)] some cleanup for js fs IO
    * [[#cca43e8](https://github.com/nfdi4plants/ARCtrl/commit/cca43e8158f96de22ecfb668320f0ebfeb5ee7d4)] add first wip version of js file system access
    * [[#a4a991a](https://github.com/nfdi4plants/ARCtrl/commit/a4a991a6da568adcd837f4fba4965ef442f0f588)] switch all file system access to async/promise functionality
* Deletions:
    * [[#3934047](https://github.com/nfdi4plants/ARCtrl/commit/39340475d8c08859f9f7179811a7cef1a677aea8)] remove second namespace definition in xlsx controller file
* Bugfixes:
    * [[#ce001af](https://github.com/nfdi4plants/ARCtrl/commit/ce001afc2acafa9e9de3413df87c7771b7927fd8)] fix test file path handling for js in linux
    * [[#4dd085a](https://github.com/nfdi4plants/ARCtrl/commit/4dd085a3bb3aaf626771e68d1b648565d8cf086e)] fix js io helpers aganst basic tests
    * [[#2478640](https://github.com/nfdi4plants/ARCtrl/commit/24786403c5f7a88a91c4a7f9967341615ffba78c)] fix js base level IO tests
    * [[#02384d8](https://github.com/nfdi4plants/ARCtrl/commit/02384d88f648ae0c6a8a79ea1b0ff1f3c5bf03c8)] fix tests for dotnet again
    * [[#1929830](https://github.com/nfdi4plants/ARCtrl/commit/1929830192327704f2e72b250f61678b66ce16a0)] further fixes for js file io
    * [[#7bc9e5a](https://github.com/nfdi4plants/ARCtrl/commit/7bc9e5aa2c75e325f2ec0946e68c58cb703007ea)] add exceljs requirement and fix fileysystem helper file name
    * [[#e72c01e](https://github.com/nfdi4plants/ARCtrl/commit/e72c01e93e8640ba589ac93896927d4705078968)] some more fixes for js file io

### 2.2.4+05faa0d (Released 2024-11-19)
* Bugfixes:
    * [[#9d2ce07](https://github.com/nfdi4plants/ARCtrl/commit/9d2ce07368e0bb61e8c4dd4e2b7260c215cbc3dc)] fix technologyPlatform not handling parentheses

### 2.2.3+440a84e (Released 2024-11-18)
* Additions:
    * [[#4429911](https://github.com/nfdi4plants/ARCtrl/commit/442991174695e09ab897014d5de5d97e593f5b0a)] loosen constraint on python requests
    * [[#852686c](https://github.com/nfdi4plants/ARCtrl/commit/852686cee17a3ede043215ae9827c45e3a2e2b0e)] update poetry.lock file
* Bugfixes:
    * [[#12e1a89](https://github.com/nfdi4plants/ARCtrl/commit/12e1a8969bb526a98dbe9db1a8d20df04c3e4bce)] fix data cells not being written as such in spreadsheets
    * [[#f5cc507](https://github.com/nfdi4plants/ARCtrl/commit/f5cc5076ead55eb3cb1c96ce7334f2053c3ac420)] fix template variable names
    * [[#d6ca1e7](https://github.com/nfdi4plants/ARCtrl/commit/d6ca1e7fe977922439b58880a8de25681c17e4cd)] fix python request dependency

### 2.2.2+4aaa6cf (Released 2024-11-7)
* Bugfixes:
    * [[#5aae159](https://github.com/nfdi4plants/ARCtrl/commit/5aae159e074d5c6cc40c8b25072898e77d2fde16)] hotfix parsing of old json-ld samples and sources

### 2.2.1+6f816b8 (Released 2024-11-6)
* Bugfixes:
    * [[#17d6e64](https://github.com/nfdi4plants/ARCtrl/commit/17d6e64c6b213ba9a0b7d7e09b88df0b19457126)] fix additionalProperties being lost in RO-Crate write read

### 2.2.0+fd5eccc (Released 2024-10-24)
* Additions:
    * [[#b8640a7](https://github.com/nfdi4plants/ARCtrl/commit/b8640a7519b1c50a2b48d6720a35949039ba3fa9)] small changes to build-chain according to PR review
    * [[#9e43e81](https://github.com/nfdi4plants/ARCtrl/commit/9e43e81945d7d204d14bed9be413ab2c10634c76)] Merge pull request #446 from nfdi4plants/ro-crate-json
    * [[#e6becb8](https://github.com/nfdi4plants/ARCtrl/commit/e6becb8973ad26cea064968a81a0fe83a0fa195a)] Enable Sequence based metadata data creation for arc assays
    * [[#3c383de](https://github.com/nfdi4plants/ARCtrl/commit/3c383de5768fd4609b2db00bfc35c243b9dc01e4)] Enable the creation of study metadata based on sequences
    * [[#12e5d75](https://github.com/nfdi4plants/ARCtrl/commit/12e5d75da146a9e7831c82a665a41dbe096f854f)] Add excel get and create template meta data from sequence of strings
    * [[#38b9ce8](https://github.com/nfdi4plants/ARCtrl/commit/38b9ce8e0ec768d356c8a448fd92c0e7375143e6)] Adapt styling
    * [[#5e0e10e](https://github.com/nfdi4plants/ARCtrl/commit/5e0e10ef961ee858710535ac362ac291e9f7a338)] Add from and to seq for arc investigation
    * [[#8050b1b](https://github.com/nfdi4plants/ARCtrl/commit/8050b1b4cefba5d243b96a5eff0469c7dcb20754)] Update build-test.yml
    * [[#d4687f3](https://github.com/nfdi4plants/ARCtrl/commit/d4687f3e4c61f7495fb9fd4ec913841d12e61d60)] Added unit tests for investigation
    * [[#8301e02](https://github.com/nfdi4plants/ARCtrl/commit/8301e0246d1c8584d9097faee19146b9c5d465fa)] restore package.json of ARCtrl
    * [[#f525619](https://github.com/nfdi4plants/ARCtrl/commit/f5256198da2c8b4beef2c25fcbc0eaa6b4d1e480)] restore pagages.lock.json of ARCtrl
    * [[#536b988](https://github.com/nfdi4plants/ARCtrl/commit/536b988e182fe35aa3ae0c6bf697197c7e4a31bd)] Apply review changes
    * [[#79f4fde](https://github.com/nfdi4plants/ARCtrl/commit/79f4fdef1c2ee756872e33ca7cfdaa0484ea5a2b)] Adapt parameter styling
    * [[#36f78d4](https://github.com/nfdi4plants/ARCtrl/commit/36f78d425a11f7a11015cecab8b42af52910516e)] small undo of styling change
    * [[#0f9028c](https://github.com/nfdi4plants/ARCtrl/commit/0f9028cecaca805fc17aa2997604eaedaf81919d)] Merge pull request #451 from nfdi4plants/Feature_Enable_Sequence_Based_Metadata_Creation
    * [[#25c8d3c](https://github.com/nfdi4plants/ARCtrl/commit/25c8d3caadcdf9e005a3a45ba908a028081d8561)] include ARCtrl Common props in package for downstream fable transpilation
    * [[#97a3d46](https://github.com/nfdi4plants/ARCtrl/commit/97a3d4644aebb9f4c7b15c3be3da8e447bd1e708)] move jsonIO dependencies from shared props file directly into project files
    * [[#850e889](https://github.com/nfdi4plants/ARCtrl/commit/850e889906fb28b845fa0fe4c49c95e0157bcacf)] created "All" Tests project which bundles all other tests
    * [[#d5d5201](https://github.com/nfdi4plants/ARCtrl/commit/d5d5201898cdfe9bb365c902d6f33129a2f3e19a)]  #433: Add api functions for mandatory dynamic props on some classes
    * [[#00c1ca7](https://github.com/nfdi4plants/ARCtrl/commit/00c1ca7d49c70cab98deec118bd75dcef6ac20df)] add requested changes, add at least some tests
    * [[#d35c76a](https://github.com/nfdi4plants/ARCtrl/commit/d35c76a83596ddf0df924dbae12e0e98675a5124)] start working in potential typed ro-crate-json parser
    * [[#9f9b9c7](https://github.com/nfdi4plants/ARCtrl/commit/9f9b9c7119a5d0eedc97b8c6a8f6543fcd13064f)] finish up first version of ROCrateObject serialization
    * [[#d9fbb33](https://github.com/nfdi4plants/ARCtrl/commit/d9fbb33f95e46e3d505e3c1d6404b437c592bf36)] add ROCrateObject decoder tests
    * [[#100fc34](https://github.com/nfdi4plants/ARCtrl/commit/100fc342aaf1990f704ca287d55fa3efe2c071ef)] Enable check for metadata sheetname and is meta data sheet
    * [[#fc2f7d9](https://github.com/nfdi4plants/ARCtrl/commit/fc2f7d92924b9357bbf000665a50061c2170e636)] rename ROCrateObject to LDObject
    * [[#a0eeeed](https://github.com/nfdi4plants/ARCtrl/commit/a0eeeed8bc8d73e807c2a779994da907cd9efa76)] Implented review changes
    * [[#081ca04](https://github.com/nfdi4plants/ARCtrl/commit/081ca0452bad26245525872637662e18bdb16bef)] Add cwl model
    * [[#ef04da1](https://github.com/nfdi4plants/ARCtrl/commit/ef04da1849b3757d180446fcaa095fed2a32f8b3)] Add cwl decode functions
    * [[#8cfb04c](https://github.com/nfdi4plants/ARCtrl/commit/8cfb04c188f8b94db04968b7610f7bd886bce185)] Add testing project
    * [[#3da6974](https://github.com/nfdi4plants/ARCtrl/commit/3da6974cea3440f17e7dacefb7c6ed79bf07e50c)] change naming of CWL to CWLProcessingUnits
    * [[#1932c44](https://github.com/nfdi4plants/ARCtrl/commit/1932c44952c0cd818f15eddee7a0e186d3ded67e)] add test strings
    * [[#6ae8e47](https://github.com/nfdi4plants/ARCtrl/commit/6ae8e47c6e7ac2fa98425885249b4f0ab12d98ae)] Add cwl test
    * [[#a9e64d3](https://github.com/nfdi4plants/ARCtrl/commit/a9e64d3f94c697293d4ff898f08295b5ed2e52ce)] rewrite tests to be more clear
    * [[#a59d834](https://github.com/nfdi4plants/ARCtrl/commit/a59d8346f2fe8981fd56780e60fce16db950024f)] reorganize tests
    * [[#3c5e2f8](https://github.com/nfdi4plants/ARCtrl/commit/3c5e2f8dfeb1f0cc352ab704b48b5bea2e13c41d)] dix dockerFile requirement
    * [[#3c6c0a3](https://github.com/nfdi4plants/ARCtrl/commit/3c6c0a3d88ea8104c29970ed6eeb6efcafd513b0)] start separating processunits for better modularity / edge case handling
    * [[#13d8a9c](https://github.com/nfdi4plants/ARCtrl/commit/13d8a9c75178e4d5427608002e06e7c3e0dfdaf5)] reorder and specify wf specific types
    * [[#c7a0082](https://github.com/nfdi4plants/ARCtrl/commit/c7a00821ed9ad34deabe358bef05c5d61f6cc277)] change in-/outputs to dynamicObj for more flexibility
    * [[#9930238](https://github.com/nfdi4plants/ARCtrl/commit/993023849ef4e1fec616b059468b836e917a0cb8)] add workflow steps
    * [[#0d049b1](https://github.com/nfdi4plants/ARCtrl/commit/0d049b16aeb5acafc412aeb804f35332e49694a8)] add workflow description type
    * [[#ccefbff](https://github.com/nfdi4plants/ARCtrl/commit/ccefbffe4887726d0be30347c41e0e357579a470)] add steps decoding
    * [[#3db45c6](https://github.com/nfdi4plants/ARCtrl/commit/3db45c64eff8b521907495a992fee34ddde79463)] add WorkflowSteps tests
    * [[#95dab47](https://github.com/nfdi4plants/ARCtrl/commit/95dab475dc58269b1983af26d9e35e65fc6c12e9)] support direct type mappings for inputs and outputs
    * [[#0fd5be9](https://github.com/nfdi4plants/ARCtrl/commit/0fd5be97fc9e5733e7e01836462b0c2d2b1580e1)] add overflow decoder
    * [[#7007a0b](https://github.com/nfdi4plants/ARCtrl/commit/7007a0b1e58f8438a47beabdcfcb288716abe3e6)] add cmd tool metadata and tests
    * [[#3fca0c1](https://github.com/nfdi4plants/ARCtrl/commit/3fca0c11671bd3cd3f75f5473bb43c1baf793a0b)] update fsproj
    * [[#4ff39f9](https://github.com/nfdi4plants/ARCtrl/commit/4ff39f9f55b85a89f123de5db4aedb5dc3433c7d)] Update YAMLicious to version 0.0.2
    * [[#ff5af9c](https://github.com/nfdi4plants/ARCtrl/commit/ff5af9c51afc9f2268f3bbab054737ededd9fb87)] rename Type to Type_ for fable
    * [[#379efb4](https://github.com/nfdi4plants/ARCtrl/commit/379efb4d4e0c0fb023947557c927dbb785dba609)] add failcase to requirements
    * [[#44112a8](https://github.com/nfdi4plants/ARCtrl/commit/44112a8638930494eabd379418b255df982fcd9a)] add wf requirements to decoder
    * [[#8b6963e](https://github.com/nfdi4plants/ARCtrl/commit/8b6963edfa4a83f891d3e029c709054ce7aa4d33)] trim strings for python
    * [[#7728ebb](https://github.com/nfdi4plants/ARCtrl/commit/7728ebbd168ff4f4b0301b81460149f89655e973)] update yamlicious version
    * [[#1222f01](https://github.com/nfdi4plants/ARCtrl/commit/1222f018133f51baebd6d5faafc7ca12067056d5)] add workflow decoding
    * [[#83cad3e](https://github.com/nfdi4plants/ARCtrl/commit/83cad3e0579d01ffa5e5d04b938efa65ef300908)] add cwl to all test list
    * [[#f452622](https://github.com/nfdi4plants/ARCtrl/commit/f452622bde4bf2b10816a3d883869a8b6d3f3cfb)] rename types and tests
    * [[#e6d9902](https://github.com/nfdi4plants/ARCtrl/commit/e6d9902b573d1c3271957cf20877b5475e2b3024)] add attachmembers attribute
    * [[#070ca96](https://github.com/nfdi4plants/ARCtrl/commit/070ca96665fbbe241b379b64ae8576f89c7808f9)] switch to resizearray and add some comments
    * [[#090a214](https://github.com/nfdi4plants/ARCtrl/commit/090a21479fed7a818dde644b1fcbeb42ade796fc)] add optional field to inputs
    * [[#de6ef63](https://github.com/nfdi4plants/ARCtrl/commit/de6ef63c19bace7c567691d8f074923dabb5cba1)] change version and class to optional on tool and wf descriptions
    * [[#fedc6d4](https://github.com/nfdi4plants/ARCtrl/commit/fedc6d4a7198db9bc2bbfa749f0965f27d1fbf3b)] reorganize modules
    * [[#77d1054](https://github.com/nfdi4plants/ARCtrl/commit/77d105451f14a5d63fbefc3c8443a22009f5f23b)] make OntologyAnnotation work with localID as TAN
    * [[#c3ed011](https://github.com/nfdi4plants/ARCtrl/commit/c3ed011ece9c38e874d954e062b897b0a9d985cf)] add template copy tests
    * [[#d9e4ae4](https://github.com/nfdi4plants/ARCtrl/commit/d9e4ae4157c19ecaa13be13a690e0115f8120365)] small cleanup of CompositeHeader
    * [[#68fc64e](https://github.com/nfdi4plants/ARCtrl/commit/68fc64e37032b26e7748e2e545096412f0a3596e)] add data cell update tests for ArcTable Copy function
    * [[#1edfd80](https://github.com/nfdi4plants/ARCtrl/commit/1edfd80dbc37dc26e48cc305d831fb974847f98f)] add test against correctly writing valueless ArcTable
    * [[#3b0fa76](https://github.com/nfdi4plants/ARCtrl/commit/3b0fa76489e1346cd30c3170bf48a47b00c00446)] treat empty strings in OntologyAnnotation as None
    * [[#f4cad69](https://github.com/nfdi4plants/ARCtrl/commit/f4cad6964a6fc815f23e98be18e5666ed3e56dbc)] set RO-Crate empty unit test to pending
* Deletions:
    * [[#12eb2b9](https://github.com/nfdi4plants/ARCtrl/commit/12eb2b92018b8ef664853796374d6acf1fa85d3a)] remove leftover catching of erros in dotnet packaging
    * [[#b038cfc](https://github.com/nfdi4plants/ARCtrl/commit/b038cfc1fd26c9bcca559c898d20e2f4943e4722)] Add '@context' support: - instance methods and static methods for set/tryGet/remove - tests for all classes
    * [[#f53e73f](https://github.com/nfdi4plants/ARCtrl/commit/f53e73f8adfcf39c820d3b67a91565594a36cf05)] remove try with for dirent initial workdir
    * [[#10e40ba](https://github.com/nfdi4plants/ARCtrl/commit/10e40baef1e30f56bce21a3d1cfb9b7874436667)] separate processing units and remove module
    * [[#b783782](https://github.com/nfdi4plants/ARCtrl/commit/b7837828e800818dbd91f884ebcf7a48b988e409)] remove type CLWClass :(
    * [[#844ca8b](https://github.com/nfdi4plants/ARCtrl/commit/844ca8be60907c50ffaaeb427642240a70b96958)] remove underlying modules
* Bugfixes:
    * [[#b2b6e8b](https://github.com/nfdi4plants/ARCtrl/commit/b2b6e8b04d0dc8e04e45e247e68ef1781bc404e1)] Merge pull request #452 from nfdi4plants/nugetFix
    * [[#55d360f](https://github.com/nfdi4plants/ARCtrl/commit/55d360fff020aab8ec9b81a74ef8bdb0e8a67629)] fix rebase conflicts, update to DynObj v4.0.3, remove Directory.Build.props
    * [[#96231bc](https://github.com/nfdi4plants/ARCtrl/commit/96231bce5763421edde1d6dd0600ad35410956a9)] add ROCrateObject writer tests and fix accordingly
    * [[#cc77790](https://github.com/nfdi4plants/ARCtrl/commit/cc77790d27c4466aed5af8b4bd781d8d989b1514)] hotfix ROCrateObject json writer writing internal keys as fields in json
    * [[#63f8cdc](https://github.com/nfdi4plants/ARCtrl/commit/63f8cdc84547ff3badb50667711a73179c2c3747)] fix references in decode
    * [[#cd3cdee](https://github.com/nfdi4plants/ARCtrl/commit/cd3cdee21ef685366919aa537e796d33d927dbe0)] fix cwl type naming
    * [[#9c36d2d](https://github.com/nfdi4plants/ARCtrl/commit/9c36d2d8b0baf5aa94f99a6960e5126ec56e9bec)] fix test fsproj
    * [[#bbc5a42](https://github.com/nfdi4plants/ARCtrl/commit/bbc5a4218825e40454898a5ee574cc95460fc415)] fix optional fields
    * [[#86913f6](https://github.com/nfdi4plants/ARCtrl/commit/86913f6cdfa3cab39d552ee543ade062e4390557)] fix EnvVarRequirement
    * [[#27c8037](https://github.com/nfdi4plants/ARCtrl/commit/27c8037c21f05ebd887d67414fe1052d5ea940df)] fix SchemaDefRequirement
    * [[#f4a662d](https://github.com/nfdi4plants/ARCtrl/commit/f4a662d4008df4ace3727a9f2b9134d3e1bdeab8)] fix SoftwareRequirements
    * [[#e44e736](https://github.com/nfdi4plants/ARCtrl/commit/e44e736bd8d8345cbf706d927dc91f475817fc60)] fix workdirReq and resourceReq
    * [[#01a3c04](https://github.com/nfdi4plants/ARCtrl/commit/01a3c04161f392ae21a5659bf168f2b912532182)] fix direct map decode
    * [[#17fabe8](https://github.com/nfdi4plants/ARCtrl/commit/17fabe8af66078f9f0834acb631c7a332a9ef4f0)] fix for new dynamic obj version
    * [[#548b2b7](https://github.com/nfdi4plants/ARCtrl/commit/548b2b754913ab043763ecfd82bd76289f7f5856)] add metadata and dynobj testing (fix test runs)
    * [[#cb87cba](https://github.com/nfdi4plants/ARCtrl/commit/cb87cbae068291812dabb43f7e0926ab7eb19973)] Implement review fix
    * [[#b335ddc](https://github.com/nfdi4plants/ARCtrl/commit/b335ddcfc97ea873d95320ca343d2bc67ba21641)] test and fix ArcTable copy member deep copying ontology annotations
    * [[#b71fb4b](https://github.com/nfdi4plants/ARCtrl/commit/b71fb4b68dd383da00d236d61991a1d8d8a26889)] minimal hotfix for writing bodyless tables

### 2.1.0+99b2659 (Released 2024-10-2)
* Additions:
    * [[#28cfd13](https://github.com/nfdi4plants/ARCtrl/commit/28cfd133152fa3cdc6606b4ccb7cda3c340e8f62)] add rocrate project
    * [[#f71faea](https://github.com/nfdi4plants/ARCtrl/commit/f71faea334538425d5f92e81c650b1cdd46446ef)] rocrate datamodel wip based on inheritance and single interface
    * [[#ef15f35](https://github.com/nfdi4plants/ARCtrl/commit/ef15f35779606a1bfcaecdadfc7532886466ec46)] implement first POC of ISA ROCrate profile
    * [[#168ead8](https://github.com/nfdi4plants/ARCtrl/commit/168ead84ca93dc0fae911e760673ef6e33180447)] add test project
    * [[#2cea57c](https://github.com/nfdi4plants/ARCtrl/commit/2cea57cc1415310e36f5931ef376ea183b85fb71)] update solution
    * [[#ccd1805](https://github.com/nfdi4plants/ARCtrl/commit/ccd18053a83c80f4ca4b7a2ac7008a4a0cf3d348)] Unify Inheritance to LDObject
    * [[#95cab6f](https://github.com/nfdi4plants/ARCtrl/commit/95cab6f0f85f71a7adea887a25bd47c714e5af35)] wip ro-crate tests
    * [[#9ef48d7](https://github.com/nfdi4plants/ARCtrl/commit/9ef48d75f5439d3a71ca943941b70c5af3ff9261)] only use primary constructor
    * [[#9deffb7](https://github.com/nfdi4plants/ARCtrl/commit/9deffb7ea81f76c067de5618f2b3388303402191)] Add basic tests for I/S/A LDObjects
    * [[#4b4a2fc](https://github.com/nfdi4plants/ARCtrl/commit/4b4a2fcc3bd32b31ec5aa3b4f1127883f7af83fc)] temp workaround in tests for https://github.com/CSBiology/DynamicObj/issues/25
    * [[#f67e78b](https://github.com/nfdi4plants/ARCtrl/commit/f67e78b068350a8e1aecf80fa8b9dd4646fb9ddd)] Use unblocking version of DynamicObj, introduce runTestProject target
    * [[#4e91e9d](https://github.com/nfdi4plants/ARCtrl/commit/4e91e9dcc5b34747a2e4a16f33aee1a227eb08ef)] finish basic property tests for isa profile types
    * [[#8b4368a](https://github.com/nfdi4plants/ARCtrl/commit/8b4368aed9a9705a78f95ec752143b1620fb0408)] Merge pull request #426 from nfdi4plants/ro-crate-data-model
    * [[#fdc4773](https://github.com/nfdi4plants/ARCtrl/commit/fdc4773125fb37bf7140476460a117e575e420ae)] add tryGetColumnByHeaderBy member and static + tests
    * [[#1a5c4d6](https://github.com/nfdi4plants/ARCtrl/commit/1a5c4d64cdd3c591a2c7af73c4bdb7a9261c5ffb)] Update ArcTable.Tests.fs
    * [[#29d1bb2](https://github.com/nfdi4plants/ARCtrl/commit/29d1bb250a2fc6ee4253e8a94f820f03bf87a09d)] update to thoth.json.core 0.4.0 and use temporary FsSpreadsheet implementations for js and py
    * [[#2e47979](https://github.com/nfdi4plants/ARCtrl/commit/2e479798a7812233b109da566db68246fc9df7a2)] start working on ro-crate-json parsing
    * [[#f6afa53](https://github.com/nfdi4plants/ARCtrl/commit/f6afa53b83b8003f16c7881a8bd7ea958466c5fb)] first buildable version after fable restructure
    * [[#755879d](https://github.com/nfdi4plants/ARCtrl/commit/755879dbf3d30d1305ae8645fdf84338ce89c26a)] move duplicate project references into props files
    * [[#93030e9](https://github.com/nfdi4plants/ARCtrl/commit/93030e915329c23b4b4f8cddc5b622312de6cedb)] Make use of Fable.Package.SDK
    * [[#99b2659](https://github.com/nfdi4plants/ARCtrl/commit/99b2659d1dd5c3815693f4df85691f9bead3e26d)] rename json IO implementations folder to reduce ambiguity in py and javascript packages
* Deletions:
    * [[#c71b8a3](https://github.com/nfdi4plants/ARCtrl/commit/c71b8a301fbdf95531a9ba0d33f8e6d0d238adc9)] correct interface implementation on LDObject, remove interface implementation from Dataset
    * [[#c429686](https://github.com/nfdi4plants/ARCtrl/commit/c4296864a7443a990257f7eb47fed92458fa0a7b)] remove javascript and pyton packages from main solution
    * [[#3586dee](https://github.com/nfdi4plants/ARCtrl/commit/3586dee6a736d3ad2ed9e79778e3ea136c638eac)] remove restorelockedmode flag
* Bugfixes:
    * [[#9d526b8](https://github.com/nfdi4plants/ARCtrl/commit/9d526b82df1d7ac04283cbe23c9832f1d5818771)] add LabProcess tests, fix schematype of LDObject base constructor
    * [[#192812d](https://github.com/nfdi4plants/ARCtrl/commit/192812d43569270845883ca116de9931dea6ba62)] fix error messages of ROCrate testing utils
    * [[#43d65db](https://github.com/nfdi4plants/ARCtrl/commit/43d65db3fb121a4832e771541880c1fe79cfe89c)] various test fixes for Fable logic transition
    * [[#8552f9e](https://github.com/nfdi4plants/ARCtrl/commit/8552f9ea7b2183c4cb382049c71793918d4128b3)] fix fable ready packaging and/by update package tags

### 2.0.1+5e6cc32 (Released 2024-8-29)
* Bugfixes:
    * [[#bfaaaf6](https://github.com/nfdi4plants/ARCtrl/commit/bfaaaf614a1a2dc788a1b7de17761b5867aa9fee)] fix single dot in paths being interpreted as folder

### 2.0.0+6c267dd (Released 2024-8-27)
* Additions:
    * [[#c250f52](https://github.com/nfdi4plants/ARCtrl/commit/c250f52322e6c22714b02dccb37b93ca2a9cb5d8)] add RO-Crate contexts to docs
    * [[#55ad878](https://github.com/nfdi4plants/ARCtrl/commit/55ad87892d23a77517cdbe380f5bc4e75cea2744)] Update fsspreadsheet
    * [[#263db68](https://github.com/nfdi4plants/ARCtrl/commit/263db68d22addaa26552a85a4dd86eac29c2153f)] include fable files
    * [[#c59b883](https://github.com/nfdi4plants/ARCtrl/commit/c59b8834d09fcdae7e47b575c4d2dff316c44a18)] Add ARCtrl.ValidationPackages project: - base type model for ValidationPackages and the config file
    * [[#cc879fd](https://github.com/nfdi4plants/ARCtrl/commit/cc879fd276e25d4ead4c9b5051f839567e1a745e)] Add ARCtrl.Yaml: - Uses YAMLicious - tries to mirror ARCtrl.Json - Encoder/Decoder logic for ValidatioNpackages type model
    * [[#c5a9e7c](https://github.com/nfdi4plants/ARCtrl/commit/c5a9e7c8f25352437c757147203f6df327957da4)] Add contracts for ValidationPackagesConfig
    * [[#532d310](https://github.com/nfdi4plants/ARCtrl/commit/532d310e4f149a4a7ff48e62d98e836db85982a1)] Update to .NET 8, requested PR changes
    * [[#137d885](https://github.com/nfdi4plants/ARCtrl/commit/137d885f1a8a76c43fca0e1d631ad70c9eae8667)] add fable files
    * [[#87ed783](https://github.com/nfdi4plants/ARCtrl/commit/87ed783823ebfa9e56486cd2d5edcb2e406216cd)] add the fable files for real. fable be fablin'
    * [[#8b55020](https://github.com/nfdi4plants/ARCtrl/commit/8b55020d557e39b78a7f512ff29370bfd53e98aa)] Add classes for index files :sparkles: #386
    * [[#27d259a](https://github.com/nfdi4plants/ARCtrl/commit/27d259a22b3d357798ac190b867b33972814033d)] Rename ARCtrl.Path to ARCtrl.ArcPathHelper #398
    * [[#3e427f1](https://github.com/nfdi4plants/ARCtrl/commit/3e427f188ecc5244fffa63e534314e30699efb29)] Add CompositeCell.ToDataCell #404
    * [[#bf79874](https://github.com/nfdi4plants/ARCtrl/commit/bf798749e0de183502ca52df000943a06ccccee2)] Unifiy expected functionality of CompositeCell conversion utility #402
    * [[#8432507](https://github.com/nfdi4plants/ARCtrl/commit/8432507efc446ca93f7c0d333a0bbd8d79a96395)] Add api to include datamap in assay/study #406
    * [[#b00b17e](https://github.com/nfdi4plants/ARCtrl/commit/b00b17eaf6c397e3d3ff30795199767697ab0572)] add swate prerelease target
    * [[#486d6a7](https://github.com/nfdi4plants/ARCtrl/commit/486d6a7e611c739452aab1d11003478c9940cf62)] rmv comment
    * [[#d036c31](https://github.com/nfdi4plants/ARCtrl/commit/d036c3161ddc53118bed5aeb5be2b32102ec6b76)] add first json logic for datamap :construction:
    * [[#22b48f5](https://github.com/nfdi4plants/ARCtrl/commit/22b48f54ae2992269a4ab891035653c5712cf0ec)] Add YAML tests for Validation packages, use custom equality
    * [[#b510ab2](https://github.com/nfdi4plants/ARCtrl/commit/b510ab2b793c87a6ce0a70fd92ae99791624f307)] add a test placeholder -.-
    * [[#9c70c5d](https://github.com/nfdi4plants/ARCtrl/commit/9c70c5dffea4a28f262f972651b712bf0e5ffefa)] Add contract handling for `validation_packages.yml` to top-level ARC API
    * [[#263db27](https://github.com/nfdi4plants/ARCtrl/commit/263db27e067fb1c6af81211be15d6eb80390783a)] wip
    * [[#6273594](https://github.com/nfdi4plants/ARCtrl/commit/6273594c1a4b7b4813ddffd4a0b0f9beea095892)] Add instance method tests for ARCtrl.ValidationPackages
    * [[#4101dcb](https://github.com/nfdi4plants/ARCtrl/commit/4101dcb2b0f38e80f77501c5e2dadd1fe422b7a4)] Add Contract tests for ValidationPackagesConfig
    * [[#995e2f8](https://github.com/nfdi4plants/ARCtrl/commit/995e2f8bef5c5f9510aa3cf9f77b7968d16eb271)] Add topm level api tests for ValidationPackagesConfig
    * [[#8de0653](https://github.com/nfdi4plants/ARCtrl/commit/8de0653bfd3d361b8571f2c59eb65823e1832022)] Use literal for yaml keys (also in tests)
    * [[#d64f841](https://github.com/nfdi4plants/ARCtrl/commit/d64f841685b5314627ef6d6829cceeb9d1dc88a3)] Use more Record-like ToString() for ValidationPackages and ValidationpackagesConfig
    * [[#88c9b9d](https://github.com/nfdi4plants/ARCtrl/commit/88c9b9d831f8918e7eadb511cdaf0e7baf9bfbdf)] add addColumnFill member function with tests
    * [[#e68ca44](https://github.com/nfdi4plants/ARCtrl/commit/e68ca449ba9aa741e96f4bb9195ac3f354625ac7)] Additional tests for AddColumnFill
    * [[#df0bda9](https://github.com/nfdi4plants/ARCtrl/commit/df0bda98b004caf1896c91a5d9d69c50885b01d3)] Reviewed AddColumnFill member function and tests
    * [[#94fba28](https://github.com/nfdi4plants/ARCtrl/commit/94fba285c10678314a9ae7fd47c146862109b77a)] add ARC.ToROCrateJsonString member #344
    * [[#064b3e0](https://github.com/nfdi4plants/ARCtrl/commit/064b3e0beb10b34cb28d32db7f55342bf5479d22)] added functions for checking assay and study metadata sheet names #414
    * [[#c5a929d](https://github.com/nfdi4plants/ARCtrl/commit/c5a929df8b140e9bb749fe2f01db386ccadd818c)] check identifiers when creating assay and study objects #415
    * [[#d40fea9](https://github.com/nfdi4plants/ARCtrl/commit/d40fea9442d34a3b7fa6bff76682423fdcff5992)] add CompositeHeader.IsUnique #416
    * [[#de4246f](https://github.com/nfdi4plants/ARCtrl/commit/de4246f16d09b81cd94f18784772618bb095a6c8)] add  ArcTable cell accession and manipulation functions
    * [[#485d778](https://github.com/nfdi4plants/ARCtrl/commit/485d778ed49f99b1ae7994d1a1e764ea08091e95)] make process conversion more robust against different outputs
    * [[#0b85fcd](https://github.com/nfdi4plants/ARCtrl/commit/0b85fcd8fc825866c10792465a33c1cf81b92e08)] base spreadsheet parsers on string columns
    * [[#142b6a2](https://github.com/nfdi4plants/ARCtrl/commit/142b6a20c3df620822bfe602136040d3b62fd944)] change main types of spreadsheet parsing collections from list to array
    * [[#61eec9f](https://github.com/nfdi4plants/ARCtrl/commit/61eec9fd15e4de9b7cd3771c4e488d6722aa7964)] small adjustments to renaming contract handling
    * [[#56f3a7a](https://github.com/nfdi4plants/ARCtrl/commit/56f3a7afaec3572882b1b67b0890ac346aa0ef93)] add manage-issues.yml
    * [[#ee1091f](https://github.com/nfdi4plants/ARCtrl/commit/ee1091f6ece4d248dc18fa5e045cbbd525c60d76)] rework datamap to be represented as a row of records
    * [[#2261883](https://github.com/nfdi4plants/ARCtrl/commit/22618830d79537c2f3cc10e2f69567432b0e8594)] adjust spreadsheet parsing to datamap datamodel change
    * [[#aa28494](https://github.com/nfdi4plants/ARCtrl/commit/aa284947a4bc69a55158495866d3edb79a2737fb)] update templates json parsing
    * [[#4358b6a](https://github.com/nfdi4plants/ARCtrl/commit/4358b6aad3d6d353c8271d52e8666c82de9f66c7)] increase robustness against OntologyAnnotation mutability issues
    * [[#44c4ded](https://github.com/nfdi4plants/ARCtrl/commit/44c4dedec6b997611cc09a295dbc1056ec28f64d)] add new uri parsing rules and core tests
    * [[#758a083](https://github.com/nfdi4plants/ARCtrl/commit/758a083841c7b4358265292dc4d52a651c7647ad)] make ArcTable header parsing patterns less strict
    * [[#ab38980](https://github.com/nfdi4plants/ARCtrl/commit/ab3898052db88d200c589596360e20d2675a6c84)] add toJsonString extensions members
    * [[#108f6f4](https://github.com/nfdi4plants/ARCtrl/commit/108f6f446b1864fede67446a1ec9b4e6fb953645)] start implementing @id in isa json writer logic
    * [[#70f9047](https://github.com/nfdi4plants/ARCtrl/commit/70f9047bc40ad258821bc28835b808154ec84961)] finish implementing @id in isa json writer logic
    * [[#8bd5d21](https://github.com/nfdi4plants/ARCtrl/commit/8bd5d21c461956c6e2e6a279563daa6425545078)] add study test file for @id json writer
    * [[#85a8e1b](https://github.com/nfdi4plants/ARCtrl/commit/85a8e1bee819540bd37a24aa865b1df664e16ef2)] add idmap test case for study
    * [[#8c93928](https://github.com/nfdi4plants/ARCtrl/commit/8c93928cfab0831c4ba3f86a662abe25b89628dd)] change encoding order of assay properties for isa json parsing
    * [[#e138dcc](https://github.com/nfdi4plants/ARCtrl/commit/e138dcc0ddde2ed0d3561e8c3de90dcc3a188975)] add useIDReferencing flag to JsonController
    * [[#8c192dd](https://github.com/nfdi4plants/ARCtrl/commit/8c192dde7df77bc8d9597d550cbc6734545124e0)] small change in ArcTable.ToString function to make it more robust
    * [[#4173454](https://github.com/nfdi4plants/ARCtrl/commit/4173454f557263df30b8359810a2a13b54d8fd4b)] small speed improvement to ArcTable move column
    * [[#fceee84](https://github.com/nfdi4plants/ARCtrl/commit/fceee84badd6bd2bd5c87d64336c2c4aeac15a3a)] add column move function to ArcTable
    * [[#3620782](https://github.com/nfdi4plants/ARCtrl/commit/362078266da41eb936f8e60a561a963096170d25)] added comparison for ontologyannotation and compositeHeader
    * [[#0ce1c7c](https://github.com/nfdi4plants/ARCtrl/commit/0ce1c7ced4eed0a1a0d3e48bfc66345b1b38d437)] added fable helper function for comment header creation
    * [[#a5f299b](https://github.com/nfdi4plants/ARCtrl/commit/a5f299b2e247845b8b43f15e461860c5634e466c)] add some more comments to isa processType conversion
    * [[#8ef35c2](https://github.com/nfdi4plants/ARCtrl/commit/8ef35c2c37b2d7311e58a8da26e96272f08267c9)] add comment column json and spreadsheet parser
    * [[#ead29a5](https://github.com/nfdi4plants/ARCtrl/commit/ead29a5b9c6a871b8b3d6e4c3f5e77bcdf7f6c8d)] add comment column header and process converter
    * [[#0b73da2](https://github.com/nfdi4plants/ARCtrl/commit/0b73da2a4243f1ad361ee950ea85e544d7c13c8b)] add study rename logic
    * [[#57f3a2e](https://github.com/nfdi4plants/ARCtrl/commit/57f3a2e6842b1d0a8c6cadab118e45557d47ed9a)] add rename contract and assay rename logic
    * [[#f565309](https://github.com/nfdi4plants/ARCtrl/commit/f56530900bb487f71b25e226a7a76959e21ba1eb)] make ArcTables construction fail for duplicate table names #332
    * [[#d986c1a](https://github.com/nfdi4plants/ARCtrl/commit/d986c1ad2e263dfdb72267df36161f52084addc8)] add some additonal info to pyproject.toml
    * [[#99d94a5](https://github.com/nfdi4plants/ARCtrl/commit/99d94a5ef020f155f0c3d8a0fad466dc3fba90e8)] reinclude inclusion of source files in fable folders
    * [[#d37a71e](https://github.com/nfdi4plants/ARCtrl/commit/d37a71e44128e910dfd8c6e03d1905b7bbe9d35d)] Update scripts
    * [[#e9c0a2e](https://github.com/nfdi4plants/ARCtrl/commit/e9c0a2e280dcf6ccce0d4008b89ffcee7d12d772)] Rename one file and rmv most code from md files for maintainability :fire:
    * [[#4a2666f](https://github.com/nfdi4plants/ARCtrl/commit/4a2666f7673a0a5e95a13f1f9c29a4474a14b107)] Update GettingStarted.md
    * [[#0fab876](https://github.com/nfdi4plants/ARCtrl/commit/0fab876dba13de32cca3eb44fdea20647ca089c5)] Finalize docs :book:
    * [[#5194df1](https://github.com/nfdi4plants/ARCtrl/commit/5194df1bf2c1c5a45d63033123aeea5c89cca7c4)] Update EXAMPLE_CreateAssayFile.js
    * [[#56ebceb](https://github.com/nfdi4plants/ARCtrl/commit/56ebcebf44984ffbf58a31be3fddccb37fe956fb)] Update js docs : book:
    * [[#a9b4fa1](https://github.com/nfdi4plants/ARCtrl/commit/a9b4fa1ecba3a229f88cf09b0797defbfe1787ee)] added tests to tab-json conversion and parsing
    * [[#3916bc4](https://github.com/nfdi4plants/ARCtrl/commit/3916bc45d8807392791edf8aa80046d15f30207d)] replace "Raw Data File", "Derived Data File" and "Image File" column headers with "Data" #93
    * [[#32e1885](https://github.com/nfdi4plants/ARCtrl/commit/32e18858d961571110963c0aab600827c5fea86f)] add tests for backwards compatability of data headers
    * [[#a55279a](https://github.com/nfdi4plants/ARCtrl/commit/a55279ac54c1752ed6bb893817dc3e1f31d6af96)] add datamap spreadsheet parsing
    * [[#239457a](https://github.com/nfdi4plants/ARCtrl/commit/239457a206a04776fc8eb9fe49d029e8ed2b0f01)] add datamap spreadsheet parser tests
    * [[#b54b5a5](https://github.com/nfdi4plants/ARCtrl/commit/b54b5a554377c452d69f195faa7e5fe3574fc9b4)] add datamap contract handling
    * [[#db9fbf8](https://github.com/nfdi4plants/ARCtrl/commit/db9fbf8a8f19120c246e4c1d8abee1cc8e2d4ab7)] add datamap contract handling
    * [[#890dfd6](https://github.com/nfdi4plants/ARCtrl/commit/890dfd642a4e9589fa7aaed53938953b168fe7de)] add datamap contract handling tests
    * [[#08fcb3b](https://github.com/nfdi4plants/ARCtrl/commit/08fcb3b32c287e0f1fb9d4876905a15382758e7d)] update Data and CompositeCell json parsing
    * [[#8c67c36](https://github.com/nfdi4plants/ARCtrl/commit/8c67c36863569e401ac561651f16aaa9ec635ea3)] add json parser and tests for updated data objects
    * [[#e5e666e](https://github.com/nfdi4plants/ARCtrl/commit/e5e666eaff8fc3309adf8ae653bb95738489cc2a)] move data file outside of process folder
    * [[#ecd9a17](https://github.com/nfdi4plants/ARCtrl/commit/ecd9a1737934392178b57d25143c205e0cbf0920)] add pending test for losless data parsing in isa json
    * [[#66beafa](https://github.com/nfdi4plants/ARCtrl/commit/66beafac306e18d56ef2c5edc969e7eb10b4ea2f)] update prerelease mechanism
    * [[#98c5228](https://github.com/nfdi4plants/ARCtrl/commit/98c5228627ceb3718915d1577fa1486e05912c87)] Add ToString overrides for study/investigation
    * [[#6b52004](https://github.com/nfdi4plants/ARCtrl/commit/6b520046a71d462b934c869333a4f77c61dfb59e)] Finalize io controller types for fable :sparkles:
    * [[#0ff28ab](https://github.com/nfdi4plants/ARCtrl/commit/0ff28abb3b1ac4ca51a93e909d3580d8d7a82874)] rmv old code :fire:
    * [[#754cac7](https://github.com/nfdi4plants/ARCtrl/commit/754cac745bf9417d3fbce127960a9966122f2ea7)] Rename to toStringObject
    * [[#98efaa0](https://github.com/nfdi4plants/ARCtrl/commit/98efaa00137025540a81f4baa896e32ac8480792)] add README.md reference to setup.cmd
    * [[#d893131](https://github.com/nfdi4plants/ARCtrl/commit/d8931310f6c415f7420aa04d3936db275f034fed)] Simplify OntologyAnnotation hashcode and equality :white_check_mark:
    * [[#7d6c107](https://github.com/nfdi4plants/ARCtrl/commit/7d6c107728970f12bc0e843ec936379ec091bce2)] Add setup script for windows :construction_worker:
    * [[#232dae3](https://github.com/nfdi4plants/ARCtrl/commit/232dae337336ab5a7d66a2a80c6e7865f1fc3d6c)] set test with umlauten pending for python
    * [[#c15a2e5](https://github.com/nfdi4plants/ARCtrl/commit/c15a2e5198af1476e5d18d8cf5188585806b1592)] Add ro-crate write tests :white_check_mark:
    * [[#7340910](https://github.com/nfdi4plants/ARCtrl/commit/73409107e1eb2aa81bc46101935b4048b330c89b)] add some more schema validation tests
    * [[#fc8f63e](https://github.com/nfdi4plants/ARCtrl/commit/fc8f63e7cc7cc622a123d2d57fc86c4ffbae7c32)] add native python tests :white_check_mark:
    * [[#e3512c7](https://github.com/nfdi4plants/ARCtrl/commit/e3512c743f1fad144c04a97e12d279071e365104)] Make js native tests pass :heavy_check_mark:
    * [[#8d50567](https://github.com/nfdi4plants/ARCtrl/commit/8d50567789556b5d6f2473b92f65b7e245992bcf)] update github CI
    * [[#18012b3](https://github.com/nfdi4plants/ARCtrl/commit/18012b3f2d78288d3069cbbfc2544bdca122b378)] rmv leftover folder :fire:
    * [[#89f976c](https://github.com/nfdi4plants/ARCtrl/commit/89f976c884cbd734e55cbc132402cefcf3ac2d3b)] Finish json (for now :construction:)
    * [[#5803eb7](https://github.com/nfdi4plants/ARCtrl/commit/5803eb726a7ced573978ecb20db2e743d9536b45)] start reworking tests for js and py
    * [[#766d46a](https://github.com/nfdi4plants/ARCtrl/commit/766d46a3db5274596571cc8a087edbc3720e5142)] move ARCtrl contract tests to Contract test project
    * [[#3f35262](https://github.com/nfdi4plants/ARCtrl/commit/3f3526271126d2ddce812633404c3b39c56697e8)] move template json tests to json test project
    * [[#4fc4a07](https://github.com/nfdi4plants/ARCtrl/commit/4fc4a071f0b435c51efde7a90c5cfe4dbf730c73)] more work on json :construction:
    * [[#cb5da3a](https://github.com/nfdi4plants/ARCtrl/commit/cb5da3a39c543c752dca92dceaf0f4f7eeed5030)] reduce test spreadsheet metadata matching reduction of parser logic
    * [[#c885111](https://github.com/nfdi4plants/ARCtrl/commit/c885111fa53bc611fca22e4c7116526ecdfce76e)] More work on json tests :construction:
    * [[#adac51c](https://github.com/nfdi4plants/ARCtrl/commit/adac51c05d2919293229e0e758c646ccefbb2f13)] Add custom ToString for OntologyAnnotation :art:
    * [[#c193629](https://github.com/nfdi4plants/ARCtrl/commit/c193629dfff93e42f00c71c02a40ec1595f897fe)] Start working on tests :construction:
    * [[#5f6c65b](https://github.com/nfdi4plants/ARCtrl/commit/5f6c65be8b50804b2f62b93545c82966719326ea)] distribute template across appropriate projects
    * [[#8af7487](https://github.com/nfdi4plants/ARCtrl/commit/8af7487057b7451c0e5bc2de5dac9b792dbc4213)] Add fable json import type #324 #285
    * [[#e3437b2](https://github.com/nfdi4plants/ARCtrl/commit/e3437b282dcdc226402920e62d5e4c96166969e7)] add ARC RO-Crate serialization
    * [[#4cd6d63](https://github.com/nfdi4plants/ARCtrl/commit/4cd6d638780acc49e4808eb955f94460095f8050)] finish investigation ro-crate
    * [[#39fe3cf](https://github.com/nfdi4plants/ARCtrl/commit/39fe3cfd2a3e105733dd486c6b988960df6e47bc)] finish moving template json :sparkles:
    * [[#c25748a](https://github.com/nfdi4plants/ARCtrl/commit/c25748a16ea1a7dbc44548cffee6dd787fe8d0d9)] Finish compressed json logic :sparkles:
    * [[#04b097b](https://github.com/nfdi4plants/ARCtrl/commit/04b097b675e921841ed26c3a121a8fae60006f49)] Start moving Arc Json logic :construction:
    * [[#e710c5a](https://github.com/nfdi4plants/ARCtrl/commit/e710c5af2f9ff336174dcdbb2a9ef09fa940e9ff)] rework ARCAssay and ArcStudy ISAJson serialization
    * [[#437bca3](https://github.com/nfdi4plants/ARCtrl/commit/437bca3f5df1bb9913ae2b829485065c8f363c52)] rework ROCrate process serialization
    * [[#19b849a](https://github.com/nfdi4plants/ARCtrl/commit/19b849a5c276ce6dd4821de2c289fb82f0aefd68)] Start with component logic rework :construction:
    * [[#f24259e](https://github.com/nfdi4plants/ARCtrl/commit/f24259e8424df783d163fb279730d914f8472673)] finish up ISA-JSON conversion for process types :sparkles:
    * [[#a51bfcc](https://github.com/nfdi4plants/ARCtrl/commit/a51bfcc49e3ad66e1abdab7cf92ad5ab0abdd37f)] Finish json table logic :sparkles:
    * [[#6b670d9](https://github.com/nfdi4plants/ARCtrl/commit/6b670d952cd0711eae4efc6e2f57ef688c0c9b4f)] Finish json for shared types :sparkles:
    * [[#a29a7f9](https://github.com/nfdi4plants/ARCtrl/commit/a29a7f9fc48bc1b3ad5c627add21a63598982f47)] start reworking arctrl.json process serialization
    * [[#5150773](https://github.com/nfdi4plants/ARCtrl/commit/5150773b8c311f281b385ad589a9611762415b86)] Start workin on new Json API layer :construction:
    * [[#f65d414](https://github.com/nfdi4plants/ARCtrl/commit/f65d4141afdfff9ca7d0f07486734df9c5a6afc4)] start reworking ARCtrl.json project
    * [[#b568074](https://github.com/nfdi4plants/ARCtrl/commit/b568074b6fa381221f9646e8763e52e4a7473208)] rework ARCtrl.Spreadsheet according to datamodel changes
    * [[#2cb9db8](https://github.com/nfdi4plants/ARCtrl/commit/2cb9db8e84e42866330f98d35f4f149b2c50fd3a)] rework ARCtrl.Core types to improve transpiled javascript
    * [[#b282dc3](https://github.com/nfdi4plants/ARCtrl/commit/b282dc3ed6c097adce071a85bde4163ba589170c)] restructure ARCtrl.Core (former ARCtrl.ISA) project
    * [[#90520bd](https://github.com/nfdi4plants/ARCtrl/commit/90520bd64f8e0d5513cfca3d692dd439259529a4)] restructure project
    * [[#ceefce8](https://github.com/nfdi4plants/ARCtrl/commit/ceefce8a3765a4e6a183452169c69c03b2045895)] move all filesystem information into filesystem project
    * [[#4b2b78f](https://github.com/nfdi4plants/ARCtrl/commit/4b2b78fc8b3ea20faf432f9de5f1d9bf0069bd4b)] move UIHelper function into compositecell
    * [[#dd69b1f](https://github.com/nfdi4plants/ARCtrl/commit/dd69b1fb2d8a207f128a0b96fd12e849bc2452e3)] add addtionalType to ProcessParameterValue
    * [[#3bf2a83](https://github.com/nfdi4plants/ARCtrl/commit/3bf2a837e6a363d698052c7d93e5045354f73c90)] WIP :construction:
    * [[#f7beae7](https://github.com/nfdi4plants/ARCtrl/commit/f7beae760ef8e3309ac29a03908072ab60bbf84c)] Updated tests for jsonld readers. Investigation test still wip.
    * [[#d8c4d0a](https://github.com/nfdi4plants/ARCtrl/commit/d8c4d0aa3a4e87a9bb5d07617b10e212b7d3150b)] adapted json export tests for new ro-crate export (Investigation still open)
    * [[#d1cbba9](https://github.com/nfdi4plants/ARCtrl/commit/d1cbba9570855cd25eea2ac0901683892b0b33b9)] new ro-crate export and context files
    * [[#280d3b8](https://github.com/nfdi4plants/ARCtrl/commit/280d3b834cc8acca8deaf744c719a91de4ab2ab2)] WIP :construction:
    * [[#9983642](https://github.com/nfdi4plants/ARCtrl/commit/9983642fe231eedfa939e58fa973a80afaa02025)] WIP :construction:
    * [[#e7b0015](https://github.com/nfdi4plants/ARCtrl/commit/e7b00158f5e10c0ece2d654e61ab929219db9fc7)] Updated tests for jsonld readers. Investigation test still wip.
    * [[#1cc83f8](https://github.com/nfdi4plants/ARCtrl/commit/1cc83f805a039a7f51de26f62da80c94aaad702b)] adapted json export tests for new ro-crate export (Investigation still open)
    * [[#9b0d7b7](https://github.com/nfdi4plants/ARCtrl/commit/9b0d7b7040e23c49e55f5e10b4e0ab2cd16d6914)] new ro-crate export and context files
    * [[#0fb24ee](https://github.com/nfdi4plants/ARCtrl/commit/0fb24ee8c7ab2f9730059a99c7be6605bbc2f59c)] Merge pull request #325 from nfdi4plants/performanceReport
    * [[#faaba01](https://github.com/nfdi4plants/ARCtrl/commit/faaba01fb783797cbb459e541577cbe35636a883)] several minor adjustments according to change request
    * [[#9c32a3d](https://github.com/nfdi4plants/ARCtrl/commit/9c32a3df12f9439217090ad1568d8479ae5170d0)] update readme with performance report and python links
    * [[#9e79685](https://github.com/nfdi4plants/ARCtrl/commit/9e796856e9b374bc13154457b74639e7a3cf2953)] move performance report into speedtest project
    * [[#27aac66](https://github.com/nfdi4plants/ARCtrl/commit/27aac66eea741cf2554198d063e217d333366068)] move performance report build targets and create first reports
    * [[#41fef58](https://github.com/nfdi4plants/ARCtrl/commit/41fef587314b4c4c1ca2fd4a73e9b0d2fa45985e)] first draft of performanceReport target
    * [[#d64e708](https://github.com/nfdi4plants/ARCtrl/commit/d64e7087e962f1ca910fbf96d6e080d4f3da3e3e)] Merge pull request #323 from nfdi4plants/update_js_docs
    * [[#1cd68ea](https://github.com/nfdi4plants/ARCtrl/commit/1cd68ea2650a4687ce378915e15cdf7405ab9834)] Update
    * [[#60d575e](https://github.com/nfdi4plants/ARCtrl/commit/60d575e78d6a8c8e85fbf601fe700f5e59a1dfab)] Merge pull request #320 from nfdi4plants/python_integration
    * [[#0aefdb2](https://github.com/nfdi4plants/ARCtrl/commit/0aefdb2341ebf3bba8560714194e6182a81256d0)] update documentation and add python docs
    * [[#abd3d89](https://github.com/nfdi4plants/ARCtrl/commit/abd3d890fb58a0962d4e7d841df7440d9a4bed90)] Merge pull request #317 from nfdi4plants/python_integration
    * [[#b6e2eca](https://github.com/nfdi4plants/ARCtrl/commit/b6e2ecab5a0a3630e1dd24ae36d34be79ad21d6d)] small change to semver in npm
* Deletions:
    * [[#cd1620c](https://github.com/nfdi4plants/ARCtrl/commit/cd1620c615ec500713ba12da7b685eca7305a12f)] remove entry when delete contract is created for validation packages config file, add related tests
    * [[#e50083c](https://github.com/nfdi4plants/ARCtrl/commit/e50083ce6638fc6e7ac607bd1a090948046d1d84)] remove UI helper functions #329
    * [[#1ce8001](https://github.com/nfdi4plants/ARCtrl/commit/1ce80017ddd48bbe217ee99ac70923cb5dca8bc1)] Remove ALL tabs from fsproj files
    * [[#f055617](https://github.com/nfdi4plants/ARCtrl/commit/f05561709a0a8d1da752d54d5a997db225a3384e)] remove erroneous exe output type from testingutils
* Bugfixes:
    * [[#ad6f37f](https://github.com/nfdi4plants/ARCtrl/commit/ad6f37f02cc24c22b9b6744fc59039bf3644eb4b)] Fix spreadsheet writer for data with freetext :bug::white_check_mark: #405
    * [[#e358522](https://github.com/nfdi4plants/ARCtrl/commit/e358522720ac8343d470c33e13e8cdd1363e18e2)] fix data column spreadsheet export for mixed columns :bug::white_check_mark:
    * [[#f60e663](https://github.com/nfdi4plants/ARCtrl/commit/f60e663aa68f6fbae2eb0da65f20950b9ee67545)] fix module references :bug:
    * [[#36ab086](https://github.com/nfdi4plants/ARCtrl/commit/36ab0864dfda70ed078cef089dcc9f3e0847341a)] Added missing datamap to compressed study json :bug:
    * [[#5cfffe9](https://github.com/nfdi4plants/ARCtrl/commit/5cfffe92e47ffd342a44debcf50ea0d08ef27359)] fix study datamap json key name :bug:
    * [[#dc3822f](https://github.com/nfdi4plants/ARCtrl/commit/dc3822fd1fec14c6009425e618531adf8828119a)] Fixed json key naming issue, added tests :bug::white_check_mark:
    * [[#a88344a](https://github.com/nfdi4plants/ARCtrl/commit/a88344a7f5f6d96b82e31af85b582b7425db8d7c)] fix test transpilation for YAML
    * [[#ba8d69e](https://github.com/nfdi4plants/ARCtrl/commit/ba8d69ef5e3668d986ee2e948699cce8dd1e977b)] Fix path helper usage after rebase
    * [[#d1b9f9b](https://github.com/nfdi4plants/ARCtrl/commit/d1b9f9b3e640ccf688af7cbd2d34d07589a6f1ac)] fix multi annotationTable writing and unify API #423
    * [[#621f769](https://github.com/nfdi4plants/ARCtrl/commit/621f769ead6a0616a78f5db0ff23c5bfd6d53a5b)] small fix for changes to spreadsheet parsing
    * [[#19b6a4c](https://github.com/nfdi4plants/ARCtrl/commit/19b6a4cb2d9ff89a68428f226ed49ffa34641c93)] fix and test rename leading to update contracts when the renamed entity is registered and remove isalight
    * [[#4ae6bd0](https://github.com/nfdi4plants/ARCtrl/commit/4ae6bd029c7ecd14cb16e6fea221983d29e43fea)] fixes and tests for updateContracts when assays or studies are added
    * [[#4136027](https://github.com/nfdi4plants/ARCtrl/commit/4136027d19e01df6b4d24c2558d7f58fa071b83a)] Fix missing fable includes #369
    * [[#ff4bad8](https://github.com/nfdi4plants/ARCtrl/commit/ff4bad8421b6bb0c1103b152d69cc471b5259ca2)] fix datamap spreadsheet parsing against tests
    * [[#0ead1eb](https://github.com/nfdi4plants/ARCtrl/commit/0ead1eb2ca62057fb9e7c2eb585df94ad50ce56f)] fix datamap spreadsheet parser against tests
    * [[#65274ec](https://github.com/nfdi4plants/ARCtrl/commit/65274ec72288016450ce9cb151fd11e9a04d23b8)] fix tests againsts iri generation changes
    * [[#2abd33f](https://github.com/nfdi4plants/ARCtrl/commit/2abd33f46a8e9dd9d56373302a13d5d9ac1cbe4b)] fix tests against @id json writer changes
    * [[#f8ae105](https://github.com/nfdi4plants/ARCtrl/commit/f8ae10529112c38cc90a1664626d80334e71e9bb)] some fixes against study idmap test case
    * [[#b367d01](https://github.com/nfdi4plants/ARCtrl/commit/b367d014eee9a9ca113367426ade1bab27b7540a)] fix id table json parsing tests
    * [[#3f164c3](https://github.com/nfdi4plants/ARCtrl/commit/3f164c3eb87aa68ecc6ad5cb50dade9f0c769752)] small fix for jsnativetests for comment column addition
    * [[#6ef030d](https://github.com/nfdi4plants/ARCtrl/commit/6ef030da7ab86ace826706731848db7098ff2ed5)] Merge pull request #358 from nfdi4plants/fixes
    * [[#e2cb695](https://github.com/nfdi4plants/ARCtrl/commit/e2cb695115ddd5a93e0871638d4649fe326adef7)] Fix python docs for contract logic :bug: #342
    * [[#d2d7c53](https://github.com/nfdi4plants/ARCtrl/commit/d2d7c5337f80f7289d0f64b7f2a9c37a496b1299)] test and fix ro-crate propertyValue parsing
    * [[#8c82636](https://github.com/nfdi4plants/ARCtrl/commit/8c82636ba0e10b7934ca354c22cf3651574d1ad6)] add tests and fix empty cell parsing behaviour
    * [[#30a9bf4](https://github.com/nfdi4plants/ARCtrl/commit/30a9bf4b01e23580a26538c492f450ec91f9947a)] Merge pull request #353 from nfdi4plants/jsonLD-fix
    * [[#eebaf4d](https://github.com/nfdi4plants/ARCtrl/commit/eebaf4d5b84166a1356d66d17b8936ddbf617129)] rework and fix Spreadsheet reader according to data node changes
    * [[#b856188](https://github.com/nfdi4plants/ARCtrl/commit/b856188e0214d6a30451fb5f5d8b16173917a066)] adjust and fix spreadsheet writing for data node changes
    * [[#bc7d952](https://github.com/nfdi4plants/ARCtrl/commit/bc7d9521a25497c5731e4c8fd2dc6b9b0e99d810)] small fixes for Fable compatability
    * [[#4b53ff2](https://github.com/nfdi4plants/ARCtrl/commit/4b53ff212e6cc015b745d409f1c7baad53c2d7b0)] fix data json parser test
    * [[#e444944](https://github.com/nfdi4plants/ARCtrl/commit/e444944b371d6472971589f2980b0fd487f3652c)] fix non utf8 json output #334
    * [[#025734d](https://github.com/nfdi4plants/ARCtrl/commit/025734d63e0319d74dbd4edbe6433e61e9dd3fca)] rmv code circumventing fable python bug :fire:
    * [[#9f570b3](https://github.com/nfdi4plants/ARCtrl/commit/9f570b3d6616273f34e16eb2346959c5ac78280e)] fix OntologyAnnotation Hashcodes :bug:
    * [[#b84919f](https://github.com/nfdi4plants/ARCtrl/commit/b84919ff37c1e8f31f648579ac7f5f4c28a5cfd7)] Fix json tests :heavy_check_mark:
    * [[#d68348e](https://github.com/nfdi4plants/ARCtrl/commit/d68348ed5d5356cc1f1eddd3f680854ef5065135)] hotfix js schema validation in tests
    * [[#2a999e8](https://github.com/nfdi4plants/ARCtrl/commit/2a999e89a1444e355cc333b14256e126c14f6fa4)] Fix all warnings by burning unused fable reflection helpers :fire:
    * [[#562ce65](https://github.com/nfdi4plants/ARCtrl/commit/562ce659e61703bdfbbbbe7147336c3623425152)] Fix test tasks still using mocha
    * [[#b15a377](https://github.com/nfdi4plants/ARCtrl/commit/b15a377fc4fb4fe9fe90768be7d089b121418b03)] fix json parsing DateTime issues
    * [[#d5eae8d](https://github.com/nfdi4plants/ARCtrl/commit/d5eae8d8081fb4d8bf1a8c0f4bc6efcaaa52cc67)] several python test fixes
    * [[#451ef11](https://github.com/nfdi4plants/ARCtrl/commit/451ef1104a0882de4f49ef4ff118d66bfb895d7e)] move and fix template hashcode and equality tests
    * [[#9da88fe](https://github.com/nfdi4plants/ARCtrl/commit/9da88feffa8a7b736d5f171e1b003ef7f0e31fa8)] Template fixes against json and core tests
    * [[#97c433e](https://github.com/nfdi4plants/ARCtrl/commit/97c433e021baa3665dc2cbb655731d5850d0029d)] move and fix spreadsheet Template tests
    * [[#fbaa958](https://github.com/nfdi4plants/ARCtrl/commit/fbaa958b364970968bafca9c6bb9beed469f28a6)] fix component name conversion
    * [[#6c9123e](https://github.com/nfdi4plants/ARCtrl/commit/6c9123ed0821f43eef51fb6ec7ad6d36985456b5)] fix arctrl.core tests and equality of reworked objects
    * [[#da18342](https://github.com/nfdi4plants/ARCtrl/commit/da183420961acc63543d02fd732d1b2875095765)] fix rocrate sample and protocol handling
    * [[#4b5af05](https://github.com/nfdi4plants/ARCtrl/commit/4b5af05e08e1e4e96249b1b3f87fea6ca02d4f8b)] fix rocrate assay serialization
    * [[#cd69fc3](https://github.com/nfdi4plants/ARCtrl/commit/cd69fc3a5c83174b1f716eba486ce1361ec1d773)] fix wrong encoder/decoder calls :sparkles:
    * [[#e053cd9](https://github.com/nfdi4plants/ARCtrl/commit/e053cd9a7a6a7717d991e8975ff9d120f446920a)] fix arctrl.core tests
    * [[#996ae0e](https://github.com/nfdi4plants/ARCtrl/commit/996ae0e9819e3a93edbd4f4e7cc256d08bce6836)] fix multiple issues with ro-crate json encoding :bug:
    * [[#5b0adba](https://github.com/nfdi4plants/ARCtrl/commit/5b0adbac1bd278b72ca374057dd0066238bd3d0d)] Added fromJsonldString for all ISA-JSON types. Also fixed some bugs.
    * [[#2c74476](https://github.com/nfdi4plants/ARCtrl/commit/2c74476bad430d8e4865744b8149a707737d3c1e)] fix build chain for linux
    * [[#295af00](https://github.com/nfdi4plants/ARCtrl/commit/295af00f9f513d6c779e42137ce141d69deb4220)] fix tests :bug:

### 1.2.0+19d850e (Released 2024-3-8)
* Additions:
    * Added Python compatability
    * [[#19d850e](https://github.com/nfdi4plants/ARCtrl/commit/19d850ed5e1474a5e82cd98537db3f398fbacc18)] several small cleanups according to PR #317 comments
    * [[#8becc67](https://github.com/nfdi4plants/ARCtrl/commit/8becc672c432eb4399fc3a0e250a87e63044de2b)] update build project for releasing python package
    * [[#df663cb](https://github.com/nfdi4plants/ARCtrl/commit/df663cbfe8cdfaa7ac48690f89539b17715ae0c0)] include python setup in CI
    * [[#66b83a2](https://github.com/nfdi4plants/ARCtrl/commit/66b83a2292b082e96676a52c1dd176695173f949)] several small changes to test stack
    * [[#3dfaad7](https://github.com/nfdi4plants/ARCtrl/commit/3dfaad77a14aa70715388ef6a1fa77cb337bd622)] adjustments to web and validation to allowe for python transpilation
    * [[#688c628](https://github.com/nfdi4plants/ARCtrl/commit/688c6283e9b2bd332fd93388b10d8661e185bc1b)] work on json io support in python
    * [[#93b6d94](https://github.com/nfdi4plants/ARCtrl/commit/93b6d9490acdf16045bd844f4cec22f2d54c7eca)] replace mocha and expecto with pyxpecto
    * [[#aa30389](https://github.com/nfdi4plants/ARCtrl/commit/aa30389f070a248e547373779de64247d0b017cc)] move json schema validation to json tests
    * [[#f0bb78d](https://github.com/nfdi4plants/ARCtrl/commit/f0bb78dcdccb1b2bfc31e6cc1c36c6905c950045)] switch towards using .venv for running transpiled python
    * [[#2d35cfb](https://github.com/nfdi4plants/ARCtrl/commit/2d35cfbb7b75ca79e5b2d5f28cb6dc56514acab0)] update Fable and pyxpecto
    * [[#32a4128](https://github.com/nfdi4plants/ARCtrl/commit/32a4128fcad38d35e695d4dd86db9b19478c1543)] add hashcode tests
    * [[#cc8c116](https://github.com/nfdi4plants/ARCtrl/commit/cc8c11661e640ce96c73eb136ae4681f710a65b8)] increase json write test timeout
    * [[#3d00621](https://github.com/nfdi4plants/ARCtrl/commit/3d00621ff1feb76f1a4628f6cdad7a3a7cdb49fd)] small adjustments in isa.json
    * [[#10c5538](https://github.com/nfdi4plants/ARCtrl/commit/10c55387c420c12804b3f10e737e1d5b1f4b3b43)] cleanup merge of json-ld and thoth.json
    * [[#58b3d5a](https://github.com/nfdi4plants/ARCtrl/commit/58b3d5a500c36dd09b34cac2cb2cc62efaf8d3d6)] merge json-ld changes into thoth.json update changes and refactor
    * [[#87ab15d](https://github.com/nfdi4plants/ARCtrl/commit/87ab15df0930389a8a9e6bdd7824a89b1ae30790)] increase json parsing test timeout
    * [[#90c60b9](https://github.com/nfdi4plants/ARCtrl/commit/90c60b90031b80efd6b23486e7e8061a2b2cc8cb)] rework json encoding
    * [[#c482c86](https://github.com/nfdi4plants/ARCtrl/commit/c482c86806d5734755a46b9c5b80bbdc32f5e001)] finish thoth conversion
    * [[#d0a99a3](https://github.com/nfdi4plants/ARCtrl/commit/d0a99a314da50eba37f23aa0e0e5c52f97dbd6ce)] start reworking json towards net Thoth.Json
    * [[#4f77082](https://github.com/nfdi4plants/ARCtrl/commit/4f77082708909086ec0113e18b2ed8102cd975bd)] Merge pull request #271 from nfdi4plants/jsonld
    * [[#3f84e17](https://github.com/nfdi4plants/ARCtrl/commit/3f84e17a2ac724a5fc74a33e5352e34b79b1022a)] increase speed of ARC to Json Type conversion
    * [[#5ceb732](https://github.com/nfdi4plants/ARCtrl/commit/5ceb73209026e512b16461ca6ef6788100bdfe18)] bump to 1.1.0
* Bugfixes:
    * [[#890048e](https://github.com/nfdi4plants/ARCtrl/commit/890048e84e06a88b4b957fc46e30113c40c014d0)] set ci fail-fast to false and fix py encoding in windows
    * [[#3e403cd](https://github.com/nfdi4plants/ARCtrl/commit/3e403cdd687be78f1cbadfafb30b148247800bdd)] small fix to CI
    * [[#b0186f4](https://github.com/nfdi4plants/ARCtrl/commit/b0186f4c3e28fbf9a6678dde5795e6e0723934c0)] small fix to CI
    * [[#89aa3c2](https://github.com/nfdi4plants/ARCtrl/commit/89aa3c2a93cb718ca77433e35599d2d22deb2886)] fixed python tests to work on all platforms
    * [[#4262833](https://github.com/nfdi4plants/ARCtrl/commit/4262833f532c3ed8deedc1ca30b6ad30c04c17b3)] hotfix js webrequest
    * [[#f75ce4d](https://github.com/nfdi4plants/ARCtrl/commit/f75ce4d3d9abc84600a769d8ead47a6bd2befe8b)] small python tests hotfix
    * [[#9385d06](https://github.com/nfdi4plants/ARCtrl/commit/9385d06c4bb728edd6c3d95f528fd0a40a19062d)] small fixes in python and setup instructions
    * [[#b5eaaed](https://github.com/nfdi4plants/ARCtrl/commit/b5eaaedd24cad0a24423aec39dbdf56bbaac14dd)] hotfix fable python hashing of person
    * [[#556e2bf](https://github.com/nfdi4plants/ARCtrl/commit/556e2bf7f0bd85417ce43078ff3c2357819bef1d)] fix http requests in python
    * [[#c534acf](https://github.com/nfdi4plants/ARCtrl/commit/c534acf43e16d1e76bb22bbc7e0d1e7134d213e5)] fix and test comment regex handling for python
    * [[#2540056](https://github.com/nfdi4plants/ARCtrl/commit/2540056063a107fb18ad30b577214e946a6ae48c)] small fix for compressed json io stringtable conversion https://github.com/fable-compiler/Fable/issues/3771
    * [[#92c72ee](https://github.com/nfdi4plants/ARCtrl/commit/92c72ee0135c712ded59c00c9a0111909ce99c3a)] array fixes in python compilation
    * [[#b55d360](https://github.com/nfdi4plants/ARCtrl/commit/b55d3608eb73273a84cca756e0662811ff7b6244)] fix and finish up thoth migration

### 1.1.0+6309e03 (Released 2024-2-15)
* Additions:
    * [[#544ffdc](https://github.com/nfdi4plants/ARCtrl/commit/544ffdcbcf2e19c12c995e408713b83157cda345)] add some additional information to failing spreadsheet parsers #306
    * [[#ee67104](https://github.com/nfdi4plants/ARCtrl/commit/ee671049018b339c8fd98975ed33960e0313f5f1)] create first version of compressed arctable json exporter
    * [[#8b36c55](https://github.com/nfdi4plants/ARCtrl/commit/8b36c55438cd0747ecc61778ff48c70fb787417b)] implement stringtable in arctable json compression
    * [[#bb98bf4](https://github.com/nfdi4plants/ARCtrl/commit/bb98bf4fd84d62bfedfe5732b502f58e586471d2)] add compressed study and assay json encoders
    * [[#6c5aa85](https://github.com/nfdi4plants/ARCtrl/commit/6c5aa85fd38e773d6e6f386da89c15a2f2e0ce06)] add tests for compressed json
    * [[#689f60c](https://github.com/nfdi4plants/ARCtrl/commit/689f60c52c42e63639d744255da97f80b4279e74)] add more efficient compressed column encoding
    * [[#a9fbdf6](https://github.com/nfdi4plants/ARCtrl/commit/a9fbdf600ab11c7cfe44cd66a71d05b49af49fad)] add rangeEncoding to compressed json
    * [[#8635d76](https://github.com/nfdi4plants/ARCtrl/commit/8635d76d663895326a387ef371faa571ff661756)] Improve speed of GetHashCode
    * [[#ed0e78e](https://github.com/nfdi4plants/ARCtrl/commit/ed0e78e2efd76f673ed23dcce31711f2ccf3ef06)] add GetUpdateContracts functionality
    * [[#73ed27e](https://github.com/nfdi4plants/ARCtrl/commit/73ed27e54d4cac2658703b76a02bce418c7b9aea)] small clarification on getUpdateContracts
    * [[#787368e](https://github.com/nfdi4plants/ARCtrl/commit/787368edf4bdfc6d972d33c5050092b3f347ccfe)] reinclude fable reference in isa.json.fsproj
* Deletions:
    * [[#f87b4b6](https://github.com/nfdi4plants/ARCtrl/commit/f87b4b6ec87fe713346956c9a4547759a38817fe)] removed unused self reference in assay and study constructors
* Bugfixes:
    * [[#c619cd9](https://github.com/nfdi4plants/ARCtrl/commit/c619cd98b5840c22c2efacc8759e59907d44f9dd)] small fix to compressed arcTable json
    * [[#9d3392d](https://github.com/nfdi4plants/ARCtrl/commit/9d3392d7d8125a252ad58ae0da749f5673f27cfb)] add update contract tests and fix against them

### 1.0.7+14c14a9 (Released 2024-1-30)
* Additions:
    * [[#14c14a9](https://github.com/nfdi4plants/ARCtrl/commit/14c14a905b42bb75207a28a2ab1721ec31965fed)] update FsSpreadsheet reference
    * [[#1d8d7b7](https://github.com/nfdi4plants/ARCtrl/commit/1d8d7b78961e117eff06244291f5e197096a7cee)] small changes to fillmissing and performance tests
    * [[#68c65d0](https://github.com/nfdi4plants/ARCtrl/commit/68c65d0d347c6a93f97a9673834a5f4eeba6e8ea)] improve design of speed tests
    * [[#d592cf3](https://github.com/nfdi4plants/ARCtrl/commit/d592cf3b8cfd8a8c262d4878a1f226254b670509)] small speed increase of setCell
    * [[#38fe3a9](https://github.com/nfdi4plants/ARCtrl/commit/38fe3a9d46c6079cf908c60fbb2c4cd650f39fe2)] rework fillMissing
    * [[#ddb8238](https://github.com/nfdi4plants/ARCtrl/commit/ddb8238426fd8212a4701cdc21461783e82b0b36)] improve speed of ArcTable.AddRows
* Bugfixes:
    * [[#2caadb7](https://github.com/nfdi4plants/ARCtrl/commit/2caadb72e0740495e32506ebbcf14f6fc8a59a52)] small fix to addRows

### 1.0.6+c6d86b1 (Released 2024-1-26)
* Additions:
    * [[#c6d86b1](https://github.com/nfdi4plants/ARCtrl/commit/c6d86b104392447cc980bf3e16fa64bfb27ca5c6)] small performance improvement for reading ARCs
    * [[#3f8c788](https://github.com/nfdi4plants/ARCtrl/commit/3f8c7883f6fc40f1be910afe20b19f0b3d0fad1d)] add case for speedtest project
    * [[#a90a1f1](https://github.com/nfdi4plants/ARCtrl/commit/a90a1f1e135497e7613975c84ae1271849a6944f)] improve and test speed of investigation file writer
    * [[#123f209](https://github.com/nfdi4plants/ARCtrl/commit/123f209fda76be179fd18cb96eab5871e764f38a)] Merge pull request #299 from nfdi4plants/small_cleanup
    * [[#70d3c85](https://github.com/nfdi4plants/ARCtrl/commit/70d3c856c3aa45135b0ef1463f399437fefb8a4c)] Merge pull request #297 from nfdi4plants/table_join_296
    * [[#13428ca](https://github.com/nfdi4plants/ARCtrl/commit/13428ca85f366eb13ba02b8557bec52bc643bab8)] Revert unintended change in default!
    * [[#66b15f6](https://github.com/nfdi4plants/ARCtrl/commit/66b15f67214f10ee3cfbb97cca0ccf738c8ffb6c)] Update to v1.0.5
* Deletions:
    * [[#d121e5c](https://github.com/nfdi4plants/ARCtrl/commit/d121e5ccddf2715958069283e272369fc731b6a2)] remove playground and move speedtest folders

### 1.0.5+b21e3bc (Released 2024-1-17)
* Additions:
    * [[#b21e3bc](https://github.com/nfdi4plants/ARCtrl/commit/b21e3bcb0a5679b5ec5494555d9e9d23822bdc9a)] Extend ArcTable.Join :sparkles::white_check_mark:
    * [[#a112468](https://github.com/nfdi4plants/ARCtrl/commit/a1124684ea02d23301b67743ff761112fe9389ab)] Update readme badge
    * [[#a0144e1](https://github.com/nfdi4plants/ARCtrl/commit/a0144e1282bb1e8206378eb6bb8b23619eb666e6)] Merge pull request #295 from nfdi4plants/Freymaurer-patch-1
    * [[#9e5c1f6](https://github.com/nfdi4plants/ARCtrl/commit/9e5c1f67b676d63dcb69b586089e1e74ca1c2814)] Update README.md
    * [[#6e0755b](https://github.com/nfdi4plants/ARCtrl/commit/6e0755b5e1c28bb72814247dad1e4ab457e25e40)] update and release version 1.0.4
* Bugfixes:
    * [[#8dd99e8](https://github.com/nfdi4plants/ARCtrl/commit/8dd99e82fc93add2950860d6eb185686be21f9ba)] Merge pull request #294 from nfdi4plants/fix_annotationValue_read

### 1.0.4+167fae8 (Released 2024-1-15)
* Additions:
    * [[#167fae8](https://github.com/nfdi4plants/ARCtrl/commit/167fae8bcaa14d8e24055f60ac031b13d8b47199)] add json functions for better function names in js :sparkles:

### 1.0.3+533dab0 (Released 2024-1-15)
* Additions:
    * [[#208839a](https://github.com/nfdi4plants/ARCtrl/commit/208839a2d50e3c7ffd2722a5e65262bea1e185cd)] change annotationValue of Ontology Annotation to string
    * [[#533dab0](https://github.com/nfdi4plants/ARCtrl/commit/533dab0039404d3dc0ba8e9612e1a083198ee056)] Add tests for edgecase :white_check_mark:
* Bugfixes:
    * [[#d42faaf](https://github.com/nfdi4plants/ARCtrl/commit/d42faaf5e41712b06991b111894bb6a7b3b09d58)] Fix template read issue. annotationvalue not correctly read :bug:

### 1.0.2+533dab0 (Released 2024-1-15)
* Additions:
    * [[#cc7f35f](https://github.com/nfdi4plants/ARCtrl/commit/cc7f35f911b3960c9996276f808d8ee4bc7204d3)] add speedtest and make some small experimental changes
    * [[#e85dba4](https://github.com/nfdi4plants/ARCtrl/commit/e85dba44d675a0896cf185c4ee39e5047896eb9c)] make some validation and checks optional for speed improvements
    * [[#208839a](https://github.com/nfdi4plants/ARCtrl/commit/208839a2d50e3c7ffd2722a5e65262bea1e185cd)] change annotationValue of Ontology Annotation to string
    * [[#a1921ef](https://github.com/nfdi4plants/ARCtrl/commit/a1921ef104e4f168cb6846b2b9de886e4ae774da)] make fillmissingcells optional for addcolumns
    * [[#2aca309](https://github.com/nfdi4plants/ARCtrl/commit/2aca309623061171dc1c3ec3897d2c5cd388bf11)] Make ArcStudy IO performance test more strict

### 1.0.1+5576d43 (Released 2023-12-21)
* Additions:
    * [[#66ff8f4](https://github.com/nfdi4plants/ARCtrl/commit/66ff8f4bb902e1234e84dfaec5411fd7ea5237b3)] improve github ci
* Bugfixes:
    * [[#443272b](https://github.com/nfdi4plants/ARCtrl/commit/443272b53bd35868b014dbb0c0d9f81b00f06081)] Fix .fsproj files
    * [[#febb0f6](https://github.com/nfdi4plants/ARCtrl/commit/febb0f6c95d8669045c761a0d389e13f6cee24f0)] fix npm release target

### 1.0.0+b42bd6f (Released 2023-12-21)
* Additions:
    * Complete overhaul of the API.
    * Shifted focus from only ISA to include full ARC.
    * Included Fable compilation to Javascript as first citizen design principles.
    * No direct filesystem access but contract based IO, which allows language agnostic IO.

### 0.7.0+ac83329 (Released 2023-1-26)
* Additions:
    * latest commit #ac83329
    * [[#ac83329](https://github.com/nfdi4plants/ISADotNet/commit/ac83329b31d4dcb69c61be5d610896522ee44eda)] impove QNode usability with documentation

### 0.6.1+7de6163 (Released 2022-12-7)
* Additions:
    * latest commit #7de6163
    * [[#477edad](https://github.com/nfdi4plants/ISADotNet/commit/477edadee8d8f5d23fefa59b5e3b9f2ec72fd92d)] Renamed src/ISADotnet to src/ISADotNet for linux build due to file name case sensitivity.
    * [[#325cb91](https://github.com/nfdi4plants/ISADotNet/commit/325cb91d00d9870a2021f4fb37b3822820448a82)] add json schema validation functions
    * [[#94ef9e5](https://github.com/nfdi4plants/ISADotNet/commit/94ef9e5fe136b561d4af68d4d2ef6b09efb04df1)] add isFatal check to json schema validation
    * [[#d496857](https://github.com/nfdi4plants/ISADotNet/commit/d49685713021c6567d971cb1caf48b0bf003da46)] ease constraints of json schema validation
    * [[#7db8fae](https://github.com/nfdi4plants/ISADotNet/commit/7db8faeea1c255261c1656f20d1b2c21590ddeb4)] validate json against json schema from HLWeil fork
    * [[#fdf248d](https://github.com/nfdi4plants/ISADotNet/commit/fdf248dc8f34b9854d1d93069e55ac46f094571a)] move jsonSchemaValidation in its own Repo
    * [[#5f133db](https://github.com/nfdi4plants/ISADotNet/commit/5f133db05a4fac4353dbf472d5187248a13c00fc)] update dotnet version in cli
    * [[#8531aac](https://github.com/nfdi4plants/ISADotNet/commit/8531aac4a2816dc91a6729d19b9d368858e03a63)] add unit retrieval functions to API
    * [[#372e07f](https://github.com/nfdi4plants/ISADotNet/commit/372e07f3579eb5de2581488620b764f6dd3ed0a5)] rename xlsx metadata headers https://github.com/nfdi4plants/arcCommander/issues/150
* Bugfixes:
    * [[#168890f](https://github.com/nfdi4plants/ISADotNet/commit/168890fd9f229447858442f4cc744f2e5c09bea2)] fix json schema validation use njonschema instead
    * [[#9c66fa8](https://github.com/nfdi4plants/ISADotNet/commit/9c66fa8ae43386b161ca02623ac4c811e7d0f4cc)] fix json test files to actually adhere to json schema
    * [[#d79605c](https://github.com/nfdi4plants/ISADotNet/commit/d79605c5afe59792d9a4a70cbb965465dbff3f2e)] fix datamodel according to json schema validation
    * [[#8f63206](https://github.com/nfdi4plants/ISADotNet/commit/8f63206dc0de554d7d6eb6b472c4ebe9e5f7b497)] fix study xlsx reader creating empty lists #71
    * [[#6d5ee0c](https://github.com/nfdi4plants/ISADotNet/commit/6d5ee0c665f65d79c3b6653f702551684cda8f82)] make DAG creation more robust against missing names fix #51

### 0.6.0+2150bc9 (Released 2022-9-7)
* Additions:
    * latest commit #2150bc9
    * harmonize xlsx IO with Swate 0.6.0 release
    * add OBO ontology IO
    * extend ontological reasoning
    * extend components and protocol types
    * [[#118f404](https://github.com/nfdi4plants/ISADotNet/commit/118f40478a328cb991c4c5f6603d333c26be4eaa)] add simple ontological reasoning for protocol type
    * [[#bdf85b2](https://github.com/nfdi4plants/ISADotNet/commit/bdf85b2a27aa452730a6b54e26442d5dffdb0aa3)] adjust assay xlsx header renumbering
    * [[#59cfdaf](https://github.com/nfdi4plants/ISADotNet/commit/59cfdafb0b3e316c6a1dbc9034a1f38574ee00b5)] small improvements to Ontology Annotation and Component type
    * [[#94b6059](https://github.com/nfdi4plants/ISADotNet/commit/94b60592011725360045c2dd4d3fa2ea27572d65)] drop protocol rowIndex when calculating rowRange
    * [[#4c050d1](https://github.com/nfdi4plants/ISADotNet/commit/4c050d12913eb0b99d52295b376473b2b5edfe5c)] revert term accession number changes
    * [[#f8f94c6](https://github.com/nfdi4plants/ISADotNet/commit/f8f94c6984e807745b4ba9830c7683d7999a5d71)] add components to querymodel and assay xlsx writing
    * [[#9b85296](https://github.com/nfdi4plants/ISADotNet/commit/9b85296754d3433b2a5f75e76b92580afde2c204)] add protocol type and protocol ref to assay and study xlsx writing
    * [[#9cb34af](https://github.com/nfdi4plants/ISADotNet/commit/9cb34af4131bf68037517842b7c16066f19f0cb9)] add component to querymodel
    * [[#1c66f1f](https://github.com/nfdi4plants/ISADotNet/commit/1c66f1fc7b2148fe71eb863e84703ad1b3100f38)] refactor return values of assay and study xlsx file readers
    * [[#1a981e9](https://github.com/nfdi4plants/ISADotNet/commit/1a981e90913e95d6c1478735b11850828dedc70c)] add component and protocoltype parsers to assay xlsx table reader
    * [[#75afe60](https://github.com/nfdi4plants/ISADotNet/commit/75afe601af2827bc8199795389609650b94d369e)] add component and protocol type parser functions
    * [[#084cee1](https://github.com/nfdi4plants/ISADotNet/commit/084cee126af2a7902dbe31ce2f13005626bdfb76)] add component casting functions
    * [[#ba490a3](https://github.com/nfdi4plants/ISADotNet/commit/ba490a30e20f109e113a0f15f7b6776d7ac75aad)] added inline documentation for querymodel methods
    * [[#f2e05eb](https://github.com/nfdi4plants/ISADotNet/commit/f2e05eb0d3a756d48b4801a9f9f84c424695ce1e)] improve Querymodel documentation
    * [[#6eb5c44](https://github.com/nfdi4plants/ISADotNet/commit/6eb5c44328d9106f0ccb1065dd3423856cc35f81)] add additional ontology helper functions to querymodel
    * [[#11ef6b8](https://github.com/nfdi4plants/ISADotNet/commit/11ef6b8b5cadcb1243a25c0b584e62e9310d5e92)] add ontology search functions
    * [[#99e3ebb](https://github.com/nfdi4plants/ISADotNet/commit/99e3ebb0f495fc398409a4432ed19aafff9943c2)] add isA and xref search on oboOntology
    * [[#af680ec](https://github.com/nfdi4plants/ISADotNet/commit/af680ec225cb8c2424ea3b35e00011ce61e29652)] integrate OBO IO and reasoning
    * [[#5ff98d8](https://github.com/nfdi4plants/ISADotNet/commit/5ff98d80b74084221dcb08460773aaa0dac91238)] extend querymodel by basic ontology logic guided querying
* Bugfixes:
    * [[#4b99f26](https://github.com/nfdi4plants/ISADotNet/commit/4b99f2691a37a96fab5786545adcf44e1a551dd6)] fix ontologyannotation equality for empty objects
    * [[#e6966aa](https://github.com/nfdi4plants/ISADotNet/commit/e6966aa19d0f67bf2a71a524e082e8facb079b6d)] fix json parsing
    * [[#d4987fe](https://github.com/nfdi4plants/ISADotNet/commit/d4987fe80d9a905659ebc0b83e6b38bd5061d802)] fix assay and study file reading tables
    * [[#f9bd7f4](https://github.com/nfdi4plants/ISADotNet/commit/f9bd7f491a1a3a889e307d976b4275bfde9d45d6)] fix component json parsing
    * [[#86e1b46](https://github.com/nfdi4plants/ISADotNet/commit/86e1b46ad101a61200797f90b065da295e8a47d7)] fix assay file writing issues #62 #63 #64
    * [[#7385792](https://github.com/nfdi4plants/ISADotNet/commit/7385792344ffb81c40218d8bcb8cae911ea67ecf)] fix study and assay xlsx write error

### 0.5.4+262f787 (Released 2022-7-12)
* Additions:
    * latest commit #262f787
    * [[#262f787](https://github.com/nfdi4plants/ISADotNet/commit/262f787b60bdcec8483132453b6aa29d96f11ac9)] improve Querymodel by adding Protocol specific functions
    * [[#60e189f](https://github.com/nfdi4plants/ISADotNet/commit/60e189f9f9037dddd9cf8e6bcf56a2ff229f5770)] add basic ontology reasoning to QueryModel
    * [[#1ba8b0c](https://github.com/nfdi4plants/ISADotNet/commit/1ba8b0cbf48737102cdb5247e818956f5bb20b1a)] move Querymodel into own project

### 0.5.3+dc96afe (Released 2022-6-29)
* Additions:
    * latest commit #dc96afe
    * [[#3b4eec6](https://github.com/nfdi4plants/ISADotNet/commit/3b4eec6cc00bf01ab7d48539114140795e9dd710)] add metadata sheets to assay and study xlsx writing
    * [[#dc96afe](https://github.com/nfdi4plants/ISADotNet/commit/dc96afe6feeca7a4ea10eee596937ff0f63a5fb5)] add QInvestigation to querymodel

### 0.5.2+43177ae (Released 2022-6-24)
* Additions:
    * latest commit #43177ae
    * [[#20b36d5](https://github.com/nfdi4plants/ISADotNet/commit/20b36d543f15ddb38392726fc8ced60a240421b2)] update readme with querymodel examples
    * [[#6640826](https://github.com/nfdi4plants/ISADotNet/commit/6640826dcc492c5354e7448c2dd726ae3f76e840)] add assay xlsx file writer
    * [[#113767c](https://github.com/nfdi4plants/ISADotNet/commit/113767c95f436145a245396a8642d65538935e76)] finish up assay xlsx file writer

### 0.5.1+af3dc20 (Released 2022-6-15)
* Additions:
    * latest commit #af3dc20
* Bugfixes:
    * [[#af3dc20](https://github.com/nfdi4plants/ISADotNet/commit/af3dc205d808b4eb19ad1f192b5f20fb238def94)] fix querymodel node updating

### 0.5.0+0646fa9 (Released 2022-6-15)
* Additions:
    * latest commit #0646fa9
    * QueryModel
    * Increase verbosity
    * Fixes
    * [[#8701a22](https://github.com/nfdi4plants/ISADotNet/commit/8701a22e9a50363fb7b95546152bffef2c44d660)] add additional methods to QueryModel
    * [[#be34819](https://github.com/nfdi4plants/ISADotNet/commit/be348197e2b41a574ab216897a0cdc75b8c1b67a)] add study type to query model
    * [[#8b7f1c7](https://github.com/nfdi4plants/ISADotNet/commit/8b7f1c726518f8d25b7111ac910b638fd99592a9)] improve query model value retrieval
    * [[#ba81b27](https://github.com/nfdi4plants/ISADotNet/commit/ba81b279c52fceec9e2b756ed71b34daed77b7ef)] add Query model capabilities
    * [[#a6470f1](https://github.com/nfdi4plants/ISADotNet/commit/a6470f1e2a58f3397b7fec94c04dc21262c8858f)] add QProcessSequence type for QueryModel
    * [[#2a9613d](https://github.com/nfdi4plants/ISADotNet/commit/2a9613d5b6c1dc2f3b5a3fa840bbea92600f5957)] small improvement to assay xlsx reading error messaging
    * [[#97406b5](https://github.com/nfdi4plants/ISADotNet/commit/97406b59030ff7afe9d2f7c6328f5dca86ee4ec2)] Add regex unit tests :white_check_mark:
    * [[#0404cbf](https://github.com/nfdi4plants/ISADotNet/commit/0404cbf8741396f8196cfcf0e3d8f7f809ffebee)] Increase flexibility of regex
    * [[#d04650f](https://github.com/nfdi4plants/ISADotNet/commit/d04650fe98edf5c20ed9a1439f5a410cb5d2e717)] move ValueIndex file from ISADotNet.XLSX into ISADotNet
    * [[#9787518](https://github.com/nfdi4plants/ISADotNet/commit/97875186dbc35be850c9eeb3641f606aa922754d)] rework Assay query model
    * [[#19cb0f6](https://github.com/nfdi4plants/ISADotNet/commit/19cb0f6b2da9592804f2cba95918743426dc3fd5)] adjust assay file header parsing -can now parse brackets inside brackets -now ignores numbers
    * [[#06f1078](https://github.com/nfdi4plants/ISADotNet/commit/06f10787beafc54b489cacd9e86379b8a8a344fb)] increase assay file io fail safety
    * [[#74b5b0f](https://github.com/nfdi4plants/ISADotNet/commit/74b5b0f63e8f50d31ebcaffa44bcbf710198f499)] increase xlsx IO verbosity
    * [[#b3a0a64](https://github.com/nfdi4plants/ISADotNet/commit/b3a0a644f2a50fbfd920e3facf9e11eca83dbd77)] include empty study by default when writing investigation file
    * [[#314acc0](https://github.com/nfdi4plants/ISADotNet/commit/314acc0ad0908a67a61ff642e33532ab83d905bc)] extend assay query model methods
    * [[#372d5d9](https://github.com/nfdi4plants/ISADotNet/commit/372d5d911e2382fe80c0a5a04806bdcc2d5cb634)] start working on assay query model
    * [[#e8e0175](https://github.com/nfdi4plants/ISADotNet/commit/e8e017592b37d6d82695b77f1dc68b255860a23e)] add study xlsx file io
    * [[#5e27192](https://github.com/nfdi4plants/ISADotNet/commit/5e271920e73a713e43120a3ff82c7d482c21f286)] switch to FsSpreadsheet for ExcelIO
    * [[#8c788d7](https://github.com/nfdi4plants/ISADotNet/commit/8c788d7392450d6ed892e20d1ff39f3bbf04a406)] add Process IO functions
    * [[#269ce79](https://github.com/nfdi4plants/ISADotNet/commit/269ce799b907ad328bc35a53749b56af3853b306)] assay xlsx file reader always assumes data type for data column
    * [[#4105894](https://github.com/nfdi4plants/ISADotNet/commit/4105894ecf720ae25200a77a68f2fcad41f67fd4)] add Assay Metadatasheet manipulation functions
* Deletions:
    * [[#476105e](https://github.com/nfdi4plants/ISADotNet/commit/476105e70e516cba375327b95ce9369d2cd1820f)] temporarily remove linux CI to avoid spam #38 this should definitely be looked into again at a later point
* Bugfixes:
    * [[#566115d](https://github.com/nfdi4plants/ISADotNet/commit/566115dcfc307472c44d4b376f3d30fff534c155)] fix assay metadatasheet not being read correctly
    * [[#f343175](https://github.com/nfdi4plants/ISADotNet/commit/f34317502944295925c219a061e4b196c69e1639)] dag vizualization now makes edges distinct per source target pair quick fix for #52
    * [[#d1c671d](https://github.com/nfdi4plants/ISADotNet/commit/d1c671d66348824c5f554c64f950d5470144a0d4)] fix outputByCharacterstic getter function fixes #50
    * [[#2101589](https://github.com/nfdi4plants/ISADotNet/commit/210158982bde87b995f8f7e453c959063b7e37aa)] fix assay reading no processes as Some empty lists fixes #54

### 0.4.0+2a9311b (Released 2021-10-22)
* Additions:
    * latest commit #2a9311b
    * [[#2a9311b](https://github.com/nfdi4plants/ISADotNet/commit/2a9311b3ff0eb5d4a27be114fcea5e3ef01b0608)] finalize first DAG visualization draft :sparkles:
    * [[#e5ad37a](https://github.com/nfdi4plants/ISADotNet/commit/e5ad37af2b6fd1525fe276d28259b7e9db44e87d)] assign characterstics from assay.xlsx files only to inputs #43
    * [[#eb985e3](https://github.com/nfdi4plants/ISADotNet/commit/eb985e3590cda6470c2e94d2b3a1943f8de1d354)] create first DAG creation function
    * [[#a9eddd6](https://github.com/nfdi4plants/ISADotNet/commit/a9eddd65f77c8070fd8c46dbb494110a3012ff72)] add schema type for DAG
    * [[#f60edbd](https://github.com/nfdi4plants/ISADotNet/commit/f60edbdc8e4e76c56fe726bd45feb54b90322b8a)] adjust xlsx io to FSharpSpreadsheetML 0.0.7
    * [[#2147cb9](https://github.com/nfdi4plants/ISADotNet/commit/2147cb93f19eeb93d0441b574bd86396cd66db0a)] prepare assay xlsx file io for FABLE compatability
    * [[#78a667c](https://github.com/nfdi4plants/ISADotNet/commit/78a667c9d2562b6e56a49e3972a5fe8a3d0c95f2)] add helper methods for assay file value ordering
    * [[#ea12093](https://github.com/nfdi4plants/ISADotNet/commit/ea1209365ecf5012e89e8d4d1277d2c5cd5c9723)] adjust investigation.xlsx file io for FABLE compatabiity
    * [[#9c52b6c](https://github.com/nfdi4plants/ISADotNet/commit/9c52b6c4e566b764cc0726af5f94aa56974048d0)] make assay.xlsx file reader save value column ordering
    * [[#0b8e26e](https://github.com/nfdi4plants/ISADotNet/commit/0b8e26e37d9d2d681a213fd880d45ffaafe062b9)] add helper functions for assayFile Column Ordering
    * [[#e1d1f1a](https://github.com/nfdi4plants/ISADotNet/commit/e1d1f1a2fdbcd7942c8f975da0f2e8b2599f5dfa)] use old formatted printing for compatibility in ISADotNet.XLSX
    * [[#ac15be6](https://github.com/nfdi4plants/ISADotNet/commit/ac15be6ff3b3b2dab2ddff42cc6f08b094b749bf)] use old formatted printing for compatibility
    * [[#c80ec0a](https://github.com/nfdi4plants/ISADotNet/commit/c80ec0a3fee123e84823965949894a14abf264f0)] Add check to exclude whitespaces from being valid values (Issue #37).
    * [[#0847b38](https://github.com/nfdi4plants/ISADotNet/commit/0847b38329b73b3e3c7b14f884b91d6a43849c52)] init ISADotNet.Viz
* Bugfixes:
    * [[#3fd7fca](https://github.com/nfdi4plants/ISADotNet/commit/3fd7fcab56dcfce284ecc1ac98fc4e2fc76e59e1)] several fixes for xlsx io
    * [[#edeed88](https://github.com/nfdi4plants/ISADotNet/commit/edeed88f1f6385577ce91e9a6693d35d34427c14)] Update tan issue fix (Issue #35).
    * [[#20cff8c](https://github.com/nfdi4plants/ISADotNet/commit/20cff8ce669a69708c5f36cb926504ecf838aed6)] Update worksheet name fix (Issue #34).
    * [[#f989114](https://github.com/nfdi4plants/ISADotNet/commit/f9891146a61ad82210efee144d7242979f779f4d)] Fix wrong term accessions (Issue #35).
    * [[#4f851d9](https://github.com/nfdi4plants/ISADotNet/commit/4f851d9e9e62c64954a0f801a945c00593d59c75)] Fix worksheet naming bug in CommonAPI (Issue #34).
    * [[#f88320f](https://github.com/nfdi4plants/ISADotNet/commit/f88320ff3e65f8014babfc3e3d2b748c4a782ba0)] fix rowMajorAssay to maintain value order

### 0.3.3+2da8c3a (Released 2021-9-10)
* Additions:
    * latest commit #2da8c3a
* Bugfixes:
    * [[#2da8c3a](https://github.com/nfdi4plants/ISADotNet/commit/2da8c3a2e73aa110b1696d41932c32dc80c91967)] fix prerelease targets

### 0.3.2+4233b78 (Released 2021-9-9)
* Additions:
    * latest commit #4233b78
    * [[#4233b78](https://github.com/nfdi4plants/ISADotNet/commit/4233b780896380764cb10a90af4b1902c8006ac9)] move SwateCustomXml datamodel to ISADotNet
    * [[#f7511ad](https://github.com/nfdi4plants/ISADotNet/commit/f7511ad7855ba10b652c38feb2c09b993d6df779)] increase function parameter consistency
    * [[#249f1af](https://github.com/nfdi4plants/ISADotNet/commit/249f1af066d0dfd9682016f6c207cf5c159bc1da)] add ISA Assay Common API parser
    * [[#9890b6a](https://github.com/nfdi4plants/ISADotNet/commit/9890b6a7932813281546d3eb2621a5ace62265ab)] Unify naming as requested (PR #31).
    * [[#a48d571](https://github.com/nfdi4plants/ISADotNet/commit/a48d5712a742631d0205dfdc35665acb7592e2df)] Add FromStringWithNumber for ProtocolParameter (Issue #24).
    * [[#b511949](https://github.com/nfdi4plants/ISADotNet/commit/b511949f3463bb1c3f6053a836c83ddd686103f7)] add isa xlsx stream reader/writer functions
    * [[#91e9ea5](https://github.com/nfdi4plants/ISADotNet/commit/91e9ea5bbfaa74e5ac5db1152be54a4b149e1f69)] Add GetName, GetNameWithNumber for xxValue DataModel (Issue #24).
    * [[#035ed55](https://github.com/nfdi4plants/ISADotNet/commit/035ed55b0fa3a417aa32627f20ca037bac129b3c)] Add FromString, FromStringWithNumber for OntologyAnnotation, Factor, MaterialAttribute (Issue #24)
    * [[#6f883e6](https://github.com/nfdi4plants/ISADotNet/commit/6f883e614e456817fcf6af8e15ce241b7d8c01b1)] Add GetValue logic for FactorValue, MaterialAttributeValue, ProcessParameterValue (Issue #24)
    * [[#01db12b](https://github.com/nfdi4plants/ISADotNet/commit/01db12ba03014ef062d6f81b662be2042ee8ebbf)] add json readers/writers

### 0.3.0+993781f (Released 2021-9-6)
* Additions:
    * latest commit #993781f
    * [[#993781f](https://github.com/nfdi4plants/ISADotNet/commit/993781fe3b9ea7d2516ee1a823894b0705db87f9)] small adjustment to assay.xlsx reader
    * [[#81a8130](https://github.com/nfdi4plants/ISADotNet/commit/81a8130767deb9f58e0b90905a6611c97c1502dd)] reduce protocol splitting complexity of assay.xlsx reader
    * [[#700bf6d](https://github.com/nfdi4plants/ISADotNet/commit/700bf6d169e6612891773244336ccc06f66e2644)] update tests to match assay.xlsx reader changes
    * [[#d65a86b](https://github.com/nfdi4plants/ISADotNet/commit/d65a86b40977fc33b98d8bdf22734fcd1de60cf0)] adjust assay.xlsx header parsers to new swate format

### 0.2.6+0fa5cb5 (Released 2021-8-25)
* Additions:
    * latest commit #0fa5cb5
    * [[#a5b970f](https://github.com/nfdi4plants/ISADotNet/commit/a5b970f5c004622878c2dbc942a120b55ef4e34b)] add pretty printer example script
* Bugfixes:
    * [[#17200f7](https://github.com/nfdi4plants/ISADotNet/commit/17200f770a5511d19b1c164e300c9b680c877db3)] fix missing commas in json deserializer
    * [[#24f9872](https://github.com/nfdi4plants/ISADotNet/commit/24f987261a479c009a28ea0da0d9f49b2e589926)] Fix Person.removeByFullName error

### 0.2.5+02f30a0 (Released 2021-3-26)
* Additions:
    * latest commit #02f30a0
    * [[#6c23554](https://github.com/nfdi4plants/ISADotNet/commit/6c23554aaa36975799aad769d1abc462088f7029)] add compact pretty printers
    * [[#ac52305](https://github.com/nfdi4plants/ISADotNet/commit/ac52305f6132c608d586211ec4972952e298ea2e)] add data column reader to assay reader
    * [[#c4f6df3](https://github.com/nfdi4plants/ISADotNet/commit/c4f6df386bf51a03d490cf72a4c69b70982cc52f)] add cyjs assay visualisation script
    * [[#327d9eb](https://github.com/nfdi4plants/ISADotNet/commit/327d9eb55f1a40d06e6745665e84d88526b19f88)] add assay file data column parser
    * [[#f783a15](https://github.com/nfdi4plants/ISADotNet/commit/f783a15f406172b91af523ba6808dbb47fba28d4)] bump to 0.2.4
* Bugfixes:
    * [[#02f30a0](https://github.com/nfdi4plants/ISADotNet/commit/02f30a0cd0640879645d28e970808d3544227757)] fix process compact printer

### 0.2.4+1c7ed14 (Released 2021-3-24)
* Additions:
    * latest commit #1c7ed14
    * [[#0c9b3e9](https://github.com/nfdi4plants/ISADotNet/commit/0c9b3e91006f8a2ee50ae14f3369110a7e54caec)] bump to 0.2.3
* Bugfixes:
    * [[#1c7ed14](https://github.com/nfdi4plants/ISADotNet/commit/1c7ed14b8fd55e1017ca5bffe2dcd324c968b744)] fix: remove printf from assay file reader

### 0.2.3+3536cd3 (Released 2021-3-23)
* Additions:
    * latest commit #3536cd3
* Bugfixes:
    * [[#3536cd3](https://github.com/nfdi4plants/ISADotNet/commit/3536cd3262ccc499547fb7c0661b648e2afc5b36)] fix assay file reader sample updating

### 0.2.2+83de052 (Released 2021-3-22)
* Additions:
    * latest commit #83de052
    * [[#c44316c](https://github.com/nfdi4plants/ISADotNet/commit/c44316c3d0848fea478d9ca6a205637354b375d1)] add process sequence convencience functions
    * [[#f62e7be](https://github.com/nfdi4plants/ISADotNet/commit/f62e7be698d1e1b5bf7e9e3d7c261b4dc8ddae20)] make assay file parser take column header number
    * [[#e2c267f](https://github.com/nfdi4plants/ISADotNet/commit/e2c267ff26ca3550f8ec574cb8b3f5224259df5d)] add functions for accessing numbered parameters
* Bugfixes:
    * [[#83de052](https://github.com/nfdi4plants/ISADotNet/commit/83de05275f9026c27cd7491ee8a748b9cdcba458)] fix process filterByProtocolName

### 0.2.1+ae00b13 (Released 2021-3-22)
* Additions:
    * latest commit #ae00b13
    * ISAXlsx Assay File Reader Tests
    * [[#93e32c4](https://github.com/nfdi4plants/ISADotNet/commit/93e32c48f47841e7707daf59565583e10b90c7d3)] bump to 0.2.0
    * [[#e6d53bf](https://github.com/nfdi4plants/ISADotNet/commit/e6d53bf881b67c491b091d460763e4cf3686caa2)] rename assay file node getter functions
    * [[#9b4884c](https://github.com/nfdi4plants/ISADotNet/commit/9b4884cd98b8f230e926b924e36e315ed2becb67)] small adjustemt to assay file sample column handling
    * [[#261e28a](https://github.com/nfdi4plants/ISADotNet/commit/261e28a086d5302a82ddf6d367bc5ed6333e76e3)] add assay file io tests
    * [[#1cc8a95](https://github.com/nfdi4plants/ISADotNet/commit/1cc8a95c1e293b6b0a571534de3cfccdbde245b8)] add assay file io tests
    * [[#cd74341](https://github.com/nfdi4plants/ISADotNet/commit/cd743416ae5e462233bd3939c8ac4b243268b0e2)] add assay sample getter functions
    * Assay Parameters accession functions
    * [[#ddab453](https://github.com/nfdi4plants/ISADotNet/commit/ddab453a86ecc1cbcbdd49a75b0230096a979c7b)] add additional assay parameter convenience functions
    * [[#622b834](https://github.com/nfdi4plants/ISADotNet/commit/622b834a26af311a6b1326d689d4388fcecf21a4)] add assay factors and characteristics convenience functions
    * [[#ae00b13](https://github.com/nfdi4plants/ISADotNet/commit/ae00b130c4c0ca15df8f7cf088fcf7987e9769fa)] add parameter value parsing functions
* Bugfixes:
    * ISAXlsx Assay File Reader Fixes
    * [[#2886bb5](https://github.com/nfdi4plants/ISADotNet/commit/2886bb55e729e541fc06e88c8b6b3c00872c2371)] fix assay file reader crashing when no custom xml is present closes #18
    * [[#bc848e7](https://github.com/nfdi4plants/ISADotNet/commit/bc848e752e407d231d1cc1fb45effec0dd878986)] fix assay file io sample merging

### 0.2.0+1c7c8c6 (Released 2021-3-7)
* Additions:
    * latest commit #1c7c8c6
    * Add ISAXlsx Assay File Reader
    * [[#9398ccb](https://github.com/nfdi4plants/ISADotNet/commit/9398ccb21ddc38c9d54666efbf5f5e2c6ef68fc8)] add assay file io tests
    * [[#9a86d09](https://github.com/nfdi4plants/ISADotNet/commit/9a86d09479814c96086e49e940ea8d05cec8393b)] add swate customXml parsing
    * [[#7c9340b](https://github.com/nfdi4plants/ISADotNet/commit/7c9340b1fb4714064df3eb30fdfd95539c88c35c)] comment Assay File IO functions and modules
    * [[#2a62081](https://github.com/nfdi4plants/ISADotNet/commit/2a62081bf61a1b4396c10342f5d0a2bc7365638b)] add sample alignment to assay file reader
    * [[#1ba9aa9](https://github.com/nfdi4plants/ISADotNet/commit/1ba9aa953f17837b96bd1919ad4a81cab139e6f2)] add protocol splitting to assay file reader
    * [[#50c2411](https://github.com/nfdi4plants/ISADotNet/commit/50c2411458e511276d9ae0b357ef1da65f186315)] tidy up ISADotNet.XLSX library structure
    * [[#dc98abe](https://github.com/nfdi4plants/ISADotNet/commit/dc98abe8b4fd386ed5f1f0743482dc8ffa443865)] add UpdateByExistingAppendLists option to record type updater
    * [[#6cedda2](https://github.com/nfdi4plants/ISADotNet/commit/6cedda23a0a3f174ca9ffd2cedc78cb672ae9c87)] add assay file reader
    * [[#0e1b990](https://github.com/nfdi4plants/ISADotNet/commit/0e1b9904402df337b9f619fb25dbb7f3916c6358)] add assayFile MetaDataReader and tests
    * [[#e33517b](https://github.com/nfdi4plants/ISADotNet/commit/e33517b52f2f41acd76fd2782e6de00e382b81be)] add assayfile process parsing functions
    * [[#8de1b95](https://github.com/nfdi4plants/ISADotNet/commit/8de1b95d010896c7c9ce190eec1e49b603c0c643)] add additional assay file column parsers
    * [[#803f642](https://github.com/nfdi4plants/ISADotNet/commit/803f6423c9b7b8340aa6cf94c2f6e1de3ecc0c95)] add assay file parameter reading
* Bugfixes:
    * [[#3af8dcd](https://github.com/nfdi4plants/ISADotNet/commit/3af8dcd701a9c9428c0298af98a7ecbc2d09ca83)] fix assay file parameter values being parsed incorrectly
    * [[#d1e7aef](https://github.com/nfdi4plants/ISADotNet/commit/d1e7aef765943d3cff3d1cbc295a6ad263c7158a)] fix AssayFileReader switching table headers

### 0.1.1+93e1117 (Released 2021-2-17)
* Additions:
    * latest commit #93e1117
    * [[#93e1117](https://github.com/nfdi4plants/ISADotNet/commit/93e11171fb1ec5030378f9123c3e6d61b530afaa)] add assayFile init function
    * [[#4c3e434](https://github.com/nfdi4plants/ISADotNet/commit/4c3e4349d8f501138204311fb04d749bdff63b52)] make investigation file prefixes optional
    * [[#235b714](https://github.com/nfdi4plants/ISADotNet/commit/235b7141dd2f40654a941449a20cbfdd032f471b)] Create README.md

### 0.1.0+5f34043 (Released 2021-2-8)
* Additions:
    * latest commit #5f34043
    * [[#9b00412](https://github.com/nfdi4plants/ISADotNet/commit/9b0041256f1a14a2824a145661d3a2ad8a2069a6)] Modernize library structure
    * [[#7f126ed](https://github.com/nfdi4plants/ISADotNet/commit/7f126ed2649a68f489d13d00e91ea2d0133ab485)] Modernize library structure
    * [[#400211a](https://github.com/nfdi4plants/ISADotNet/commit/400211ac3f76c77e5c77e5de1b370572437c2a62)] Apply requested changes
    * [[#a05b17e](https://github.com/nfdi4plants/ISADotNet/commit/a05b17e20f5bf1bf24faf61b6b7cda040259a27c)] Update global.json to "latestMajor"
    * [[#23a6d87](https://github.com/nfdi4plants/ISADotNet/commit/23a6d876759e16adbbe18a1791a175438e2a7940)] Add --noapidocs flag to docs tasks
    * [[#ad13b35](https://github.com/nfdi4plants/ISADotNet/commit/ad13b3591d1a5a928c578a02224c676cec133962)] Merge pull request #9 from Freymaurer/developer
    * [[#79b9bc2](https://github.com/nfdi4plants/ISADotNet/commit/79b9bc2cdd5f0c3342771d3a2a4c32d64aa17982)] Merge pull request #13 from Freymaurer/developer
    * [[#5f34043](https://github.com/nfdi4plants/ISADotNet/commit/5f34043638999b8c046ffcecf200f516318faa6b)] Merge pull request #14 from Freymaurer/developer
* Bugfixes:
    * [[#fa4d1c5](https://github.com/nfdi4plants/ISADotNet/commit/fa4d1c50975765e3838fcebe2aa642220938a77e)] fix remove functions fixes #8

### 0.0.4+2f78a5fa (Released 2023-8-3)
* Additions:
    * latest commit #2f78a5fa
    * 	* Add append optional list functionality when updating record types
    * 	* Add tests
    * 	* Soften version constraint of FSharp.Core package dependancy

### 0.0.2 (Released 2023-8-3)
    * Increased the minimum version of the System.Json.Text dependancy
    * Minor additions to the API functions

### 0.0.1 (Released 2023-8-3)
    * Moved Project from nfdi4Plants/ArcCommander
    * Full ISA Datamodel
    * Reworked API
    * ISA XLSX Investigation file parser
    * Added ISA Json IO

