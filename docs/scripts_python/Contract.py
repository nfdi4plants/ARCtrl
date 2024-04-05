import os
from arctrl.Contract.contract import Contract
from fsspreadsheet.xlsx import Xlsx
from pathlib import Path

#  Write

def fulfill_write_contract (basePath : str, contract : Contract) :
    def ensure_directory (filePath) :
        if not(Path.is_dir (filePath)) :
            Path.mkdir(filePath)
        
    p = Path.joinpath(basePath,contract.Path)
    if contract.Operation == "CREATE" :
        if contract.DTO == None :
            ensure_directory(p)
            Path.write_text(p, "")
        elif contract.DTOType == "ISA_Assay" or contract.DTOType == "ISA_Assay" or contract.DTOType == "ISA_Investigation" :
            ensure_directory(p)
            Xlsx.to_file(p, contract.DTO)
        elif contract.DTOType == "PlainText" :
            ensure_directory(p)
            Path.write_text(p, contract.DTO)
        else :
            print("Warning: The given contract is not a correct ARC write contract: ", contract)      
    

#  Read

def fulfill_read_contract (basePath : str, contract : Contract) :
    if contract.Operation == "READ" :
        normalizedPath = os.path.normpath(Path.joinpath(basePath, contract.Path))
        if contract.DTOType == "ISA_Assay" or contract.DTOType == "ISA_Assay" or contract.DTOType == "ISA_Investigation" :        
            fswb = Xlsx.from_xlsx_file(normalizedPath)
            contract.DTO = fswb
        elif contract.DTOType ==  "PlainText" :
            content = Path.read_text(normalizedPath)
            contract.DTO = content
        else :
            print("Handling of ${contract.DTOType} in a READ contract is not yet implemented")

    else :
        print("Error (fulfillReadContract): ${contract} is not a READ contract")
