using Godot;
using System;

public class HPHud : ColorRect
{
    [Export] public int currentHP = Robot.MAXHP;
    [Export] public string name = "name"; 

    private ColorRect hpbar;
    private Label nameTag;
    public override void _Ready()
    {
        nameTag = GetNode<Label>("Name");
        hpbar = GetNode<ColorRect>("CurrentHP");
        
        updateData(currentHP, name);
    }

    public void updateData(int hp=-999, string n=""){
        if(hp>-999) currentHP = hp;
        if(n.Length>0) name = n;
        
        nameTag.Text = name;
        hpbar.RectSize = new Vector2( ( (float)currentHP/(Robot.MAXHP))*360, 30 );
    }


}
