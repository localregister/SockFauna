using System.Collections;
using System.Collections.Generic;
using SockFauna.Mono;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using Nautilus.Assets;
using Nautilus.Extensions;
using Nautilus.Utility;
using UnityEngine;
using static SentrySdkManager;

namespace SockFauna.Creatures;

public class MultiGarg : CreatureAsset
{

    private readonly bool PlaceableInACU;
    public MultiGarg(PrefabInfo prefabInfo, bool ACU) : base(prefabInfo)
    {
        PlaceableInACU = ACU;
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("MultigargPrefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000f);
        template.CellLevel = LargeWorldEntity.CellLevel.Global;
        template.SwimRandomData = new SwimRandomData(0.1f, 20f, new Vector3(200, 0, 200), 2.5f, 2.0f, false);
        template.StayAtLeashData = new StayAtLeashData(0.2f, 20, 300);
        template.LocomotionData = new LocomotionData(10f, 0.01f, 1.5f, 0.5f, false, false, false);
        template.Mass = 20000;
        template.BehaviourLODData = new BehaviourLODData(50, 300, 5000);
        template.AnimateByVelocityData = new AnimateByVelocityData(12, 30, 45, true);
        template.CanBeInfected = false;
        template.AcidImmune = true;
        template.RespawnData = new RespawnData(false);
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.005f, 0.005f, 0.9f, 1f, true, false, PrefabInfo.ClassID));
        if (PlaceableInACU)
        {
            template.PickupableFishData = new PickupableFishData(TechType.Peeper, "Multigarg", "Multigarg");
        }
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        if (!PlaceableInACU)
            prefab.AddComponent<BossCollisions>();

        if (!PlaceableInACU)
        {
            var trailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Spine2"));
            trailManagerBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
            trailManagerBuilder.AllowDisableOnScreen = false;
            trailManagerBuilder.Apply();
        }
        else
        {
            var trailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Spine2"), 10f, 1f);
            trailManagerBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
            trailManagerBuilder.AllowDisableOnScreen = false;
            trailManagerBuilder.Apply();
        }

        if (!PlaceableInACU)
        {
            var voice = prefab.AddComponent<CreatureVoice>();
            voice.closeIdleSound = AudioUtils.GetFmodAsset("MultiGargRoar");
            voice.minInterval = 25;
            voice.maxInterval = 60;
            voice.animator = components.Animator;
            voice.animatorTriggerParam = "roar";
            var gargEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            gargEmitter.followParent = true;
            voice.emitter = gargEmitter;

            var MouthVoiceEmitter = prefab.AddComponent<FMOD_CustomLoopingEmitter>();
            MouthVoiceEmitter.SetAsset(AudioUtils.GetFmodAsset("MultiGargAmbience"));
            MouthVoiceEmitter.playOnAwake = true;
            MouthVoiceEmitter.followParent = true;
        }
        yield break;
    }
    protected override void PostRegister()
    {
        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(this, "Lifeforms/Fauna/Leviathans", "Pentamosa", "A gargantuan, surprisingly shallow dwelling leviathan, being one of the largest ever recorded on 4546B.\n\n1. Appearance:\nThe Pentamosa is a massive creature, spanning hundreds of meters in length. It possesses five separate heads, which is incredibly unusual for fauna on 4546B. It is unclear at this time if each head operates individually. Further study will be required.\n\n2. Behavior:\nDespite how it looks, the Pentamosa is entirely passive, suggesting it doesn't actively hunt much, if at all. The large body could be storing as many nutrients as it'd need to sustain it for months, possibly even years.\n\n3. Rarity:\nOnly one specimen has been found so far, likely meaning this species is nearing extinction. What exactly could have caused this is unknown, though a likely reason is its food supply decreasing as a result of the Kharaa outbreak.\n\nAssessment: Miraculously, harmless", 12f, null, null);
    }
}