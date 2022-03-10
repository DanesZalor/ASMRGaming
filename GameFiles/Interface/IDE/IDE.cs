using Godot;
using System;

public class IDE : Node
{
    private static PackedScene[] preloads = {
        GD.Load<PackedScene>("res://GameFiles/Interface/RobotPlaceHolder/RobotPlaceHolder.tscn"),
        GD.Load<PackedScene>("res://GameFiles/Robot/Robot.tscn")
    };
    private InterfaceConsole console;
    private Node robots;
    private Spatial camHolder; private Camera cam;

    public override void _Ready()
    {
        base._Ready();
        camHolder = GetNode<Spatial>("EnvironmentStuff/CamHolder");
        cam = camHolder.GetNode<Camera>("Camera");

        robots = GetNode("Robots");
        console = GetNode<InterfaceConsole>("Console");
    }

    private bool precommand_RobotExists(string name){
        foreach(Node n in robots.GetChildren())
            if(n.Name==name) return true;
        return false;
    }
    public string Command_AddRobot(string name, string steering="Tank", string combat="Drill", 
                                string sensor="Laser", string x="0", string y="0", string r="0"
    ){
        name = name.ToLower();
        if(precommand_RobotExists(name)) return String.Format("Robot:{0} exists",name);

        RobotPlaceHolder temp = preloads[0].Instance<RobotPlaceHolder>();
        temp.Name = name;
        temp.steering_peripheral = steering; temp.combat_peripheral = combat; temp.sensor_peripheral = sensor;
        temp.Translation = new Vector3(Convert.ToInt32(x), 0, Convert.ToInt32(y) );
        temp.RotationDegrees = Vector3.Up * Convert.ToInt32(r);
        robots.AddChild(temp);
        return String.Format("Added Robot:[b]{0}[/b]",name);
    }

    public string Command_ListRobots(){
        if(robots.GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in robots.GetChildren())
            s += String.Format("[b]{0}[/b]\n");
        return s;
    }

    public string Command_DeleteRobot(string name){
        name = name.ToLower();
        for(int i = 0; i<robots.GetChildCount(); i++){
            if(robots.GetChild(i).Name==name){
                robots.RemoveChild(robots.GetChild(i));
                return String.Format("Deleted Robot:[b]{0}[/b]", name);
            }
        }
        return String.Format("Robot:[b]{0}[/b] does not exist",name);
    }
}
