import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { ProtocolParameter_$reflection } from "./ProtocolParameter.js";
import { Component_$reflection } from "./Component.js";
import { Comment$_$reflection } from "./Comment.js";
import { empty, filter, map, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { defaultArg, map as map_1 } from "../../../fable_modules/fable-library.4.1.4/Option.js";

export class Protocol extends Record {
    constructor(ID, Name, ProtocolType, Description, Uri, Version, Parameters, Components, Comments) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.ProtocolType = ProtocolType;
        this.Description = Description;
        this.Uri = Uri;
        this.Version = Version;
        this.Parameters = Parameters;
        this.Components = Components;
        this.Comments = Comments;
    }
}

export function Protocol_$reflection() {
    return record_type("ISA.Protocol", [], Protocol, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["ProtocolType", option_type(OntologyAnnotation_$reflection())], ["Description", option_type(string_type)], ["Uri", option_type(string_type)], ["Version", option_type(string_type)], ["Parameters", option_type(list_type(ProtocolParameter_$reflection()))], ["Components", option_type(list_type(Component_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Protocol_make(id, name, protocolType, description, uri, version, parameters, components, comments) {
    return new Protocol(id, name, protocolType, description, uri, version, parameters, components, comments);
}

export function Protocol_create_Z4D717767(Id, Name, ProtocolType, Description, Uri, Version, Parameters, Components, Comments) {
    return Protocol_make(Id, Name, ProtocolType, Description, Uri, Version, Parameters, Components, Comments);
}

export function Protocol_get_empty() {
    return Protocol_create_Z4D717767();
}

/**
 * If a protocol with the given identfier exists in the list, returns it
 */
export function Protocol_tryGetByName(name, protocols) {
    return tryFind((p) => equals(p.Name, name), protocols);
}

/**
 * If a protocol with the given name exists in the list exists, returns true
 */
export function Protocol_existsByName(name, protocols) {
    return exists((p) => equals(p.Name, name), protocols);
}

/**
 * Adds the given protocol to the protocols
 */
export function Protocol_add(protocols, protocol) {
    return append(protocols, singleton(protocol));
}

/**
 * Updates all protocols for which the predicate returns true with the given protocol values
 */
export function Protocol_updateBy(predicate, updateOption, protocol, protocols) {
    if (exists(predicate, protocols)) {
        return map((p) => {
            if (predicate(p)) {
                const this$ = updateOption;
                const recordType_1 = p;
                const recordType_2 = protocol;
                return (this$.tag === 2) ? makeRecord(Protocol_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 1) ? makeRecord(Protocol_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 3) ? makeRecord(Protocol_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : recordType_2));
            }
            else {
                return p;
            }
        }, protocols);
    }
    else {
        return protocols;
    }
}

/**
 * Updates all protocols with the same name as the given protocol with its values
 */
export function Protocol_updateByName(updateOption, protocol, protocols) {
    return Protocol_updateBy((p) => equals(p.Name, protocol.Name), updateOption, protocol, protocols);
}

/**
 * If a protocol with the given name exists in the list, removes it
 */
export function Protocol_removeByName(name, protocols) {
    return filter((p) => !equals(p.Name, name), protocols);
}

/**
 * Returns comments of a protocol
 */
export function Protocol_getComments_4D8AC666(protocol) {
    return protocol.Comments;
}

/**
 * Applies function f on comments of a protocol
 */
export function Protocol_mapComments(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, map_1(f, protocol.Comments));
}

/**
 * Replaces comments of a protocol by given comment list
 */
export function Protocol_setComments(protocol, comments) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, comments);
}

/**
 * Returns protocol type of a protocol
 */
export function Protocol_getProtocolType_4D8AC666(protocol) {
    return protocol.ProtocolType;
}

/**
 * Applies function f on protocol type of a protocol
 */
export function Protocol_mapProtocolType(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, map_1(f, protocol.ProtocolType), protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol type of a protocol by given protocol type
 */
export function Protocol_setProtocolType(protocol, protocolType) {
    return new Protocol(protocol.ID, protocol.Name, protocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns protocol version of a protocol
 */
export function Protocol_getVersion_4D8AC666(protocol) {
    return protocol.Version;
}

/**
 * Applies function f on protocol version of a protocol
 */
export function Protocol_mapVersion(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, map_1(f, protocol.Version), protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol version of a protocol by given protocol version
 */
export function Protocol_setVersion(protocol, version) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Protocol Name
 * Returns protocol name of a protocol
 */
export function Protocol_getName_4D8AC666(protocol) {
    return protocol.Name;
}

/**
 * Applies function f on protocol name of a protocol
 */
export function Protocol_mapName(f, protocol) {
    return new Protocol(protocol.ID, map_1(f, protocol.Name), protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol name of a protocol by given protocol name
 */
export function Protocol_setName(protocol, name) {
    return new Protocol(protocol.ID, name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns protocol description of a protocol
 */
export function Protocol_getDescription_4D8AC666(protocol) {
    return protocol.Description;
}

/**
 * Applies function f on protocol description of a protocol
 */
export function Protocol_mapDescription(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, map_1(f, protocol.Description), protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol description of a protocol by given protocol description
 */
export function Protocol_setDescription(protocol, description) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns protocol URI of a protocol
 */
export function Protocol_getUri_4D8AC666(protocol) {
    return protocol.Uri;
}

/**
 * Applies function f on protocol URI of a protocol
 */
export function Protocol_mapUri(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, map_1(f, protocol.Uri), protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol URI of a protocol by given protocol URI
 */
export function Protocol_setUri(protocol, uri) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns components of a protocol
 */
export function Protocol_getComponents_4D8AC666(protocol) {
    return protocol.Components;
}

/**
 * Applies function f on components of a protocol
 */
export function Protocol_mapComponents(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, map_1(f, protocol.Components), protocol.Comments);
}

/**
 * Replaces components of a protocol by given component list
 */
export function Protocol_setComponents(protocol, components) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, components, protocol.Comments);
}

export function Protocol_addComponent(comp, protocol) {
    return Protocol_setComponents(protocol, append(defaultArg(protocol.Components, empty()), singleton(comp)));
}

/**
 * Returns protocol parameters of a protocol
 */
export function Protocol_getParameters_4D8AC666(protocol) {
    return protocol.Parameters;
}

/**
 * Applies function f on protocol parameters of a protocol
 */
export function Protocol_mapParameters(f, protocol) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, map_1(f, protocol.Parameters), protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol parameters of a protocol by given protocol parameter list
 */
export function Protocol_setParameters(protocol, parameters) {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, parameters, protocol.Components, protocol.Comments);
}

export function Protocol_addParameter(parameter, protocol) {
    return Protocol_setParameters(protocol, append(defaultArg(protocol.Parameters, empty()), singleton(parameter)));
}

