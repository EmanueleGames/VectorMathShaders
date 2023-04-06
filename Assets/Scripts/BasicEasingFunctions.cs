using UnityEngine;

public class BasicEasingFunctions : MonoBehaviour
{
    [Space]
    [Header("Animation Parameters")]
    [Space]
    [Range(1, 5)]
    public int movementUnits;       // how many units per input we shift
    [Range(.1f, 2f)]
    public float motionTime = .5f;  // how much time the animation will require

    public Transform Cube1Tf;
    public Transform Cube2Tf;
    public Transform Cube3Tf;
    public Transform Cube4Tf;
    public Transform Cube5Tf;

    [Space]
    [Header("Custom Easing Parameters")]
    [Space]
    [Range(-10f, 10f)]
    public float startingDerivative = -5;
    [Range(-10f, 10f)]
    public float endingDerivative = -5;

    bool isAnimating;
    float animationTime;

    // To store first and final positions of our animation
    Vector2 cube1StartPosition;
    Vector2 cube1FinalPosition;
    Vector2 cube2StartPosition;
    Vector2 cube2FinalPosition;
    Vector2 cube3StartPosition;
    Vector2 cube3FinalPosition;
    Vector2 cube4StartPosition;
    Vector2 cube4FinalPosition;
    Vector2 cube5StartPosition;
    Vector2 cube5FinalPosition;

    void Update()
    {
        if (!isAnimating) // we can only moves when animation is over
            CheckForInput();
        else
            Animate();
    }

    // Functions
    void CheckDirection(KeyCode keyPressed, Vector3 dir)
    {
        if (Input.GetKeyDown(keyPressed))
        {
            cube1StartPosition = Cube1Tf.position;
            cube2StartPosition = Cube2Tf.position;
            cube3StartPosition = Cube3Tf.position;
            cube4StartPosition = Cube4Tf.position;
            cube5StartPosition = Cube5Tf.position;
            cube1FinalPosition = Cube1Tf.position + dir * movementUnits;
            cube2FinalPosition = Cube2Tf.position + dir * movementUnits;
            cube3FinalPosition = Cube3Tf.position + dir * movementUnits;
            cube4FinalPosition = Cube4Tf.position + dir * movementUnits;
            cube5FinalPosition = Cube5Tf.position + dir * movementUnits;
            isAnimating = true;
            animationTime = 0;
        }
    }

    void CheckForInput()
    {
        CheckDirection(KeyCode.A, Vector3.left);
        CheckDirection(KeyCode.D, Vector3.right);
    }

    void Animate()
    {
        animationTime += Time.deltaTime;
        // here we use the total time of our animation and the time passed since it started
        // to calculate at what % of the animation we are, then we get the position at that time
        // Example: our animation time is set to 2 seconds, and 0.2 sec have passed since the first frame
        //          then we know that we are at 1/10 of our animation, so we LARP with t = 0.1
        float t = Mathf.Clamp01(animationTime / motionTime);

        // Easing Functions
        float t_easeIn    = t * t;                      // quadratic EaseIn
        float t_easeOut   = 1 - (1 - t) * (1 - t);      // quadratic EaseOut
        float t_easeInOut = 3 * t * t - 2 * t * t * t;  // cubic  EaseInOut
        float t_custom    = CustomEasing(t);            // custom Ease (tweaked with derivative values in input)

        Cube1Tf.position = Vector3.Lerp(cube1StartPosition, cube1FinalPosition, t);
        Cube2Tf.position = Vector3.Lerp(cube2StartPosition, cube2FinalPosition, t_easeIn);
        Cube3Tf.position = Vector3.Lerp(cube3StartPosition, cube3FinalPosition, t_easeOut);
        Cube4Tf.position = Vector3.Lerp(cube4StartPosition, cube4FinalPosition, t_easeInOut);
        Cube5Tf.position = Vector3.LerpUnclamped(cube5StartPosition, cube5FinalPosition, t_custom);

        // after we reach the final position we stop the animation (we can check again for input)
        if (t >= 1f)
            isAnimating = false;
    }

    float CustomEasing(float t)
    {
        float d0 = startingDerivative;
        float d1 = endingDerivative;
        return (d0 + d1 - 2)*t*t*t + (3 - 2*d0 - d1)*t*t + (d0)*t;
    }
}
