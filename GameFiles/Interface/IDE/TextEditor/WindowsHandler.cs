using Godot;
using System;

public class WindowsHandler : Node
{
    public static PackedScene TEXT_EDITOR_PRELOAD = GD.Load<PackedScene>("res://GameFiles/Interface/IDE/TextEditor/TextEditor.tscn");
    public override void _Ready()
    {
        
    }

    private string create_KeyFile(string filename){ // assuming filename does not exist (it isn't in IDE.SaveFile.DATA)
        IDE.SaveFile.DATA.Add(filename,"");
        IDE.SaveFile.Save();
        return "";//"cat : \""+filename+"\" created";
    }

    private string delete_KeyFile(string filename){ // assuming filename exist (it is in IDE.SaveFile.DATA)
        IDE.SaveFile.DATA.Remove(filename);
        IDE.SaveFile.Save();
        return "";
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

            if(args.Length<2)
                return String.Format("{0} : requires an arguement", args[0]);

            else if( Global.match(args[0], "(touch|cat|edit|rm)") ){
                
                if(args.Length>2) return "[b]"+args[0]+"[/b] requires filename arguement";

                if( IDE.SaveFile.DATA.Contains(args[1]) ){ // keyfile exists
                    
                    if( args[0].Equals("touch") ) 
                        return "touch : \""+args[1]+"\" exists";
                    
                    else{ // args[0] == (cat|edit|rm)
                        
                        if( args[0].Equals("cat") ) 
                            return (IDE.SaveFile.DATA[args[1]] as String);
                        
                        else if(args[0].Equals("edit"))
                            return openTextEditor(args[1]);

                        else if(args[0].Equals("rm"))
                            return delete_KeyFile(args[1]);
                    }
                }
                else{ // keyfile does not exist

                    if( args[0].Equals("touch") )
                        return create_KeyFile(args[1]);

                }
            }
            //else if(args[0].Equals("cat"))

        }
        return "";
    }

}
