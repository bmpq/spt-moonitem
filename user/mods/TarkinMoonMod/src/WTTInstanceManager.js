"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.WTTInstanceManager = void 0;
const node_path_1 = __importDefault(require("node:path"));
class WTTInstanceManager {
    //#region Accessible in or after preSptLoad
    modName;
    debug;
    // Useful Paths
    profilePath = node_path_1.default.join(process.cwd(), "\\user\\profiles");
    // Instances
    container;
    PreSptModLoader;
    configServer;
    saveServer;
    itemHelper;
    logger;
    staticRouter;
    dynamicRouter;
    profileController;
    profileCallbacks;
    //#endregion
    //#region Acceessible in or after postDBLoad
    database;
    customItem;
    imageRouter;
    jsonUtil;
    profileHelper;
    eventOutputHolder;
    ragfairPriceService;
    importerUtil;
    traderAssortService;
    applicationContext;
    //#endregion
    // Call at the start of the mods postDBLoad method
    preSptLoad(container, mod) {
        this.modName = mod;
        this.container = container;
        this.PreSptModLoader = container.resolve("PreSptModLoader");
        this.imageRouter = container.resolve("ImageRouter");
        this.configServer = container.resolve("ConfigServer");
        this.saveServer = container.resolve("SaveServer");
        this.itemHelper = container.resolve("ItemHelper");
        this.eventOutputHolder = container.resolve("EventOutputHolder");
        this.profileController = container.resolve("ProfileController");
        this.profileCallbacks = container.resolve("ProfileCallbacks");
        this.logger = container.resolve("WinstonLogger");
        this.staticRouter = container.resolve("StaticRouterModService");
        this.dynamicRouter = container.resolve("DynamicRouterModService");
        this.traderAssortService = container.resolve("TraderAssortService");
    }
    postDBLoad(container) {
        this.database = container.resolve("DatabaseServer").getTables();
        this.customItem = container.resolve("CustomItemService");
        this.jsonUtil = container.resolve("JsonUtil");
        this.profileHelper = container.resolve("ProfileHelper");
        this.ragfairPriceService = container.resolve("RagfairPriceService");
        this.importerUtil = container.resolve("ImporterUtil");
        this.applicationContext = container.resolve("ApplicationContext");
    }
    colorLog(message, color) {
        const colorCodes = {
            red: "\x1b[31m",
            green: "\x1b[32m",
            yellow: "\x1b[33m",
            blue: "\x1b[34m",
            magenta: "\x1b[35m",
            cyan: "\x1b[36m",
            white: "\x1b[37m",
            gray: "\x1b[90m",
            brightRed: "\x1b[91m",
            brightGreen: "\x1b[92m",
            brightYellow: "\x1b[93m",
            brightBlue: "\x1b[94m",
            brightMagenta: "\x1b[95m",
            brightCyan: "\x1b[96m",
            brightWhite: "\x1b[97m"
        };
        const resetCode = "\x1b[0m";
        const colorCode = colorCodes[color] || "\x1b[37m"; // Default to white if color is invalid.
        console.log(`${colorCode}${message}${resetCode}`); // Log the colored message here
    }
}
exports.WTTInstanceManager = WTTInstanceManager;
//# sourceMappingURL=WTTInstanceManager.js.map