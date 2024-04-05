from arctrl.arctrl import ArcInvestigation, Comment, XlsxController, JsonController
from fsspreadsheet.xlsx import Xlsx

# Comments
investigation_comments = ArcInvestigation.init("My Investigation")

new_comment = Comment("The Name", "The Value")
new_comment2 = Comment("My other Name", "My other Value")

investigation_comments.Comments.append(new_comment)
investigation_comments.Comments.append(new_comment2)

print(investigation_comments)

# IO

# XLSX - Write
fswb = XlsxController.Investigation().to_fs_workbook(investigation_comments)

Xlsx.to_xlsx_file("test.isa.investigation.xlsx", fswb)

# Json - Write
investigation = ArcInvestigation.init("My Investigation")

json_str = JsonController.Investigation().to_json_string(investigation)

print(json_str)

# Json - Read
json_string = json_str

investigation_2 = JsonController.Investigation().from_json_string(json_string)

print(investigation_2.__eq__(investigation))