using Godot;
using System;

public class TankSteering : Peripheral
{
    
    public override void _Ready()
    {
        RAMcoordLength = 2;
    }

    public override void setCPU(CPU.CPU c, Spatial p)
    {
        base.setCPU(c, p);
        writeToRam(0, 127);
        writeToRam(1, 127);
    }
    public override void Steer(float delta){

        byte lram = readFromRam(1);
        byte rram = readFromRam(0);

        int lw = (lram == 127) ? 0 : (lram<127? -1:1);
        int rw = (rram == 127) ? 0 : (rram<127? -1:1);

        //GD.Print(lw); GD.Print(rw); GD.Print("--");

        float lwspeed = Godot.Mathf.Abs(lw-127)*delta*0.1f;
        float rwspeed = Godot.Mathf.Abs(rw-127)*delta*0.1f;
        
        parent.Rotation += new Vector3(0f, lw, 0f) * lwspeed;
        parent.Translation += GlobalTransform.basis.z * lw*2f * lwspeed;

        parent.Rotation -= new Vector3(0f, rw, 0f) * rwspeed;
        parent.Translation += GlobalTransform.basis.z * rw*2f * rwspeed;
    }
}
