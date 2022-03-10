using Godot;
using System;

public class CamSensor : Peripheral
{
    // Presentational stuff
    private Spatial[] spotLights; // R & B
    private Spatial spotLightParent;
    
    // Marker stuff
    private Spatial targetMarker; private Sprite3D targetMarkerSprite; private Spatial targetCircle;
    
    // Area stuff
    private Area RadiusArea;

    private float FOV = 30;

    private bool ON { get => (ram[0] & 0b100) > 0; }
    private bool DETECTED { get => (ram[0] & 0b10) > 0; }

    public override void _Ready()
    {
        ram = new byte[3]{
            0b000   /* [ ON/OFF, DETECTED/NO, LEFT/RIGHT ]*/, 
            0       /* Angle (Percentage of FOV)*/, 
            0       /* Range */
        };
        base._Ready();
        
        spotLightParent = GetNode<Spatial>("MainMesh/Spotlight");
        spotLights = new Spatial[2]{
            spotLightParent.GetNode<Spatial>("R"),
            spotLightParent.GetNode<Spatial>("B"),
        };

        RadiusArea = GetNode<Area>("RadiusArea");

        targetMarker = GetNode<Spatial>("TargetMarker");
        targetMarkerSprite = targetMarker.GetNode<Sprite3D>("Sprite3D");
        targetCircle = targetMarker.GetNode<Spatial>("Circle3D");
        targetMarker.SetAsToplevel(true);       
    }

    public override void tickLogical(float delta)
    {
        if(Global.FRAME % 3 == 0 && ON)
            updateEnemyDetected();
        
        if(nearestBody != null){
            Vector3 local = parent.ToLocal(nearestBody.GlobalTransform.origin);
            GD.Print(local.x);
        }
        
    }

    public override void tickPresentational(float delta)
    {
        spotLightParent.Visible = ON;
        spotLights[0].Visible = nearestBody != null;
        spotLights[1].Visible = !spotLights[0].Visible;
        targetCircle.Visible = spotLights[0].Visible;
        //targetCircle.Rotation += Vector3.Up;

        /*Target Marker*/{
            targetMarkerSprite.Opacity = Mathf.Lerp(
                targetMarkerSprite.Opacity, spotLights[0].Visible ? 1f : 0f, 0.2f
            );

            targetMarkerSprite.PixelSize = Mathf.Lerp(
                targetMarkerSprite.PixelSize, spotLights[0].Visible ? 0.015f : 0.04f, 0.2f  
            );

            if(spotLights[0].Visible){
                targetMarker.Translation = nearestBody.GlobalTransform.origin ;
            }
        }
        
    }

    Godot.Collections.Array bodies = new Godot.Collections.Array();
    public void _on_RadiusArea_bodyEnteredOrExit(Node body){

        if(!(body is Robot) || body.Equals(parent) ) return;
        
        //nearestBody = null;
        bodies.Clear();
        Godot.Collections.Array temp = RadiusArea.GetOverlappingBodies();
        foreach(Node b in temp){
            if((b is Robot) && !b.Equals(parent) ) 
                bodies.Add(b);
        }
        
    }

    public float getAngle(Spatial body){
        Vector3 nA = parent.GlobalTransform.basis.z;
        Vector3 nB = (body.GlobalTransform.origin - parent.GlobalTransform.origin).Normalized(); 
        
        return Mathf.Rad2Deg(
            Mathf.Acos(nA.Dot(nB))
        );
    }

    private Spatial nearestBody;
    public void updateEnemyDetected(){
        bool r = false;

        if(bodies != null ){
            
            float minDistance = float.MaxValue;
            Vector3 nA = parent.GlobalTransform.basis.z;

            foreach(Spatial b in bodies){
                if(b.Equals(parent)) continue;
                
                if( getAngle(b) <= FOV ){
                    
                    r = true;
                    
                    float distance = b.GlobalTransform.origin.DistanceSquaredTo(
                        parent.GlobalTransform.origin
                    );
                    if( distance < minDistance ){
                        minDistance = distance; nearestBody = b;
                    }
                }
            }
        }
        if(r==false) nearestBody = null;
        
        ram[0] |= (byte)(r ? 0b10: 0b0);
    }

}
