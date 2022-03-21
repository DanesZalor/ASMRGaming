using Godot;
using System;

public class StartCLI : Control
{
    private static class SETTINGS{
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
                Convert.ToString(Robot.CLOCKSPEED*Engine.TargetFps) + " Hz" 
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
            
            int currChoice = Mathf.Clamp( (int)((controlparent.RectSize.x / 1280f)*100), 100, 150);
            currChoice = (int)(Mathf.Round( (float)currChoice/5f) * 5f);
            currChoice = Mathf.Clamp(currChoice + appendChoice * 5, 100, 150);

            float tempf = (float)currChoice/100f;
            controlparent.RectSize = new Vector2(1280,720) * tempf;
            controlparent.RectScale = new Vector2(tempf, tempf);
            controlparent.GetTree().SetScreenStretch(
                        SceneTree.StretchMode.Mode2d,
                        SceneTree.StretchAspect.Keep, 
                        controlparent.RectSize, 1f);
            
            Menu_Args[1].Text = editStringElement(
                Menu_Args[1].Text, 0, 
                Convert.ToString((int)controlparent.RectSize.x)+"x"+Convert.ToString((int)controlparent.RectSize.y)
            ); 
        }
        static void setFullScreen(){
            OS.WindowFullscreen = !OS.WindowFullscreen;

            Menu_Args[1].Text = editStringElement(
                Menu_Args[1].Text, 1, OS.WindowFullscreen ? "ON" : "OFF"
            );
        }

        static void setMSAA( int appendChoice){
            //GD.Print((int)Viewport.MSAA.Msaa2x);
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
                case 2: setFullScreen(); break;
                case 3: setMSAA(choiceAppend); break;

           }
        }
    }

    private AnimationPlayer animationPlayer;
    private bool IS_BOOTLOGO { get => animationPlayer.IsPlaying();}
    private Control[] menuContent;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

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

        //Engine.TargetFps = 30;
        //OS.VsyncEnabled = false;
        //OS.WindowFullscreen = true;

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
        if(IS_BOOTLOGO) return;

        if((@event is InputEventKey) && (@event as InputEventKey).Pressed){
            
            // Play Game
            if(Input.IsActionJustPressed("Enter")){
                GetNode<Label>("Menu/Play").Text = "Booting Game...";
                GetNode<Control>("Menu/Content").Visible = false;
                GetNode<Control>("Menu/Headers").Visible = false;
                
                async void delayThenStart(){
                    await ToSignal(GetTree().CreateTimer(1f), "timeout");
                    GetTree().ChangeScene("res://GameFiles/Interface/IDE/IDE.tscn");
                }
                delayThenStart();
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

                //GD.Print(currH);
            }

        }
    }
}
