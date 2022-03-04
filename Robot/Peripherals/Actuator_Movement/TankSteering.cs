using Godot;
using System;

public class TankSteering : Peripheral
{
    
    public override void _Ready()
    {
        RAMcoordLength = 2;
    }

    public override void setCPU(CPU.CPU c)
    {
        base.setCPU(c);
        writeToRam(0, 127);
        writeToRam(1, 127);
    }
    public override void Steer(float delta){

        byte lram = readFromRam(1);
        byte rram = readFromRam(0);

        int lw = (lram == 127) ? 0 : (lram<127? -1:1);
        int rw = (rram == 127) ? 0 : (rram<127? -1:1);

        //GD.Print(lw); GD.Print(rw); GD.Print("--");

        float lwspeed = Godot.Mathf.Abs(lw-127)*delta;
        float rwspeed = Godot.Mathf.Abs(rw-127)*delta;

        Rotation -= new Vector3(0f, lw, 0f) * lwspeed;
        Translation += GlobalTransform.basis.z * lw*2f * lwspeed;

        Rotation += new Vector3(0f, rw, 0f) * rwspeed;
        Translation += GlobalTransform.basis.z * rw*2f * rwspeed;
    }
}
