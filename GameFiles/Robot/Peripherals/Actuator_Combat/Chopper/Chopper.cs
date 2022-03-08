using Godot;
using System;

public class Chopper : Peripheral
{
    private Spatial blades;
    private float rotvel = 0;
    public override void _Ready()
    {
        RAMcoordLength = 1;
        base._Ready();
        blades = GetNode<Spatial>("MainMesh/Blades"); 
        ram[0] = 0; 
    }

    public override void tickLogical(float delta)
    {
        //rotvel = Mathf.Lerp(rotvel, (readFromRam(0)==0 ? 0f : 1f), 0.05f );
        rotvel = Mathf.MoveToward(rotvel, (ram[0]==0 ? 0f : 1f), delta );
    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
        blades.Rotation += new Vector3(0,0.2f,0) * rotvel;
    }

}
