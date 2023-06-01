import { Assay } from "ISA/Assay";
import { ARC } from "./ARC";

let arc = ARC.create(null, null, null)

let assay = Assay.create({ FileName: "soos" })

ARC.addAssay(assay,"",arc)