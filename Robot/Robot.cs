using Godot;
using System;

public class Robot : KinematicBody
{

    public static PackedScene[] preloadedPeripherals = new PackedScene[4]{
        // movement actuators
        GD.Load<PackedScene>("res://Robot/Peripherals/Actuator_Movement/Tank/Tank.tscn"),
        GD.Load<PackedScene>("res://Robot/Peripherals/Actuator_Movement/Car/Car.tscn"),
        // combat actuators
        GD.Load<PackedScene>("res://Robot/Peripherals/Actuator_Combat/Drill/Drill.tscn"),
        GD.Load<PackedScene>("res://Robot/Peripherals/Actuator_Combat/Saw/RingSaw.tscn"),
    };

    [Export(PropertyHint.Enum, "Tank,Car")]
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
                case "Car":
                    steering = preloadedPeripherals[1].Instance<Peripheral>();
                    break;
            }
            peripherals.AddChild(steering); 
            steering.Init();

            switch(Combat_Device){
                case "Drill":
                    combat = preloadedPeripherals[2].Instance<Peripheral>();
                    break;
                case "Saw":
                    combat = preloadedPeripherals[3].Instance<Peripheral>();
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
