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
    public AbyssalBlaza(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("BlazaV2_Prefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000f);
        template.CellLevel = LargeWorldEntity.CellLevel.VeryFar;
        template.SwimRandomData = new SwimRandomData(0.1f, 20f, new Vector3(175, 20, 175), 2.5f, 0.8f, true);
        template.StayAtLeashData = new StayAtLeashData(0.2f, 20, 175);
        template.AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>()
        {
            new(EcoTargetType.Shark, 1.2f, 35f, 2)
        };
        template.AttackLastTargetData = new AttackLastTargetData(0.3f, 45f, 0.5f, 10f, 12f);
        template.AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(25f, 0.5f);
        template.Mass = 3000;
        template.BehaviourLODData = new BehaviourLODData(50, 300, 5000);
        template.AnimateByVelocityData = new AnimateByVelocityData(12);
        template.CanBeInfected = false;
        template.AcidImmune = true;
        template.RespawnData = new RespawnData(false);
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
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

        var trailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Spine_1"));
        trailManagerBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
        trailManagerBuilder.AllowDisableOnScreen = false;
        trailManagerBuilder.Apply();

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

        yield break;
    }
}