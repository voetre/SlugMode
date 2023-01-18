using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using SlugMode.Windows;
using System;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace SlugMode
{
    public unsafe class Plugin : IDalamudPlugin
    {
        public string Name => "Slug Mode";
        private const string CommandName = "/slugmode";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("SlugMode");
        public static IntPtr PlayerAddress => DalamudApi.ClientState.LocalPlayer.Address;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);


            var imagePath2 = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage2 = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this, goatImage, goatImage2));

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
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
            DalamudApi.ChatGui.PrintError($"{SlugsMadeCount}");
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            //WindowSystem.GetWindow("My Amazing Window").IsOpen = true;
            SlugMode(Int32.Parse(args));
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow("A Wonderful Configuration Window").IsOpen = true;
        }
    }
}
