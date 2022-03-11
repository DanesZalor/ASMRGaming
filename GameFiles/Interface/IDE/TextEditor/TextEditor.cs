using Godot;
using System;

public class TextEditor : ColorRect
{
    [Export]
    private string fileName = ""; public string FILE_NAME { get => fileName; }

    private Label titleLabel; private TextEdit textBox;
    private WindowsHandler parent;
    public override void _Ready()
    {
        parent = GetParent<WindowsHandler>();
        titleLabel = GetNode<Label>("TitleBar/Label");
        textBox = GetNode<TextEdit>("TextEdit");

        IDE.SaveFile.Load();
        if(IDE.SaveFile.DATA.Contains(fileName))
            textBox.Text = (IDE.SaveFile.DATA[fileName] as String);
        else{
            GD.Print(fileName + " not found");
            QueueFree();
        }
            
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
            RectPosition += (@event as InputEventMouseMotion).Relative; 
            RectPosition = new Vector2(
                Mathf.Clamp(RectPosition.x, 0, 860),
                Mathf.Clamp(RectPosition.y, 0, 300)
            );
        }
    }

    public void _on_TextureButton_pressed(){
        IDE.SaveFile.DATA[fileName] = textBox.Text;
        IDE.SaveFile.Save();
        QueueFree();
    }
}
