using Godot;
using System;

public class Robot : KinematicBody
{
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
            
            switch(Steering_Device){
                case "Tank":
                    steering = GetNode<Peripheral>("Peripherals/Actuator_Steering");
                    break;
                case "Diferential":
                    break;
            } 
            steering.Init();

            switch(Combat_Device){
                case "Drill":
                    combat = GetNode<Peripheral>("Peripherals/Actuator_Combat");
                    break;
                case "Saw":
                    break;
            } 
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
