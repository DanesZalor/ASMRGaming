using Godot;
using System;

public class IDE : Node
{
    
    private InterfaceConsole console;
    public RobotsHolder robots;
    private Spatial camHolder; private Camera cam;

    public override void _Ready()
    {
        base._Ready();
        camHolder = GetNode<Spatial>("EnvironmentStuff/CamHolder");
        cam = camHolder.GetNode<Camera>("Camera");

        robots = GetNode<RobotsHolder>("Robots");
        console = GetNode<InterfaceConsole>("Console");
    }

}
