import { fold } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { Protocol_get_empty, Protocol_addComponent, Protocol_addParameter, Protocol_setName, Protocol_setDescription, Protocol_setUri, Protocol_setVersion, Protocol_setProtocolType } from "../JsonTypes/Protocol.js";
import { ProtocolParameter_create_Z6C54B221 } from "../JsonTypes/ProtocolParameter.js";
import { Component_create_61502994 } from "../JsonTypes/Component.js";
import { Value, Value_fromString_Z721C83C5 } from "../JsonTypes/Value.js";

export function toProtocol(row) {
    return fold((p, hc) => {
        let matchResult, oa, v, v_1, v_2, v_3, oa_1, oa_2, unit, v_4, oa_3, t;
        switch (hc[0].tag) {
            case 4: {
                if (hc[1].tag === 0) {
                    matchResult = 0;
                    oa = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 7: {
                if (hc[1].tag === 1) {
                    matchResult = 1;
                    v = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 6: {
                if (hc[1].tag === 1) {
                    matchResult = 2;
                    v_1 = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 5: {
                if (hc[1].tag === 1) {
                    matchResult = 3;
                    v_2 = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 8: {
                if (hc[1].tag === 1) {
                    matchResult = 4;
                    v_3 = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 3: {
                matchResult = 5;
                oa_1 = hc[0].fields[0];
                break;
            }
            case 0: {
                switch (hc[1].tag) {
                    case 2: {
                        matchResult = 6;
                        oa_2 = hc[0].fields[0];
                        unit = hc[1].fields[1];
                        v_4 = hc[1].fields[0];
                        break;
                    }
                    case 0: {
                        matchResult = 7;
                        oa_3 = hc[0].fields[0];
                        t = hc[1].fields[0];
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
                return Protocol_setProtocolType(p, oa);
            case 1:
                return Protocol_setVersion(p, v);
            case 2:
                return Protocol_setUri(p, v_1);
            case 3:
                return Protocol_setDescription(p, v_2);
            case 4:
                return Protocol_setName(p, v_3);
            case 5:
                return Protocol_addParameter(ProtocolParameter_create_Z6C54B221(void 0, oa_1), p);
            case 6:
                return Protocol_addComponent(Component_create_61502994(void 0, Value_fromString_Z721C83C5(v_4), unit, oa_2), p);
            case 7:
                return Protocol_addComponent(Component_create_61502994(void 0, new Value(0, [t]), void 0, oa_3), p);
            default:
                return p;
        }
    }, Protocol_get_empty(), row);
}

