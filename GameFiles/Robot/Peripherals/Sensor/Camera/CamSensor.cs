using Godot;
using System;

public class CamSensor : Peripheral
{
    public override void _Ready()
    {
        ram = new byte[1]{0};
        base._Ready();
    }


    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
    }
}
