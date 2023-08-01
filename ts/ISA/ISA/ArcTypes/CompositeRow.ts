import { fold } from "../../../fable_modules/fable-library-ts/Seq.js";
import { Protocol_get_empty, Protocol, Protocol_addComponent, Protocol_addParameter, Protocol_setName, Protocol_setDescription, Protocol_setUri, Protocol_setVersion, Protocol_setProtocolType } from "../JsonTypes/Protocol.js";
import { ProtocolParameter_create_2769312B } from "../JsonTypes/ProtocolParameter.js";
import { Component_create_Z33AADEE0 } from "../JsonTypes/Component.js";
import { Value_Ontology, Value_fromString_Z721C83C5 } from "../JsonTypes/Value.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { CompositeCell_$union, CompositeCell } from "./CompositeCell.js";
import { CompositeHeader_$union, CompositeHeader } from "./CompositeHeader.js";

export function toProtocol(row: Iterable<[CompositeHeader_$union, CompositeCell_$union]>): Protocol {
    return fold<[CompositeHeader_$union, CompositeCell_$union], Protocol>((p: Protocol, hc: [CompositeHeader_$union, CompositeCell_$union]): Protocol => {
        let matchResult: int32, oa: OntologyAnnotation, v: string, v_1: string, v_2: string, v_3: string, oa_1: OntologyAnnotation, oa_2: OntologyAnnotation, unit: OntologyAnnotation, v_4: string, oa_3: OntologyAnnotation, t: OntologyAnnotation;
        switch (hc[0].tag) {
            case /* ProtocolType */ 4: {
                if (hc[1].tag === /* Term */ 0) {
                    matchResult = 0;
                    oa = (hc[1] as CompositeCell<0>).fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case /* ProtocolVersion */ 7: {
                if (hc[1].tag === /* FreeText */ 1) {
                    matchResult = 1;
                    v = (hc[1] as CompositeCell<1>).fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case /* ProtocolUri */ 6: {
                if (hc[1].tag === /* FreeText */ 1) {
                    matchResult = 2;
                    v_1 = (hc[1] as CompositeCell<1>).fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case /* ProtocolDescription */ 5: {
                if (hc[1].tag === /* FreeText */ 1) {
                    matchResult = 3;
                    v_2 = (hc[1] as CompositeCell<1>).fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case /* ProtocolREF */ 8: {
                if (hc[1].tag === /* FreeText */ 1) {
                    matchResult = 4;
                    v_3 = (hc[1] as CompositeCell<1>).fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case /* Parameter */ 3: {
                matchResult = 5;
                oa_1 = (hc[0] as CompositeHeader<3>).fields[0];
                break;
            }
            case /* Component */ 0: {
                switch (hc[1].tag) {
                    case /* Unitized */ 2: {
                        matchResult = 6;
                        oa_2 = (hc[0] as CompositeHeader<0>).fields[0];
                        unit = (hc[1] as CompositeCell<2>).fields[1];
                        v_4 = (hc[1] as CompositeCell<2>).fields[0];
                        break;
                    }
                    case /* Term */ 0: {
                        matchResult = 7;
                        oa_3 = (hc[0] as CompositeHeader<0>).fields[0];
                        t = (hc[1] as CompositeCell<0>).fields[0];
                        break;
                    }
                    default:
                        matchResult = 8;
                }
                break;
            }
            default:
                matchResult = 8;
        }
        switch (matchResult) {
            case 0:
                return Protocol_setProtocolType(p, oa!);
            case 1:
                return Protocol_setVersion(p, v!);
            case 2:
                return Protocol_setUri(p, v_1!);
            case 3:
                return Protocol_setDescription(p, v_2!);
            case 4:
                return Protocol_setName(p, v_3!);
            case 5:
                return Protocol_addParameter(ProtocolParameter_create_2769312B(void 0, oa_1!), p);
            case 6:
                return Protocol_addComponent(Component_create_Z33AADEE0(void 0, Value_fromString_Z721C83C5(v_4!), unit!, oa_2!), p);
            case 7:
                return Protocol_addComponent(Component_create_Z33AADEE0(void 0, Value_Ontology(t!), void 0, oa_3!), p);
            default:
                return p;
        }
    }, Protocol_get_empty(), row);
}

