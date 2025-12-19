# RoboticArmSystem
An automatically sorting Robotic Arm System. To build the scene, just download the Assets, packages, and project settings.

Gameplay
The player observes a robotic arm operating on a conveyor belt and ensures that the arm correctly classifies continuously generated cubes.
A correct classification means that a cube is picked up by the robotic arm and transferred into the container that matches its color.

If the player detects an incorrect classification, they must intervene during the fixing time window by pressing a button
(keyboard: E key / VR controller: GRAB button) to correct the mistake.

Example
If the player notices that the robotic arm has incorrectly placed a blue cube into a black container, the player should press the black button (corresponding to the black container).
This action instructs the robotic arm to retrieve the cube from the black container and automatically reclassify it into the correct blue container.

Before entering the game, the player can configure: Conveyor belt speed/Overall classification error rate

Key Scripts

ConveyorBelt
Controls cube spawn positions (spawn point) and pickup positions (pickup position)
Manages spawn probability (spawn weight) for different cube types
Controls cube spawn interval (spawn interval)
Defines total conveyor travel distance (total distance)
Controls conveyor movement speed (fast / medium / slow)

RoboticArmController
Target Tags: Determines which objects can be picked up based on their tags (e.g., CubeBlue)
Gripper Settings: Controls the rotation angles of the left and right grippers after grabbing an object to create a realistic “gripping” visual effect

Forearm Script
Controls the robotic arm’s forearm rotation, grabbing, and releasing actions
Executes different rotation sequences based on the object’s tag to complete classification
The global tag misidentification rate determines the probability of incorrect classification by the robotic arm

0922Button
Detects whether the player (with the Player tag) is within interaction range of a button’s collider
Allows interaction via keyboard or VR controller when the player is close enough
When pressed, the robotic arm rotates to a predefined angle set by ForearmReference
After rotation, the system detects nearby objects with Target Tags within a range centered on DetectionCenter
If a valid object is found, the Forearm script is triggered to grab and reclassify the object

SpawnCountdownDisplay
Controls the total system runtime (GlobalCountDownDisplay)
Configurable parameters include:
Player intervention window (FixingTime)
Arm rotation duration (RotationTime)
Countdown text format and color settings
Monitored conveyor belt objects (ConveyorBelts)

ItemCounter
Controls data display text content, layout, and color

VRIntroText
Displays instructional text to the player in the VR environment

VRHighLight
Objects with this script will be highlighted when the VR controller ray intersects with their collider

JarvisHudCanvas
Assigns the VR Camera so that the movable HUD panel rotates together with the player’s view

JarvisUIDisplay
Controls text position, content, and color on the movable HUD panel
