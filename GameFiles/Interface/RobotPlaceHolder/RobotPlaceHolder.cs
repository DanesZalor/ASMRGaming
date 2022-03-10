using Godot;
using System;

public class RobotPlaceHolder : Spatial
{
    [Export(PropertyHint.Enum, "Tank,Car")] private string steering_peripheral = "Tank";
    [Export(PropertyHint.Enum, "Drill,Chopper")] private string combat_peripheral = "Drill";
    [Export(PropertyHint.Enum, "Laser,Camera")] private string sensor_peripheral = "Laser";

    private Spatial[,] peripherals;

    public override void _Ready()
    {
        peripherals = new Spatial[3,2]{
            {
                GetNode<Spatial>("ActuatorMovement/Tank"),
                GetNode<Spatial>("ActuatorMovement/Car")
            },{
                GetNode<Spatial>("ActuatorCombat/Drill"),
                GetNode<Spatial>("ActuatorCombat/Chopper")
            },{
                GetNode<Spatial>("Sensor/Laser"),
                GetNode<Spatial>("Sensor/Camera")
            }
        };
    }

    public void setVisible(string ca="Drill", string ma="Tank", string s="Laser"){
        
        foreach(Spatial p in peripherals){
            p.Visible = false;

            if(p.Name==ca || p.Name==ma || p.Name==s) p.Visible = true;
        }

    }


}
