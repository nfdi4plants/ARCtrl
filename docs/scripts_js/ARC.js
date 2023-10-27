import {ARC} from "@nfdi4plants/arctrl";
import {fulfillWriteContract} from "./Contracts.js";
import fs from "fs";
import path from "path";
import { Xlsx } from "fsspreadsheet";
import ExcelJs from "exceljs"

// # Create

let arc = new ARC()

// # Write
const arcRootPath = "C:/Users/Kevin/Desktop/NewTestARCJS"

async function write(arcPath, arc)  {
    let contracts = arc.GetWriteContracts()
    contracts.forEach(async contract => {
        // from Contracts.js docs
        await fulfillWriteContract(arcPath,contract)
    });
}

// await write(arcRootPath, arc)

// # Read

function normalizePathSeparators (str) {
    const normalizedPath = path.normalize(str)
    return normalizedPath.replace(/\\/g, '/');
}

function getAllFilePaths(basePath) {
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

async function read(basePath) {
    let allFilePaths = getAllFilePaths(basePath)
    // Initiates an ARC from FileSystem but no ISA info.
    let arc = ARC.fromFilePaths(allFilePaths)
    // Read contracts will tell us what we need to read from disc.
    let readContracts = arc.GetReadContracts()
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

read(arcRootPath).then(arc => console.log(arc))
