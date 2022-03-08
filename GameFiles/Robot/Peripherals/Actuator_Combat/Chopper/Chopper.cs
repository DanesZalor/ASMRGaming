using Godot;
using System;

public class Chopper : Peripheral
{
    private Spatial blades;
    private float rotvel = 0;
    public override void _Ready()
    {
        RAMcoordLength = 1;
        blades = GetNode<Spatial>("MainMesh/Blades");   
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0,0);
    }

    public override void tickLogical(float delta)
    {
        //rotvel = Mathf.Lerp(rotvel, (readFromRam(0)==0 ? 0f : 1f), 0.05f );
        rotvel = Mathf.MoveToward(rotvel, (readFromRam(0)==0 ? 0f : 1f), delta );
    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
        blades.Rotation += new Vector3(0,0.2f,0) * rotvel;
    }

}
