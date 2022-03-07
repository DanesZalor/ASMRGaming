using Godot;
using System;

public class Car : Peripheral
{
    private Spatial[] wheels; // {Rear, Left, Right}
    public override void _Ready()
    {
        RAMcoordLength = 2;
        wheels = new Spatial[3]{
            GetNode<Spatial>("MainMesh/WheelRear"),
            GetNode<Spatial>("MainMesh/WheelFrontL"),
            GetNode<Spatial>("MainMesh/WheelFrontR"),
        };
    }

    public override void Init()
    {
        base.Init();
        writeToRam(0, 127);
        writeToRam(1, 127);
    }

    float accel, steering;
    public override void tickLogical(float delta)
    {
        accel = (((float)readFromRam(1) - 127f) / 127f) * delta * 45f;
        steering = (((float)readFromRam(0) - 127f) / 127f)  ;

        parent.RotationDegrees -= Vector3.Up * steering * delta * 135f * accel;

        Vector3 vel = GlobalTransform.basis.z * delta * 15f * accel;
        parent.MoveAndCollide(vel);
    }

    public override void tickPresentational(float delta)
    {
        
        for(int i = 0; i<3; i++){
            // wheel spinning effect
            wheels[i].Rotation += Vector3.Back * accel * 0.3f;

            // wheel turning effect
            if(i>0)
                wheels[i].Rotation = Vector3.Down * steering * 0.35f;
        }

    }
}
