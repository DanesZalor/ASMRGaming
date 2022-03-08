using Godot;
using System;

public class CamSensor : Peripheral
{
    public override void _Ready()
    {
        RAMcoordLength = 2;
        base._Ready();
        ram[0] = 0;
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
