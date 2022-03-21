using Godot;
using System;

public class StartCLI : Control
{
    private Control[] menuContent;
    public override void _Ready()
    {
        menuContent = new Control[3]{
            GetNode<Control>("Menu/Content/Game"),
            GetNode<Control>("Menu/Content/Settings"),
            GetNode<Control>("Menu/Content/About")
        };

        //Engine.TargetFps = 30; 
        //OS.VsyncEnabled = false; 
        //OS.WindowFullscreen = true; 

    }

    public void headerPressed(int idx){
        GD.Print("header press");
        GD.Print(idx);
        for(int i=0; i<menuContent.Length; i++)
            menuContent[i].Visible = (i==idx);
        
    }
}
