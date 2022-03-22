using Godot;
using System;

public class Window : ColorRect
{
    private bool mousePress = false;
    private bool mouseInTitleBar = false; public void mouseEnterInTitlebar(bool entered){ mouseInTitleBar = entered; }
    private bool anchorPressed = false; public void anchorButtonAction(bool pressed){ anchorPressed = pressed; }
    private Control anchorBtn, titleBar, exitBtn;

    public override void _Ready()
    {
        anchorBtn = GetNode<Control>("Anchor");
        titleBar = GetNode<Control>("titlebar");
        exitBtn = GetNode<Control>("titlebar/ExitBtn");
    }

    public override void _Input(InputEvent @event){

        if( @event is InputEventMouseButton ){

            mousePress = (@event as InputEventMouseButton).Pressed;

        }

        else if ( @event is InputEventMouseMotion ){
            
            InputEventMouseMotion e = (@event as InputEventMouseMotion);

            if(mousePress && mouseInTitleBar){
                
                Vector2 POSLIMIT = GetViewportRect().Size - RectSize;

                RectPosition = new Vector2(
                    Mathf.Clamp( RectPosition.x + e.Relative.x, 0, POSLIMIT.x),   
                    Mathf.Clamp( RectPosition.y + e.Relative.y, 0, POSLIMIT.y)   
                );
                return; 
            }

            else if(anchorPressed){

                Vector2 SIZELIMIT = GetViewportRect().Size - RectPosition;
                
                RectSize = new Vector2(
                    Mathf.Clamp(RectSize.x + e.Relative.x, 300, SIZELIMIT.x ),
                    Mathf.Clamp(RectSize.y + e.Relative.y, 400, SIZELIMIT.y )
                );
                anchorBtn.RectPosition = RectSize - anchorBtn.RectSize;
                titleBar.RectSize = new Vector2(RectSize.x - 10, 30);
                exitBtn.RectPosition = new Vector2( titleBar.RectSize.x - 29, 1);
            }

        }

    }
    
}
