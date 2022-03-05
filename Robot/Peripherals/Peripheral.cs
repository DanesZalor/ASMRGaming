using Godot;
using System;


/// <summary> Memory-mapped IO interface </summary>
public abstract class Peripheral : Spatial
{
    protected Robot parent; 
    protected byte[] ram;
    private byte RAMcoordStart = 255;     // starting coordinate 
    protected byte RAMcoordLength = 0;      // length of reading, set on _Ready() by inheritor
    private bool initialized = false;

    public byte SIZE{ get => RAMcoordLength; }


    /// <summary> should be called for Initialization. </summary>
    public virtual void Init(){
        initialized = true;
        parent = GetParent().GetParent<Robot>();

        // RAMcoordStart is where SP is right now
        RAMcoordStart = parent.CPU.getStackPointerValue();
        
        // subtract SP by length of the peripheral
        parent.CPU.setStackPointerValue( 
            System.Convert.ToByte(
                parent.CPU.getStackPointerValue() - RAMcoordLength
        ));
    }

    private bool addressInRange(byte address){
        return address < RAMcoordLength;
    }
    protected byte readFromRam(byte address){
        
        if(addressInRange(address))
            return parent.CPU.readFromRAM( (byte)(RAMcoordStart - address) );

        else{
            GD.Print("ERROR: out of bounds");
            throw new System.IndexOutOfRangeException();
            return 0;
        }
    }

    protected void writeToRam(byte address, byte data){
        if(addressInRange(address))
            parent.CPU.writeToRAM((byte)(RAMcoordStart - address),data);
        
        else{
            GD.Print("ERROR: out of bounds");
            throw new System.IndexOutOfRangeException();
        }
    }

    /// <summary> 
    public abstract void tickLogical(float delta);

    public abstract void tickPresentational(float delta);

    public void tick(float delta){
        if(!initialized) GD.Print("WARNING: Peripheral \""+Name+"\" not initialized");

        tickLogical(delta);
        tickPresentational(delta);
    }

}
