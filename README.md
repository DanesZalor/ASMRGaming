# Assembly Robot Game

## Setup

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

Make sure to remove the *test-only* parts of the code

## Game design
The game is about controlling a robot using assembly programming. 

### CPU
The robot contains a **CPU** which contains **RAM** and **ALU** that will run a program. This  program is compiled from our made up *Assembly language* with it's own compiler from [MachineCodeSimulator](https://github.com/DanesZalor/MachineCodeSimulator "Repository for the MachineCodeSimulator").

### Peripherals

Robots are given **Peripherals** with categories such as **Actuators** <sup><sub>(Input Peripheral)</sub></sup> and **Sensors** <sup><sub>(Output Peripheral)</sub></sup>. These peripherals are controlled/read through the **RAM**, like a *memory-mapped* IO interface. Each peripheral is assigned a few cells in RAM for them to interact with.

#### Actuators

Peripherals that do things in the world. Categories are: *Movement Actuators* <sup><sub>(gives the robot movement capabilities)</sub></sup> and *Combat Actuators* <sup><sub>(gives the robot attacking/damaging capabilities)</sub></sup>. Actuators read data from RAM to determine what it does in the world.

#### Sensors

Peropherals that acquire data from the world. Sensors write data in RAM based on the data absorbed from the world.