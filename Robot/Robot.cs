using Godot;
using System;

public class Robot : Spatial
{
    [Export(PropertyHint.MultilineText)]
    private string program = "";
    
    private Peripheral steering;
    private CPU.CPU cpu;

    public override void _Ready()
    {
        GD.Print(program);
        byte temp_sp = 255;

        /* Count Peripherals */{
            steering = GetNode<Peripheral>("Peripherals/Actuator_Steering");
            steering.Init(temp_sp);
            temp_sp -= steering.SIZE;
        }

        /* Initialize CPU */
        cpu = new CPU.CPU(Assembler.Assembler.compile(program), temp_sp );

        /* Final Init Peripherals */{
            steering.setCPU(cpu, this);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        cpu.InstructionCycleTick();
        GD.Print(cpu.readFromRAM(255));

        steering.Steer(delta*0.1f);
        
    }
}
