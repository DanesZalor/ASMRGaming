using Godot;
using System;


/// <summary> Memory-mapped IO interface </summary>
public abstract class Peripheral : Spatial
{
    protected Robot parent; 
    protected byte[] ram;
    protected byte RAMcoordStart = 255;     // starting coordinate 
    

    /// <summary> make sure ram[] is initialized before calling base._Ready()
    public override void _Ready(){
        if(ram == null) 
            throw new NullReferenceException(
                "Peripheral.ram==NULL. Set ram on Subclass _Ready() before calling base._Ready()"
            );

        parent = GetParent().GetParent<Robot>();

        // RAMcoordStart is where SP is right now
        RAMcoordStart = parent.CPU.getStackPointerValue();
        
        // subtract SP by length of the peripheral
        parent.CPU.setStackPointerValue( 
            System.Convert.ToByte(
                parent.CPU.getStackPointerValue() - ram.Length
        ));

        updateRam();
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

    private void copyRam(){
        if(ram.Length==0) return;
        for(byte i = (byte)ram.Length; i>0; i--) // read ram
            ram[i-1] = parent.CPU.readFromRAM( 
                (byte)((RAMcoordStart - ram.Length) + i)
            );
    }

    private void updateRam(){
        if(ram.Length==0) return;
        for(byte i = (byte)ram.Length; i>0; i--) // write ram
            parent.CPU.writeToRAM(
                (byte)((RAMcoordStart - ram.Length) + i), ram[i-1]
            );
    }
}
