using Godot;
using System;

public class Window : ColorRect
{
    private bool mousePress = false;
    private bool mouseInTitleBar = false; public void mouseEnterInTitlebar(bool entered){ mouseInTitleBar = entered; }

    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent @event){

        if( @event is InputEventMouseButton ){

            mousePress = (@event as InputEventMouseButton).Pressed;

        }

        else if ( @event is InputEventMouseMotion && mousePress ){

            InputEventMouseMotion e = (@event as InputEventMouseMotion);
            Vector2 screenSize = GetViewportRect().Size;

            RectPosition = new Vector2(
                Mathf.Clamp( RectPosition.x + e.Relative.x, 0, screenSize.x),   
                Mathf.Clamp( RectPosition.y + e.Relative.y, 0, screenSize.y)   

            );

        }

    }
    
}
