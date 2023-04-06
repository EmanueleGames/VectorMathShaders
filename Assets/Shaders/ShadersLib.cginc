#define PI  3.1415926535

// Inverse Lerp Function
float InverseLerp(float start, float end, float step)
{
    return (step - start) / (end - start);
}

// Function used to map the UV Skybox texture to the ViewDirection reference used by Unity Skybox
float2 DirToRectilinear(float3 dir)
{
    // X coordinate
    float x = atan2(dir.z, dir.x); // between [-PI, PI]
    x = (x / (2 * PI)) + 0.5;      // between [0, 1]
    // Y coordinate
    float y = dir.y * 0.5 + 0.5;   // between [0, 1]
    return float2 (x, y);
}