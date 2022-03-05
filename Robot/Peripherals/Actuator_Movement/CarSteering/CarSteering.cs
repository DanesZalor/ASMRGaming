using Godot;
using System;

public class CarSteering : Peripheral
{
    public override void _Ready()
    {
        RAMcoordLength = 2;       
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0, 0);
        writeToRam(1, 127);
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
