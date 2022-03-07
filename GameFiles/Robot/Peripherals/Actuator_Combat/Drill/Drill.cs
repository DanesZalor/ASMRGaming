using Godot;
using System;

public class Drill : Peripheral
{

    private Spatial drillShaft;
    private float rotvel = 0;

    public override void _Ready()
    {
        base._Ready();
        RAMcoordLength = 1;

        drillShaft = GetNode<Spatial>("MainMesh/DrillShaft");
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0,0);
    }
    
    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
        rotvel = Mathf.MoveToward(rotvel, (readFromRam(0)==0 ? 0f : 1f), delta );
    }
    public override void tickPresentational(float delta){
        drillShaft.Rotation += new Vector3(0.2f,0,0) * rotvel;
    }
    
}
