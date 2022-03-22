using Godot;
using System;

public class StartCLI : Control
{
    public static class SETTINGS{
        public static bool SHADOWS = true;
        public static Control controlparent;       
        public static Label[] Menu_Args;
        
        private static string editStringElement(string multiline, int idx, string newVal){
            
            string[] splitted = multiline.Split(new char[1]{'\n'}, StringSplitOptions.RemoveEmptyEntries);

            //GD.Print("["); for(int i = 0; i<splitted.Length; i++) GD.Print(splitted[i]); GD.Print("]");

            splitted[idx] = newVal;
            multiline = "";

            for(int i = 0; i<splitted.Length; i++)
                multiline += splitted[i] + "\n\n";
            return multiline;
        }
        static void setClockSpeed(int choice){
            Robot.CLOCKSPEED = (byte) Mathf.Clamp(choice, 1, 10);
            Menu_Args[0].Text = editStringElement(
                Menu_Args[0].Text, 0, 
                Convert.ToString(Robot.CLOCKSPEED*Engine.IterationsPerSecond) + " Hz" 
            );
        }
        static void setRobotMaxHP(int choice){
            Robot.MAXHP = (byte) Mathf.Clamp(choice,5,255);

            Menu_Args[0].Text = editStringElement(
                Menu_Args[0].Text, 1, 
                Convert.ToString(Robot.MAXHP) 
            );
        }
        static void setMaxRobots(int choice){
            RobotsHolder.MAXCHILD = Mathf.Clamp(choice, 2 , 10);

            Menu_Args[0].Text = editStringElement(
                Menu_Args[0].Text, 2,
                Convert.ToString(RobotsHolder.MAXCHILD)
            );
        }
        
        static void setResolution(int appendChoice){
            
            controlparent.GetTree().SetScreenStretch( SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Keep, new Vector2(
                Mathf.Clamp(controlparent.GetViewportRect().Size.x + (appendChoice * 320), 1280, 1920),
                Mathf.Clamp(controlparent.GetViewportRect().Size.y + (appendChoice * 180), 720, 1080)
            ));
            
            controlparent.RectScale = Vector2.One * (controlparent.GetViewportRect().Size.x / 1280f);

            Menu_Args[1].Text = editStringElement(
                Menu_Args[1].Text, 0,
                String.Format("{0}x{1}", controlparent.GetViewportRect().Size.x, controlparent.GetViewportRect().Size.y ) 
            ); 
            
        }
        static void setFullScreen(bool on){
            OS.WindowFullscreen = on;

            Menu_Args[1].Text = editStringElement(
                Menu_Args[1].Text, 1, OS.WindowFullscreen ? "ON" : "OFF"
            );
        }
        static void setMSAA( int appendChoice){
            int choice = Mathf.Clamp( 
                (int)controlparent.GetViewport().Msaa + appendChoice, 
                (int)Viewport.MSAA.Disabled, (int)Viewport.MSAA.Msaa16x
            );
            
            controlparent.GetViewport().Msaa = (Viewport.MSAA)choice;

            string[] msaaStr = { "Disabled", "MSAA 2x", "MSAA 4x", "MSAA 8x", "MSAA 16x"}; 
            Menu_Args[1].Text = editStringElement(
                Menu_Args[1].Text, 2, msaaStr[(int)controlparent.GetViewport().Msaa] 
            );
        }
        static void setFPS(int fps){
            Engine.TargetFps = Mathf.Clamp(fps, 30, 60);

            Menu_Args[1].Text = editStringElement(
                Menu_Args[1].Text, 3, Convert.ToString(Engine.TargetFps)
            );
        }
        static void setVsync(bool on){
            OS.VsyncEnabled = on;

            Menu_Args[1].Text = editStringElement(Menu_Args[1].Text, 4, OS.VsyncEnabled?"ON":"OFF");
        }
        static void setShadows(bool on){
            SHADOWS = on;

            Menu_Args[1].Text = editStringElement(Menu_Args[1].Text, 5, SHADOWS?"ON":"OFF");
        }


        public static void setGameSetting(int settingIdx, int choiceAppend){
            switch(settingIdx){
                case 1: 
                    setClockSpeed( Robot.CLOCKSPEED + choiceAppend); break;
                
                case 2:
                    setRobotMaxHP( Robot.MAXHP + choiceAppend * 10); break;
                
                case 3:
                    setMaxRobots( RobotsHolder.MAXCHILD + choiceAppend); break;
            }
        }

        public static void setSystemSetting(int settingIdx, int choiceAppend){
           switch(settingIdx){
                case 1: setResolution( choiceAppend ); break;
                case 2: setFullScreen(!OS.WindowFullscreen); break;
                case 3: setMSAA(choiceAppend); break;
                case 4: setFPS(Engine.TargetFps + (choiceAppend * 15)); break;
                case 5: setVsync(!OS.VsyncEnabled); break;
                case 6: setShadows(!SHADOWS); break;
           }
        }

        public static void LoadSettings(){
            SettingsFile.Load();
            string getData(string key, string fallback){
                if(SettingsFile.DATA == null) throw new Exception("SettingsFile.JSON is null");
                
                if(SettingsFile.DATA.Contains(key)) return (SettingsFile.DATA[key] as string);
                
                SettingsFile.DATA[key] = fallback;
                return fallback;

            }

            // load resolution
            float resX = (float)Convert.ToInt32( getData("Resolution", "1280") );
            controlparent.GetViewport().Size = new Vector2( resX, resX * 0.5625f );
            setResolution(0);
        
            
            setFullScreen( Convert.ToBoolean(getData("FullScreen", "false")) );
            
            controlparent.GetViewport().Msaa = (Viewport.MSAA)Convert.ToInt32(getData("MSAA", "0")); setMSAA(0);

            setFPS(Convert.ToInt32(getData("FPS","60")));

            setVsync( Convert.ToBoolean(getData("VSync", "false")) );

            setShadows( Convert.ToBoolean(getData("Shadows","true")));

        }

        public static void SaveSettings(){
            SettingsFile.DATA["Resolution"] = Convert.ToString(controlparent.RectSize.x);
            SettingsFile.DATA["FullScreen"] = Convert.ToString(OS.WindowFullscreen);
            SettingsFile.DATA["MSAA"] = Convert.ToString( (int)controlparent.GetViewport().Msaa );
            SettingsFile.DATA["FPS"] = Convert.ToString( Engine.TargetFps );
            SettingsFile.DATA["VSync"] = Convert.ToString( OS.VsyncEnabled );
            SettingsFile.DATA["Shadows"] = Convert.ToString( SHADOWS );
            SettingsFile.Save();
        }

        private static class SettingsFile{
            public const string path = "user://GameSettings.json";
            private static Godot.File file = new Godot.File();
            private static Godot.Collections.Dictionary data;
            public static Godot.Collections.Dictionary DATA { get => data; }

            private static string Read(){
                string r = "";
                if(file.FileExists(path)){
                    file.Open(path, Godot.File.ModeFlags.Read);
                    r = file.GetAsText();
                
                }else file.Open(path, Godot.File.ModeFlags.Write);

                file.Close(); return r;
            }
            private static void Write(string content){
                file.Open(path, Godot.File.ModeFlags.Write);
                file.StoreString(content);
                file.Close();
            }
            public static void Load(){
                Godot.JSONParseResult jparse = Godot.JSON.Parse(Read());
                
                if(jparse.Result == null) data = new Godot.Collections.Dictionary();
                else data = jparse.Result as Godot.Collections.Dictionary;
            }
            public static void Save(){ Write(JSON.Print(data)); }
        }
    }

    private AnimationPlayer animationPlayer;
    private bool IS_BOOTLOGO { get => animationPlayer.IsPlaying();}
    private Control[] menuContent;

    public override void _Ready()
    {
        
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GD.Print("wtf");
        menuContent = new Control[3]{
            GetNode<Control>("Menu/Content/Game"),
            GetNode<Control>("Menu/Content/Settings"),
            GetNode<Control>("Menu/Content/About")
        };

        SETTINGS.Menu_Args = new Label[2]{
            GetNode<Label>("Menu/Content/Game/Arguements"),
            GetNode<Label>("Menu/Content/Settings/Arguements"),
        };
        SETTINGS.controlparent = this;
        SETTINGS.LoadSettings();

        headerPressed(0);
    }

    private int currentHeaderIdx = 0; public void headerPressed(int idx){
        currentHeaderIdx = idx;

        for(int i=0; i<menuContent.Length; i++)
            menuContent[i].Visible = (i==idx);
    }

    int[] biosHLimit = {3, 6, 0}; int currH = 0; Vector2 arrows = Vector2.Zero;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        //if(IS_BOOTLOGO) return;
        GD.Print("startscreen event");

        if((@event is InputEventKey) && (@event as InputEventKey).Pressed){
            
            // Play Game
            if(Input.IsActionJustPressed("Enter")){
                GetNode<Label>("Menu/Play").Text = "Booting Game...";
                GetNode<Control>("Menu/Content").Visible = false;
                GetNode<Control>("Menu/Headers").Visible = false;
                
                playGame();
                return;
            }

            arrows = new Vector2( 
                ( 
                    ( Input.IsActionJustPressed("ArrowRight") ? 1 :0 ) - 
                    ( Input.IsActionJustPressed("ArrowLeft")  ? 1 :0 )
                ),(
                    ( Input.IsActionJustPressed("ArrowDown") ? 1 :0 ) - 
                    ( Input.IsActionJustPressed("ArrowUp")  ? 1 :0 )
                )                     
            );

            if(arrows.x != 0){ // left/right arrow keys
                
                if(currH==0) // at header
                    headerPressed( Global.ClampRound(currentHeaderIdx + (int)arrows.x,0,2) );

                else{
                    if(currentHeaderIdx==0) 
                        SETTINGS.setGameSetting(currH, (int)arrows.x);
                    
                    else if(currentHeaderIdx==1) 
                        SETTINGS.setSystemSetting(currH, (int)arrows.x);
                }
            }
            else if(arrows.y != 0){ // up/down arrow keys
                
                Control pointerRect = GetNode<Control>("Menu/Content/Pointer"); 
                
                currH = Global.ClampRound(currH + (int)arrows.y, 0, biosHLimit[currentHeaderIdx]);

                pointerRect.Visible = (currH!=0);
                pointerRect.RectPosition = new Vector2(390, 80 * currH );
            }

        }
    }

    private void playGame(){
        
        SETTINGS.SaveSettings();

        GetNode<AnimationPlayer>("AnimationPlayer").Play("Load");

        async void delayThenStart(){
            GD.Print(String.Format( "Settings:\n Resolution: {0}x{1}\n MSAA: {2}\n FPS: {3}",
                RectSize.x, RectSize.y, GetViewport().Msaa, Engine.TargetFps            
            ));
            await ToSignal(GetTree().CreateTimer(3f), "timeout");
            GetTree().ChangeScene("res://GameFiles/Interface/IDE/IDE.tscn");
        } delayThenStart();
    }
}
