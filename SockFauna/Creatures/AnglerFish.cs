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
    private readonly bool PlaceableInACU;
    public Anglerfish(PrefabInfo prefabInfo, bool ACU) : base(prefabInfo)
    {
        PlaceableInACU = ACU;
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(() => Plugin.AssetBundle.LoadAsset<GameObject>("Anglerfish_Prefab"),
            BehaviourType.Leviathan, EcoTargetType.Leviathan, 5000f);
        template.CellLevel = LargeWorldEntity.CellLevel.VeryFar;
        template.SwimRandomData = new SwimRandomData(0.1f, 5f, new Vector3(200, 3, 200), 25f, 2.5f, true);
        template.StayAtLeashData = new StayAtLeashData(0.2f, 5, 200);
        template.AttackLastTargetData = new AttackLastTargetData(0.3f, 50f, 0.5f, 10f, 12f);
        template.Mass = 5000;
        template.BehaviourLODData = new BehaviourLODData(50, 300, 5000);
        template.LocomotionData = new LocomotionData(10f, 0.2f, 1.5f, 0.5f, false, false, false);
        template.AnimateByVelocityData = new AnimateByVelocityData(12, 30 , 45, true);
        template.CanBeInfected = false;
        template.AcidImmune = true;
        template.RespawnData = new RespawnData(false);
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.02f, 0.02f, 0.9f, 1f, true, false, PrefabInfo.ClassID));
        if (PlaceableInACU)
        {
            template.PickupableFishData = new PickupableFishData(TechType.Peeper, "Angular", "Angular");
        }
        return template;
    }
    
    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        var behaviour = prefab.AddComponent<AnglerBehaviour>();
        behaviour.creature = components.Creature;

        if (PlaceableInACU)
        {
            foreach (var light in prefab.GetComponentsInChildren<Light>())
            {
                light.enabled = false;
            }
        }

        if (!PlaceableInACU)
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
            meleeEmitter.SetAsset(AudioUtils.GetFmodAsset("MultiGargBite"));
            meleeAttack.attackEmitter = meleeEmitter;

            mouth.AddComponent<OnTouch>();
        }

        yield break;
    }
    protected override void PostRegister()
    {
        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(this, "Lifeforms/Fauna/Leviathans", "Mirage Fish", "The Mirage Fish is a huge leviathan, resembling the long extinct footballfish of Earth.\n\n1. Diet:\nThis leviathan, despite its size, seems to exclusively feed on whatever small fauna happen to wander into its mouth.\n\n2. Behavior:\nThe Mirage Fish lures prey in with a large bulb, before either swiftly chomping down or chasing evasive prey at an alarmingly high speed.\nWith that in mind, congrats on managing to live to see this entry!\n\nAssessment: Avoid at all costs - be on guard in the vicinity of one", 2f, null, null);
    }
}

