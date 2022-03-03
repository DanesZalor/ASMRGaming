using Godot;
using System;

public class Robot : Spatial
{
    [Export(PropertyHint.MultilineText)]
    private string program = "";
    
    //MSCR_CPU cpu;

    public override void _Ready()
    {
        
        //cpu = new MSCR_CPU(Assembler.Assembler.compile(program));
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
    }
}
