using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace SlugMode
{
    public unsafe class Plugin : IDalamudPlugin
    {
        public string Name => "Slug Mode";
        private const string SlugModeCommand = "/slugmode";
        private const string UnSlugCommand = "/unslug";

        private CommandManager CommandManager { get; init; }
        public static IntPtr PlayerAddress => DalamudApi.ClientState.LocalPlayer.Address;

        public static void PrintEcho(string message) => DalamudApi.ChatGui.Print($"[SlugMode] {message}");
        public static void PrintError(string message) => DalamudApi.ChatGui.PrintError($"[SlugMode] {message}");

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            DalamudApi.Initialize(this, pluginInterface);

            this.CommandManager = commandManager;

            this.CommandManager.AddHandler(SlugModeCommand, new CommandInfo(SlugCommand)
            {
                HelpMessage = "Activate the SlugMode engines, provide a model ID if you aren't fond of slugs..."
            });

            this.CommandManager.AddHandler(UnSlugCommand, new CommandInfo(CureCommand)
            {
                HelpMessage = "Cures all transformed players."
            });

        }
        public static void SlugMode(int ModelID)
        {
            byte[] Bytes_ModelID = BitConverter.GetBytes(ModelID);
            var SlugsMadeCount = 0;
            foreach (var o in DalamudApi.ObjectTable)
            {
                if (o is not PlayerCharacter || o == DalamudApi.ClientState.LocalPlayer) continue;
                var obj = (GameObject*)o.Address;
                var p = (Character*)o.Address;
                obj->RenderFlags = 2;
                p->ModelCharaId = ModelID;
                DalamudApi.Framework.RunOnTick(() => obj->RenderFlags = 0, default, 2);
                SlugsMadeCount++;
            }
            //PrintEcho($"Transformed {SlugsMadeCount} players!");
        }

        public void Dispose()
        {
            this.CommandManager.RemoveHandler(SlugModeCommand);
            this.CommandManager.RemoveHandler(UnSlugCommand);
        }

        private void SlugCommand(string command, string args)
        {
            try
            {
                    if (args == "")
                    {
                        SlugMode(50);
                    }
                    else
                    {
                        SlugMode(Int32.Parse(args));
                    }
            }
            catch
            {
                PrintError($"the argument '{args}' was invalid.");
                PrintError("You must specify a model number e.g. '/slugmode 50'.");
            }
        }

        private void CureCommand(string command, string args)
        {
            SlugMode(0);
        }

    }
}
