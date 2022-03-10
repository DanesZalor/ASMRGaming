using Godot;
using System;

public class EnumSelector : Control
{
    [Export]
    private string[] choices = new string[2]{"a","b"};
    private int index = 0;

    private Label label;

    public override void _Ready()
    {
        if(choices==null || choices.Length < 2) throw new Exception("EnumSelector must have atleast 2 choices");
        label = GetNode<Label>("Label");
        label.Text = choices[index];
    }


    public void updateSelected(bool right=true){
        
        index = Mathf.Abs((index + (right?1:-1)) % choices.Length);
        label.Text = choices[index];
    }
    public void _on_Lbtn_pressed(){
        updateSelected(false);
    }

    public void _on_Rbtn_pressed(){
        updateSelected(true);
    }

    public string getChoice(){
        return choices[index];
    }

}
