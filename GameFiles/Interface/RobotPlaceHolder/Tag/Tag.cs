using Godot;
using System;

public class Tag : Control
{

    private Label[] labels;
    private Spatial robotparent;
    public override void _Ready()
    {
        labels = new Label[3]{
            GetNode<Label>("Name"),
            GetNode<Label>("XLabel"),
            GetNode<Label>("YLabel"),
        };
        robotparent = GetParent().GetParent().GetParent<Spatial>();
    }

    public void updateData(){
        labels[0].Text = robotparent.Name;
        labels[1].Text = "x: "+Convert.ToString(robotparent.Translation.x);
        labels[2].Text = "y: "+Convert.ToString(robotparent.Translation.z);
    }

}
