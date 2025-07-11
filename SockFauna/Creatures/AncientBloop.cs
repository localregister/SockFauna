﻿using System.Collections;
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

public class AncientBloop : CreatureAsset
{
    private readonly bool PlaceableInACU;
    public AncientBloop(PrefabInfo prefabInfo, bool ACU) : base(prefabInfo)
    {
        PlaceableInACU = ACU;
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("BloopV2_Prefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000);
        template.CellLevel = LargeWorldEntity.CellLevel.VeryFar;
        template.SwimRandomData = new SwimRandomData(0.1f, 6f, new Vector3(125, 10, 125), 5, 1.5f, true);
        template.AnimateByVelocityData = new AnimateByVelocityData(3);
        template.AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>()
        {
            new(EcoTargetType.Shark, 1.5f, 70, 2)
        };
        template.AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(17f, 0.4f);
        template.AttackLastTargetData = new AttackLastTargetData(0.3f, 13f, 0.5f, 13f, 40f);
        template.Mass = 1500;
        template.StayAtLeashData = new StayAtLeashData(0.2f, 10f, 125f);
        template.AvoidTerrainData = new AvoidTerrainData(0.2f, 10, 30, 30, 0.5f, 10f);
        template.BehaviourLODData = new BehaviourLODData(50, 300, 5000);
        template.LocomotionData = new LocomotionData(10f,
            (0.05f), 3f, 0.9f);
        template.CanBeInfected = false;
        template.RespawnData = new RespawnData(false);
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.02f, 0.02f, 0.9f, 1f, true, false, PrefabInfo.ClassID));
        if (PlaceableInACU)
        {
            template.PickupableFishData = new PickupableFishData(TechType.Peeper, "Bloop", "Bloop");
        }
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        if (!PlaceableInACU)
        {
            var trailBuilder = new TrailManagerBuilder(components, prefab.SearchChild("Spine1").transform, 4f);
            trailBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
            trailBuilder.AllowDisableOnScreen = false;
            trailBuilder.Apply();
        }
        else
        {
            var trailBuilder = new TrailManagerBuilder(components, prefab.SearchChild("Spine1").transform, 8f, 1f);
            trailBuilder.SetTrailArrayToChildrenWithKeywords("Spine");
            trailBuilder.AllowDisableOnScreen = false;
            trailBuilder.Apply();
        }

        if (PlaceableInACU)
        {
            foreach (var light in prefab.GetComponentsInChildren<Light>())
            {
                light.enabled = false;
            }
        }

        if (!PlaceableInACU)
        {
            var seamothTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);
            yield return seamothTask;
            var seamoth = seamothTask.GetResult().GetComponent<SeaMoth>();
            var vortexVfxPrefab = seamoth.torpedoTypes[0].prefab.GetComponent<SeamothTorpedo>().explosionPrefab
                .GetComponent<AddressablesPrefabSpawn>().prefab;
            var handle = vortexVfxPrefab.InstantiateAsync();
            var request = AddressablesPrefabSpawnRequest.Get(handle);
            var complete = false;
            GameObject vortexVfx = null;
            request.RegisterCallback(spawn =>
            {
                vortexVfx = Object.Instantiate(spawn.GetResultAndRelease());
                vortexVfx.SetActive(false);
                Object.DontDestroyOnLoad(vortexVfx);
                vortexVfx.AddComponent<SceneCleanerPreserve>();
                complete = true;
            });
            yield return new WaitUntil(() => complete);
            Object.DestroyImmediate(vortexVfx.GetComponent<VFXDestroyAfterSeconds>());

            prefab.AddComponent<Mono.SwimAmbience>();

            var vortexAttack = prefab.AddComponent<BloopVortexAttack>();
            var bloopVortexEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            bloopVortexEmitter.followParent = true;
            bloopVortexEmitter.SetAsset(AudioUtils.GetFmodAsset("AncientBloopVortexAttack"));
            vortexAttack.vortexAttackEmitter = bloopVortexEmitter;
            vortexAttack.vortexVfx = vortexVfx;

            var mouth = prefab.SearchChild("MouthTrigger");
            var meleeAttack = prefab.AddComponent<BloopMeleeAttack>();
            meleeAttack.mouth = mouth;
            meleeAttack.canBeFed = false;
            meleeAttack.biteInterval = 3f;
            meleeAttack.biteDamage = 75f;
            meleeAttack.eatHungerDecrement = 0.05f;
            meleeAttack.eatHappyIncrement = 0.1f;
            meleeAttack.biteAggressionDecrement = 0.02f;
            meleeAttack.biteAggressionThreshold = 0.1f;
            meleeAttack.lastTarget = components.LastTarget;
            meleeAttack.creature = components.Creature;
            meleeAttack.liveMixin = components.LiveMixin;
            meleeAttack.animator = components.Animator;
            var meleeAttackEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            meleeAttackEmitter.followParent = true;
            meleeAttackEmitter.SetAsset(AudioUtils.GetFmodAsset("AbyssalBlazaBite"));
            meleeAttack.emitter = meleeAttackEmitter;

            mouth.AddComponent<OnTouch>();

            var attackCyclops = prefab.AddComponent<AttackCyclops>();
            attackCyclops.swimVelocity = 15f;
            attackCyclops.aggressiveToNoise = new CreatureTrait(0f, 0.01f);
            attackCyclops.evaluatePriority = 0.6f;
            attackCyclops.maxDistToLeash = 70f;
            attackCyclops.attackAggressionThreshold = 0.4f;
            var bloopVoiceEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            bloopVoiceEmitter.followParent = true;
            var bloopVoice = prefab.AddComponent<CreatureVoice>();
            bloopVoice.emitter = bloopVoiceEmitter;
            bloopVoice.closeIdleSound = AudioUtils.GetFmodAsset("AncientBloopIdle");
            bloopVoice.minInterval = 25f;
            bloopVoice.maxInterval = 40f;
        }
    }
    protected override void PostRegister()
    {
        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(this, "Lifeforms/Fauna/Leviathans", "Ancient Bloop", "A vast, scaly leviathan, with incredible mass. Elder form of the Bloop from shallower waters.\n\n1. Appearance:\nInstead of a round body, the Ancient Bloop has a more reptilian form, like prehistoric earth dinosaurs. This appears to grant it more maneuverability, despite being larger.\n\n2. Behavior:\nLike the juveniles, the Ancient Bloop utilizes a vortex attack to suck in prey. The force of this attack is strong enough to quickly devour even a Seamoth, so it's not advised to remain close.\n\nAssessment: Threat - avoid the path of its vortex", 6f, null, null);
    }
}
