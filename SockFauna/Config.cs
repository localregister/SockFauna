using System.Collections.Generic;
using Nautilus.Options;
using SockFauna;

namespace SockFauna;

public class SockOptions : ModOptions
{
    public SockOptions() : base("Sockfauna")
    {
        AddItem(Plugin.registerAbyssalBlazaSpawns.ToModToggleOption());
        AddItem(Plugin.registerAncientBloopSpawns.ToModToggleOption());
        AddItem(Plugin.registerMirageSpawns.ToModToggleOption());
        AddItem(Plugin.registerOculusSpawns.ToModToggleOption());
        AddItem(Plugin.registerPentaSpawns.ToModToggleOption());
        AddItem(Plugin.registerMouthSpawns.ToModToggleOption());
    }

    public override void BuildModOptions(uGUI_TabbedControlsPanel panel, int modsTabIndex, IReadOnlyCollection<OptionItem> options)
    {
        panel.AddHeading(modsTabIndex, Name + "\n" +
                                       "<size=60%><color=#FFFFFF>Warning: Enable the Void Creature and Pentamosa with caution." +
                                       " \nIf enabled, they will always be loaded into the world. This is permanent for the save!</color></size>");

        options.ForEach(option => option.AddToPanel(panel, modsTabIndex));
        options.ForEach(option => ((ModToggleOption)option).OnChanged += NotifyChangeRecommended);
    }

    private void NotifyChangeRecommended(object value, ToggleChangedEventArgs args)
    {
        ErrorMessage.AddMessage("<color=#FFFFFF>You must restart the game in order for changes to apply.</color>");
    }
}
