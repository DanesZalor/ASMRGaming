using Godot;
using System;

public class Car : Peripheral
{
    public override void _Ready()
    {
        RAMcoordLength = 2;       
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0, 127);
        writeToRam(1, 127);
    }

    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
        float accel = (((float)readFromRam(1) - 127f) / 127f) * delta * 45f;
        float steering = (((float)readFromRam(0) - 127f) / 127f)  ;

        parent.RotationDegrees -= Vector3.Up * steering * delta * 135f * accel;

        Vector3 vel = GlobalTransform.basis.z * delta * 15f * accel;
        parent.MoveAndCollide(vel);
    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
    }
}
