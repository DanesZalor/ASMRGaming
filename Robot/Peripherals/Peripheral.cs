using Godot;
using System;


/// <summary> Memory-mapped IO interface </summary>
public class Peripheral : Spatial
{
    protected Robot parent; 
    protected byte[] ram;
    private byte RAMcoordStart = 255;     // starting coordinate 
    protected byte RAMcoordLength = 0;      // length of reading, set on _Ready() by inheritor


    public virtual void Init(){
        parent = GetParent().GetParent<Robot>();

        // RAMcoordStart is where SP is right now
        RAMcoordStart = parent.CPU.getStackPointerValue();
        
        // subtract SP by length of the peripheral
        parent.CPU.setStackPointerValue( 
            System.Convert.ToByte(
                parent.CPU.getStackPointerValue() - RAMcoordLength
        ));
    }

    public byte SIZE{ get => RAMcoordLength; }

    private bool addressInRange(byte address){
        return address < RAMcoordLength;
    }
    protected byte readFromRam(byte address){
        
        if(addressInRange(address))
            return parent.CPU.readFromRAM( (byte)(RAMcoordStart - address) );

        else{
            GD.Print("ERROR: out of bounds");
            // some code to throw exception or some shit
            return 0;
        }
    }

    protected void writeToRam(byte address, byte data){
        if(addressInRange(address))
            parent.CPU.writeToRAM((byte)(RAMcoordStart - address),data);
        
        else{
            GD.Print("ERROR: out of bounds");
            // some code to throw exception or some shit
        }
    }

    public virtual void tick(float delta){}

}
