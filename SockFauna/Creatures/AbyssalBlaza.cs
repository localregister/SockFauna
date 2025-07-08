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

namespace SockFauna.Creatures;

public class AbyssalBlaza : CreatureAsset
{
    private readonly bool PlaceableInACU;
    public AbyssalBlaza(PrefabInfo prefabInfo, bool ACU) : base(prefabInfo)
    {
        PlaceableInACU = ACU;
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("BlazaV2_Prefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000f);
        template.CellLevel = LargeWorldEntity.CellLevel.VeryFar;
        template.SwimRandomData = new SwimRandomData(0.1f, 15f, new Vector3(175, 20, 175), 2.5f, 0.8f, true);
        template.StayAtLeashData = new StayAtLeashData(0.2f, 15, 175);
        template.AvoidTerrainData = new AvoidTerrainData(0.2f, 15, 30, 30, 0.5f, 10f);
        template.AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>()
        {
            new(EcoTargetType.Shark, 1.2f, 35f, 2)
        };
        template.AttackLastTargetData = new AttackLastTargetData(0.3f, 20f, 0.5f, 10f, 12f);
        template.AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(20f, 0.5f);
        template.Mass = 3000;
        template.BehaviourLODData = new BehaviourLODData(50, 300, 5000);
        template.AnimateByVelocityData = new AnimateByVelocityData(12);
        template.CanBeInfected = false;
        template.AcidImmune = true;
        template.RespawnData = new RespawnData(false);
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.025f, 0.025f, 0.9f, 1f, true, false, PrefabInfo.ClassID));
        if (PlaceableInACU)
        {
            template.PickupableFishData = new PickupableFishData(TechType.Peeper, "Blaza", "Blaza");
        }
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        var trailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Spine_1"));
        trailManagerBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
        trailManagerBuilder.AllowDisableOnScreen = false;
        trailManagerBuilder.Apply();

        if (!PlaceableInACU)
        {
            var voice = prefab.AddComponent<CreatureVoice>();
            voice.closeIdleSound = AudioUtils.GetFmodAsset("AbyssalBlazaIdle");
            voice.minInterval = 15;
            voice.maxInterval = 25;
            voice.animator = components.Animator;
            voice.animatorTriggerParam = "roar";
            var blazaEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            blazaEmitter.followParent = true;
            voice.emitter = blazaEmitter;

            var behaviour = prefab.AddComponent<BlazaBehaviour>();
            behaviour.creature = components.Creature;
            var grabVehicleEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            grabVehicleEmitter.followParent = true;
            behaviour.grabVehicleEmitter = grabVehicleEmitter;

            var mouth = prefab.SearchChild("MouthTrigger");
            var meleeAttack = prefab.AddComponent<BlazaMeleeAttack>();
            meleeAttack.mouth = mouth;
            meleeAttack.canBeFed = false;
            meleeAttack.biteInterval = 2f;
            meleeAttack.biteDamage = 75f;
            meleeAttack.eatHungerDecrement = 0.05f;
            meleeAttack.eatHappyIncrement = 0.1f;
            meleeAttack.biteAggressionDecrement = 0.02f;
            meleeAttack.biteAggressionThreshold = 0.1f;
            meleeAttack.lastTarget = components.LastTarget;
            meleeAttack.creature = components.Creature;
            meleeAttack.liveMixin = components.LiveMixin;
            meleeAttack.animator = components.Animator;
            var meleeEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            meleeEmitter.followParent = true;
            meleeEmitter.SetAsset(AudioUtils.GetFmodAsset("AbyssalBlazaBite"));
            meleeAttack.attackEmitter = meleeEmitter;

            mouth.AddComponent<OnTouch>();
        }

        yield break;
    }
    protected override void PostRegister()
    {
        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(this, "Lifeforms/Fauna/Leviathans", "Abyssal Blaza", "An impressively long leviathan, and the elder form of the Blaza Leviathan typically found in cave systems.\n\n1. Appearance:\nGiven the fact that the Abyssal Blaza is found in more open and shallow waters, it's likely that the larger body is less sustainable in the tight caves that the juveniles tend to be found in. It's unknown what the bioluminescent body does for the Blaza, but it's possible that the color patterns lure in fauna.\n\n2. Behavior:\nWhen it finds prey, it'll tend to lunge directly at it and hope for the best. Its jaws are designed for tearing through flesh and crushing bones, so the bite is devastating.\n\nAssessment: Extreme threat - don't get too close, and try not to get mesmerized by it", 6f, null, null);
    }
}