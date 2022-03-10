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
        
        bool match(string line, string grammar, bool exact=true){
            return Assembler.Common.match(line,grammar,exact);
        }

        cmd = cmd.Trim().ToLower();
        
        string[] args = cmd.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
        
        if(args.Length==0) return "";
        
        else if(args.Length>=2 && args[0]=="bot"){
            
            if( match(args[1],"(clear|list|help)") ){
                
                if(args.Length>2) return args[1]+" takes no arguements";
                // run code clear|list

            }else if( match(args[1],"(add|mod|del)") ){

                if(args.Length==2) return "[b]"+args[1]+ "[b] needs atleast one arguement";
                
                for(int i = 2; i<args.Length; i++){
                    
                    if(!match(args[i], 
                        "^((--name=.{1,})|"+"(--steering=(tank|car))|"+"(--combat=(drill|chopper))|"+
                        "(--sensor=(laser|camera))|"+"(--[xyr]=(-|)(\\d){1,})|"+")", 
                    true)) 
                        return args[i] + " unrecognized arguement";
                    
                }

            }

        }
        else 
            return cmd + " : command not found";
        return "";
    }

    
}
