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

### 0.0.4+2f78a5fa - Wednesday, February 03, 2021
* Additions:
    * latest commit #2f78a5fa
	* Add append optional list functionality when updating record types
	* Add tests
	* Soften version constraint of FSharp.Core package dependancy

#### 0.0.2 - Thursday, January 21, 2021

* Increased the minimum version of the System.Json.Text dependancy
* Minor additions to the API functions

#### 0.0.1 - Monday, January 11, 2021

* Moved Project from nfdi4Plants/ArcCommander
* Full ISA Datamodel
* Reworked API
* ISA XLSX Investigation file parser
* Added ISA Json IO
