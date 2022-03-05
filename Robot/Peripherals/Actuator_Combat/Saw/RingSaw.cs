using Godot;
using System;

public class RingSaw : Peripheral
{
    private Spatial sawmesh; 
    public override void _Ready()
    {
        RAMcoordLength = 1;

        sawmesh = GetNode<Spatial>("MeshPlaceholder");   
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

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
        sawmesh.Rotation += new Vector3(0,0.1f,0) * (readFromRam(0) > 0? 1 : 0);
    }

}
