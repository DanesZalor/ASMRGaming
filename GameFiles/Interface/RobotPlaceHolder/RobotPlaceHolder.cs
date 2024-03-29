using Godot;
using System;

public class RobotPlaceHolder : Spatial
{

    private static ShaderMaterial[] preloads = {
        GD.Load<ShaderMaterial>("res://GameFiles/Interface/RobotPlaceHolder/GreenTeam.tres"),
        GD.Load<ShaderMaterial>("res://GameFiles/Interface/RobotPlaceHolder/BlueTeam.tres")
    };


    [Export(PropertyHint.Enum, "Tank,Car")] public string steering_peripheral = "Tank";
    [Export(PropertyHint.Enum, "Drill,Chopper")] public string combat_peripheral = "Drill";
    [Export(PropertyHint.Enum, "Laser,Camera")] public string sensor_peripheral = "Laser";
    [Export(PropertyHint.Range, "0,1,")] public byte teamIdx = 0;
    [Export(PropertyHint.MultilineText)] public string program = ""; 
    private MeshInstance[] peripherals;
    public Tag robotTag;
    private Spatial hudGizmo;
    

    public override void _Ready()
    {
        peripherals = new MeshInstance[6]{
            GetNode<MeshInstance>("Parts/Tank"),
            GetNode<MeshInstance>("Parts/Car"),
            GetNode<MeshInstance>("Parts/Drill"),
            GetNode<MeshInstance>("Parts/Chopper"),
            GetNode<MeshInstance>("Parts/Laser"),
            GetNode<MeshInstance>("Parts/Camera")
        };
        updatePeripherals();
        updateTeam();
        hudGizmo = GetNode<Spatial>("Gizmo/HUD"); hudGizmo.SetAsToplevel(true); hudGizmo.Visible = false; hudGizmo.Rotation = Vector3.Zero;
        
        robotTag = GetNode<Tag>("3DNameTag/Viewport/Tag"); robotTag.updateData();
    }

    public void updateTeam(){

        teamIdx = (byte) Mathf.Clamp(teamIdx,0,1);
        
        foreach(MeshInstance m in peripherals) 
            m.SetSurfaceMaterial(0, preloads[teamIdx]);
    }

    public void updatePeripherals(){
        foreach(Spatial p in peripherals){
            
            p.Visible = false;
            if( p.Name.Equals(steering_peripheral) || 
                p.Name.Equals(combat_peripheral) || 
                p.Name.Equals(sensor_peripheral)
            ) p.Visible = true;
        }
    }

    
    bool mousePress = false; Vector3 rayOrigin, rayEnd, hitPoint;
    /*Signal*/ public override void _UnhandledInput(InputEvent @event){
        if( @event is InputEventMouseButton && 
            !(@event as InputEventMouseButton).Pressed
        ) mousePress = false;
    }
    /*Signal*/ public void _on_Area_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shape_idx){
        
        if(@event is InputEventMouseButton){
            InputEventMouseButton e = (@event as InputEventMouseButton);
            mousePress = e.Pressed;
                  
            if(e.ButtonIndex==4) RotationDegrees += Vector3.Up*5;
            else if(e.ButtonIndex==5) RotationDegrees -= Vector3.Up*5;
            
            RotationDegrees = new Vector3( 0, (int)Math.Round(RotationDegrees.y, 5), 0);
        }
        else if(@event is InputEventMouseMotion && mousePress){
            
            Vector2 mousePos = (@event as InputEventMouseMotion).Position;
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            
            rayOrigin = ((Camera)camera).ProjectRayOrigin(mousePos);
            rayEnd = rayOrigin + ((Camera)camera).ProjectRayNormal(mousePos) * 2000;

            Godot.Collections.Dictionary intersect = spaceState.IntersectRay(rayOrigin, rayEnd);

            if(intersect.Count>0){
                hitPoint = (Vector3)intersect["position"];
                Translation = new Vector3( 
                    Mathf.Clamp((int)hitPoint.x, -50, 50) ,0, Mathf.Clamp((int)hitPoint.z, -50, 50)) ;

                hudGizmo.Translation = Translation;
                robotTag.updateData();
            }
            
        }
        
        
    }

    /*Signal*/ public void AreaMouseEnterExit(bool enter){ 
        hudGizmo.Visible = enter;
    }
}
