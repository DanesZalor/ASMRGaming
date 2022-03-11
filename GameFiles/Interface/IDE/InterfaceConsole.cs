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

            }else if( match(args[1],"(add|mod)") ){

                if(args.Length==2) return "[b]"+args[1]+ "[/b] needs atleast one arguement";
                
                string[] argsGrammar = new string[7]{
                    "(--name=.{1,})", "(--steering=(tank|car))", "(--combat=(drill|chopper))",
                    "(--sensor=(laser|camera))","(--x=(-|)(\\d){1,})","(--y=(-|)(\\d){1,})","(--r=(-|)(\\d){1,})"
                }; 
                string[] argsValue = args[1]=="add"?
                    new string[7]{"","Tank","Drill","Laser","0","0","0"}:
                    new String[7]{"","","","","","",""};
                
                for(int i = 2; i<args.Length; i++){
                    
                    bool matched = false;
                    for(int j = 0; j < argsGrammar.Length; j++){
                        if(match(args[i], argsGrammar[j])){
                            matched = true;
                            argsValue[j] = args[i].Split(new char[1]{'='})[1];
                            argsValue[j] = char.ToUpper(argsValue[j][0]) + argsValue[j].Substring(1);
                        } 
                    }
                    if(!matched) return "[u]"+args[i] + "[/u] unrecognized arguement";
                }
                if(argsValue[0].Length==0) return "[b]bot "+args[1]+"[/b] requires [u]--name=[/u] arguement";
                else{
                    if(args[1]=="add")
                        return ideparent.robots.AddRobot(
                            argsValue[0], argsValue[1], argsValue[2], argsValue[3], 
                            argsValue[4], argsValue[5], argsValue[6]
                        );
                    else if(args[1]=="mod")
                        return ideparent.robots.ModRobot(
                            argsValue[0], argsValue[1], argsValue[2], argsValue[3], 
                            argsValue[4], argsValue[5], argsValue[6]
                        );
                }                    
            }

        }
        else 
            return cmd + " : command not found";
        return "";
    }

    
}
