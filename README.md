# Vectors, Linear Algebra and Shaders

This is not a game, the first part is just a demo with different use-cases for Vectors and Linear Algebra in games.  
Its purpose is to show different solutions to problems frequently found in videogames.  
The second part contains several basic shaders created in Unity using HLSL (High Level Shader Language).

<br/>
<p align="center">
  <img src="http://emanuelecarrino.altervista.org/images/portfolio/VectorMathShaders_1920x1080.png" />
</p>
<br/>

**Disclaimer:**  
In some scenarios, I'm intentionally avoiding the use of built-in functions or assets provided by Unity.   
This is because the goal of this project is to show and appreciate the math and the thought behind every situation.


## Development
**Engine:** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Unity  
**IDE:** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Microsoft Visual Studio  
**Language:** &nbsp;&nbsp; C#  , HLSL (High Level Shader Language)
  
  
  
## Vectors and Linear Algebra content:

- 2D Radial Trigger  
> **Circular trigger used to detect proximity in any game.**  
> Instead of using Unity built-in funcions for magnitude and distance  
> I manually calculated the space between the objects using ***`Pythagoras' Theorem`***.  
> Optimization: instead of using the (expensive) squared root, the distance is compared when still squared.  

- 2D Look-At Trigger  
> **Cone shaped trigger mostly used to detect if the players is inside the field of vision (FoV) of an enemy**  
> **(frequently seen in stealth games) or to determine hits for cone-shaped attacks or AoE weapons.**  
> It might look counterintuitive but instead of using angles, the only check needed here is the ***`Dot Product`*** between the two directions.   

- 2D Convex Polygon Trigger  
> **Customizable trigger used to detect when player enters a specific region**  
> **It has countless uses: scena change, spawn enemies, trigger events, load/unload assets, etc..**  
> The only concept used here is the ***`Exterior/Wedge Product`***, nothing else.

- 3D Mirror Reflection  
> **Hot to calculate the reflection given a direction and a surface.**  
> **Used to simulate mirror reflection or bouncing, also very common in graphics programming.**  
> Instead of using Unity built-in funcions, the reflection was calculated using only the ***`Dot Product`*** and the ***`Surface Normal`***.  

- Space Transformation
> **How to interpret and convert positions between two different coordinate systems.**  
> **Used when we need to switch between spaces or convert a position from local to global and vice versa.**  
> The implementation does not use any rotation or Unity built-in transformation functions (LocalToWorld or WorldToLocal matrices).  
> It's realized by extracting the ***`Components of the Vector`*** representing the position using the ***`Dot Product`***.  
> Using these, we can ***`Build an Offset`*** that, coupled with the sum or difference of vectors, returns the new position.

- Orthonormalization
> **How to build a Local Reference System on a specific point of a surface.**  
> **Use cases for this can be placement of object on irregular terrain, and also rendering/graphic programming.**  
> We use the ***`Cross Product`*** twice, the first time between the Raycast direction and the Surface Normal to get a ***`Bitangent Vector`***  
> then a second time between the normal and the Bitangent vector we just found, to get a ***`Tangent Vector`***.  
> This way we build a reference system Orthonormal to the surface.  

- 3D Slice Trigger
> **Cylindrical sector shaped trigger with customizable parameters, and object position tracker.**  
> **We can use this to set up a 3D FoV trigger, for example to make NPCs turn their head towards the player when he's in front of them.**  
> Here we use many concepts introduced in previous examples to build a customizable trigger.  
> It can detect and follow an Object by aiming at its position until it moves inside the trigger region.  
> Main concepts used are ***`Dot Product`***, ***`Vector Projection`***, ***`Distance`*** and ***`Space Transformation`***  
  
- 3D Spherical Sector Trigger
> **Conical shaped trigger with customizable parameters, another type of 3D FoV trigger.**  
> This time we use a different approach, we tranform everything from Global to Local space.  
> Doing so, both the condition checks and the drawings are much simplier and fast (less vector operations).  
> We keep using the Dot Product but we also introduce new elements: ***`Angles`***, ***`Rotation`*** and ***`Trigonometry`***  

- 2D Clock
> **Creation of a 2D Clock that shows the system time.**  
> Based on a simple conversion from hours, minutes and seconds to ***`Angles`***.   
> There is also an option to smooth the second hand movement by using milliseconds in the conversion.  

- Helathbar (Linear Interpolation)
> **Simple and plain Healthbar found in any kind of game.**  
> The main concept here is the ***`Linear Interpolation`*** used both for the position and the color.  
> The position is calculated using the LERP function with the current life percentage in input.  
> The color is calculated using two LERP function between 3 colors set by the user in the inspector.  

- Proximity Damage (Remapping)
> **Remapping of a damage (proportional to distance) caused by an explosion with a set min and max damage.**  
> To realize the ***`Remap`*** we use both the ***`Inverse Linar Interpolation`*** and the ***`Direct Linear Interpolation`***.  
> With the inverse LERP, we get the step to use on the direct LERP based on the player distance from the explosion.  
> Then we use the direct LERP to remap this step between the values of min and max damage set by design.

- Parabolic Trajectory
> **Kinematic description of the deterministic motion experienced by an object that is projected in a gravitational field.**  
> Particular case of the more general concept of ***`Uniformly Accelerated Motion`***  
> Realized with basic kinematic concepts and ***`Quadratic Equation`*** solving.  

- Basic Procedural Animation and Easing Function
> **Comparison of different Easing function to animate procedural movement.**  
> ***`Easing Function`*** categories: Linear, EaseIn, EaseOut, EaseInOut, Custom Cubic Hermite interpolation (tweakable with derivative values).  
> Everything is realized using Math functions, without any Unity animation tool (AnimationCurve, etc).
  
  
## Shaders Content:

- Trasparent Shader
> **Color animation and trasparent effect that can be used to mark a Point of Interest or a Shell Effect.**  
> Concepts used: ***`Additive Blending`***, no writing of the ***`Depth Buffer`***, no ***`Face Culling`***.  
> The animation is realized with two cosine functions and color interpolation.  

- Vertex Animation Shader
> **Geometry animations on a tessellated mesh that can be used to create a water effect (weaves or ripples).**  
> Concepts used: ***`Vertex animation`***, definition of a ***`Radial Signed Distance Field (SDF)`***.  
> The animation is realized with a cosine function and color interpolation.  

- Textured Healthbar
> **A more advanced Healthbar that changes color based on a texture and pulse when the HP get below 20%.**  
> Concepts used: ***`Texture Sampling`***, ***`Signed Distance Field (SDF)`*** with ***`Partial Derivative Anti Aliasing`***.  
> The pulsing animation is realized with a cosine function.  

- Lighting Multi-pass Shader
> **A shader that can handle multiple light sources and gives a silhouette effect effect when the object is behind something.**  
> Concepts used: compositing of diffuse ***`Lambertian Light`*** and specular ***`Blinn-Phong Light`***,  
> ***`Additive Blending`***, ***`Depth Buffer`*** and ***`Fresnel Effect`***.   
> The pulsing animation is realized with a cosine function.  

- 3D Texture Shader
> **A shader combine Albedo, Normal Map and Displacement Map to create a more realistic surface.**  
> Concepts used: compositing of diffuse ***`Lambertian Light`*** and specular ***`Blinn-Phong Light`***,  
> ***`Additive Blending`*** and ***`Tangent Space`***.   

- Image Based Lighting (IBL) Shader
> **A shader that lights up an object based on the environment around it, a rectilinear Skybox in this example.**  
> Concepts used: compositing of ***`Diffuse IBL`*** and ***`Specular IBL`***, ***`3D Texture`*** techniques used in previous example.

<br/>
<br/>

`Code is a little over-commented just to help anyone interested to navigate it better`
