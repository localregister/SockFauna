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
public class AbyssalMouth : CreatureAsset
{
    public AbyssalMouth(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("AbyssalMouth_Prefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 500000f);
        template.CellLevel = LargeWorldEntity.CellLevel.Global;
        template.SwimRandomData = new SwimRandomData(0.1f, 0f, new Vector3(0, 0, 0), 25f, 2.5f);
        //template.StayAtLeashData = new StayAtLeashData(0.2f, 5, 200);
        /*
        template.AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>()
        {
            new(EcoTargetType.Shark, 1.2f, 35f, 2)
        };
        */
        //template.AttackLastTargetData = new AttackLastTargetData(0.3f, 50f, 0.5f, 10f, 12f);
        //template.AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(40f, 0.5f);
        template.Mass = 500000;
        //template.BehaviourLODData = new BehaviourLODData(50, 300, 5000);
        template.AnimateByVelocityData = new AnimateByVelocityData(12);
        template.CanBeInfected = false;
        template.AcidImmune = true;
        template.RespawnData = new RespawnData(false);
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        var behaviour = prefab.AddComponent<MouthBehaviour>();
        behaviour.creature = components.Creature;

        var MouthVoiceEmitter = prefab.AddComponent<FMOD_CustomLoopingEmitter>();
        MouthVoiceEmitter.SetAsset(AudioUtils.GetFmodAsset("MouthAmbience"));
        MouthVoiceEmitter.playOnAwake = true;
        MouthVoiceEmitter.followParent = true;
        /*
        var mouth = prefab.SearchChild("MouthTrigger");
        var meleeAttack = prefab.AddComponent<MouthMeleeAttack>();
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
        */

        yield break;
    }

}


