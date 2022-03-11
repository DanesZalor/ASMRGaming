using Godot;
using System;


public class IDE : Node
{
    public static class SaveFile{
        public const string path = "user://saveFile.json";
        private static Godot.File file = new Godot.File();
        private static Godot.Collections.Dictionary data; 
        public static Godot.Collections.Dictionary DATA { get => data; }

        private static string Read(){
            string r = "";
            if(file.FileExists(path)){
                file.Open(path, Godot.File.ModeFlags.Read);
                r = file.GetAsText();
            
            }else
                file.Open(path, Godot.File.ModeFlags.Write);

            file.Close();
            return r;
        }
        private static void Write(string content){
            file.Open(path, Godot.File.ModeFlags.Write);
            file.StoreString(content);
            file.Close();
        }
        public static void Load(){
            Godot.JSONParseResult jparse = Godot.JSON.Parse(Read());
            data = jparse.Result as Godot.Collections.Dictionary;
        }
        public static void Save(){
            Write(JSON.Print(data));
        }
    }
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
