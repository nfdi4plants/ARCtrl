import {ARC} from "@nfdi4plants/arctrl";
import {fulfillWriteContract, fulfillReadContract} from "./Contracts.js";

// # Create

let arc = new ARC()

// # Write
const arcRootPath = "C:/Users/Kevin/Desktop/NewTestARCJS"

async function write(arcPath, arc)  {
    let contracts = arc.GetWriteContracts()
    for (const contract of contracts) {
        // from Contracts.js docs
        await fulfillWriteContract(arcPath,contract)
    };
}

await write(arcRootPath, arc)

// # Read

// Setup
function normalizePathSeparators (str) {
  const normalizedPath = path.normalize(str)
  return normalizedPath.replace(/\\/g, '/');
}

export function getAllFilePaths(basePath) {
  const filesList = []
  function loop (dir) {
      const files = fs.readdirSync(dir);
      for (const file of files) {
          const filePath = path.join(dir, file);
  
          if (fs.statSync(filePath).isDirectory()) {
              // If it's a directory, recursively call the function on that directory
              loop(filePath);
          } else {
              // If it's a file, calculate the relative path and add it to the list
              const relativePath = path.relative(basePath, filePath);
              const normalizePath = normalizePathSeparators(relativePath)
              filesList.push(normalizePath);
          }
      }
  }
  loop(basePath)
  return filesList;
}

// put it all together
async function read(basePath) {
    let allFilePaths = getAllFilePaths(basePath)
    // Initiates an ARC from FileSystem but no ISA info.
    let arc = ARC.fromFilePaths(allFilePaths)
    // Read contracts will tell us what we need to read from disc.
    let readContracts = arc.GetReadContracts()
    console.log(readContracts)
    let fcontracts = await Promise.all(
        readContracts.map(async (contract) => {
            let content = await fulfillReadContract(basePath, contract)
            contract.DTO = content
            return (contract) 
        })
    )
    arc.SetISAFromContracts(fcontracts)
    return arc
}

// Execution

await read(arcRootPath).then(arc => console.log(arc))
