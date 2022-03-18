using Godot;
using System;

public class Sparks : CPUParticles
{ 
    public override void _Ready()
    {
        SetAsToplevel(true);       
    }


    /// <summary> to disable emission, set point to Vector3.Inf </summary>
    public void Emit(Vector3 point){
        Emitting = (!point.Equals(Vector3.Zero)); 
        Translation = point;
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
