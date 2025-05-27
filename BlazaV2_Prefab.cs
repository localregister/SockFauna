using System;
using System.Collections;
using System.Collections.Generic;
using BloopAndBlaza.Mono;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using Nautilus.Assets;
using Nautilus.Extensions;
using Nautilus.Utility;
using UnityEngine;

namespace SockFauna.Creatures
{
    // Token: 0x0200002B RID: 43
    public class BlazaV2_Prefab : CreatureAsset
    {
        // Token: 0x0600008D RID: 141 RVA: 0x0000437F File Offset: 0x0000257F
        public AbyssalBlaza(PrefabInfo prefabInfo) : base(prefabInfo)
        {
        }

        // Token: 0x0600008E RID: 142 RVA: 0x0000438C File Offset: 0x0000258C
        protected override CreatureTemplate CreateTemplate()
        {
            return new CreatureTemplate(Plugin.AssetBundle.LoadAsset<GameObject>("BlazaV2_Prefab"), 6, 2045, 5000f)
            {
                CellLevel = 3,
                SwimRandomData = new SwimRandomData(0.1f, 30f, new Vector3(45f, 3f, 45f), 2f, 0.5f, false),
                StayAtLeashData = new StayAtLeashData(0.2f, 30f, 60f, 1f, 3f),
                AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData>
                {
                    new AggressiveWhenSeeTargetData(2030, 1.2f, 35f, 2, true, false, 0f, 0f)
                },
                AttackLastTargetData = new AttackLastTargetData(0.3f, 45f, 0.5f, 10f, 12f, 3f, 5f, true),
                AggressiveToPilotingVehicleData = new AggressiveToPilotingVehicleData(25f, 0.5f, 1f),
                Mass = 2300f,
                BehaviourLODData = new BehaviourLODData(50f, 300f, 5000f),
                AnimateByVelocityData = new AnimateByVelocityData(12f, 30f, 45f, false, 0.5f),
                CanBeInfected = false,
                AcidImmune = true,
                RespawnData = new RespawnData(false, false, 300f)
            };
        }

        // Token: 0x0600008F RID: 143 RVA: 0x0000450B File Offset: 0x0000270B
        protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
        {
            CreatureVoice voice = prefab.AddComponent<CreatureVoice>();
            voice.closeIdleSound = AudioUtils.GetFmodAsset("AbyssalBlazaIdle", null);
            voice.minInterval = 15f;
            voice.maxInterval = 25f;
            voice.animator = components.Animator;
            voice.animatorTriggerParam = "roar";
            FMOD_CustomEmitter blazaEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            blazaEmitter.followParent = true;
            voice.emitter = blazaEmitter;
            TrailManagerBuilder trailManagerBuilder = new TrailManagerBuilder(components, GameObjectExtensions.SearchChild(prefab.transform, "Spine.016"), 5f, -1f);
            trailManagerBuilder.SetTrailArrayToAllChildren();
            trailManagerBuilder.AllowDisableOnScreen = false;
            trailManagerBuilder.Apply();
            BlazaBehaviour behaviour = prefab.AddComponent<BlazaBehaviour>();
            behaviour.creature = components.Creature;
            FMOD_CustomEmitter grabVehicleEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            grabVehicleEmitter.followParent = true;
            behaviour.grabVehicleEmitter = grabVehicleEmitter;
            GameObject mouth = GameObjectExtensions.SearchChild(prefab, "Mouth");
            BlazaMeleeAttack meleeAttack = prefab.AddComponent<BlazaMeleeAttack>();
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
            FMOD_CustomEmitter meleeEmitter = prefab.AddComponent<FMOD_CustomEmitter>();
            meleeEmitter.followParent = true;
            meleeEmitter.SetAsset(AudioUtils.GetFmodAsset("BlazaBite", null));
            meleeAttack.attackEmitter = meleeEmitter;
            mouth.AddComponent<OnTouch>();
            yield break;
        }

        // Token: 0x06000090 RID: 144 RVA: 0x00004528 File Offset: 0x00002728
        protected override void PostRegister()
        {
            CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(this, "Lifeforms/Fauna/Leviathans", null, null, 6f/*, Plugin.AssetBundle.LoadAsset<Texture2D>("Blaza_Ency") Plugin.AssetBundle.LoadAsset<Sprite>("Blaza_Popup")*/);
        }
    }
}
