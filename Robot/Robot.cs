using Godot;
using System;

public class Robot : Spatial
{
    [Export(PropertyHint.MultilineText)]
    private string program = "";
    
    CPU.CPU cpu;

    public override void _Ready()
    {
        GD.Print(program);
        cpu = new CPU.CPU(Assembler.Assembler.compile(program));
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
    }
}
