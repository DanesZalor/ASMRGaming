using Godot;
using System;


/// <summary> Memory-mapped IO interface </summary>
public class Peripheral : Spatial
{
    protected byte RAMcoordStart = 255;     // starting coordinate 
    protected byte RAMcoordLength = 0;    // length of reading
    public override void _Ready()
    {}

    public void Init(byte ramCoord){
        RAMcoordStart = ramCoord;
    }

    protected CPU.CPU cpuref; public void setCPU(CPU.CPU c){ cpuref = c;}

    public byte SIZE{ get => RAMcoordLength; }

    protected bool addressInRange(byte address){
        return 
            (address <= RAMcoordStart) && 
            (address > (RAMcoordStart-RAMcoordLength));
    }
    protected byte readFromRam(byte address){
        
        if(addressInRange(address))
            return cpuref.readFromRAM(address);

        else{
            GD.Print("ERROR: out of bounds");
            // some code to throw exception or some shit
            return 0;
        }
    }

    protected void writeToRam(byte address, byte data){
        if(addressInRange(address))
            cpuref.writeToRAM(address,data);
        
        else{
            GD.Print("ERROR: out of bounds");
            // some code to throw exception or some shit
        }
    }

}
