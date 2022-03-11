using Godot;
using System;


public class InterfaceConsole : Node
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

    public void onPromptEnteredSignal(String cmd){
        string feedback = interpretCommand(cmd);
        logs.BbcodeText += "[color=#8fff7f]>>[/color] " + cmd + "\n" + (feedback.Length > 0? (feedback + "\n") :"");
        prompt.Text = "";
    }

    private string interpretCommand(string cmd){
        
        

        cmd = cmd.Trim().ToLower();
        
        string[] args = cmd.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
        
        if(args.Length==0) return "";
        
        else if(args.Length>=1 && args[0]=="bot")
            return ideparent.robots.interpretCommand(args);
        else 
            return cmd + " : command not found";
    }

    
}
