using Godot;
using System;

public class TankSteering : Peripheral
{

    public override void _Ready()
    {
        base._Ready();
        RAMcoordLength = 2;
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0, 127);
        writeToRam(1, 127);
    }

    public override void tick(float delta){

        float lram = (float)(Godot.Mathf.Min(readFromRam(1),254));
        float rram = (float)(Godot.Mathf.Min(readFromRam(0),254));

        float lwp = ( ((lram-127f) / 127f) * 0.25f ) * delta * 30f;
        float rwp = ( ((rram-127f) / 127f) * 0.25f ) * delta * 30f;

        //GD.Print(String.Format("[254] = {0}\t[255] = {1}",lram, rram));

        parent.Rotation -= new Vector3(0f, lwp, 0f);
        Vector3 lvel = GlobalTransform.basis.z * lwp *3f;
        
        parent.Rotation += new Vector3(0f, rwp, 0f);
        Vector3 rvel = GlobalTransform.basis.z * rwp *3f;

        parent.MoveAndCollide(lvel + rvel);

    }
}
