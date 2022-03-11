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
        IDE.SaveFile.DATA.Remove(filename);
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
            if(!keyFileExists(args[i])) 
                s += String.Format("rm: cannot remove \'{0}\': No such file\n", args[i]); // BUGGY does not show for more than one line
            else
                delete_KeyFile(args[i]);   
        }
        return s;
    }

    private string openTextEditor(string filename){
        ASMRTextEditor te = TEXT_EDITOR_PRELOAD.Instance<ASMRTextEditor>();
        te.setFileName(filename);
        AddChild(te);
        return "";
    }

    public string interpretCommand(string[] args){
        IDE.SaveFile.Load();
        if(args.Length>=1){
            
            if(args[0].Equals("ls")){

                if(args.Length>1) return "[b]ls[/b] requires no arguements";

                string r = "";
                foreach(string s in IDE.SaveFile.DATA.Keys)
                    r += String.Format("[color=#c0c0c0]{0}[/color]\t", s);
                return r;
            }

            //if(args.Length<2) return String.Format("{0} : requires an arguement", args[0]);

            else if( Global.match(args[0], "(touch|cat|edit|rm)") ){
                
                if(args.Length<2) return "[b]"+args[0]+"[/b] requires at least 1 filename arguement";

                if( keyFileExists(args[1]) ){ // keyfile exists
                    
                    if( args[0].Equals("touch") ) 
                        return "touch : \""+args[1]+"\" exists";
                    
                    else{ // args[0] == (cat|edit|rm)
                        
                        if( args[0].Equals("cat") ) 
                            return (IDE.SaveFile.DATA[args[1]] as String);
                        
                        else if(args[0].Equals("edit"))
                            return openTextEditor(args[1]);

                        else if(args[0].Equals("rm"))
                            return rmCommand(args);
                    }
                }
                else{ // keyfile does not exist

                    if( args[0].Equals("touch") )
                        return create_KeyFile(args[1]);
                    
                    else // args[0] == cat|edit|rm
                        return String.Format("[b]{0}[/b] : {1} : no such file", args[0], args[1]);

                }
            }
            else if( args[0].Equals("mv")){

                if(args.Length!=3) return "mv : requires 2 filename arguements";
                else return rename_KeyFile(args[1], args[2]);

            }

        }
        return "";
    }

}
