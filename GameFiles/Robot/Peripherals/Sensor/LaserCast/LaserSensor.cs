using Godot;
using System;

public class LaserSensor : Peripheral
{
    private class Laser{
        private RayCast rayCast;
        private Spatial laserMesh, redBeam, blueBeam;
        private float laserLength = 0f; private bool enemyDetected = false;
        public float LENGTH { get => laserLength; }
        public bool COLLIDING { get => redBeam.Visible; }
        public Laser(RayCast rc){
            rayCast = rc;
            laserMesh = rc.GetNode<Spatial>("LaserMesh");
            redBeam = laserMesh.GetNode<Spatial>("W/Red");
            blueBeam = laserMesh.GetNode<Spatial>("W/Blue");
        }

        public void tickLogical(bool on=true){
            if(!on) return;

            bool isColliding = rayCast.IsColliding();
            laserLength = (!isColliding ? rayCast.CastTo.x : rayCast.ToLocal(rayCast.GetCollisionPoint()).x) / rayCast.CastTo.x ;

            enemyDetected = isColliding && (rayCast.GetCollider() is Robot);
        }

        public void tickPresentational(bool on=true){
            laserMesh.Visible = on;

            laserMesh.Scale = new Vector3(170 * laserLength,1,1) ;
            laserMesh.Translation = new Vector3(-12.75f * laserLength,0,0);

            redBeam.Visible = enemyDetected;
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

        ram[0] = 0b000; // 3rd bit: if the peripheral is on/off. 2nd and 3rd are whether if the lasers are detecting an enemy (if red)
        ram[1] = 0;     // length of Left laser (0 if no enemy detected)
        ram[2] = 0;     // length of right laser (0 if no enemy detected)
    }

    public override void tickLogical(float delta)
    {
        for(byte i=0; i<2; i++){
            lasers[i].tickLogical((ram[0] & 0b100) > 0 );
            
            //GD.Print(lasers[i].LENGTH);
            //writeToRam(i+1, (lasers[i].COLLIDING ? lasers[i].LENGTH : 0) );
        }
    }

    public override void tickPresentational(float delta)
    {
        for(byte i=0; i<2; i++){
            lasers[i].tickPresentational(( ram[0] | 0b100) > 0);
        }
    }
}
