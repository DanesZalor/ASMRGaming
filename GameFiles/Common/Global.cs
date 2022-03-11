using Godot;
using System;

public class Global : Node
{
    private static byte curr_frame = 0; public static byte FRAME { get => curr_frame;}
    
    private static SceneTree worldTree; public static SceneTree WORLDTREE { get => worldTree; }
    public static Vector2 SCREENSIZE { get => WORLDTREE.Root.GetViewport().GetVisibleRect().Size; }


    public static bool match(string line, string grammar, bool exact=true){
        return Assembler.Common.match(line,grammar,exact);
    }

    public static bool isIn(Control c, Vector2 mousePos){
        return ( // within Control arg 
            (c.RectGlobalPosition.x < mousePos.x) && (c.RectGlobalPosition.y < mousePos.y) &&
            (mousePos.x < c.RectGlobalPosition.x + c.RectSize.x) && 
            (mousePos.y < c.RectGlobalPosition.y + c.RectSize.y)
        );
    }

    public static bool isOnScreen(Vector2 mousePos){
        return (
            (0 <= mousePos.x) && (0 <= mousePos.y) &&
            (mousePos.x <= OS.WindowSize.x) && (mousePos.y <= OS.WindowSize.y)
        );
    }

    public override void _Ready()
    {
        worldTree = GetTree();
    }


    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        curr_frame = (byte)( (curr_frame + 1) % 60 );
    }
}
