
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using ECCLibrary;
using Nautilus.Assets.Gadgets;

namespace SockFauna.Creatures;
    public class LostOculusSmall
{
    Texture2D mainTex;
    Texture2D specTex;
    Texture2D emissiveTex;
    Texture2D normalTex;

    public LostOculusSmall()
    {
        mainTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01");
        emissiveTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_illum");
        normalTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_normal");
        specTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_spec");
    }
    public static void Register()
    {
        PrefabInfo info = PrefabInfo.WithTechType("LostOculusJuvenile");
        var prefab = new CustomPrefab(info);
        var template = new CloneTemplate(info, TechType.Oculus)
        {
            ModifyPrefab = obj =>
            {
                obj.transform.GetChild(0).localScale = Vector3.one * 2f;
                obj.EnsureComponent<EcoTarget>().type = EcoTargetType.Oculus;
                obj.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Far;
                obj.GetComponent<SphereCollider>().radius = 1.2f;
                //obj.GetComponent<Rigidbody>().mass = 50f;
                obj.GetComponent<LiveMixin>().health = 75f;
                obj.GetComponent<Creature>().Tired = new CreatureTrait(0, 1);
                var rendererObj = obj.transform.Find("model/Oculus");
                obj.transform.Find("model/Oculus_LOD1").gameObject.SetActive(false);
                obj.transform.Find("model/Oculus_LOD2").gameObject.SetActive(false);
                obj.transform.Find("model/Oculus_LOD3").gameObject.SetActive(false);
                var renderer = rendererObj.GetComponent<Renderer>();
                var material = renderer.material;
                material.SetTexture("_MainTex", Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01"));
                material.SetTexture("_SpecTex", Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_spec"));
                material.SetTexture("_BumpMap", Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_normal"));
                material.SetTexture("_Illum", Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_illum"));
            }
        };
        prefab.SetGameObject(template);
        CreatureDataUtils.SetAcidImmune(info.TechType);
        //CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(info.TechType, "Lifeforms/Fauna/LargeHerbivores", "Lost Oculus", "string desc", 2f, null, null);
        if (Plugin.registerOculusSpawns.Value)
        {
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(info.TechType, new[]
            {
                    //Precursor Cache
                    new SpawnLocation(new Vector3(-1120, -675, -687)),
                    new SpawnLocation(new Vector3(-1120, -685, -687)),
                    new SpawnLocation(new Vector3(-1120, -670, -687))
            });
            GadgetExtensions.SetSpawns(prefab, new LootDistributionData.BiomeData[]
            {
                new()
                {
                    biome = BiomeType.BonesField_Cave_Ground,
                    probability = 0.75f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.BonesField_LakePit_Floor,
                    probability = 0.4f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.BonesField_Lake_Floor,
                    probability = 0.25f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.BonesField_Corridor_Stream,
                    probability = 0.25f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.LostRiverJunction_LakeFloor,
                    probability = 0.4f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.Canyon_Lake_Floor,
                    probability = 0.3f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.SkeletonCave_Lake_Floor,
                    probability = 1.0f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.LostRiverCorridor_ThermalVents,
                    probability = 0.25f,
                    count = 1
                },
                new()
                {
                    biome = BiomeType.GhostTree_Lake_Floor,
                    probability = 1.5f,
                    count = 1
                }
            });
        }
        prefab.Register();
    }
}
