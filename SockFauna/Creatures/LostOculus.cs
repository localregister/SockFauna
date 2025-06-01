
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using ECCLibrary;

namespace SockFauna.Creatures;
    public class LostOculusClone
{
    Texture2D mainTex;
    Texture2D specTex;
    Texture2D emissiveTex;
    Texture2D normalTex;

    public LostOculusClone()
    {
        mainTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01");
        emissiveTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_illum");
        normalTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_normal");
        specTex = Plugin.AssetBundle.LoadAsset<Texture2D>("oculus_01_spec");
    }
        public static void Register()
        {
            PrefabInfo info = PrefabInfo.WithTechType("LostOculus");
            var prefab = new CustomPrefab(info);
            var template = new CloneTemplate(info, TechType.Oculus)
            {
                ModifyPrefab = obj => {
                    obj.transform.GetChild(0).localScale = Vector3.one * 6f;
                    Object.DestroyImmediate(obj.GetComponent<Pickupable>());
                    obj.EnsureComponent<EcoTarget>().type = EcoTargetType.MediumFish;
                    obj.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Far;
                    obj.GetComponent<SphereCollider>().radius = 2.8f;
                    obj.GetComponent<Rigidbody>().mass = 50f;
                    obj.GetComponent<LiveMixin>().health = 300f;
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
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawnsForOneTechType(info.TechType, new[]
        {
                //Skeleton Cave
                new SpawnLocation(new Vector3(-989, -675, -626)),
                new SpawnLocation(new Vector3(-1109, -679, -647)),
                new SpawnLocation(new Vector3(-1076, -713, -570)),
                new SpawnLocation(new Vector3(-996, -636, -616)),
                //Precursor Cache
                new SpawnLocation(new Vector3(-1120, -680, -687)),
                //Disease Research Facility
                new SpawnLocation(new Vector3(-235, -801, 338)),
                new SpawnLocation(new Vector3(-217, -790, 307)),
                new SpawnLocation(new Vector3(-355, -840, 261)),
                new SpawnLocation(new Vector3(-282, -701, 123)),
                new SpawnLocation(new Vector3(-363, -701, 148)),
                new SpawnLocation(new Vector3(-148, -820, 226)),
                //Garg Skull
                new SpawnLocation(new Vector3(-724, -760, -269)),
                new SpawnLocation(new Vector3(-717, -741, -231)),
                new SpawnLocation(new Vector3(-678, -757, -279)),
                //Sea Dragon Fossil
                new SpawnLocation(new Vector3(-635, -826, 328)),
                new SpawnLocation(new Vector3(-645, -830, 284)),
                new SpawnLocation(new Vector3(-620, -838, 262)),
                //Misc
                new SpawnLocation(new Vector3(-1042, -684, -152)),
                new SpawnLocation(new Vector3(-1111, -683, -57)),
                new SpawnLocation(new Vector3(-1007, -682, -64)),
                new SpawnLocation(new Vector3(-883, -633, 665)),
                new SpawnLocation(new Vector3(-922, -614, 1004)),
                new SpawnLocation(new Vector3(-948, -617, 988)),
                new SpawnLocation(new Vector3(-816, -744, -327)),
                new SpawnLocation(new Vector3(387, -833, 863))
        });
        prefab.Register();
        }
}
