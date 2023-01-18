using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace SlugMode.Windows;

public class MainWindow : Window, IDisposable
{
    private TextureWrap GoatImage;
    private TextureWrap GoatImage2;
    private Plugin Plugin;

    public MainWindow(Plugin plugin, TextureWrap goatImage, TextureWrap goatImage2) : base(
        "My Amazing Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {

        this.GoatImage = goatImage;
        this.GoatImage2 = goatImage2;
        this.Plugin = plugin;
    }

    public void Dispose()
    {
        this.GoatImage.Dispose();
    }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }

        ImGui.Image(this.GoatImage.ImGuiHandle, new Vector2(this.GoatImage.Width, this.GoatImage.Height));

        ImGui.SameLine();
        ImGui.Image(this.GoatImage2.ImGuiHandle, new Vector2(this.GoatImage2.Width, this.GoatImage2.Height));

        if (ImGui.Button("Slug Mode")){
            Plugin.SlugMode(50);
        }
    }
}
