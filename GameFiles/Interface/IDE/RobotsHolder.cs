using Godot;
using System;

public class RobotsHolder : Node
{
    static int MAXCHILD = 4; 
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
            if(n.Name.Equals(name)) return true;
        return false;
    }

    private int getIndex(string name){
        
        for(int i = 0; i<GetChildCount(); i++ ){
            if( GetChild(i).Name.Equals(name) )
                return i;
        }
        return -1;
    }

    private string AddRobot(string name, string steering="Tank", string combat="Drill", 
                            string sensor="Laser", string x="0", string y="0", string r="0", string team="0"
    ){
        name = name.ToLower();
        if(exists(name)) return String.Format("Robot:{0} exists",name); 
        RobotPlaceHolder temp = preloads[0].Instance<RobotPlaceHolder>();
        
        temp.Name = name;
        temp.steering_peripheral = steering; temp.combat_peripheral = combat; temp.sensor_peripheral = sensor;
        temp.Translation = new Vector3(Convert.ToInt32(x), 0, Convert.ToInt32(y) );
        temp.RotationDegrees = Vector3.Up * Convert.ToInt32(r);
        temp.teamIdx = (byte)Convert.ToInt32(team);
        AddChild(temp);
        temp.updatePeripherals();
        temp.updateTeam();
        //temp.robotTag.updateData();
        return String.Format("Added {0} Robot:[b]{0}[/b]",name);
    }

    private string ModRobot(string name, string ma="", string ca="", string s="", string x="", string y="", string r="", string team=""){
        
        int index = getIndex(name);
        if(index<0) return String.Format("Robot:{0} does not exist",name);
        RobotPlaceHolder temp = GetChild<RobotPlaceHolder>(index);

        if(ma!="") temp.steering_peripheral = ma;
        if(ca!="") temp.combat_peripheral = ca;
        if(s!="") temp.sensor_peripheral = s;
        temp.updatePeripherals();

        temp.Translation = new Vector3(
            (x==""?temp.Translation.x:Convert.ToInt32(x)), 0,
            (y==""?temp.Translation.z:Convert.ToInt32(y))
        );

        if(team!="") temp.teamIdx = (byte)Convert.ToInt32(team);
        if(r!="") temp.RotationDegrees = Vector3.Up * Convert.ToInt32(r);
        
        temp.robotTag.updateData();
        temp.updateTeam();

        return String.Format("Updated {0} Robot:[b]{0}[/b]",name);
    }

    private string ListRobots(){
        if(GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in GetChildren())
            s += String.Format("[color=#1b7fff]{0}[/color]\t", n.Name);
        return s;
    }

    private string generateBotName(){
        
        Godot.Collections.Array<String> randNames = new Godot.Collections.Array<string>(new string[]{
            "Joe", "Bob", "Abe", "Tom", "Rob", "Ann", "Eve", "Ken", "Gil", "Sam", "Gus", "Jodi", "Jas", "Ash",
            "Jon", "Ian", "Leo", "Rex", "Tim", "Wil", "Vic", "Kat", "Jeb", "Avi", "Art", "Zac", "Jef", "Eli",
        });

        // tries out randomNames until it finds one that isn't used. Only up to 10 times to mitigate infinite tries
        string choice = randNames[Global.RandInt(0, randNames.Count)];
        
        for(int i = 0; i<10 && exists(choice); i++) 
            choice = randNames[Global.RandInt(0, randNames.Count)];
        
        return choice;
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

    private string asmCommand(string name, string keyfilename){ 
        IDE.SaveFile.Load();

        // PRECONDITIONS
        string r = "";
        if(!exists(name))  r += "asm : bot \'"+name+"\" does not exist";
        if(!IDE.SaveFile.DATA.Contains(keyfilename)) r+= "asm : \'"+keyfilename+"\' No such file";

        if(r.Length>0) return r;

        string content = IDE.SaveFile.DATA[keyfilename] as string;
        string syntaxErrors = Assembler.SyntaxChecker.evaluateProgram(content);
        if(syntaxErrors.Length>0) return "asm : [color=#ff4411]"+syntaxErrors+"[/color]";
        
        foreach(RobotPlaceHolder rph in GetChildren()){
            if(rph.Name.Equals(name))
                rph.program = content;
        }
        
        return "";
    }

    public string ClearRobots(){
        if(GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in GetChildren()){
            n.QueueFree();
            s += String.Format("Deleted Robot:[b]{0}[/b]\n", n.Name);
        }
        return s;
    }

    public string deSetup(){
        RobotPlaceHolder[] robotsInit = new RobotPlaceHolder[GetChildCount()];

        string r = "";
        for(int i = 0; i < robotsInit.Length; i++){
            Robot rbt = GetChild<Robot>(i);
            robotsInit[i] = preloads[0].Instance<RobotPlaceHolder>();
            
            string tempName = rbt.Name;
            rbt.Name = "0";
            robotsInit[i].Name = tempName; 
            
            robotsInit[i].Translation = rbt.Translation;
            robotsInit[i].Rotation = rbt.Rotation;
            robotsInit[i].steering_peripheral = rbt.Steering_Device;    
            robotsInit[i].combat_peripheral = rbt.Combat_Device;    
            robotsInit[i].sensor_peripheral = rbt.Sensor_Device;    
            robotsInit[i].program = rbt.program;
            robotsInit[i].teamIdx = rbt.teamIdx;
            rbt.QueueFree();
            r += "removed bot \'"+robotsInit[i].Name+"\'\n";    
        }

        foreach(RobotPlaceHolder rph in robotsInit)
            AddChild(rph);
        return r;
    }

    public string setUp(){
        
        Robot[] robotsInit = new Robot[GetChildCount()];
        
        string r = "";
        for(int i = 0; i < robotsInit.Length; i++){
            RobotPlaceHolder rph = GetChild<RobotPlaceHolder>(i);
            robotsInit[i] = preloads[1].Instance<Robot>();
            
            string tempName = rph.Name;
            rph.Name = "0";
            robotsInit[i].Name = tempName; 
            
            robotsInit[i].Translation = rph.Translation;
            robotsInit[i].Rotation = rph.Rotation;
            robotsInit[i].Steering_Device = rph.steering_peripheral;    
            robotsInit[i].Combat_Device = rph.combat_peripheral;    
            robotsInit[i].Sensor_Device = rph.sensor_peripheral;    
            robotsInit[i].program = rph.program;
            robotsInit[i].teamIdx = rph.teamIdx;
            rph.QueueFree();
            r += "initialized bot \'"+robotsInit[i].Name+"\'\n";    
        }

        foreach(Robot rbt in robotsInit)
            AddChild(rbt);
        
        return r;
    }

    public void tick(float delta){
        for(int i = 0; i< GetChildCount(); i++){
            //GD.Print(GetChild(i).Name);
            if(GetChild(i) is Robot)
                (GetChild(i) as Robot).tick(delta);
        }
    }

    /// <summary> this is meant for the InterfaceConsole to access 
    /// <br>returns bbcode text</summary>
    public string interpretCommand(string[] args, IDE.STATE state=IDE.STATE.SETUP){
        
        const string help = "[b]bot[/b] <options> --<arg>=<value>\n"+
                "\toptions: \tclear|list|help|add|mod|del\n"+
                "\targs\n\t\t--name=<string>\n\t\t--steering=tank|car\n\t\t--combat=drill|chopper\n\t\t--sensor=laser|camera"+
                "\n\t\t--x|y|r=<int>" 
                //"\n\t clear|list|help\t no arguements\n\t add|mod --name= --steering? --combat? --sensor? --x? --y? --r?"+
                //"\n\t del --name="
                ;
        
        char[] argSeparator = new char[1]{'='}; 

        // reject preconditions
        if(args.Length<2) return help;
        else if( state==IDE.STATE.PLAYING && Global.match(args[0], "(clear|add|mod|del)"))
            return "bot : unable to execute command in current state";

        if( args[0].Equals("asm") ){

            if( args.Length==3){
                
                
                string[] argGrammars = {"(--name=.{1,})", "(--src=.{1,})"};
                string[] argValues = {"",""};

                for(int i = 1; i<args.Length; i++){
                    for(int j = 0; j < argGrammars.Length; j++){
                        if( Global.match(args[i], argGrammars[j]))
                            argValues[j] = args[i].Split(argSeparator)[1]; 
                    }
                }
                
                if(argValues[0]=="" && argValues[1]==""){
                    argValues[0] = args[1];
                    argValues[1] = args[2];
                }

                return asmCommand(argValues[0], argValues[1]);
                
            }else if (args.Length==2)
                return asmCommand(args[1], args[1]);
            
            else return "asm usage : asm --name=botname --src=filename";
        }
        else if( Global.match(args[1],"(clear|ls|help)") ){
                
            if(args.Length>2) return "[b]"+args[1]+"[/b] takes no arguements";
            
            switch(args[1]){
                case "ls": return ListRobots();
                case "help": return help;
                case "clear": return ClearRobots();
            }
                
        }
        else if( Global.match(args[1],"(add|mod)") ){

            // failing precondition for "bot add" and "bot mod"
            if(args.Length==2 && args[1].Equals("mod")) 
                return "bot: mod: requires --name= arguement";
            
            else if(args[1].Equals("add") && GetChildCount()>=MAXCHILD)
                return "bot: add: bot limit reached. Delete some.";

            string[] argsGrammar = new string[8]{
                "(--name=(\\w){1,})", "(--steering=(tank|car))", "(--combat=(drill|chopper))","(--sensor=(laser|camera))",
                "(--x=(-|)(\\d){1,})","(--y=(-|)(\\d){1,})","(--r=(-|)(\\d){1,})","(--team=(0|1))"
            }; 
            string[] argsValue = args[1]=="add"?
                new string[8]{ // default arguements
                    generateBotName(), 
                    Global.RandInt()%2==0?"Tank":"Car",
                    Global.RandInt()%2==0?"Chopper":"Drill",
                    Global.RandInt()%2==0?"Laser":"Camera",
                    Convert.ToString(Global.RandInt(-30,30)),   // random x-coord
                    Convert.ToString(Global.RandInt(-30,30)),   // random y-coord
                    Convert.ToString(Global.RandInt(0,60)*5),   // random rotation
                    Convert.ToString(Global.RandInt(0,2)),      // team
                }: 
                new String[8]{"","","","","","","",""};
            
            for(int i = 2; i<args.Length; i++){
                
                bool matched = false;
                for(int j = 0; j < argsGrammar.Length; j++){
                    if(Global.match(args[i], argsGrammar[j])){
                        matched = true;
                        argsValue[j] = args[i].Split(new char[1]{'='})[1];
                        if(j>0) argsValue[j] = char.ToUpper(argsValue[j][0]) + argsValue[j].Substring(1);
                    } 
                }
                if(!matched) return "[u]"+args[i] + "[/u] unrecognized parameter or arguement";
            }
            if(argsValue[0].Length==0) return "[b]bot "+args[1]+"[/b] requires [u]--name=[/u] arguement";
            else{
                if(args[1]=="add")
                    return AddRobot(
                        argsValue[0], argsValue[1], argsValue[2], argsValue[3], 
                        argsValue[4], argsValue[5], argsValue[6], argsValue[7]
                    );
                else// if(args[1]=="mod")
                    return ModRobot(
                        argsValue[0], argsValue[1], argsValue[2], argsValue[3], 
                        argsValue[4], argsValue[5], argsValue[6], argsValue[7]
                    );
            }                    
        }
        else if( args[1].Equals("rm") ){
            
            if(args.Length==3 && Global.match(args[2], "(--name=.{1,})") )
                return DeleteRobot(args[2].Replace("--name=",""));
            
            else return "bot del usage: bot rm --name=botname";
        }

        return "bot: wrong usage";
    }
}
