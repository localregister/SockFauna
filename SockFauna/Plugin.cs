using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using SockFauna.Creatures;
using UnityEngine;

namespace SockFauna
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.lee23.ecclibrary")]
    //[BepInDependency("com.lee23.bloopandblaza")]

    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static AssetBundle AssetBundle { get; } =
            AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly, "socknautica");

        public static ConfigEntry<bool> registerAbyssalBlazaSpawns;
        public static ConfigEntry<bool> registerAncientBloopSpawns;
        public static ConfigEntry<bool> registerMirageSpawns;
        public static ConfigEntry<bool> registerOculusSpawns;
        public static ConfigEntry<bool> registerMouthSpawns;
        public static ConfigEntry<bool> registerPentaSpawns;

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            InitializeConfig();

            // Initialize custom prefabs
            InitializePrefabs();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            LanguageHandler.RegisterLocalizationFolder();
            ModAudio.RegisterAllAudio();
        }

        private void InitializeConfig()
        {
            registerAbyssalBlazaSpawns = Config.Bind(
                "Spawns",
                "Enable Abyssal Blaza spawns",
                true,
                "Enable Abyssal Blaza spawns");

            registerAncientBloopSpawns = Config.Bind(
                "Spawns",
                "Enable Ancient Bloop spawns",
                true,
                "Enable Ancient Bloop spawns");

            registerMirageSpawns = Config.Bind(
                "Spawns",
                "Enable Mirage Fish spawns",
                true,
                "Enable Mirage Fish spawns");

            registerPentaSpawns = Config.Bind(
                "Spawns",
                "Enable Pentamosa (MultiGarg) spawn",
                false,
                "Enable Pentamosa (MultiGarg) spawn");

            registerOculusSpawns = Config.Bind(
                "Spawns",
                "Enable Lost Oculus spawns",
                true,
                "Enable Lost Oculus spawns");

            registerMouthSpawns = Config.Bind(
                "Spawns",
                "Enable Void Creature spawn",
                false,
                "Enable Void Creature spawn");

            OptionsPanelHandler.RegisterModOptions(new SockOptions());
        }

        private void InitializePrefabs()
        {
            var blazaInfo = PrefabInfo.WithTechType("AbyssalBlaza");
            var bloopInfo = PrefabInfo.WithTechType("AncientBloop");
            var anglerInfo = PrefabInfo.WithTechType("MirageFish");
            var mouthInfo = PrefabInfo.WithTechType("VoidMouth");
            var gargInfo = PrefabInfo.WithTechType("MultiGarg");

            var acublazaInfo = PrefabInfo.WithTechType("AbyssalBlazaACU");
            var acubloopInfo = PrefabInfo.WithTechType("AncientBloopACU");
            var acuanglerInfo = PrefabInfo.WithTechType("MirageFishACU");
            var acugargInfo = PrefabInfo.WithTechType("MultiGargACU");

            if (registerAbyssalBlazaSpawns.Value)
            {
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(blazaInfo.TechType, new[]
                {
                    //Underwater Islands
                    new SpawnLocation(new Vector3(-111, -125, 993)),
                    //Crag Field
                    //new SpawnLocation(new Vector3(435, -150, -1535)),
                    //new SpawnLocation(new Vector3(-107, -90, -1058)),
                    //Grand Reef/Sea Treader Path
                    new SpawnLocation(new Vector3(-1279, -240, -1275)),
                    new SpawnLocation(new Vector3(-374, -196, -936)),
                    //new SpawnLocation(new Vector3(-565, -195, -1508)),
                    new SpawnLocation(new Vector3(-1582, -239, -800)),
                    new SpawnLocation(new Vector3(-957, -225, -1655)),
                    //Blood Kelp
                    //new SpawnLocation(new Vector3(-367, -285, 1371)),
                    //Dunes
                    //new SpawnLocation(new Vector3(-1771, -180, 678)),
                    new SpawnLocation(new Vector3(-1433, -210, 748)),
                    //Bulb Zone
                    new SpawnLocation(new Vector3(1017, -165, 608)),
                    new SpawnLocation(new Vector3(1545, -246, 819))
                });
            }
            var blaza = new AbyssalBlaza(blazaInfo, false);
            blaza.Register();

            var blazaacu = new AbyssalBlaza(acublazaInfo, true);
            blazaacu.Register();
            if (registerAncientBloopSpawns.Value)
            {
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(bloopInfo.TechType, new[]
                {
                    //Crash Zone
                    //new SpawnLocation(new Vector3(710, -262, -1302)),
                    new SpawnLocation(new Vector3(1118, -396, -1502)),
                    //Crag Field
                    new SpawnLocation(new Vector3(-95, -245, -1445)),
                    new SpawnLocation(new Vector3(-107, -110, -1058)),
                    //Mountains
                    new SpawnLocation(new Vector3(731, -215, 1227)),
                    //new SpawnLocation(new Vector3(1035, -150, 969)),
                    new SpawnLocation(new Vector3(233, -125, 1307)),
                    //Dunes
                    //new SpawnLocation(new Vector3(-1714, -320, 354)),
                    //Sparse Reef
                    new SpawnLocation(new Vector3(-1032, -125, -784)),
                    new SpawnLocation(new Vector3(-722, -150, -749))
                });
            }
            var bloop = new AncientBloop(bloopInfo, false);
            bloop.Register();

            var bloopacu = new AncientBloop(acubloopInfo, true);
            bloopacu.Register();
            if (registerMirageSpawns.Value)
            {
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(anglerInfo.TechType, new[]
                {
                    //Grand Reef
                    new SpawnLocation(new Vector3(-586, -360, -1845)),
                    //Crash Zone
                    new SpawnLocation(new Vector3(1679, -327, 370)),
                    //Dunes
                    new SpawnLocation(new Vector3(-1245, -320, 1341)),
                    //Blood Kelp
                    new SpawnLocation(new Vector3(-367, -285, 1371))
                });
            }
            var angler = new Anglerfish(anglerInfo, false);
            angler.Register();

            var angleracu = new Anglerfish(acuanglerInfo, true);
            angleracu.Register();
            if (registerMouthSpawns.Value)
            {
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(mouthInfo.TechType, new[]
                {
                    new SpawnLocation(new Vector3(-395, -544, 2003))
                });
            }
            var mouth = new AbyssalMouth(mouthInfo);
            mouth.Register();
            if (registerPentaSpawns.Value)
            {
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(gargInfo.TechType, new[]
                {
                    new SpawnLocation(new Vector3(418, -120, -1536)),
                    //new SpawnLocation(new Vector3(-1555, -100, -328))
                });
            }
            var garg = new MultiGarg(gargInfo, false);
            garg.Register();

            var gargacu = new MultiGarg(acugargInfo, true);
            gargacu.Register();

            LostOculusClone.Register();
            LostOculusSmall.Register();
        }
    }
}
