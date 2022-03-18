using Godot;
using System;

public class Drill : Peripheral
{

    private class RCHitBox{
        private RayCast[] rcs;
        private Godot.Object currentCollider;
        private Vector3 currentCollisionPoint;
        private bool colliding;

        public Godot.Object COLLIDER { get=> currentCollider;} 
        public Vector3 COLLISIONPOINT { get=> currentCollisionPoint;} 
        public bool COLLIDING { get=> colliding;}

        public RCHitBox(RayCast[] r){
            rcs = r;
        }

        public void tick(){
            currentCollisionPoint = Vector3.Zero;
            colliding = false;

            for(int i = 0; i<rcs.Length; i++){
                if(rcs[i].IsColliding()){
                    colliding = true;
                    currentCollider = rcs[i].GetCollider();
                    currentCollisionPoint = rcs[i].GetCollisionPoint();
                    return;
                } 
            }
        }
        
    }

    private Spatial drillShaft;
    private float rotvel = 0;
    private RCHitBox hitbox;

    private Sparks sparkParticles;

    public override void _Ready()
    {
        ram = new byte[1]{0};
        base._Ready();
        drillShaft = GetNode<Spatial>("MainMesh/DrillShaft");

        hitbox = new RCHitBox(new RayCast[3]{
            GetNode<RayCast>("HitScan/RC1"),
            GetNode<RayCast>("HitScan/RC2"),
            GetNode<RayCast>("HitScan/RC3"),
        });
        sparkParticles = GetNode<Sparks>("HitScan/Sparks");
    }


    public override void tickLogical(float delta){
        rotvel = Mathf.MoveToward(rotvel, (ram[0]==0 ? 0f : 1f), delta );
        hitbox.tick();

        if(Global.FRAME%2==0 && hitbox.COLLIDING){

            Godot.Object hitbody = hitbox.COLLIDER;
            if(isEnemy(hitbody))
                (hitbody as Robot).recieveDamage(2);
        }
    }
    public override void tickPresentational(float delta){
        drillShaft.Rotation += new Vector3(0.2f,0,0) * rotvel;
        sparkParticles.Emit( hitbox.COLLISIONPOINT );
    }
    
}
