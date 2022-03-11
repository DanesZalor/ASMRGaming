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

    public string AddRobot(string name, string steering="Tank", string combat="Drill", 
                                string sensor="Laser", string x="0", string y="0", string r="0"
    ){
        name = name.ToLower();
        //GD.Print(String.Format("{0} {1} {2} {3} {4} {5} {6}", name, steering, combat, sensor, x, y, r));
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

    public string ModRobot(string name, string ma="", string ca="", string s="", string x="", string y="", string r=""){
        
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

    public string ListRobots(){
        if(GetChildCount()==0) return "No Robots found";
        string s = "";
        foreach(Node n in GetChildren())
            s += String.Format("[b]{0}[/b]\n");
        return s;
    }

    public string DeleteRobot(string name){
        name = name.ToLower();
        for(int i = 0; i<GetChildCount(); i++){
            if(GetChild(i).Name==name){
                RemoveChild(GetChild(i));
                return String.Format("Deleted Robot:[b]{0}[/b]", name);
            }
        }
        return String.Format("Robot:[b]{0}[/b] does not exist",name);
    }
}
