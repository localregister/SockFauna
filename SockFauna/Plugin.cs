using System.Reflection;
using BepInEx;
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

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // Initialize custom prefabs
            InitializePrefabs();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            LanguageHandler.RegisterLocalizationFolder();
            ModAudio.RegisterAllAudio();
        }


        private void InitializePrefabs()
        {
            var blazaInfo = PrefabInfo.WithTechType("AbyssalBlaza");
            var bloopInfo = PrefabInfo.WithTechType("AncientBloop");
            var anglerInfo = PrefabInfo.WithTechType("MirageFish");
            var mouthInfo = PrefabInfo.WithTechType("VoidMouth");
            var gargInfo = PrefabInfo.WithTechType("MultiGarg");

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
            var blaza = new AbyssalBlaza(blazaInfo);
            blaza.Register();

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
            var bloop = new AncientBloop(bloopInfo);
            bloop.Register();
            LostOculusClone.Register();
            
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
            var angler = new Anglerfish(anglerInfo);
            angler.Register();
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(mouthInfo.TechType, new[]
            {
                new SpawnLocation(new Vector3(-395, -544, 2003))
            });
            var mouth = new AbyssalMouth(mouthInfo);
            mouth.Register();
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(gargInfo.TechType, new[]
            {
                new SpawnLocation(new Vector3(418, -120, -1536)),
                //new SpawnLocation(new Vector3(-1555, -100, -328))
            });
            var garg = new MultiGarg(gargInfo);
            garg.Register();
        }
    }
}
