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
        else{
            GD.Print(fileName + " not found");
            QueueFree();
        }
        ready = true;     
        
        textBox.GrabFocus();
    }

    public void setFileName(string s){
        if(!ready) fileName = s;
        else throw new Exception("Assigned ASMRTextEditor.fileName after _Ready()");
    }

    /// ------------    EVENTS---------
    private bool mouseIn = false, mousePress = false; 
    public void _on_TitleBar_mouse_entered(){
        mouseIn = true;
    }

    public void _on_TitleBar_mouse_exited(){
        mouseIn = false;
        mousePress = false;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if(mouseIn && @event is InputEventMouseButton)
            mousePress = (@event as InputEventMouseButton).Pressed;

        else if(@event is InputEventMouseMotion && mousePress){

            Vector2 screenSize = Global.SCREENSIZE; 
            RectPosition += (@event as InputEventMouseMotion).Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, screenSize.x - RectSize.x),
                Mathf.Clamp(RectPosition.y, 0, screenSize.y - RectSize.y)
            );
        }
    }

    public void _on_TextureButton_pressed(){
        IDE.SaveFile.DATA[fileName] = textBox.Text;
        IDE.SaveFile.Save();
        QueueFree();
    }
}
