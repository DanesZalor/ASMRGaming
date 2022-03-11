using Godot;
using System;

public class RobotsHolder : Node
{
    private static PackedScene[] preloads = {
        GD.Load<PackedScene>("res://GameFiles/Interface/RobotPlaceHolder/RobotPlaceHolder.tscn"),
        GD.Load<PackedScene>("res://GameFiles/Robot/Robot.tscn")
    };
    Node ideparent;
    public override void _Ready()
    {
        base._Ready();
        ideparent = GetParent();
    }

    private bool exists(string name){
        foreach(Node n in GetChildren())
            if(n.Name==name) return true;
        return false;
    }

    private int getIndex(string name){
        int r = -1;
        foreach(Node n in GetChildren()){
            r++;
            if(n.Name.Equals(name)) return r;
        }
        return r;

    }

    private string AddRobot(string name, string steering="Tank", string combat="Drill", 
                                string sensor="Laser", string x="0", string y="0", string r="0"
    ){
        name = name.ToLower();
        if(exists(name)) return String.Format("Robot:{0} exists",name); 
        RobotPlaceHolder temp = preloads[0].Instance<RobotPlaceHolder>();
        
        temp.Name = name;
        temp.steering_peripheral = steering; temp.combat_peripheral = combat; temp.sensor_peripheral = sensor;
        temp.Translation = new Vector3(Convert.ToInt32(x), 0, Convert.ToInt32(y) );
        temp.RotationDegrees = Vector3.Up * Convert.ToInt32(r);
        AddChild(temp);
        temp.updatePeripherals();
        temp.robotTag.updateData();
        return String.Format("Added {0} Robot:[b]{0}[/b]",name);
    }

    private string ModRobot(string name, string ma="", string ca="", string s="", string x="", string y="", string r=""){
        
        RobotPlaceHolder temp = GetChild<RobotPlaceHolder>( getIndex(name) );
        if(temp==null) return String.Format("Robot:{0} does not exist",name);

        if(ma!="") temp.steering_peripheral = ma;
        if(ca!="") temp.combat_peripheral = ca;
        if(s!="") temp.sensor_peripheral = s;
        temp.updatePeripherals();

        temp.Translation = new Vector3(
            (x==""?temp.Translation.x:Convert.ToInt32(x)), 0,
            (y==""?temp.Translation.z:Convert.ToInt32(y))
        );

        if(r!="") temp.RotationDegrees = Vector3.Up * Convert.ToInt32(r);
        temp.robotTag.updateData();

        return String.Format("Updated {0} Robot:[b]{0}[/b]",name);
    }

    private string ListRobots(){
        if(GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in GetChildren())
            s += String.Format("[color=#1b7fff]{0}[/color]\t", n.Name);
        return s;
    }

    private string DeleteRobot(string name){
        name = name.ToLower();
        for(int i = 0; i<GetChildCount(); i++){
            if(GetChild(i).Name==name){
                RemoveChild(GetChild(i));
                return String.Format("Deleted Robot:[b]{0}[/b]", name);
            }
        }
        return String.Format("Robot:[b]{0}[/b] does not exist",name);
    }

    private string ClearRobots(){
        if(GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in GetChildren()){
            n.QueueFree();
            s += String.Format("Deleted Robot:[b]{0}[/b]\n", n.Name);
        }
        return s;
    }

    /// <summary> this is meant for the InterfaceConsole to access 
    /// <br>returns bbcode text</summary>
    public string interpretCommand(string[] args){
        
        const string help = "[b]bot[/b] <options> --<arg>=<value>\n"+
                "\toptions: \tclear|list|help|add|mod|del\n"+
                "\targs\n\t\t--name=<string>\n\t\t--steering=tank|car\n\t\t--combat=drill|chopper\n\t\t--sensor=laser|camera"+
                "\n\t\t--x|y|r=<int>" 
                //"\n\t clear|list|help\t no arguements\n\t add|mod --name= --steering? --combat? --sensor? --x? --y? --r?"+
                //"\n\t del --name="
                ;

        bool match(string line, string grammar, bool exact=true){
            return Assembler.Common.match(line,grammar,exact);
        }

        if(args.Length<2) return help;
        if( match(args[1],"(clear|list|help)") ){
                
            if(args.Length>2) return "[b]"+args[1]+"[/b] takes no arguements";
            
            switch(args[1]){
                case "list": return ListRobots();
                case "help": return help;
                case "clear": return "robots cleared";
            }
                
        }
        else if( match(args[1],"(add|mod)") ){

            if(args.Length==2) return "[b]"+args[1]+ "[/b] needs atleast one arguement";
            
            string[] argsGrammar = new string[7]{
                "(--name=(\\w){1,})", "(--steering=(tank|car))", "(--combat=(drill|chopper))",
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
                if(!matched) return "[u]"+args[i] + "[/u] unrecognized parameter or arguement";
            }
            if(argsValue[0].Length==0) return "[b]bot "+args[1]+"[/b] requires [u]--name=[/u] arguement";
            else{
                if(args[1]=="add")
                    return AddRobot(
                        argsValue[0], argsValue[1], argsValue[2], argsValue[3], 
                        argsValue[4], argsValue[5], argsValue[6]
                    );
                else// if(args[1]=="mod")
                    return ModRobot(
                        argsValue[0], argsValue[1], argsValue[2], argsValue[3], 
                        argsValue[4], argsValue[5], argsValue[6]
                    );
            }                    
        }
        else if( match(args[1], "del")){
            
            if(args.Length==3 && match(args[2], "(--name=.{1,})") )
                return DeleteRobot(args[2].Replace("--name=",""));
            
            else return "[b]del[/b] only requires [u]--name=[/u] arguement";
            
            

        }
        return "";
    }
}
