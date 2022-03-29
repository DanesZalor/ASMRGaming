using Godot;
using System;

public class CommandPrompt : ColorRect
{
    private RichTextLabel logs; private LineEdit prompt; private LinkedList<string> cmd_history;
    private Window windowParent; private IDE ideparent;
    public override void _Ready()
    {
        windowParent = GetParent().GetParent<Window>();
        ideparent = windowParent.GetParent().GetParent<IDE>();
        cmd_history = new LinkedList<string>();
        logs = GetNode<RichTextLabel>("Logs");
        prompt = GetNode<LineEdit>("Prompt");
        Connect("item_rect_changed", this, "RectUpdated");
        RectUpdated();
        windowParent.setTitle("Command Prompt");
    }

    public void RectUpdated(){
        logs.RectSize = RectSize;
        prompt.RectPosition = new Vector2(27, RectSize.y - 28);
        prompt.RectSize = new Vector2(logs.RectSize.x - 38, prompt.RectSize.y );
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

    int logLines = 0; LinkedListNode<string> cmd_history_NodePointer;
    public void onPromptEnteredSignal(String cmd){
        
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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if(@event is InputEventKey && prompt.HasFocus() && (@event as InputEventKey).Pressed ){

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
