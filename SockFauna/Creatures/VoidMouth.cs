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
        template.Mass = 500000;
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

        yield break;
    }

}


