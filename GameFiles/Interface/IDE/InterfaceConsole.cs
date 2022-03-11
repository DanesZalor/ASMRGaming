using Godot;
using System;


public class InterfaceConsole : Control
{
    private IDE ideparent;    
    private RichTextLabel logs;
    private LineEdit prompt;
    private Control titleBar;

    private LinkedList<string> cmd_history;
    public override void _Ready()
    {
        cmd_history = new LinkedList<string>();
        ideparent = GetParent<IDE>();
        logs = GetNode<RichTextLabel>("Logs");
        prompt = GetNode<LineEdit>("Prompt");
        titleBar = GetNode<Control>("BlackBG/TitleBar");
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }
    private string interpretCommand(string cmd){

        cmd = cmd.Trim().ToLower();
        
        string[] args = cmd.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
        
        if(args.Length==0) return "";
        
        else if(args.Length>=1 && args[0]=="bot")
            return ideparent.robots.interpretCommand(args);
        else if(args.Length>=1 && Global.match(args[0], "(ls|touch|cat|edit|rm|mv)") )
            return ideparent.windowsHandler.interpretCommand(args);

        else 
            return cmd + " : command not found";
    }
    // EVENTS
    public void onPromptEnteredSignal(String cmd){
        string feedback = interpretCommand(cmd);
        logs.BbcodeText += "[color=#8fff7f]>>[/color] " + cmd + "\n" + (feedback.Length > 0? (feedback + "\n") :"");
        prompt.Text = "";
        
        cmd_history.AddFirst(cmd);
        if(cmd_history.Count >= 10) cmd_history.RemoveLast();
        cmd_history_NodePointer = null;
    }

    private bool mousePress = false, mouseIn = false;

    public void _on_TitleBarMouseEnterOrExit(bool enter){
        mouseIn = enter;
    }

    LinkedListNode<string> cmd_history_NodePointer;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if( (@event is InputEventMouseButton) ){
            mousePress = (@event as InputEventMouseButton).Pressed && mouseIn;
        }

        else if(@event is InputEventMouseMotion && mousePress){
            
            InputEventMouseMotion e = (@event as InputEventMouseMotion);
            Vector2 screenSize = Global.SCREENSIZE;
            RectPosition += e.Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, screenSize.x - RectSize.x),
                Mathf.Clamp(RectPosition.y, 0, screenSize.y - RectSize.y)
            );
            
            //if(!Global.isOnScreen(e.Position)) mousePress = false; // we can disable this if its too clunky or some shit
        }
        else if(@event is InputEventKey && prompt.HasFocus() && (@event as InputEventKey).Pressed ){
            
            bool[] arrow = {Input.IsActionJustPressed("ArrowUp"), Input.IsActionJustPressed("ArrowDown")};

            if(arrow[0] || arrow[1]){
                
                if(cmd_history_NodePointer==null) 
                    cmd_history_NodePointer = cmd_history.First;

                else if(arrow[0] && !arrow[1] && cmd_history_NodePointer.Next != null) // arrow up
                    cmd_history_NodePointer = cmd_history_NodePointer.Next;

                else if(arrow[1] && !arrow[0] ) // arrow down
                    cmd_history_NodePointer = cmd_history_NodePointer.Previous;
                
                if(cmd_history_NodePointer != null){
                    prompt.Text = cmd_history_NodePointer.Value;
                    
                    async void setCaretPositionLast(){
                        await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
                        prompt.CaretPosition = prompt.Text.Length;
                    } setCaretPositionLast();
                }
            }

        }
    }
    
    public void _on_ConsoleHitBox_pressed(){
        prompt.GrabFocus();
    }
}
