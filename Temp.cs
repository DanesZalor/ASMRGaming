using Godot;
using System;

public class Temp : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta){
        foreach(Node n in GetChildren()){
            if (n is Robot)
                (n as Robot).tick(delta);
        }
    }
}