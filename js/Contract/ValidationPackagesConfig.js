import { combineMany } from "../FileSystem/Path.js";
import { Contract } from "./Contract.js";
import { item, equalsWith } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_fromYamlString_Static_Z721C83C5, ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toYamlString_Static_71136F3F } from "../Yaml/ValidationPackagesConfig.js";

export const ValidationPackagesConfigHelper_ConfigFilePath = combineMany([".arc", "validation_packages.yml"]);

export const ValidationPackagesConfigHelper_ReadContract = new Contract("READ", ValidationPackagesConfigHelper_ConfigFilePath, "YAML", undefined);

export function ValidationPackagesConfigExtensions_$007CValidationPackagesYamlPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 2)) {
        if (item(0, input) === ".arc") {
            if (item(1, input) === "validation_packages.yml") {
                matchResult = 0;
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return combineMany(input);
        default:
            return undefined;
    }
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_ToCreateContract(this$) {
    return Contract.createCreate(ValidationPackagesConfigHelper_ConfigFilePath, "YAML", ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toYamlString_Static_71136F3F()(this$));
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_ToDeleteContract(this$) {
    return Contract.createDelete(ValidationPackagesConfigHelper_ConfigFilePath);
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toDeleteContract_Static_724DAE55(config) {
    return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_ToDeleteContract(config);
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toCreateContract_Static_724DAE55(config) {
    return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_ToCreateContract(config);
}

export function ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_tryFromReadContract_Static_7570923F(c) {
    let yaml;
    let matchResult, p_1, yaml_1;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "YAML") {
                if (c.DTO != null) {
                    if (typeof c.DTO === "string") {
                        if ((yaml = c.DTO, c.Path === ValidationPackagesConfigHelper_ConfigFilePath)) {
                            matchResult = 0;
                            p_1 = c.Path;
                            yaml_1 = c.DTO;
                        }
                        else {
                            matchResult = 1;
                        }
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_fromYamlString_Static_Z721C83C5(yaml_1);
        default:
            return undefined;
    }
}

