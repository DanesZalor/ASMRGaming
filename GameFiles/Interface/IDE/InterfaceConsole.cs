using Godot;
using System;


public class InterfaceConsole : Control
{
    private IDE ideparent;    
    private RichTextLabel logs;
    private LineEdit prompt;
    public override void _Ready()
    {
        ideparent = GetParent<IDE>();
        logs = GetNode<RichTextLabel>("Logs");
        prompt = GetNode<LineEdit>("Prompt");
    }

    private string interpretCommand(string cmd){

        cmd = cmd.Trim().ToLower();
        
        string[] args = cmd.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
        
        if(args.Length==0) return "";
        
        else if(args.Length>=1 && args[0]=="bot")
            return ideparent.robots.interpretCommand(args);
        else if(args.Length>=1 && Global.match(args[0], "(cat|touch|ls|edit)") )
            return ideparent.windowsHandler.interpretCommand(args);

        else 
            return cmd + " : command not found";
    }

    // EVENTS
    public void onPromptEnteredSignal(String cmd){
        string feedback = interpretCommand(cmd);
        logs.BbcodeText += "[color=#8fff7f]>>[/color] " + cmd + "\n" + (feedback.Length > 0? (feedback + "\n") :"");
        prompt.Text = "";
    }

    private bool mouseIn = false, mousePress = false; 
    public void _on_TitleBar_mouse_entered(){
        mouseIn = true;
        //GD.Print("mousein");
    }

    public void _on_TitleBar_mouse_exited(){
        mouseIn = false;
        mousePress = false;
        //GD.Print("mouseout");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if(mouseIn && @event is InputEventMouseButton)
            mousePress = (@event as InputEventMouseButton).Pressed;
        else if(@event is InputEventMouseMotion && mousePress){
            RectPosition += (@event as InputEventMouseMotion).Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, 848),
                Mathf.Clamp(RectPosition.y, 0, 357)
            );
        }
    }
}
