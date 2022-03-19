using Godot;
using System;

public class IDEControls : Control
{
    private IDE ideparent;
    private TextureButton[] buttons; 
    public override void _Ready()
    {
        ideparent = GetParent<IDE>();
        
        // set position relative to window size
        RectPosition = new Vector2(GetViewportRect().Size.x - (256 * 0.9f), 0);
        
        buttons = new TextureButton[3]{
            GetNode<TextureButton>("PlayPause"),
            GetNode<TextureButton>("Stop"),
            GetNode<TextureButton>("ShowHide")
        };
    }

    public void refreshButtons(){
        buttons[1].Visible = ideparent.GetSTATE()!=IDE.STATE.SETUP;
        buttons[0].Pressed = ideparent.GetSTATE()==IDE.STATE.PLAYING;
    }
    /*Signal*/ public void onButtonIDEControlPress(int btnIdx){

        switch(btnIdx){
            case 0:
                ideparent.interpretCommand( buttons[0].Pressed?"play":"pause" ); 
            break;
            case 1:
                ideparent.interpretCommand( "reset" );
            break;
            case 2:
                ideparent.windowsHandler.Visible = buttons[2].Pressed; 
            break;
        }
    }
}
