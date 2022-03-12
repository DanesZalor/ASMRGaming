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
    
    private string interpretCommand(string cmd){
        cmd = cmd.Trim().ToLower();
        string[] args = cmd.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
        return ideparent.interpretCommand(args);
    }

    public void clearCommand(){
        logLines = 0;
        logs.BbcodeText = "";
    }

    // EVENTS
    int logLines = 0;
    /*Signal*/ public void onPromptEnteredSignal(String cmd){
        
        prompt.Text = "";
        if(Global.match(cmd, "clear")) {
            clearCommand();
            return;
        }
        
        string feedback = interpretCommand(cmd);
        logs.BbcodeText += "[color=#8fff7f]>>[/color] " + cmd + "\n" + (feedback.Length > 0? (feedback + "\n") :"");
        
        cmd_history.AddFirst(cmd);
        if(cmd_history.Count >= 10) cmd_history.RemoveLast();
        cmd_history_NodePointer = null;

        /*Line capping*/{
            logLines += 1;
            if(logLines>20){
                string[] lineSplit = logs.BbcodeText.Split(new string[1]{"[color=#8fff7f]>>[/color] "}, 2, StringSplitOptions.RemoveEmptyEntries);
                //foreach(string line in lineSplit) GD.Print(line +"-----"); GD.Print("\n\n\n\n\n");
                logs.BbcodeText = lineSplit[1];
                logLines = 20;
            }
        }
        
    }

    private bool mousePressInTitleBar = false, mouseInTitleBar = false, mouseInLogs = false;

    /*Signal*/ public void _on_TitleBarMouseEnterOrExit(bool enter){ mouseInTitleBar = enter; }
    LinkedListNode<string> cmd_history_NodePointer;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if( (@event is InputEventMouseButton) ){
            
            InputEventMouseButton e = (@event as InputEventMouseButton);
            mousePressInTitleBar = e.Pressed && mouseInTitleBar;
        }

        else if(@event is InputEventMouseMotion && mousePressInTitleBar){
            
            InputEventMouseMotion e = (@event as InputEventMouseMotion);
            Vector2 screenSize = Global.SCREENSIZE;
            RectPosition += e.Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, screenSize.x - RectSize.x),
                Mathf.Clamp(RectPosition.y, 0, screenSize.y - RectSize.y)
            );
            
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
    
}
