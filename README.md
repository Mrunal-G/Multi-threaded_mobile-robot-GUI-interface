# Mobile robot object detection and tracking project


[![Awesome](https://cdn.rawgit.com/sindresorhus/awesome/d7305f38d29fed78fa85652e3a63e154dd8e8829/media/badge.svg)](https://github.com/sindresorhus/awesome)


 Robotics project involving **computer vision** and **multithreading** in `C#` with a small mobile robot communicating with PC using `Xbee` module as shown in the demonstration [here](https://youtu.be/60YgQBg4V3Q).

This project involves creating a user interface using C# to view real-time video from a mobile robot, to use computer vision programming techinques to extract  red/blue coloured objects from the real-time video using filters and send commands to the robot to search and move towards the red/blue object.
The mobile communicates to the PC using XBee module and can receive manual commands from the user. The user can  also capture pictures of the object from the user interface. As well as select the colour of the object the robot needs to detect and move towards. The C# program uses backgroundWorker threads so that the user interface is responsive at all times.

The project source code can be viewed [here](https://github.com/Mrunal-G/Mobile-Robot-computer-vision-object-detection-and-tracking-multi-threaded-user-interface/blob/master/robotics/trObject1/Submit.cs)

## Working Principle of mobile robot for object detection and tracking
The sample output of a red object  which is detected at different portions of the screen is shown in the interface screen, the robot is made to move and hence follow the red object. 
- If the red object is in the center of the frame but appears to be at distance, the robot will continue moving forward till the red object covers more space in the frame.
 Meaning, how further the red object is from the mobile robot is determined by how big red object appears in the frame. If the red object appears small in the center of the frame then, it means the red object is further away from the mobile robot in the straight direction. Whereas, if the red objct appears big in the center of the frame then it means that the red object is closer to the mobile robot in the srtaight direction. When the red object is closest to the mobile robot, the mobile robot will come to a stop.
 
 - Besides, if the red object appears to on the extreme left of the frame the mobile robot turns right to move closer to the red object. Note that this is due to lateral inversion effect of camera.
 - Similary, if the red object appears to on the extreme right of the frame the mobile robot turns left to move closer to the red object.

### Support
To report bugs, suggest improvements, or ask questions, create issues.
To contribute improvements to this document, fork this project and create pull request! 😃

<!---
Refer to [this](https://help.github.com/en/articles/working-with-forks) for understanding more about Fork and PR workflow. 

<!---[Forking Guide](https://guides.github.com/activities/forking/) 

<!---Refer to [this](https://help.github.com/en/articles/creating-releases) for understanding more about creating releases.
-->



[![IMAGE ALT text here](http://img.youtube.com/vi/60YgQBg4V3Q/0.jpg)](http://www.youtube.com/watch?v=60YgQBg4V3Q)

<table>
<tr>
    <td align="center" valign="center">
    <em>Desiging Loveable Robots</em>
    </td>
</tr>
</table>


![Robot-Control Interface](https://github.com/MruanlPraksh/ROBOT-following-object-computer-vision-Aforge-project-/blob/master/Images%20and%20Video/UserInterface.jpg)

<!-- [Similar project of mobile robot for automation](https://www.researchgate.net/publication/323947831_Object_tracking_robot_using_adaptive_color_thresholding) -->
