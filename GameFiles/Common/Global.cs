using Godot;
using System;

public class Global : Node
{
    private static byte curr_frame = 0;
    public static byte FRAME {
        get => curr_frame;
    }
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        curr_frame = (byte)( (curr_frame + 1) % 60 );
    }
}
