using System.Collections.Generic;
using System.Linq;
using FMOD;
using Nautilus.FMod;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SockFauna;

public static class ModAudio
{
    private static AssetBundle Bundle => Plugin.AssetBundle;

    private const string CreaturesBus = "bus:/master/SFX_for_pause/PDA_pause/all/SFX/creatures";

    public static void RegisterAllAudio()
    {
        // Blaza:
        RegisterSoundWithVariants("AbyssalBlazaIdle", new string[] { "AbyssalBlazaIdle1", "AbyssalBlazaIdle2", "AbyssalBlazaIdle3", "AbyssalBlazaIdle4", "AbyssalBlazaIdle5" }, CreaturesBus, 5f, 150f);
        RegisterSoundWithVariants("AbyssalBlazaBite", new string[] { "AbyssalBlazaBite1", "AbyssalBlazaBite2" }, CreaturesBus, 5f, 60f);
        RegisterSound("AbyssalBlazaExosuitAttack", "AbyssalBlazaExosuit", CreaturesBus, 5f, 30f);
        RegisterSound("AbyssalBlazaSeamothAttack", "AbyssalBlazaSeamoth", CreaturesBus, 5f, 30f);

        // Bloop:
        RegisterSoundWithVariants("AncientBloopIdle", new string[] { "AncientBloopRoar1", "AncientBloopRoar2", "AncientBloopRoar3", "AncientBloopRoar4", "AncientBloopRoar5" }, CreaturesBus, 20f, 150f);
        RegisterSound("AncientBloopVortexAttack", "AncientBloopVortexAttack", CreaturesBus, 5f, 40f);
        RegisterSoundWithVariants("AncientBloopSwim", new string[] { "AncientBloopSwim1", "AncientBloopSwim2", "AncientBloopSwim3", "AncientBloopSwim4", "AncientBloopSwim5" }, CreaturesBus, 5f, 60f);

        RegisterSound("AnglerRoar", "AnglerJumpscare", CreaturesBus, 5f, 150f);

        // Deep bloop:
        var sound = AudioUtils.CreateSound(Bundle.LoadAsset<AudioClip>("MassiveLeviathanIdle"),
            AudioUtils.StandardSoundModes_3D | MODE.LOOP_NORMAL);
        sound.set3DMinMaxDistance(5, 300);

        CustomSoundHandler.RegisterCustomSound("MouthAmbience", sound, CreaturesBus);
    }

    private static void RegisterSound(string id, string clipName, string bus, float minDistance = 10f,
        float maxDistance = 200f)
    {
        var sound = AudioUtils.CreateSound(Bundle.LoadAsset<AudioClip>(clipName), AudioUtils.StandardSoundModes_3D);
        sound.set3DMinMaxDistance(minDistance, maxDistance);

        CustomSoundHandler.RegisterCustomSound(id, sound, bus);
    }

    private static void RegisterSoundWithVariants(string id, string[] clipNames, string bus, float minDistance = 10f, float maxDistance = 200f)
    {
        var clipList = new List<AudioClip>();
        clipNames.ForEach(clipName => clipList.Add(Bundle.LoadAsset<AudioClip>(clipName)));

        var sounds = AudioUtils.CreateSounds(clipList, AudioUtils.StandardSoundModes_3D);
        sounds.ForEach(sound => sound.set3DMinMaxDistance(minDistance, maxDistance));

        var multiSoundsEvent = new FModMultiSounds(sounds.ToArray(), bus, true);

        CustomSoundHandler.RegisterCustomSound(id, multiSoundsEvent);
    }
}
