"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.CustomAssortSchemeService = void 0;
const CustomAssortSchemes_json_1 = __importDefault(require("../db/CustomAssortSchemes/CustomAssortSchemes.json"));
const configConsts_1 = require("./references/configConsts");
class CustomAssortSchemeService {
    instanceManager;
    preSptLoad(instanceManager) {
        this.instanceManager = instanceManager;
    }
    postDBLoad() {
        const tables = this.instanceManager.database;
        for (const traderId in CustomAssortSchemes_json_1.default) {
            const traderIdFromMap = configConsts_1.traderIDs[traderId];
            const finalTraderId = traderIdFromMap || traderId;
            const trader = tables.traders[finalTraderId];
            if (!trader) {
                return;
            }
            const newAssort = CustomAssortSchemes_json_1.default[traderId];
            for (const item of newAssort.items) {
                trader.assort.items.push(item);
            }
            for (const [itemName, scheme] of Object.entries(newAssort.barter_scheme)) {
                trader.assort.barter_scheme[itemName] = scheme;
            }
            for (const [itemName, count] of Object.entries(newAssort.loyal_level_items)) {
                trader.assort.loyal_level_items[itemName] = count;
            }
        }
    }
}
exports.CustomAssortSchemeService = CustomAssortSchemeService;
//# sourceMappingURL=CustomAssortSchemeService.js.map