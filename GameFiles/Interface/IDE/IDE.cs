using Godot;
using System;


public class IDE : Node
{
    public static class SaveFile{
        public const string path = "user://saveFile.json";
        private static Godot.File file = new Godot.File();
        
        /// <summary> read file as string </summary>
        public static string Load(){
            string r = "";
            if(file.FileExists(path)){
                file.Open(path, Godot.File.ModeFlags.Read);
                r = file.GetAsText();
            
            }else
                file.Open(path, Godot.File.ModeFlags.Write);

            file.Close();
            return r;
        }

        public static Godot.Collections.Dictionary LoadDict(){
            Godot.JSONParseResult jparse = Godot.JSON.Parse(Load());
            return jparse.Result as Godot.Collections.Dictionary;
        }

        public static void Save(string content){
            file.Open(path, Godot.File.ModeFlags.Write);
            file.StoreString(content);
            file.Close();
        }

        public static void Save(Godot.Collections.Dictionary<string,string> dict){
            Save(JSON.Print(dict));
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
