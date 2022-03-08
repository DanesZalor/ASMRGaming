using Godot;
using System;


/// <summary> Memory-mapped IO interface </summary>
public abstract class Peripheral : Spatial
{
    protected Robot parent; 
    protected byte[] ram;
    protected byte RAMcoordStart = 255;     // starting coordinate 
    protected byte RAMcoordLength = 0;      // length of reading, set on _Ready() by inheritor
    

    /// <summary> make sure to Set RAMCoordLength first before calling base._Ready()
    public override void _Ready(){
        parent = GetParent().GetParent<Robot>();

        // RAMcoordStart is where SP is right now
        RAMcoordStart = parent.CPU.getStackPointerValue();
        
        // subtract SP by length of the peripheral
        parent.CPU.setStackPointerValue( 
            System.Convert.ToByte(
                parent.CPU.getStackPointerValue() - RAMcoordLength
        ));

        ram = new byte[RAMcoordLength];
    }

    /// <summary> 
    public abstract void tickLogical(float delta);

    public abstract void tickPresentational(float delta);

    public void tick(float delta){
        copyRam();        
        tickLogical(delta);
        tickPresentational(delta);
        updateRam();
    }

    protected void copyRam(){
        if(ram.Length==0) return;
        for(byte i = (byte)ram.Length; i>0; i--) // read ram
            ram[i-1] = parent.CPU.readFromRAM( 
                (byte)((RAMcoordStart - RAMcoordLength) + i)
            );
    }

    protected void updateRam(){
        if(ram.Length==0) return;
        for(byte i = (byte)ram.Length; i>0; i--) // write ram
            parent.CPU.writeToRAM(
                (byte)((RAMcoordStart - RAMcoordLength) + i), ram[i-1]
            );
    }
}
