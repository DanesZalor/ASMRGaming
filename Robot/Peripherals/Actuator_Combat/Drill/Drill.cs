using Godot;
using System;

public class Drill : Peripheral
{

    private Spatial drillbit; // temporary

    public override void _Ready()
    {
        base._Ready();
        RAMcoordLength = 1;

        drillbit = GetNode<Spatial>("MeshPlaceHolder/Drill");
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0,0);
    }

    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
    }
    public override void tickPresentational(float delta){
        drillbit.Rotation += new Vector3(0,0,0.5f) * (readFromRam(0) > 0 ? 1 : 0);
    }
    
}
