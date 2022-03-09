using Godot;
using System;

public class CamSensor : Peripheral
{
    private Spatial[] spotLights; // R & B
    private Area RadiusArea;
    public override void _Ready()
    {
        ram = new byte[1]{0};
        base._Ready();
        
        spotLights = new Spatial[2]{
            GetNode<Spatial>("MainMesh/Spotlight/R"),
            GetNode<Spatial>("MainMesh/Spotlight/B")
        };
        //bodies = new Spatial[0];
        RadiusArea = GetNode<Area>("RadiusArea");
    }

    private bool enemyDetected = false;
    public override void tickLogical(float delta)
    {
        enemyDetected = EnemyDetected();
    }

    public override void tickPresentational(float delta)
    {
        spotLights[0].Visible = enemyDetected;
        spotLights[1].Visible = !enemyDetected;
    }

    Godot.Collections.Array bodies;
    public void _on_RadiusArea_bodyEnteredOrExit(Node body){
        if(!(body is Robot) || body.Equals(parent) ) return;
        bodies = RadiusArea.GetOverlappingBodies();
    }

    public bool EnemyDetected(){
        //bool r = false;
        if(bodies != null){
            Vector3 nA = parent.GlobalTransform.basis.z;
            foreach(Spatial b in bodies){
                if(b.Equals(parent)) continue;
                Vector3 nB = (b.GlobalTransform.origin - parent.GlobalTransform.origin).Normalized(); 
                float theta = Mathf.Rad2Deg(
                    Mathf.Acos(nA.Dot(nB))
                );

                if(theta<30) return true;
            }
        }
        return false;
    }

}
