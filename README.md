# Assembly Robot Game

This is the repo for the actual game itself but it uses code from [MachineCodeSimulator](https://github.com/DanesZalor/MachineCodeSimulator "Repository for the MachineCodeSimulator"), specifically the following files:
- `CPU/CPU.cs`
- `CPU/RAM.cs`
- `CPU/ALU.cs`
- `Assembler/Assembler/Assembler.cs`
- `Assembler/Assembler/Common.cs`
- `Assembler/Assembler/LEXICON.cs`
- `Assembler/Assembler/PreprocessorDirectives.cs`
- `Assembler/Assembler/SyntaxChecker.cs`
- `Assembler/Assembler/Translator.cs`

There are a few migration measures to take after copying these files.
##### 1. Adding them in **AutoLoad**
Click *Project*→*Project Settings* → **Autoload**<br>
and add all the files
![autoload settings](/.docuimages/autoload.png)

##### 2. Adding some lines of code
At their current state, still isn't useable by the Godot engine.
make sure these files are inheriting `Godot.Node`
And for all applicable, make an empty parameterless constructor.

> Also I don't think we can use static files in Godot so we better change the fucking Assembler  