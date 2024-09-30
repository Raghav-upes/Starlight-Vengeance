using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float startPosOffset; // Offset from original position to define the starting Z position
    public float endPosOffset;   // Offset from original position to define the ending Z position
    public float moveSpeed = 5f; // Speed of movement
    public float rotationSpeed = 100f; // Speed of rotation during the rotation phase

    private Vector3 originalPos;
    private Vector3 startPos;
    private Vector3 endPos;

    private bool movingToStart = true; // Track if moving towards startPos


    void Start()
    {
        // Store the original position of the obstacle
        originalPos = transform.position;

        // Define start and end positions relative to the original position
        startPos = new Vector3(originalPos.x, originalPos.y, startPosOffset);
        endPos = new Vector3(originalPos.x, originalPos.y, endPosOffset);
    }
    void Update()
    {
        // Check if moving towards startPos or endPos
        if (movingToStart)
        {
            // Move the object towards startPos
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);

            // Check if the object has reached startPos
            if (Vector3.Distance(transform.position, startPos) < 0.001f)
            {
                // Switch direction to move towards endPos
                movingToStart = false;
            }
        }
        else
        {
            // Move the object towards endPos
            transform.position = Vector3.MoveTowards(transform.position, endPos, moveSpeed * Time.deltaTime);

            // Check if the object has reached endPos
            if (Vector3.Distance(transform.position, endPos) < 0.001f)
            {
                // Switch direction to move back to startPos
                movingToStart = true;
            }
        }

/*        // Rotate the object around the Z axis
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);*/
    }
}
