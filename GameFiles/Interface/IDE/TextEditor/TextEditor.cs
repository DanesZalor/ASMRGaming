using Godot;
using System;

public class TextEditor : ColorRect
{

    public override void _Ready()
    {
        
    }

    private bool mouseIn = false, mousePress = false; 
    public void _on_TitleBar_mouse_entered(){
        mouseIn = true;
    }

    public void _on_TitleBar_mouse_exited(){
        mouseIn = false;
        mousePress = false;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if(mouseIn && @event is InputEventMouseButton)
            mousePress = (@event as InputEventMouseButton).Pressed;
        else if(@event is InputEventMouseMotion && mousePress){
            RectPosition += (@event as InputEventMouseMotion).Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, 860),
                Mathf.Clamp(RectPosition.y, 0, 300)
            );
        }
    }
}
