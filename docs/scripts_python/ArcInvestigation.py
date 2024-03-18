from arctrl.arctrl import ArcInvestigation, Comment
from fsspreadsheet.xlsx import Xlsx
from arctrl.ISA.ISA_Spreadsheet.arc_investigation import to_fs_workbook, from_fs_workbook
from arctrl.ISA.ISA_Json.ArcTypes.arc_investigation import ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString

# Comments
investigation_comments = ArcInvestigation.init("My Investigation")

new_comment = Comment.create("The Id", "The Name", "The Value")
new_comment2 = Comment.create("My other ID", "My other Name", "My other Value")

investigation_comments.Comments.append(new_comment)
investigation_comments.Comments.append(new_comment2)

print(investigation_comments)

# IO

# XLSX - Write
fswb = to_fs_workbook(investigation_comments)

Xlsx.to_file("test.isa.investigation.xlsx", fswb)

# Json - Write
investigation = ArcInvestigation.init("My Investigation")

json_str = ArcInvestigation_toJsonString(investigation)

print(json_str)

# Json - Read
json_string = json_str

investigation_2 = ArcInvestigation_fromJsonString(json_string)

print(investigation_2.__eq__(investigation))