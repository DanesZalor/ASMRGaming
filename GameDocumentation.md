# Game Documentation of "A PROGRAMMABLE VIRTUAL ROBOT GAME AS A SUPPLEMENT TO LEARNING COMPUTER ARCHITECTURE AND ASSEMBLY PROGRAMMING LANGUAGE"


# PERIPHERALS


## Laser Sensors
The Laser Sensors(LS) takes 3 bytes in the RAM:
```
At RAM[0], it has three bits each has its corresponding value to determine whats turned on/off.

    100 bit - first bit determines if the peripheral is turned on(1)/off(0)

    010 bit - second bit determines if the left laser (in perspective of the robot) 
    is colliding with another robot

    001 bit - third bit determines if the right laser (in perspective of the robot) 
    is colliding with another robot

At RAM[1], if the left laser is colliding with another robot, it writes 
the distance of the collided robot

At RAM[2], same as RAM[1] but on the right laser.
```

As shown in the image below, the current active bits of RAM[0] would be 110, first bit is turned on because the lasers are turned on, and the second bit is turned on since it spots another robot. The peripheral writes the distance value of the spotted robot at RAM[1].

![LaserSensor1](.images/LaserSensor1.png?raw=true "Robot detects another robot through its left laser sensor")

For this image as shown below, RAM[0] value would be 101 since the right laser is turned on and detects the robot while the left laser is turned off because it didnt. Same as before, the peripheral writes the distance value of the spotted robot but it will be on RAM[2] rather than RAM[1].

![LaserSensor1](.images/LaserSensor2.png?raw=true "Robot detects another robot through its right laser sensor")

<br> <br/>
Other examples;

![LaserSensorNone](.images/LaserSensorNone.png?raw=true "Robot did not detect the other robot")

For this image, at RAM[0] the values would be 100 the second and third bit is 0 because the other robot did not "hit" the laser sensor of your robot.

![LaserSensorBoth](.images/LaserSensorBoth.png?raw=true "Robot detects other robot on both Laser Sensors")

For this image, at RAM[0] the values would be 111, all of the bits at RAM[0] are turned on because both laser sensors detects the other robot. The peripheral writes the distance values of the spotted robot both at RAM[1] and RAM[2], however RAM[1] and RAM[2] doesn't always have the same value it depends because the other robot may be moving diagonally.

## Camera Sensor
The Camera Sensor (CS) takes the same bytes as Laser Sensor (3 bytes) but it is different the way it functions.

```
At RAM[0], same as LS it has 3 bits. However, it is not the same as LS.

100 - first bit determines if peripheral is on(1) or off(0)
010 - second bit determines if an enemy is detected(1) or not(0)
001 - third bit determines if an enemy is on the left(0) or right(1)

At RAM[1], if another robot is detected, it shows the angle of the detected robot non negative value, calculated by normalized vector dot product, basically the closer it is to 0, the nearer it is to the direct front.

At RAM[2], if another robot is detected, it shows the range from your robot to the detected robot basically, the closer it is to 0, the nearer it is to your robot.
```