using Godot;
using System;

public class Chopper : Peripheral
{
    private Area hitArea; private Godot.Collections.Array<Robot> bodiesInRange = new Godot.Collections.Array<Robot>(); 
    private Vector3 impactPoint;
    /*Signal*/public void hitAreabodyEnteredExit(Node body){

        //if( !(body is Robot) || body.Equals(parent) ) return;

        bodiesInRange.Clear();
        impactPoint = Vector3.Zero;

        Godot.Collections.Array tempBodies = hitArea.GetOverlappingBodies();
        foreach(Node b in tempBodies){
            if(isEnemy(b) && !b.Equals(parent) )
                bodiesInRange.Add( (Robot)b );
        }

        if(bodiesInRange.Count>0)
            impactPoint = bodiesInRange[Global.RandInt(0, bodiesInRange.Count)].Translation;
    }
    private Sparks sparkParticles;
    private Spatial blades;
    private float rotvel = 0;
    public override void _Ready()
    {
        ram = new byte[1]{0};
        base._Ready();
        blades = GetNode<Spatial>("MainMesh/Blades"); 
        hitArea = GetNode<Area>("HitScan/Area");
        sparkParticles = GetNode<Sparks>("HitScan/Sparks");
        
    }

    public override void tickLogical(float delta)
    {
        rotvel = Mathf.MoveToward(rotvel, (ram[0]==0 ? 0f : 1f), delta );

        if(Global.FRAME%2==0 && bodiesInRange.Count>0){ // inflict damage
            for(int i = 0; i < bodiesInRange.Count<Robot>(); i++){
                
                if(bodiesInRange[i]!=null &&rotvel>0)
                    sparkParticles.playImpact(impactPoint, bodiesInRange[i], 2);
                    //bodiesInRange[i].recieveDamage(2);

                else // simulate area entered/exit to refresh the bodies array
                    hitAreabodyEnteredExit(null);
            }
        }
    }

    public override void tickPresentational(float delta)
    {
        blades.Rotation += new Vector3(0,0.2f,0) * rotvel;
    }

}
