using Godot;
using System;

public class Drill : Peripheral
{

    private Spatial drillShaft;
    private float rotvel = 0;

    public override void _Ready()
    {
        ram = new byte[1]{0};
        base._Ready();
        drillShaft = GetNode<Spatial>("MainMesh/DrillShaft");
    }

    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
        rotvel = Mathf.MoveToward(rotvel, (ram[0]==0 ? 0f : 1f), delta );
    }
    public override void tickPresentational(float delta){
        drillShaft.Rotation += new Vector3(0.2f,0,0) * rotvel;
    }
    
}
