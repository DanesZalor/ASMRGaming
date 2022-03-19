using Godot;
using System;

public class ASMRTextEditor : ColorRect
{
    [Export]
    private string fileName = "";

    private Label titleLabel; private TextEdit textBox;
    private WindowsHandler parent;

    private bool ready = false;
    public override void _Ready()
    {
        parent = GetParent<WindowsHandler>();
        titleLabel = GetNode<Label>("TitleBar/Label");
        textBox = GetNode<TextEdit>("TextEdit");

        IDE.SaveFile.Load();
        if(IDE.SaveFile.DATA.Contains(fileName)){
            textBox.Text = (IDE.SaveFile.DATA[fileName] as String);
            titleLabel.Text = "Textpad - " + fileName;
        }
        else
            throw new Exception(fileName + " not found");
        
        ready = true;     
        
        textBox.GrabFocus();
        /*random window placing*/{
            Vector2 windowSize = Global.SCREENSIZE;
            int offset = (20*parent.GetChildCount());
            RectPosition = new Vector2(
                windowSize.x - (430 + offset), 10 + offset
            );
        }
    }

    public void setFileName(string s){
        if(!ready) fileName = s;
        else throw new Exception("Assigned ASMRTextEditor.fileName after _Ready()");
    }

    /// ------------    EVENTS---------
    private bool mouseIn = false, mousePress = false; 
    public void _on_TitleBarMouseEnterOrExit(bool enter){ mouseIn = enter; }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if(mouseIn && @event is InputEventMouseButton){
            mousePress = (@event as InputEventMouseButton).Pressed;
            parent.RaiseWindow(GetIndex());
        }

        else if(@event is InputEventMouseMotion && mousePress){

            Vector2 screenSize = Global.SCREENSIZE; 
            RectPosition += (@event as InputEventMouseMotion).Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, screenSize.x - RectSize.x),
                Mathf.Clamp(RectPosition.y, 0, screenSize.y - RectSize.y)
            );
        }

        else if((@event is InputEventKey) && (@event as InputEventKey).Pressed && textBox.HasFocus()){
            // puts an * if the text has changes unsaved
            if(Input.IsActionJustPressed("save")){
                IDE.SaveFile.DATA[fileName] = textBox.Text;
                IDE.SaveFile.Save();
                titleLabel.Text = "Textpad - " + fileName;
            }else{
                titleLabel.Text = "Textpad - " + fileName + (!IDE.SaveFile.DATA[fileName].Equals(textBox.Text)?"*":"");
            }
        }
    }

    /*Signal - Exit button*/ public void onExitBtnPressed(){
        IDE.SaveFile.DATA[fileName] = textBox.Text;
        IDE.SaveFile.Save();
        QueueFree();
    }
    /*Signal*/ public void _on_TextEdit_focus_entered(){
        parent.RaiseWindow(GetIndex());
    }
}
