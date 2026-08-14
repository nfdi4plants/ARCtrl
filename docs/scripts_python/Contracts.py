import os
from arctrl import Contract, DTO_Spreadsheet, DTO_Text
from fsspreadsheet.xlsx import Xlsx
from pathlib import Path

#  Write

def fulfill_write_contract (basePath : str, contract : Contract) :
    def ensure_directory (filePath: Path) :
        if filePath.suffix or filePath.name.startswith(".") :
            filePath = filePath.parent    
        path_str = str(filePath)
        # Split the path into individual directories
        directories = path_str.split(os.path.sep)
        current_path = ""
        # Iterate through each directory in the path
        for directory in directories:
            # Append the current directory to the current path
            current_path = os.path.join(current_path, directory)
            # Check if the current path exists as a directory
            exists = os.path.exists(current_path)
            if not exists:
                os.makedirs(current_path)

        
    p = Path(basePath).joinpath(contract.Path)
    if contract.Operation == "CREATE" :
        if contract.DTO == None :
            ensure_directory(p)
            Path.write_text(p, "")
        elif contract.DTOType.name == "ISA_Assay" or contract.DTOType.name == "ISA_Study" or contract.DTOType.name == "ISA_Investigation" :
            ensure_directory(p)
            Xlsx.to_xlsx_file(p, contract.DTO.item)
        elif contract.DTOType.name == "PlainText" :
            ensure_directory(p)
            Path.write_text(p, contract.DTO.item)
        else :
            print("Warning: The given contract is not a correct ARC write contract: ", contract)      
    

#  Read

def fulfill_read_contract (basePath : str, contract : Contract) :
    if contract.Operation == "READ" :
        normalizedPath = os.path.normpath(Path(basePath).joinpath(contract.Path))
        if contract.DTOType.name == "ISA_Assay" or contract.DTOType.name == "ISA_Study" or contract.DTOType.name == "ISA_Investigation" :        
            fswb = Xlsx.from_xlsx_file(normalizedPath)
            contract.DTO = DTO_Spreadsheet(fswb)
        elif contract.DTOType.name == "PlainText" :
            content = Path.read_text(normalizedPath)
            contract.DTO = DTO_Text(content)
        else :
            print("Handling of ${contract.DTOType} in a READ contract is not yet implemented")

    else :
        print("Error (fulfillReadContract): ${contract} is not a READ contract")
