using Godot;
using System;


/// <summary> Memory-mapped IO interface </summary>
public class Peripheral : Spatial
{
    protected Spatial parent; 
    protected byte RAMcoordStart = 255;     // starting coordinate 
    protected byte RAMcoordLength = 0;    // length of reading
    public override void _Ready()
    {
        //parent = GetParent<Spatial>().GetParent<Spatial>();
    }

    public void Init(byte ramCoord){
        RAMcoordStart = ramCoord;
    }

    protected CPU.CPU cpuref; public virtual void setCPU(CPU.CPU c, Spatial p){ 
        cpuref = c;
        parent = p;
    }

    public byte SIZE{ get => RAMcoordLength; }

    protected bool addressInRange(byte address){
        return address < RAMcoordLength;
    }
    protected byte readFromRam(byte address){
        
        if(addressInRange(address))
            return cpuref.readFromRAM( (byte)(RAMcoordStart - address) );

        else{
            GD.Print("ERROR: out of bounds");
            // some code to throw exception or some shit
            return 0;
        }
    }

    protected void writeToRam(byte address, byte data){
        if(addressInRange(address))
            cpuref.writeToRAM((byte)(RAMcoordStart - address),data);
        
        else{
            GD.Print("ERROR: out of bounds");
            // some code to throw exception or some shit
        }
    }

    public virtual void Steer(float delta){}

}
