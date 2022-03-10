using Godot;
using System;

public class RobotPlaceHolder : Spatial
{
    [Export(PropertyHint.Enum, "Tank,Car")] public string steering_peripheral = "Tank";
    [Export(PropertyHint.Enum, "Drill,Chopper")] public string combat_peripheral = "Drill";
    [Export(PropertyHint.Enum, "Laser,Camera")] public string sensor_peripheral = "Laser";

    private Spatial[] peripherals;

    public override void _Ready()
    {
        peripherals = new Spatial[6]{
            GetNode<Spatial>("Parts/ActuatorMovement/Tank"),
            GetNode<Spatial>("Parts/ActuatorMovement/Car"),
            GetNode<Spatial>("Parts/ActuatorCombat/Drill"),
            GetNode<Spatial>("Parts/ActuatorCombat/Chopper"),
            GetNode<Spatial>("Parts/Sensor/Laser"),
            GetNode<Spatial>("Parts/Sensor/Camera")
        };
        updatePeripherals();
    }

    public void updatePeripherals(string ca="", string ma="", string s=""){
        
        if(ca=="") ca = combat_peripheral;
        if(ma=="") ma = steering_peripheral;
        if(s=="") ma = sensor_peripheral;
        
        foreach(Spatial p in peripherals){
            p.Visible = false;

            if(p.Name==steering_peripheral || p.Name==combat_peripheral || p.Name==sensor_peripheral) p.Visible = true;
        }

    }

}
