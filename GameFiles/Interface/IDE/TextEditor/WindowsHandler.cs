using Godot;
using System;

public class WindowsHandler : Control
{
    public static PackedScene TEXT_EDITOR_PRELOAD = GD.Load<PackedScene>("res://GameFiles/Interface/IDE/TextEditor/TextEditor.tscn");
    public static PackedScene WINDOW = GD.Load<PackedScene>("res://GameFiles/Interface/Window/WindowContainer.tscn");
    public static PackedScene NOTEPAD = GD.Load<PackedScene>("res://GameFiles/Interface/Window/Notepad/Notepad.tscn");
    
    public override void _Ready()
    {
        
    }
    public void RaiseWindow(int index){
        MoveChild(GetChild(index), GetChildCount()-1);
    }

}
