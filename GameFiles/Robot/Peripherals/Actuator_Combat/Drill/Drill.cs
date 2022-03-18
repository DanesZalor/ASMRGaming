using Godot;
using System;

public class Drill : Peripheral
{

    private Spatial drillShaft;
    private float rotvel = 0;

    private RayCast drillHitScan; private CPUParticles sparkParticles;

    public override void _Ready()
    {
        ram = new byte[1]{0};
        base._Ready();
        drillShaft = GetNode<Spatial>("MainMesh/DrillShaft");

        drillHitScan = GetNode<RayCast>("HitScan/RC1");
        sparkParticles = GetNode<CPUParticles>("HitScan/Sparks");
    }

    public override void tickLogical(float delta){
        rotvel = Mathf.MoveToward(rotvel, (ram[0]==0 ? 0f : 1f), delta );
        
        if(Global.FRAME%2==0 && drillHitScan.IsColliding()){
            GD.Print("colliding");
            Godot.Object hitbody = drillHitScan.GetCollider();
            if(isEnemy(hitbody))
                (hitbody as Robot).recieveDamage(2);
        }
    }
    public override void tickPresentational(float delta){
        drillShaft.Rotation += new Vector3(0.2f,0,0) * rotvel;
        sparkParticles.Emitting = drillHitScan.IsColliding();
    }
    
}
