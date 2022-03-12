using Godot;
using System;

public class RobotPlaceHolder : Spatial
{
    [Export(PropertyHint.Enum, "Tank,Car")] public string steering_peripheral = "Tank";
    [Export(PropertyHint.Enum, "Drill,Chopper")] public string combat_peripheral = "Drill";
    [Export(PropertyHint.Enum, "Laser,Camera")] public string sensor_peripheral = "Laser";
    [Export(PropertyHint.MultilineText)] public string program = ""; 
    private Spatial[] peripherals;
    public Tag robotTag;

    public override void _Ready()
    {
        peripherals = new Spatial[6]{
            GetNode<Spatial>("Parts/Tank"),
            GetNode<Spatial>("Parts/Car"),
            GetNode<Spatial>("Parts/Drill"),
            GetNode<Spatial>("Parts/Chopper"),
            GetNode<Spatial>("Parts/Laser"),
            GetNode<Spatial>("Parts/Camera")
        };
        updatePeripherals();
        robotTag = GetNode<Tag>("3DNameTag/Viewport/Tag");
    }

    public void updatePeripherals(){
        foreach(Spatial p in peripherals){
            
            p.Visible = false;
            if( p.Name.Equals(steering_peripheral) || 
                p.Name.Equals(combat_peripheral) || 
                p.Name.Equals(sensor_peripheral)
            ) p.Visible = true;
        }
    }

}
