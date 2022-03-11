using Godot;
using System;

public class WindowsHandler : Node
{
    public static PackedScene TEXT_EDITOR_PRELOAD = GD.Load<PackedScene>("res://GameFiles/Interface/IDE/TextEditor/TextEditor.tscn");
    public override void _Ready()
    {
        
    }

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

    private string rename_KeyFile(string filename, string newname){ // ASSUMING it's used by MV IDE.SaveFile.Load()ed 
        bool[] exists = {keyFileExists(filename), keyFileExists(newname)};
        
        if(exists[0] && !exists[1]){
            create_KeyFile(newname, IDE.SaveFile.DATA[filename] as String);
            delete_KeyFile(filename);
        }
        else if(!exists[0]) return "mv: "+ filename+": no such file";
        else if(exists[1]) return  "mv: "+ filename+": file exists";
        return "";
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
                if(!content.EndsWith("\n")) content += "\n";
                s += content;
            }
            else s+= String.Format("cat: \'{0}\': No such file\n", args[i]);
        }
        return s + "[/color]";
    }

    private void openTextEditor(string filename){
        ASMRTextEditor te = TEXT_EDITOR_PRELOAD.Instance<ASMRTextEditor>();
        te.setFileName(filename);
        AddChild(te);
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
        
        if(Global.match(args[0],"(touch|rm|edit|cat)") && args.Length<2) 
            return String.Format("{0}: needs atleast one filename arguement", args[0]);

        if(args[0].Equals("rm"))
            return rmCommand(args);
        
        else if(args[0].Equals("touch"))
            return touchCommand(args);
        
        else if(args[0].Equals("edit"))
            return editCommand(args);

        else if(args[0].Equals("cat"))
            return catCommand(args);
        
        return "";
    }

}
