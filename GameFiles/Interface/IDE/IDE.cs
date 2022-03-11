using Godot;
using System;


public class IDE : Node
{
    /// <summary> access DATA to get data. make sure to call Load() first before anything and Save() after using </summary>
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
            if(jparse.Result == null)
                data = new Godot.Collections.Dictionary();
            else
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

    public void moveCamera(){
        if(Global.FRAME%3==0) return;

        Vector2 ave = Vector2.Zero;
        if(robots.GetChildCount()>=1){
            foreach(Spatial b in robots.GetChildren()){
                ave.x += b.GlobalTransform.origin.x;
                ave.y += b.GlobalTransform.origin.z;
            }
            
            ave /= robots.GetChildCount();
        } 

        camHolder.Translation = new Vector3(
            Mathf.Lerp(camHolder.Translation.x, ave.x, 0.2f), 0,
            Mathf.Lerp(camHolder.Translation.z, ave.y, 0.2f)
        );
    } 

    public override void _PhysicsProcess(float delta){
        moveCamera();
    }
}
