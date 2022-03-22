using Godot;
using System;

public class Window : ColorRect
{
    private bool mousePress = false;
    private bool mouseInTitleBar = false; public void mouseEnterInTitlebar(bool entered){ mouseInTitleBar = entered; }
    private bool anchorPressed = false; public void anchorButtonAction(bool pressed){ anchorPressed = pressed; }
    private Control anchorBtn, titleBar, exitBtn, contentHolder;
    private WindowsHandler windowHandlerParent;

    public override void _Ready()
    {
        windowHandlerParent = GetParent<WindowsHandler>();
        anchorBtn = GetNode<Control>("Anchor");
        titleBar = GetNode<Control>("titlebar");
        exitBtn = GetNode<Control>("titlebar/ExitBtn");
        contentHolder = GetNode<Control>("ContentHolder");

        RaiseWindow();
        setPosition( Vector2.One * (20f + windowHandlerParent.GetChildCount()*20f ) );
    }

    public bool isTopWindow(){
        return GetIndex()==0;
    }

    public void setContent(Control c){
        contentHolder = GetNode<Control>("ContentHolder");
        if(contentHolder.GetChildCount()==0)
            contentHolder.AddChild(c);
        
        c.Connect("focus_entered", this, "RaiseWindow");
    }

    public void setPosition(Vector2 pos){
        Vector2 POSLIMIT = GetViewportRect().Size - RectSize;

        RectPosition = new Vector2(
            Mathf.Clamp( pos.x, -150, POSLIMIT.x + 150),   
            Mathf.Clamp( pos.y, 0, POSLIMIT.y + 380)   
        );
    }

    public void setSize(Vector2 size){
        Vector2 SIZELIMIT = GetViewportRect().Size - RectPosition;
                
        RectSize = new Vector2(
            Mathf.Clamp(size.x, 300, SIZELIMIT.x ),
            Mathf.Clamp(size.y, 400, SIZELIMIT.y )
        );
    }

    public void setTitle(string t){ GetNode<Label>("titlebar/title").Text = t; }

    public void RaiseWindow(){ 
        WindowsHandler winHnd = GetParent<WindowsHandler>();
        
        winHnd.RaiseWindow(GetIndex());

        foreach(Window win in winHnd.GetChildren()){
            win.GetNode<ColorRect>("titlebar").Color = new Color("#808080");
            win.GetNode<Label>("titlebar/title").Modulate = new Color("#000000");
            //win.ReleaseFocus();
        }
        GetNode<ColorRect>("titlebar").Color = new Color("#000080");
        GetNode<Label>("titlebar/title").Modulate = new Color("#ffffff");
        //GrabFocus();
    }

    public override void _Input(InputEvent @event){

        if( @event is InputEventMouseButton ){
            mousePress = (@event as InputEventMouseButton).Pressed;
        }

        else if ( @event is InputEventMouseMotion ){
            
            InputEventMouseMotion e = (@event as InputEventMouseMotion);

            if(mousePress && mouseInTitleBar){
                
                RaiseWindow();
                setPosition(RectPosition + e.Relative);
                return; 
            }

            else if(anchorPressed){
                RaiseWindow();
                setSize(RectSize + e.Relative);
                anchorBtn.RectPosition = RectSize - anchorBtn.RectSize;
                titleBar.RectSize = new Vector2(RectSize.x - 10, 30);
                exitBtn.RectPosition = new Vector2( titleBar.RectSize.x - 29, 1);

                contentHolder.RectSize = new Vector2(RectSize.x - 10, RectSize.y - 42);
                contentHolder.GetChild<Control>(0).RectSize = contentHolder.RectSize;

                return;
            }

        }

    }

    public void ExitBtnPressed(){
        QueueFree();
    }
    
}
