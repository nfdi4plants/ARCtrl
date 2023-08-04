import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { defaultArg, map as map_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { empty, filter, map, singleton, append, exists, tryFind, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { ProtocolParameter_$reflection, ProtocolParameter } from "./ProtocolParameter.js";
import { Component_$reflection, Component } from "./Component.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";

export class Protocol extends Record implements IEquatable<Protocol> {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly ProtocolType: Option<OntologyAnnotation>;
    readonly Description: Option<string>;
    readonly Uri: Option<string>;
    readonly Version: Option<string>;
    readonly Parameters: Option<FSharpList<ProtocolParameter>>;
    readonly Components: Option<FSharpList<Component>>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, Name: Option<string>, ProtocolType: Option<OntologyAnnotation>, Description: Option<string>, Uri: Option<string>, Version: Option<string>, Parameters: Option<FSharpList<ProtocolParameter>>, Components: Option<FSharpList<Component>>, Comments: Option<FSharpList<Comment$>>) {
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

export function Protocol_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Protocol", [], Protocol, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["ProtocolType", option_type(OntologyAnnotation_$reflection())], ["Description", option_type(string_type)], ["Uri", option_type(string_type)], ["Version", option_type(string_type)], ["Parameters", option_type(list_type(ProtocolParameter_$reflection()))], ["Components", option_type(list_type(Component_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Protocol_make(id: Option<string>, name: Option<string>, protocolType: Option<OntologyAnnotation>, description: Option<string>, uri: Option<string>, version: Option<string>, parameters: Option<FSharpList<ProtocolParameter>>, components: Option<FSharpList<Component>>, comments: Option<FSharpList<Comment$>>): Protocol {
    return new Protocol(id, name, protocolType, description, uri, version, parameters, components, comments);
}

export function Protocol_create_Z7DFD6E67(Id?: string, Name?: string, ProtocolType?: OntologyAnnotation, Description?: string, Uri?: string, Version?: string, Parameters?: FSharpList<ProtocolParameter>, Components?: FSharpList<Component>, Comments?: FSharpList<Comment$>): Protocol {
    return Protocol_make(Id, Name, ProtocolType, Description, Uri, Version, Parameters, Components, Comments);
}

export function Protocol_get_empty(): Protocol {
    return Protocol_create_Z7DFD6E67();
}

/**
 * If a protocol with the given identfier exists in the list, returns it
 */
export function Protocol_tryGetByName(name: string, protocols: FSharpList<Protocol>): Option<Protocol> {
    return tryFind<Protocol>((p: Protocol): boolean => equals(p.Name, name), protocols);
}

/**
 * If a protocol with the given name exists in the list exists, returns true
 */
export function Protocol_existsByName(name: string, protocols: FSharpList<Protocol>): boolean {
    return exists<Protocol>((p: Protocol): boolean => equals(p.Name, name), protocols);
}

/**
 * Adds the given protocol to the protocols
 */
export function Protocol_add(protocols: FSharpList<Protocol>, protocol: Protocol): FSharpList<Protocol> {
    return append<Protocol>(protocols, singleton(protocol));
}

/**
 * Updates all protocols for which the predicate returns true with the given protocol values
 */
export function Protocol_updateBy(predicate: ((arg0: Protocol) => boolean), updateOption: Update_UpdateOptions_$union, protocol: Protocol, protocols: FSharpList<Protocol>): FSharpList<Protocol> {
    if (exists<Protocol>(predicate, protocols)) {
        return map<Protocol, Protocol>((p: Protocol): Protocol => {
            if (predicate(p)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Protocol = p;
                const recordType_2: Protocol = protocol;
                return (this$.tag === /* UpdateAllAppendLists */ 2) ? (makeRecord(Protocol_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol) : ((this$.tag === /* UpdateByExisting */ 1) ? (makeRecord(Protocol_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol) : ((this$.tag === /* UpdateByExistingAppendLists */ 3) ? (makeRecord(Protocol_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol) : recordType_2));
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
export function Protocol_updateByName(updateOption: Update_UpdateOptions_$union, protocol: Protocol, protocols: FSharpList<Protocol>): FSharpList<Protocol> {
    return Protocol_updateBy((p: Protocol): boolean => equals(p.Name, protocol.Name), updateOption, protocol, protocols);
}

/**
 * If a protocol with the given name exists in the list, removes it
 */
export function Protocol_removeByName(name: string, protocols: FSharpList<Protocol>): FSharpList<Protocol> {
    return filter<Protocol>((p: Protocol): boolean => !equals(p.Name, name), protocols);
}

/**
 * Returns comments of a protocol
 */
export function Protocol_getComments_Z5F51792E(protocol: Protocol): Option<FSharpList<Comment$>> {
    return protocol.Comments;
}

/**
 * Applies function f on comments of a protocol
 */
export function Protocol_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, map_1<FSharpList<Comment$>, FSharpList<Comment$>>(f, protocol.Comments));
}

/**
 * Replaces comments of a protocol by given comment list
 */
export function Protocol_setComments(protocol: Protocol, comments: FSharpList<Comment$>): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, comments);
}

/**
 * Returns protocol type of a protocol
 */
export function Protocol_getProtocolType_Z5F51792E(protocol: Protocol): Option<OntologyAnnotation> {
    return protocol.ProtocolType;
}

/**
 * Applies function f on protocol type of a protocol
 */
export function Protocol_mapProtocolType(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, map_1<OntologyAnnotation, OntologyAnnotation>(f, protocol.ProtocolType), protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol type of a protocol by given protocol type
 */
export function Protocol_setProtocolType(protocol: Protocol, protocolType: OntologyAnnotation): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns protocol version of a protocol
 */
export function Protocol_getVersion_Z5F51792E(protocol: Protocol): Option<string> {
    return protocol.Version;
}

/**
 * Applies function f on protocol version of a protocol
 */
export function Protocol_mapVersion(f: ((arg0: string) => string), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, map_1<string, string>(f, protocol.Version), protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol version of a protocol by given protocol version
 */
export function Protocol_setVersion(protocol: Protocol, version: string): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Protocol Name
 * Returns protocol name of a protocol
 */
export function Protocol_getName_Z5F51792E(protocol: Protocol): Option<string> {
    return protocol.Name;
}

/**
 * Applies function f on protocol name of a protocol
 */
export function Protocol_mapName(f: ((arg0: string) => string), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, map_1<string, string>(f, protocol.Name), protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol name of a protocol by given protocol name
 */
export function Protocol_setName(protocol: Protocol, name: string): Protocol {
    return new Protocol(protocol.ID, name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns protocol description of a protocol
 */
export function Protocol_getDescription_Z5F51792E(protocol: Protocol): Option<string> {
    return protocol.Description;
}

/**
 * Applies function f on protocol description of a protocol
 */
export function Protocol_mapDescription(f: ((arg0: string) => string), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, map_1<string, string>(f, protocol.Description), protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol description of a protocol by given protocol description
 */
export function Protocol_setDescription(protocol: Protocol, description: string): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, description, protocol.Uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns protocol URI of a protocol
 */
export function Protocol_getUri_Z5F51792E(protocol: Protocol): Option<string> {
    return protocol.Uri;
}

/**
 * Applies function f on protocol URI of a protocol
 */
export function Protocol_mapUri(f: ((arg0: string) => string), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, map_1<string, string>(f, protocol.Uri), protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol URI of a protocol by given protocol URI
 */
export function Protocol_setUri(protocol: Protocol, uri: string): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, uri, protocol.Version, protocol.Parameters, protocol.Components, protocol.Comments);
}

/**
 * Returns components of a protocol
 */
export function Protocol_getComponents_Z5F51792E(protocol: Protocol): Option<FSharpList<Component>> {
    return protocol.Components;
}

/**
 * Applies function f on components of a protocol
 */
export function Protocol_mapComponents(f: ((arg0: FSharpList<Component>) => FSharpList<Component>), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, map_1<FSharpList<Component>, FSharpList<Component>>(f, protocol.Components), protocol.Comments);
}

/**
 * Replaces components of a protocol by given component list
 */
export function Protocol_setComponents(protocol: Protocol, components: FSharpList<Component>): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, protocol.Parameters, components, protocol.Comments);
}

export function Protocol_addComponent(comp: Component, protocol: Protocol): Protocol {
    return Protocol_setComponents(protocol, append<Component>(defaultArg(protocol.Components, empty<Component>()), singleton(comp)));
}

/**
 * Returns protocol parameters of a protocol
 */
export function Protocol_getParameters_Z5F51792E(protocol: Protocol): Option<FSharpList<ProtocolParameter>> {
    return protocol.Parameters;
}

/**
 * Applies function f on protocol parameters of a protocol
 */
export function Protocol_mapParameters(f: ((arg0: FSharpList<ProtocolParameter>) => FSharpList<ProtocolParameter>), protocol: Protocol): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, map_1<FSharpList<ProtocolParameter>, FSharpList<ProtocolParameter>>(f, protocol.Parameters), protocol.Components, protocol.Comments);
}

/**
 * Replaces protocol parameters of a protocol by given protocol parameter list
 */
export function Protocol_setParameters(protocol: Protocol, parameters: FSharpList<ProtocolParameter>): Protocol {
    return new Protocol(protocol.ID, protocol.Name, protocol.ProtocolType, protocol.Description, protocol.Uri, protocol.Version, parameters, protocol.Components, protocol.Comments);
}

export function Protocol_addParameter(parameter: ProtocolParameter, protocol: Protocol): Protocol {
    return Protocol_setParameters(protocol, append<ProtocolParameter>(defaultArg(protocol.Parameters, empty<ProtocolParameter>()), singleton(parameter)));
}

