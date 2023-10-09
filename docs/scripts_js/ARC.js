import {ARC} from "@nfdi4plants/arctrl";
import {Xlsx} from "fsspreadsheet";
import fs from "fs";
import path from "path";

// Create

let arc = new ARC()

// Write
const arcRootPath = "C:/Users/Kevin/Desktop/NewTestARCJS"

async function fulfillWriteContract (basePath, contract) {
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
        } else if (contract.DTOType == "ISA_Assay" || contract.DTOType == "ISA_Assay" || "ISA_Investigation") {
            ensureDirectory(p)
            await Xlsx.toFile(p, contract.DTO)
            console.log("ISA", p)
        } else if (contract.DTOType == "PlainText") {
            ensureDirectory(p)
            fs.writeFileSync(p, contract.DTO)
        } else {
            console.log("Warning: The given contract is not a correct ARC write contract: ", contract)
        }
    }
}

async function write(arcPath, arc)  {
    let contracts = arc.GetWriteContracts()
    contracts.forEach(async contract => {
        await fulfillWriteContract(arcPath,contract)
    });
}

await write(arcRootPath,arc)