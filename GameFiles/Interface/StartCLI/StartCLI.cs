using Godot;
using System;

public class StartCLI : Control
{
    public static class SETTINGS{
        public const int IDX_CLOCKSPEED = 0, 
                  IDX_PROCCESS_SPEED = 1,
                  IDX_MAX_ROBOTS = 2;
        
        private static void setClockSpeed(int choice){
            
            Mathf.Clamp(choice, 1, 10);

            Robot.CLOCKSPEED = (byte)choice;
        }

        //private static void setProcessSpeed

    }

    private AnimationPlayer animationPlayer;
    private bool IS_BOOTLOGO { get => animationPlayer.IsPlaying();}
    private Control[] menuContent;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        menuContent = new Control[3]{
            GetNode<Control>("Menu/Content/Game"),
            GetNode<Control>("Menu/Content/Settings"),
            GetNode<Control>("Menu/Content/About")
        };

        //Engine.TargetFps = 30;
        //OS.VsyncEnabled = false;
        //OS.WindowFullscreen = true;
    }

    private int currentHeaderIdx = 0; public void headerPressed(int idx){
        
        if(idx<0) idx = menuContent.Length - 1;
        else if(idx>menuContent.Length-1) idx = 0;

        currentHeaderIdx = idx;

        for(int i=0; i<menuContent.Length; i++)
            menuContent[i].Visible = (i==idx);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if(IS_BOOTLOGO) return;

        if((@event is InputEventKey) && (@event as InputEventKey).Pressed){
            
            // Play Game
            if(Input.IsActionJustPressed("Enter")){
                GetNode<Label>("Menu/Play").Text = "Booting Game...";
                GetNode<Control>("Menu/Content").Visible = false;
                GetNode<Control>("Menu/Headers").Visible = false;
                async void delayThenStart(){
                    await ToSignal(GetTree().CreateTimer(1f), "timeout");
                    GetTree().ChangeScene("res://GameFiles/Interface/IDE/IDE.tscn");
                }
                delayThenStart();
                return;
            }

            Vector2 arrows = new Vector2( 
                (Input.GetActionStrength("ArrowRight") - Input.GetActionStrength("ArrowLeft")),
                (Input.GetActionStrength("ArrowDown") - Input.GetActionStrength("ArrowUp"))                     
            );

            if(arrows.x != 0){
                headerPressed(currentHeaderIdx + (int)arrows.x);
            }
            else if(arrows.y != 0){
                
            }
        
        }
    }
}
