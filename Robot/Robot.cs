using Godot;
using System;

public class Robot : KinematicBody
{
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
            steering = GetNode<Peripheral>("Peripherals/Actuator_Steering");
            steering.Init();
            combat = GetNode<Peripheral>("Peripherals/Actuator_Combat");
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
