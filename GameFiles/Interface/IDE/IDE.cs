using Godot;
using System;


public class IDE : Node
{
    public enum STATE : byte 
        { SETUP, PLAYING, PAUSED } 
        private STATE CurrentState = STATE.SETUP;
    
    private string StateCommand(string[] args){ // (play|pause|reset)
        
        STATE targetState = STATE.SETUP;
        if( args[0].Equals("play") ) targetState = STATE.PLAYING;
        else if( args[0].Equals("pause") ) targetState = STATE.PAUSED;
        else if( args[0].Equals("(reset|setup)") ) targetState = STATE.SETUP;

        //GD.Print(CurrentState+" -> "+targetState);

        if(targetState==STATE.PAUSED && CurrentState==STATE.SETUP)
                return "IDE: state is not running";
        else if(targetState==CurrentState)
                return "IDE: already in \'"+targetState+"\'";

        string r = "";
        // SETUP -> PLAYING
        if( CurrentState==STATE.SETUP && targetState==STATE.PLAYING ){
            
            // some init code shit, replace the robotplaceholders with robots, compile their programs, etc
            r = robots.setUp();
        }

        // PLAYING|PAUSED -> SETUP
        else if ( ( CurrentState==STATE.PLAYING || CurrentState==STATE.PAUSED ) && targetState==STATE.SETUP ){
            // some code to reset the field
            r = robots.deSetup();
        }

        else if( 
            (CurrentState==STATE.PLAYING && targetState==STATE.PAUSED) ||
            (CurrentState==STATE.PAUSED && targetState==STATE.PLAYING)
         ){
             // some code to toggle between playing and paused
             // actually dont need to do anything here lmao since the pause/play is handled by the _PhysicsProcess()
            
            r = "ide: "+(targetState==STATE.PAUSED?"paused":"played"); 
        }

        CurrentState = targetState;    
        return r;
    }

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
    public WindowsHandler windowsHandler;
    private Spatial camHolder; private Camera cam;
    private Shell shell;

    public override void _Ready()
    {
        base._Ready();
        camHolder = GetNode<Spatial>("EnvironmentStuff/CamHolder");
        cam = camHolder.GetNode<Camera>("Camera");
        robots = GetNode<RobotsHolder>("Robots");
        windowsHandler = GetNode<WindowsHandler>("WindowsHandler");
        console = GetNode<InterfaceConsole>("Console");
        shell = new Shell(this);
    }

    private void moveCamera(){
        if(Global.FRAME%3==0) return;

        Vector2 ave = Vector2.Zero;
        if(robots.GetChildCount()>=1){
            
            Vector2 min = new Vector2(int.MaxValue, int.MaxValue); 
            Vector2 max = new Vector2(int.MinValue, int.MinValue);

            foreach(Spatial b in robots.GetChildren()){
                ave.x += b.GlobalTransform.origin.x;
                ave.y += b.GlobalTransform.origin.z;

                min = new Vector2(
                    Mathf.Min(min.x, b.GlobalTransform.origin.x),
                    Mathf.Min(min.y, b.GlobalTransform.origin.z)
                );
                max = new Vector2(
                    Mathf.Max(max.x, b.GlobalTransform.origin.x),
                    Mathf.Max(max.y, b.GlobalTransform.origin.z)
                );
            }

            float maxdiff = Mathf.Max(
                Mathf.Max( max.x-min.x, (max.y-min.y)*1.2f )
            ,40);
            cam.Size = Mathf.Lerp(cam.Size, 30 * (maxdiff/40f), 0.1f);
            
            ave /= robots.GetChildCount();
        } 

        camHolder.Translation = new Vector3(
            Mathf.Lerp(camHolder.Translation.x, ave.x, 0.1f), 0,
            Mathf.Lerp(camHolder.Translation.z, ave.y, 0.1f)
        );
    }

    public string interpretCommand(string[] args){
        if(args.Length==0) return "";
        
        else if( Global.match(args[0], "(ls|touch|cat|edit|rm|mv|cp)") )
            return shell.interpretCommand(args);
        
        else if( Global.match(args[0], "(play|pause|(reset|setup))"))
            return StateCommand(args);

        else if( Global.match(args[0],"(bot|asm)") ){

            if(CurrentState==STATE.SETUP)    
                return robots.interpretCommand(args, CurrentState);
            else
                return String.Format("{0} : cannot be used on current state", args[0]);
        }
        else 
            return args[0] + " : command not found";
    }

    public override void _PhysicsProcess(float delta){
        moveCamera();
        if(CurrentState==STATE.PLAYING){
            robots.tick(delta);
        }
    }
}
