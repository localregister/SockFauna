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

public class MultiGarg : CreatureAsset
{
    public MultiGarg(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("MultigargPrefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000f);
        template.CellLevel = LargeWorldEntity.CellLevel.Global;
        template.SwimRandomData = new SwimRandomData(0.1f, 20f, new Vector3(200, 0, 200), 2.5f, 2.0f, false);
        template.StayAtLeashData = new StayAtLeashData(0.2f, 20, 300);
        template.LocomotionData = new LocomotionData(10f, 0.01f, 1.5f, 0.5f, false, false, false);
        //template.AvoidTerrainData = new AvoidTerrainData(0.2f, 20, 30, 30, 0.5f, 10f);
        //template.AttackLastTargetData = new AttackLastTargetData(0.3f, 50f, 0.5f, 10f, 12f);
        //template.AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(25f, 0.5f);
        template.Mass = 20000;
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
        voice.closeIdleSound = AudioUtils.GetFmodAsset("MultiGargRoar");
        voice.minInterval = 25;
        voice.maxInterval = 60;
        voice.animator = components.Animator;
        voice.animatorTriggerParam = "roar";
        var gargEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
        gargEmitter.followParent = true;
        voice.emitter = gargEmitter;
        

        var trailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Spine2"));
        trailManagerBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
        trailManagerBuilder.AllowDisableOnScreen = false;
        trailManagerBuilder.Apply();
        //var necktrailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Spine1"));
        //necktrailManagerBuilder.SetTrailArrayToChildrenWithKeywords("Neck");
        //necktrailManagerBuilder.AllowDisableOnScreen = false;
        //necktrailManagerBuilder.Apply();

        /*
        var behaviour = prefab.AddComponent<BlazaBehaviour>();
        behaviour.creature = components.Creature;
        var grabVehicleEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
        grabVehicleEmitter.followParent = true;
        behaviour.grabVehicleEmitter = grabVehicleEmitter;
        */
        
        var MouthVoiceEmitter = prefab.AddComponent<FMOD_CustomLoopingEmitter>();
        MouthVoiceEmitter.SetAsset(AudioUtils.GetFmodAsset("MultiGargAmbience"));
        MouthVoiceEmitter.playOnAwake = true;
        MouthVoiceEmitter.followParent = true;
        /*
        var mouth = prefab.SearchChild("MouthTrigger");
        var meleeAttack = prefab.AddComponent<GargMeleeAttack>();
        meleeAttack.mouth = mouth;
        meleeAttack.canBeFed = false;
        meleeAttack.biteInterval = 2f;
        meleeAttack.biteDamage = 100f;
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
        meleeEmitter.SetAsset(AudioUtils.GetFmodAsset("MultiGargBite"));
        meleeAttack.attackEmitter = meleeEmitter;

        mouth.AddComponent<OnTouch>();
        */
        yield break;
    }
}