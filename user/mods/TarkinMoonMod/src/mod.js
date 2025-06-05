"use strict";
/* eslint-disable @typescript-eslint/naming-convention */
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const node_fs_1 = __importDefault(require("node:fs"));
const node_path_1 = __importDefault(require("node:path"));
const LogTextColor_1 = require("C:/snapshot/project/obj/models/spt/logging/LogTextColor");
const WTTInstanceManager_1 = require("./WTTInstanceManager");
const CustomItemService_1 = require("./CustomItemService");
const CustomAssortSchemeService_1 = require("./CustomAssortSchemeService");
class TarkinMoonMod {
    instanceManager = new WTTInstanceManager_1.WTTInstanceManager();
    version;
    modName = "TarkinMoonMod";
    customItemService = new CustomItemService_1.CustomItemService();
    customAssortSchemeService = new CustomAssortSchemeService_1.CustomAssortSchemeService();
    debug = false;
    // Anything that needs done on preSptLoad, place here.
    preSptLoad(container) {
        // Initialize the instance manager DO NOTHING ELSE BEFORE THIS
        this.instanceManager.preSptLoad(container, this.modName);
        this.instanceManager.debug = this.debug;
        // EVERYTHING AFTER HERE MUST USE THE INSTANCE
        this.getVersionFromJson();
        this.displayCreditBanner();
        this.customItemService.preSptLoad(this.instanceManager);
        this.customAssortSchemeService.preSptLoad(this.instanceManager);
    }
    // Anything that needs done on postDBLoad, place here.
    postDBLoad(container) {
        // Initialize the instance manager DO NOTHING ELSE BEFORE THIS
        this.instanceManager.postDBLoad(container);
        // EVERYTHING AFTER HERE MUST USE THE INSTANCE
        this.customItemService.postDBLoad();
        this.customAssortSchemeService.postDBLoad();
        this.instanceManager.colorLog(`[${this.modName}] Database: Loading complete.`, LogTextColor_1.LogTextColor.GREEN);
    }
    getVersionFromJson() {
        const packageJsonPath = node_path_1.default.join(__dirname, "../package.json");
        node_fs_1.default.readFile(packageJsonPath, "utf-8", (err, data) => {
            if (err) {
                console.error("Error reading file:", err);
                return;
            }
            const jsonData = JSON.parse(data);
            this.version = jsonData.version;
        });
    }
    displayCreditBanner() {
        this.instanceManager.colorLog(`[${this.modName}] Tarkin's Moon Mod`, "green");
    }
}
module.exports = { mod: new TarkinMoonMod() };
//# sourceMappingURL=mod.js.map