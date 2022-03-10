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
    public string Command_AddRobot(string name, string steering="Tank", string combat="Drill", string sensor="Laser"){
        name = name.ToLower();
        if(precommand_RobotExists(name)) return String.Format("Robot:{0} exists",name);

        RobotPlaceHolder temp = preloads[0].Instance<RobotPlaceHolder>();
        temp.Name = name;
        temp.steering_peripheral = steering; temp.combat_peripheral = combat; temp.sensor_peripheral = sensor;
        robots.AddChild(temp);
        return String.Format("Added Robot:{0}",name);
    }

    public string Command_ListRobots(){
        if(robots.GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in robots.GetChildren())
            s += n.Name + "\n";
        return s;
    }

    public string Command_DeleteRobot(string name){
        name = name.ToLower();
        for(int i = 0; i<robots.GetChildCount(); i++){
            if(robots.GetChild(i).Name==name){
                robots.RemoveChild(robots.GetChild(i));
                return String.Format("Deleted Robot:{0}", name);
            }
        }
        return String.Format("Robot:{0} does not exist",name);
    }
}
