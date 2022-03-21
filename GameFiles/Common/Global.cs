using Godot;
using System;

public class Global : Node
{
    private static byte curr_frame = 0; public static byte FRAME { get => curr_frame;}
    
    private static Global self;

    public static Vector2 SCREENSIZE { get => self.GetTree().Root.GetViewport().GetVisibleRect().Size; }


    public static int ClampRound(int val, int min, int max){
        
        if(val<min) return max;
        else if(val>max) return min;

        return val;
    }
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

    private static Random random;
    public static int RandInt(int min=int.MinValue, int max=int.MaxValue){
        return random.Next(min,max);
    }

    public override void _Ready()
    {
        random = new Random();
        self = this;
        Engine.TargetFps = 30;
    }


    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        curr_frame = (byte)( (curr_frame + 1) % 60 );
    }
}
