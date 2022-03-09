using Godot;
using System;

public class CamSensor : Peripheral
{
    private Spatial[] spotLights; // R & B
    private Spatial targetMarker;
    private Sprite3D targetMarkerSprite;
    private Area RadiusArea;
    private float FOV = 30;
    public override void _Ready()
    {
        ram = new byte[3]{0b0 /*Detected*/, 0 /*Range*/, 0 /*Angle (Percentage of FOV)*/};
        base._Ready();
        
        spotLights = new Spatial[2]{
            GetNode<Spatial>("MainMesh/Spotlight/R"),
            GetNode<Spatial>("MainMesh/Spotlight/B")
        };
        RadiusArea = GetNode<Area>("RadiusArea");
        targetMarker = GetNode<Spatial>("TargetMarker");
        targetMarkerSprite = GetNode<Sprite3D>("TargetMarker/Sprite3D");
        targetMarker.SetAsToplevel(true);
    }

    private bool enemyDetected = false;
    public override void tickLogical(float delta)
    {
        if(Global.FRAME % 3 == 0)
            enemyDetected = EnemyDetected();
        
            
        ram[0] = (byte)(enemyDetected ? 0b1: 0b0);
        
    }

    public override void tickPresentational(float delta)
    {
        spotLights[0].Visible = enemyDetected;
        spotLights[1].Visible = !enemyDetected;
        
        /*Target Marker*/{
            bool targetOn = nearestBody!=null;

            //targetMarker.Visible = targetOn;

            targetMarkerSprite.Opacity = Mathf.Lerp(
                targetMarkerSprite.Opacity, targetOn ? 1f : 0f, 0.2f
            );

            targetMarkerSprite.PixelSize = Mathf.Lerp(
                targetMarkerSprite.PixelSize, targetOn ? 0.015f : 0.04f, 0.2f  
            );

            if(targetOn) targetMarker.Translation = nearestBody.GlobalTransform.origin ;
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



    public bool inFOV(Spatial body){
        Vector3 nA = parent.GlobalTransform.basis.z;
        Vector3 nB = (body.GlobalTransform.origin - parent.GlobalTransform.origin).Normalized(); 
        
        return Mathf.Rad2Deg(
            Mathf.Acos(nA.Dot(nB))
        ) <= FOV;
    }

    private Spatial nearestBody;
    public bool EnemyDetected(){
        bool r = false;

        if(bodies != null){
            
            float minDistance = float.MaxValue;
            Vector3 nA = parent.GlobalTransform.basis.z;

            foreach(Spatial b in bodies){
                if(b.Equals(parent)) continue;
                if(inFOV(b)){
                    r = true;
                    float distance = b.GlobalTransform.origin.DistanceSquaredTo(parent.GlobalTransform.origin);
                    if(  distance < minDistance ){
                        minDistance = distance;
                        nearestBody = b;
                    }
                }
            }
        }
        if(r==false) nearestBody = null;
        return r;
    }

}
