using Godot;
using System;

public class WindowsHandler : Control
{
    public static PackedScene WINDOW = GD.Load<PackedScene>("res://GameFiles/Interface/Window/WindowContainer.tscn");
    public static PackedScene NOTEPAD = GD.Load<PackedScene>("res://GameFiles/Interface/Window/Notepad/Notepad.tscn");
    public static PackedScene COMMAND_PROMPT = GD.Load<PackedScene>("res://GameFiles/Interface/Window/CommandPrompt/CommandPrompt.tscn");
    public override void _Ready()
    {
        OpenConsole();
    }

    public void OpenConsole(){
        Window win = WindowsHandler.WINDOW.Instance<Window>();
        CommandPrompt cmd = COMMAND_PROMPT.Instance<CommandPrompt>();

        win.setContent(cmd);
        AddChild(win);
    }
    public void RaiseWindow(int index){
        MoveChild(GetChild(index), GetChildCount()-1);
    }

}
