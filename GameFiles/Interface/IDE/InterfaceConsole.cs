using Godot;
using System;


public class InterfaceConsole : Control
{
    private class Shell{
        private InterfaceConsole parent;
        public Shell(InterfaceConsole ic){ parent = ic;}
        private bool keyFileExists(string fileName){ // ASSUMING IDE.SaveFile.Load()ed
            return IDE.SaveFile.DATA.Contains(fileName);
        }
        private string create_KeyFile(string filename, string data=""){ // ASSUMING filename does not exist (it isn't in IDE.SaveFile.DATA) and IDE.SaveFile.Load()ed
            IDE.SaveFile.DATA.Add(filename,data);
            IDE.SaveFile.Save();
            return "";//"cat : \""+filename+"\" created";
        }
        private void delete_KeyFile(string filename){ // ASSUMING filename exist (it is in IDE.SaveFile.DATA) and IDE.SaveFile.Load()ed
            if(filename.Equals("*")) IDE.SaveFile.DATA.Clear();
            else IDE.SaveFile.DATA.Remove(filename);
            IDE.SaveFile.Save();
        }
        private string mv_or_cp_Command(string[] args, bool cp=false){ // ASSUMING it's used by MV IDE.SaveFile.Load()ed 
            bool[] exists = {keyFileExists(args[1]), keyFileExists(args[2])};
            
            
            if(exists[0] && !exists[1]){
                create_KeyFile(args[2], IDE.SaveFile.DATA[args[1]] as String);
                if(!cp) delete_KeyFile(args[1]);
                return "";
            }
            string s = "";
            if(!exists[0]) s += args[0]+": "+ args[1]+": no such file\n";
            if(exists[1])  s += args[0]+": "+ args[2]+": file exists" + (!exists[0]?"":"\n");
            return s;
        }
        private string rmCommand(string[] args){
            string s = "";
            for(int i = 1; i < args.Length; i++){
                if( !keyFileExists(args[i]) && !args[i].Equals("*") ) 
                    s += String.Format("rm: cannot remove \'{0}\': No such file\n", args[i]); 
                else
                    delete_KeyFile(args[i]);   
            }
            return s;
        }
        private string touchCommand(string[] args){
            string s = "";
            for(int i = 1; i<args.Length; i++){
                if(keyFileExists(args[i]))
                    s+= String.Format("touch: cannot create \'{0}\': file already exists\n", args[i]);
                else{
                    if(args[i].Equals("*")) return "touch: cannot create \'*\'";
                    else create_KeyFile(args[i]);
                }
            }
            return s;
        }
        private string editCommand(string[] args){
            string s = "";
            for(int i=1; i< args.Length; i++){
                
                if(!keyFileExists(args[i]))
                    s += String.Format("edit: \'{0}\': created since no such file existed\n",args[i]);
                openTextEditor(args[i]);
            }
            return s;
        }
        private string catCommand(string[] args){
            string s = "[color=#c0c0c0]";
            for(int i=1; i<args.Length; i++){
                if(keyFileExists(args[i])){
                    string content = IDE.SaveFile.DATA[args[i]] as String;
                    if(!content.EndsWith("\n") && i<args.Length-1) content += "\n";
                    s += content;
                }
                else s+= String.Format("cat: \'{0}\': No such file\n", args[i]);
            }
            return s + "[/color]";
        }
        private void openTextEditor(string filename){
            ASMRTextEditor te = WindowsHandler.TEXT_EDITOR_PRELOAD.Instance<ASMRTextEditor>();
            te.setFileName(filename);
            parent.ideparent.windowsHandler.AddChild(te);
        }
        public string interpretCommand(string[] args){
            IDE.SaveFile.Load();
    
            if(args[0].Equals("ls")){

                if(args.Length>1) return "[b]ls[/b] requires no arguements";

                string r = "";
                foreach(string s in IDE.SaveFile.DATA.Keys)
                    r += String.Format("[color=#c0c0c0]{0}[/color]\t", s);
                return r;
            }
            
            if(Global.match(args[0],"(touch|rm|edit|cat)")){
                if(args.Length<2)
                    return String.Format("{0}: needs atleast one filename arguement", args[0]);

                if(args[0].Equals("rm"))
                    return rmCommand(args);
                
                else if(args[0].Equals("touch"))
                    return touchCommand(args);
                
                else if(args[0].Equals("edit"))
                    return editCommand(args);

                else if(args[0].Equals("cat"))
                    return catCommand(args);
            }
            if (Global.match(args[0], "(mv|cp)")){
                if(args.Length!=3)
                    return String.Format("{0}: needs exactly 2 filename arguements", args[0]);
                
                return mv_or_cp_Command(args, args[0].Equals("cp"));
            }
            return "";
        }
    }
    private IDE ideparent;    
    private RichTextLabel logs;
    private LineEdit prompt;
    private Control titleBar;
    private Shell shell;

    private LinkedList<string> cmd_history;
    public override void _Ready()
    {  
        shell = new Shell(this);
        cmd_history = new LinkedList<string>();
        ideparent = GetParent<IDE>();
        logs = GetNode<RichTextLabel>("Logs");
        prompt = GetNode<LineEdit>("Prompt");
        titleBar = GetNode<Control>("BlackBG/TitleBar");
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }
    private string interpretCommand(string cmd){

        cmd = cmd.Trim().ToLower();
        
        string[] args = cmd.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
        
        if(args.Length==0) return "";
        
        else if(args.Length>=1 && args[0]=="bot")
            return ideparent.robots.interpretCommand(args);
        else if(args.Length>=1 && Global.match(args[0], "(ls|touch|cat|edit|rm|mv|cp)") )
            return shell.interpretCommand(args);

        else 
            return cmd + " : command not found";
    }
    // EVENTS
    /*Signal*/ public void onPromptEnteredSignal(String cmd){
        string feedback = interpretCommand(cmd);
        logs.BbcodeText += "[color=#8fff7f]>>[/color] " + cmd + "\n" + (feedback.Length > 0? (feedback + "\n") :"");
        prompt.Text = "";
        
        cmd_history.AddFirst(cmd);
        if(cmd_history.Count >= 10) cmd_history.RemoveLast();
        cmd_history_NodePointer = null;
    }

    private bool mousePressInTitleBar = false, mouseInTitleBar = false, mouseInLogs = false;

    /*Signal*/ public void _on_TitleBarMouseEnterOrExit(bool enter){ mouseInTitleBar = enter; }
    LinkedListNode<string> cmd_history_NodePointer;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if( (@event is InputEventMouseButton) ){
            
            InputEventMouseButton e = (@event as InputEventMouseButton);
            mousePressInTitleBar = e.Pressed && mouseInTitleBar;
        }

        else if(@event is InputEventMouseMotion && mousePressInTitleBar){
            
            InputEventMouseMotion e = (@event as InputEventMouseMotion);
            Vector2 screenSize = Global.SCREENSIZE;
            RectPosition += e.Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, screenSize.x - RectSize.x),
                Mathf.Clamp(RectPosition.y, 0, screenSize.y - RectSize.y)
            );
            
        }
        else if(@event is InputEventKey && prompt.HasFocus() && (@event as InputEventKey).Pressed ){
            
            bool[] arrow = {Input.IsActionJustPressed("ArrowUp"), Input.IsActionJustPressed("ArrowDown")};

            if(arrow[0] || arrow[1]){
                
                if(cmd_history_NodePointer==null) 
                    cmd_history_NodePointer = cmd_history.First;

                else if(arrow[0] && !arrow[1] && cmd_history_NodePointer.Next != null) // arrow up
                    cmd_history_NodePointer = cmd_history_NodePointer.Next;

                else if(arrow[1] && !arrow[0] ) // arrow down
                    cmd_history_NodePointer = cmd_history_NodePointer.Previous;
                
                if(cmd_history_NodePointer != null){
                    prompt.Text = cmd_history_NodePointer.Value;
                    
                    async void setCaretPositionLast(){
                        await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
                        prompt.CaretPosition = prompt.Text.Length;
                    } setCaretPositionLast();
                }
            }

        }
    }
    
}
