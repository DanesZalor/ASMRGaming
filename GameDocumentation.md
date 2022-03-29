# Game Documentation of ARGS

# User Interface

![UserInterface](.images/UserInterface.png?raw=true "Different buttons of User Interface")

As shown in the image above, this is an example of the game's interface. Each has its own buttons, prompts, and such and the functions of these buttons are;
```
The Play/Pause Button - it allows the user to play/pause the game

Stop/Reset button - it stops and resets the current game making it 
revert to the state before the game was played

Hide Windows - hides the command prompt and source window

Open Command Prompt - opens a new command prompt window

Resizing square - resizes the windows of command prompt and source file

Source/Instruction file - This is where the user makes instructions for their robot, and if there are changes of the current file do "ctrl + s" to save the file.

Lastly, the command prompt. It is where you do certain console commands. The list of commands are shown below.
```

# Console Commands
Bot commands;
```
bot <options> --<arg>=<value>
    options: clear | ls | help | add | mod | rm
    args:   --team=0/1  
            --name=<string>
            --steering=tank|car
            --combat=drill|chopper
            --sensor=laser|camera
            --x|y|r=<int>

; note that if you add a bot  it would randomly choose/generate a
component e.g. bot add --name=<testbot> , it would randomly generate the bot's steering, combat, sensor, and its starting coordinates.
```

Source/Instruction file commands;
```
touch <filename> - creates a source/instruction file
edit <filename> - opens and edit the source/instruction file
```

ASM (assemble) command - assembles the bot, it places the instructions you have created from
the source/instruction file and places it into the robot;
```
asm --bot name=<botname> --src=<filename>
```

Other commands are;
```
ls - shows the list of all source/instruction files
play - plays the game
pause - pauses the game
reset - resets the game
```

# Peripherals

## Laser Sensors
The Laser Sensors(LS) takes 3 bytes in the RAM:
```
At RAM[250], it has three bits each has its corresponding value to determine whats turned on/off.

    100 bit - first bit determines if the peripheral is turned on(1)/off(0)

    010 bit - second bit determines if the left laser (in perspective of the robot) 
    is colliding with another robot

    001 bit - third bit determines if the right laser (in perspective of the robot) 
    is colliding with another robot

At RAM[251], if the left laser is colliding with another robot, it writes 
the distance of the collided robot

At RAM[252], same as RAM[251] but on the right laser.
```

As shown in the image below, the current active bits of RAM[250] would be 110, first bit is turned on because the lasers are turned on, and the second bit is turned on since it spots another robot. The peripheral writes the distance value of the spotted robot at RAM[251].

![LaserSensor1](.images/LaserSensor1.png?raw=true "Robot detects another robot through its left laser sensor")

For this image as shown below, RAM[250] value would be 101 since the right laser is turned on and detects the robot while the left laser is turned off because it didnt. Same as before, the peripheral writes the distance value of the spotted robot but it will be on RAM[252] rather than RAM[251].

![LaserSensor1](.images/LaserSensor2.png?raw=true "Robot detects another robot through its right laser sensor")

<br> <br/>
Other examples;

![LaserSensorNone](.images/LaserSensorNone.png?raw=true "Robot did not detect the other robot")

For this image, at RAM[250] the values would be 100 the second and third bit is 0 because the other robot did not "hit" the laser sensor of your robot.

![LaserSensorBoth](.images/LaserSensorBoth.png?raw=true "Robot detects other robot on both Laser Sensors")

For this image, at RAM[250] the values would be 111, all of the bits at RAM[250] are turned on because both laser sensors detects the other robot. The peripheral writes the distance values of the spotted robot both at RAM[251] and RAM[252], however RAM[251] and RAM[252] doesn't always have the same value it depends because the other robot may be moving diagonally.

## Camera Sensor
The Camera Sensor (CS) takes the same bytes as Laser Sensor (3 bytes) but it is different the way it functions.

```
At RAM[250], same as LS it has 3 bits. However, it is not the same as LS.

100 - first bit determines if peripheral is on(1) or off(0)
010 - second bit determines if an enemy is detected(1) or not(0)
001 - third bit determines if an enemy is on the left(0) or right(1)

At RAM[251], if another robot is detected, it shows the angle of the detected robot non negative value, 
calculated by normalized vector dot product, 
basically the closer it is to 0, the nearer it is to the direct front.

At RAM[252], if another robot is detected, it shows 
the range from your robot to the detected robot basically, 
the closer it is to 0, the nearer it is to your robot.
```

Image examples shown below:

From the image below, the robot detects another robot to its left so values of RAM[250] would be 110.

![CameraSensor1](.images/CameraSensor1.png?raw=true "Robot detects another robot through the camera sensor to its left")



![CameraSensor2](.images/CameraSensor2.png?raw=true "Robot detects another robot through the camera sensor to its right")



![CameraSensorNone](.images/CameraSensorNone.png?raw=true "Robot did not detect the other robot anymore")

## Steering
There are two types of steering, these are tracks(tank) and tires(car). The steering is located at the [254] and [255] RAM Address.

As shown in the  image below, this is an example of a tank robot that has a track as its wheels. As stated before, [254] and [255] is the RAM address for its steering, [254] for its left tracks and [255] for its right tracks.

![CarWithDrill](.images/CarWithDrill.png?raw=true "Robot that has a tire for steering and a drill weapon")

A tank robot can move forward, backward, or be stationary depending on the values placed on the RAM address.
```
127 - stationary
>127 - forward movement
<127 - backward movement
```

Another type of steering is the Car(tires) as shown below.

![TankWithDrill](.images/TankWithDrill.png?raw=true "Robot that has a track for steering and a drill weapon")

The Car has the same RAM address as the Tank, RAM address [254] and [255]. However, it acts/functions differently.

```
At RAM[254] - it functions as the acceleration of the Car, 
the values are the same as the tank being; 
127 is stationary, >127 is accelerate, <127 is deaccelerate.
Additionally, RAM[254] is the rear wheel.

At RAM[255] - it functions as the direction of the Car,
the direction it goes depends on the value placed in the address the values are;
127 to go straight, >127 to go left, <127 to go right
Additionally, RAM[255] are the front wheels.
```

## Combat
There are two types of combat weapons that can be used for the robots, these are the drill and chopper. Images shown below;

![TankWithDrill](.images/TankWithDrill.png?raw=true "Robot that has a track for steering and a drill weapon")

![TankWithChopper](.images/TankWithChopper.png?raw=true "Robot that has a track for steering and a chopper weapon")

These peripherals can be modified at RAM address [253], to switch between the two (the drill and chopper) the values would be
```
0 = Chopper
1 = Drill
```
