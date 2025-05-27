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

public class Anglerfish : CreatureAsset
{
    public Anglerfish(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("Anglerfish_Prefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000f);
        template.CellLevel = LargeWorldEntity.CellLevel.Global;
        template.SwimRandomData = new SwimRandomData(0.1f, 5f, new Vector3(200, 3, 200), 25f, 2.5f, true);
        template.StayAtLeashData = new StayAtLeashData(0.2f, 5, 200);
        /*
        template.AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>()
        {
            new(EcoTargetType.Shark, 1.2f, 35f, 2)
        };
        */
        template.AttackLastTargetData = new AttackLastTargetData(0.3f, 50f, 0.5f, 10f, 12f);
        template.AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(40f, 0.5f);
        template.Mass = 5000;
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
        voice.closeIdleSound = AudioUtils.GetFmodAsset("AnglerRoar");
        voice.minInterval = 30;
        voice.maxInterval = 60;
        voice.animator = components.Animator;
        voice.animatorTriggerParam = "roar";
        var anglerEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
        anglerEmitter.followParent = true;
        voice.emitter = anglerEmitter;

        var behaviour = prefab.AddComponent<AnglerBehaviour>();
        behaviour.creature = components.Creature;

        var mouth = prefab.SearchChild("MouthTrigger");
        var meleeAttack = prefab.AddComponent<AnglerMeleeAttack>();
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
        meleeEmitter.SetAsset(AudioUtils.GetFmodAsset("BlazaBite"));
        meleeAttack.attackEmitter = meleeEmitter;

        mouth.AddComponent<OnTouch>();

        yield break;
    }
    
}

