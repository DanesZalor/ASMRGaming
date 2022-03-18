using Godot;
using System;

public class Robot : KinematicBody
{
    public static byte CLOCKSPEED = 5;
    public static class preloads{
        public static PackedScene[] ACTUATOR_MOVEMENT = new PackedScene[2]{
            GD.Load<PackedScene>("res://GameFiles/Robot/Peripherals/Actuator_Movement/Tank/Tank.tscn"),
            GD.Load<PackedScene>("res://GameFiles/Robot/Peripherals/Actuator_Movement/Car/Car.tscn"),
        };

        public static PackedScene[] ACTUATOR_COMBAT = new PackedScene[2]{
            GD.Load<PackedScene>("res://GameFiles/Robot/Peripherals/Actuator_Combat/Drill/Drill.tscn"),
            GD.Load<PackedScene>("res://GameFiles/Robot/Peripherals/Actuator_Combat/Chopper/Chopper.tscn"),
        };

        public static PackedScene[] SENSORS = new PackedScene[2]{
            GD.Load<PackedScene>("res://GameFiles/Robot/Peripherals/Sensor/LaserCast/LaserSensor.tscn"),
            GD.Load<PackedScene>("res://GameFiles/Robot/Peripherals/Sensor/Camera/CamSensor.tscn"),
        };
    }

    [Export(PropertyHint.Range, "0,1,")] public byte teamIdx = 0;
    [Export(PropertyHint.Enum, "Tank,Car")] public string Steering_Device = "Tank";
    [Export(PropertyHint.Enum, "Drill,Chopper")] public string Combat_Device = "Drill";
    [Export(PropertyHint.Enum, "Laser,Camera")] public string Sensor_Device = "Laser";  
    [Export(PropertyHint.MultilineText)] public string program = "";

    private int HealthPoints = 255; 

    private CPU.CPU cpu;    public CPU.CPU CPU { get { return cpu; } }

    private Peripheral[] peripherals;
    private HPHud hpHud;

    public override void _Ready()
    {
        hpHud = GetNode<HPHud>("Sprite3D/HPHUD/HPBar");
        hpHud.updateData(HealthPoints, Name);
        //GD.Print(Name+":\n"+program+"----\n\n");
        // remove editor placeholder
        GetNode("MeshPlaceHolder").QueueFree();

        cpu = new CPU.CPU(Assembler.Assembler.compile(program));

        /* Init Peripherals */{
            Node p = GetNode("Peripherals");
            switch(Steering_Device){
                case "Tank":
                    p.AddChild(preloads.ACTUATOR_MOVEMENT[0].Instance<Peripheral>());
                    break;
                case "Car":
                    p.AddChild(preloads.ACTUATOR_MOVEMENT[1].Instance<Peripheral>());
                    break;
            }
            switch(Combat_Device){
                case "Drill":
                    p.AddChild(preloads.ACTUATOR_COMBAT[0].Instance<Peripheral>());
                    break;
                case "Chopper":
                    p.AddChild(preloads.ACTUATOR_COMBAT[1].Instance<Peripheral>());
                    break;
            } 
            switch(Sensor_Device){
                case "Laser":
                    p.AddChild(preloads.SENSORS[0].Instance<Peripheral>());
                    break;
                case "Camera":
                    p.AddChild(preloads.SENSORS[1].Instance<Peripheral>());
                    break;
            }
            
            peripherals = new Peripheral[p.GetChildCount()];
            for(byte i = 0; i<p.GetChildCount(); i++){
                peripherals[i] = p.GetChild<Peripheral>(i);
            }

        }
    }

    public void tick(float delta)
    {
        base._PhysicsProcess(delta);

        for(int i = 0; i < Mathf.Max(CLOCKSPEED,1); i++ )
            cpu.InstructionCycleTick();

        foreach(Peripheral p in peripherals)
                p.tick(delta);
        
        int squareLimit = 49;
        Translation = new Vector3(
            Mathf.Clamp(Translation.x, -squareLimit, squareLimit), 0,
            Mathf.Clamp(Translation.z, -squareLimit, squareLimit)
        );
    }

    public void recieveDamage(int damage){
        HealthPoints -= damage;
        hpHud.updateData(HealthPoints);
        if(HealthPoints<=0) QueueFree();
    }
}
