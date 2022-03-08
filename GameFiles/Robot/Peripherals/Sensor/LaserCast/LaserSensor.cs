using Godot;
using System;

public class LaserSensor : Peripheral
{
    private class Laser{
        private RayCast rayCast;
        private Spatial laserMesh;
        private Spatial redBeam;
        private Spatial blueBeam;
        public Laser(RayCast rc){
            rayCast = rc;
            laserMesh = rc.GetNode<Spatial>("LaserMesh");
            redBeam = laserMesh.GetNode<Spatial>("Red");
            blueBeam = laserMesh.GetNode<Spatial>("Blue");
        }

        public void process(){
            bool isColliding = rayCast.IsColliding();
            float laserLength = isColliding ? rayCast.CastTo.x : 0f;

            redBeam.Visible = !blueBeam.Visible;
        }

    }
    public override void _Ready()
    {
        RAMcoordLength = 3;
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0, 0b000);
        writeToRam(1, 0);
        writeToRam(2, 0);
    }

    public override void tickLogical(float delta)
    {
        //throw new NotImplementedException();
    }

    public override void tickPresentational(float delta)
    {
        //throw new NotImplementedException();
    }
}
