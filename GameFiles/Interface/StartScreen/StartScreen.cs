using Godot;
using System;

public class StartScreen : Control
{
    public static Vector2 UIScale = Vector2.One;
    private OptionButton resolutionButton;
    private Button playBtn;
    public override void _Ready()
    {
        resolutionButton = GetNode<OptionButton>("Buttons/resBtn");
        resolutionButton.GetPopup().AddItem("1280x720");
        resolutionButton.GetPopup().AddItem("1600x900");
        resolutionButton.GetPopup().AddItem("1920x1080");
        resolutionButton.GetPopup().Connect("id_pressed", this, "changeResolution");

        playBtn = GetNode<Button>("Buttons/play");
        playBtn.Connect("pressed", this, "playGame");
    }

    public void changeResolution(int id){
        GD.Print("resbtn choice:"+id);
        Vector2[] resolutions = new Vector2[3]{
            new Vector2(1280,720),
            new Vector2(1600,900),
            new Vector2(1920,1080),
        };
        float[] scales = {1f,1.25f, 1.5f}; 
        
        GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep, resolutions[id], 1f);
        RectScale = new Vector2(scales[id], scales[id]);
        UIScale = RectScale;
    }

    public void playGame(){
        GD.Print("spawn game");
        GetTree().ChangeScene("res://GameFiles/Interface/IDE/IDE.tscn");
    }
}
