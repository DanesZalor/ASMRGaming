using Godot;
using System;

public class WindowsHandler : Node
{
    public static PackedScene TEXT_EDITOR_PRELOAD = GD.Load<PackedScene>("res://GameFiles/Interface/IDE/TextEditor/TextEditor.tscn");
    public override void _Ready()
    {
        
    }

    public string interpretCommand(string[] args){
        IDE.SaveFile.Load();
        if(args.Length>=1){
            if(args[0].Equals("ls")){
                string r = "";
                foreach(string s in IDE.SaveFile.DATA.Keys)
                    r += String.Format("[color=#c0c0c0]{0}[/color]\t", s);
                return r;
            }
        }
        return "";
    }

}
