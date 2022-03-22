using Godot;
using System;

public class Notepad : TextEdit
{
    private Window windowParent;
    public string fileName;
    public override void _Ready()
    {
        windowParent = GetParent().GetParent<Window>();
        IDE.SaveFile.Load();
        if(IDE.SaveFile.DATA.Contains(fileName)){
            Text = (IDE.SaveFile.DATA[fileName] as String);
            windowParent.setTitle("Textpad - " + fileName);
        }
        else throw new Exception(fileName + " not found");

        GrabFocus();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if((@event is InputEventKey) && (@event as InputEventKey).Pressed && HasFocus()){
            // puts an * if the text has changes unsaved
            if(Input.IsActionJustPressed("save")){
                IDE.SaveFile.DATA[fileName] = Text;
                IDE.SaveFile.Save();
                windowParent.setTitle("Textpad - " + fileName);
            }
            else
                windowParent.setTitle("Textpad - " + fileName + (!IDE.SaveFile.DATA[fileName].Equals(Text)?"*":""));
        }
    }

    public override void _ExitTree()
    {
        IDE.SaveFile.DATA[fileName] = Text;
        IDE.SaveFile.Save();
        QueueFree();
        base._ExitTree();
    }
}
