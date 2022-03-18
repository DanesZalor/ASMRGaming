using Godot;
using System;

public class Sparks : CPUParticles
{ 
    public override void _Ready()
    {
        SetAsToplevel(true);       
    }

}
