using Godot;
using System;

public class Tank : Peripheral
{

    private MeshInstance Ltracks;
    private ShaderMaterial Ltracks_Mat;
    private MeshInstance Rtracks;
    private ShaderMaterial Rtracks_Mat;
    private Spatial[] Lwheels = new Spatial[3];
    private Spatial[] Rwheels = new Spatial[3]; 

    public override void _Ready()
    {
        base._Ready();
        RAMcoordLength = 2;
        
        /* Get Presentational Layer Init*/{
            Ltracks = GetNode<MeshInstance>("MainMesh/TracksL");
            Ltracks_Mat = (Ltracks.GetSurfaceMaterial(0) as ShaderMaterial);

            Rtracks = GetNode<MeshInstance>("MainMesh/TracksR");
            Rtracks_Mat = (Rtracks.GetSurfaceMaterial(0) as ShaderMaterial);

            Lwheels[0] = Ltracks.GetNode<Spatial>("WheelL1");
            Lwheels[1] = Ltracks.GetNode<Spatial>("WheelL2");
            Lwheels[2] = Ltracks.GetNode<Spatial>("WheelL3");

            Rwheels[0] = Rtracks.GetNode<Spatial>("WheelR1");
            Rwheels[1] = Rtracks.GetNode<Spatial>("WheelR2");
            Rwheels[2] = Rtracks.GetNode<Spatial>("WheelR3");
        }
        


    }

    public override void Init()
    {
        base.Init();
        writeToRam(0, 127);
        writeToRam(1, 127);
    }

    float lwp, rwp;
    public override void tickLogical(float delta){

        float lram = (float)(Godot.Mathf.Min(readFromRam(1),254));
        float rram = (float)(Godot.Mathf.Min(readFromRam(0),254));

        lwp = ( ((lram-127f) / 127f) * 0.25f ) * delta * 30f;
        rwp = ( ((rram-127f) / 127f) * 0.25f ) * delta * 30f;

        //GD.Print(String.Format("[254] = {0}\t[255] = {1}",lram, rram));

        parent.Rotation -= new Vector3(0f, lwp, 0f);
        Vector3 lvel = GlobalTransform.basis.z * lwp *3f;
        
        parent.Rotation += new Vector3(0f, rwp, 0f);
        Vector3 rvel = GlobalTransform.basis.z * rwp *3f;

        parent.MoveAndCollide(lvel + rvel);

    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
        float lwp2 = lwp + rwp * 0.7f;
        float rwp2 = rwp + lwp * 0.7f;

        
        for(int i = 0; i<3; i++){
            Lwheels[i].Rotation += Vector3.Right * lwp2 * 3f;
            Rwheels[i].Rotation -= Vector3.Right * rwp2 * 3f;
        }

        Ltracks_Mat.SetShaderParam("offset", 
            (Vector2)(Ltracks_Mat.GetShaderParam("offset")) + 
            Vector2.Right * lwp2 * 3f
        );  
        Rtracks_Mat.SetShaderParam("offset", 
            (Vector2)(Rtracks_Mat.GetShaderParam("offset")) + 
            Vector2.Right * rwp2 * 3f
        );  
            

    }
}
