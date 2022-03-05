using Godot;
using System;

public class Robot : KinematicBody
{

    public static PackedScene[] preloadedPeripherals = new PackedScene[2]{
        // movement actuators
        GD.Load<PackedScene>("res://Robot/Peripherals/Actuator_Movement/TankSteering/TankSteering.tscn"),
        // combat actuators
        GD.Load<PackedScene>("res://Robot/Peripherals/Actuator_Combat/Drill/Drill.tscn"),
    };

    [Export(PropertyHint.Enum, "Tank,Differential")]
    private string Steering_Device = "Tank";

    [Export(PropertyHint.Enum, "Drill,Saw")]
    private string Combat_Device = "Drill";

    [Export(PropertyHint.MultilineText)]
    private string program = "";
    
    private Peripheral steering;
    private Peripheral combat;
    private CPU.CPU cpu;

    public CPU.CPU CPU { 
        get { return cpu; } 
    }

    public override void _Ready()
    {
        GD.Print(program);
        cpu = new CPU.CPU(Assembler.Assembler.compile(program));

        /* Init Peripherals */{
            Node peripherals = GetNode("Peripherals");

            switch(Steering_Device){
                case "Tank":
                    steering = preloadedPeripherals[0].Instance<Peripheral>();
                    break;
                case "Diferential":
                    break;
            }
            peripherals.AddChild(steering); 
            steering.Init();

            switch(Combat_Device){
                case "Drill":
                    combat = preloadedPeripherals[1].Instance<Peripheral>();
                    
                    break;
                case "Saw":
                    break;
            } 
            peripherals.AddChild(combat);
            combat.Init();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        cpu.InstructionCycleTick();

        steering.tick(delta);
        combat.tick(delta);
        
    }
}
