using Godot;
using System;
public class Shell{
    private IDE ide;
    public Shell(IDE i){ ide = i;}
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
            
            if(!keyFileExists(args[i])){
                create_KeyFile(args[i]);
                s += String.Format("edit: \'{0}\': created since no such file existed\n",args[i]);
            }
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
        ide.windowsHandler.AddChild(te);
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