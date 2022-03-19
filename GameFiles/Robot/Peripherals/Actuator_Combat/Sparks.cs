using Godot;
using System;

public class Sparks : CPUParticles
{ 
    [Export(PropertyHint.Range, "1,2,")] private float atkSpeed = 1f;
    public AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        Visible = true;
        SetAsToplevel(true);       
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.PlaybackSpeed = atkSpeed;
        SpeedScale = atkSpeed;
        Emitting = false;
    }

    public void playImpact(Vector3 point, Robot damageReciever, int damage=1){
        
        if(animationPlayer.IsPlaying()) return;
        
        Translation = point;
        animationPlayer.Play("Emit");
        damageReciever.recieveDamage(damage);
    }
}
