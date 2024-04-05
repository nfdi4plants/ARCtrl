import {Xlsx} from "@fslab/fsspreadsheet";
import fs from "fs";
import path from "path";

// Write

export function normalizePathSeparators (str) {
  const normalizedPath = path.normalize(str)
  return normalizedPath.replace(/\\/g, '/');
}


export async function fulfillWriteContract (basePath, contract) {
    function ensureDirectory (filePath) {
        let dirPath = path.dirname(filePath)
        if (!fs.existsSync(dirPath)){
            fs.mkdirSync(dirPath, { recursive: true });
        }
    }
    const p = path.join(basePath,contract.Path)
    if (contract.Operation = "CREATE") {
        if (contract.DTO == undefined) {
            ensureDirectory(p)
            fs.writeFileSync(p, "")
        } else if (contract.DTOType == "ISA_Assay" || contract.DTOType == "ISA_Assay" || contract.DTOType == "ISA_Investigation") {
            ensureDirectory(p)
            await Xlsx.toFile(p, contract.DTO)
        } else if (contract.DTOType == "PlainText") {
            ensureDirectory(p)
            fs.writeFileSync(p, contract.DTO)
        } else {
            console.log("Warning: The given contract is not a correct ARC write contract: ", contract)
        }
    }
}

// Read

export async function fulfillReadContract (basePath, contract) {
  async function fulfill() {
      const normalizedPath = normalizePathSeparators(path.join(basePath, contract.Path))
      switch (contract.DTOType) {
          case "ISA_Assay":
          case "ISA_Study":
          case "ISA_Investigation":
              let fswb = await Xlsx.fromXlsxFile(normalizedPath)
              return fswb
              break;
          case "PlainText":
              let content = fs.readFile(normalizedPath)
              return content
              break;
          default:
              console.log(`Handling of ${contract.DTOType} in a READ contract is not yet implemented`)
      }
  }
  if (contract.Operation == "READ") {
      return await fulfill()
  } else {
      console.error(`Error (fulfillReadContract): "${contract}" is not a READ contract`)
  }
}