using Godot;
using System;

public class Sparks : CPUParticles
{ 
    [Export(PropertyHint.Range, "1,2,")] private float atkSpeed = 1f;
    public AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        Emitting = false;
        Visible = true;
        SetAsToplevel(true);   
            
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.PlaybackSpeed = atkSpeed;
        SpeedScale = atkSpeed;
        
    }

    public void playImpact(Vector3 point, Godot.Collections.Array<Robot> damageRecievers, int damage=1){
        
        if(animationPlayer.IsPlaying()) return;
        
        Translation = point;
        animationPlayer.Play("Emit");
        
        foreach(Robot rbt in damageRecievers)
            rbt.recieveDamage(damage);
    }

    public void playImpact(Vector3 point, Robot damageReciever, int damage=1){
        playImpact(point, 
            new Godot.Collections.Array<Robot>(new Robot[1]{damageReciever}),
            damage
        );
    }
}
