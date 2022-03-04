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
            steering = GetNode<Peripheral>("Actuator_Steering");
            steering.Init(temp_sp);
            temp_sp -= steering.SIZE;
        }

        /* Initialize CPU */
        cpu = new CPU.CPU(Assembler.Assembler.compile(program), temp_sp );

        /* Final Init Peripherals */{
            steering.setCPU(cpu);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        cpu.InstructionCycleTick();
        //GD.Print(cpu.readFromRAM(1));

        steering.Steer(delta*0.1f);
        
    }
}
