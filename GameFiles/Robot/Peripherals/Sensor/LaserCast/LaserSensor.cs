using Godot;
using System;

public class LaserSensor : Peripheral
{
    private class Laser{
        private RayCast rayCast;
        private Spatial laserMesh, redBeam, blueBeam;
        private float laserLength = 0f;
        public float LENGTH { get => laserLength; }
        public bool COLLIDING { get => redBeam.Visible; }
        public Laser(RayCast rc){
            rayCast = rc;
            laserMesh = rc.GetNode<Spatial>("LaserMesh");
            redBeam = laserMesh.GetNode<Spatial>("W/Red");
            blueBeam = laserMesh.GetNode<Spatial>("W/Blue");
        }

        public void process(){
            bool isColliding = rayCast.IsColliding();
            laserLength = (!isColliding ? rayCast.CastTo.x : rayCast.ToLocal(rayCast.GetCollisionPoint()).x) / rayCast.CastTo.x ;

            laserMesh.Scale = new Vector3(170 * laserLength,1,1) ;
            laserMesh.Translation = new Vector3(-12.75f * laserLength,0,0);

            redBeam.Visible = rayCast.IsColliding() && (rayCast.GetCollider() is Robot);
            blueBeam.Visible = !redBeam.Visible;

            // flicker effect
            float beamScaleOffset = (Global.FRAME % 4) * 0.15f;
            redBeam.Scale = new Vector3(1, 1 + beamScaleOffset, 1 + beamScaleOffset);
            blueBeam.Scale = redBeam.Scale;
        }

    }

    private Laser[] lasers = new Laser[2]; // Left and Right

    public override void _Ready()
    {
        RAMcoordLength = 3;
        base._Ready();
        lasers[0] = new Laser(GetNode<RayCast>("MainMesh/RCL"));
        lasers[1] = new Laser(GetNode<RayCast>("MainMesh/RCR"));
        writeToRam(0, 0b000);
        writeToRam(1, 0);
        writeToRam(2, 0);
    }

    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
        lasers[0].process();
        lasers[1].process();
    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
    }
}
