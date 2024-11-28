using System.Collections;
using UnityEngine;

// this script is responseble for moving the seeker towards its target
public class SeekerUnit : MonoBehaviour
{
    #region Variables
    [Header("Scriptable Objects")]

    [SerializeField] private AIStatistics aiStatistics;


    [Header("Target")]

    [SerializeField] private Transform target; 
    
    
    private Vector3[] path;
    private int targetIndex;


    private float thisSpeed;
    #endregion


    // Called when the script is first loaded.
    private void Awake()
    {
        // Find the Transform of the GameObject named "Target" and assign it to the target variable.
        target = GameObject.Find("Target").transform;


        // sets the Speed of the Unit to the speed VAriable of AIStatistics.
        thisSpeed = aiStatistics.movementSpeed;
    }


    // called before the first frame.
    private void Start()
    {
        // executes the UpdatePath void.
        UpdatePath();
    }


    // Called Every Frame.
    void Update()
    {
        // Executes the LookAtPlayer void.
        LookAtTarget();


        // Continuously check if the target's position has changed
        if (Vector3.Distance(transform.position, target.position) > 0f)
        {
            // Update the path if the target has moved
            UpdatePath();
        }


        // Stops the coroutine "FollowPath".
        StopCoroutine("FollowPath");
    }


    // Is called wwhen the pathfinding system successfully calculates a path.
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        // Checks wheter the path was succesfull.
        if (pathSuccessful == true)
        {
            // Makes it so that the new path is assinged to the "path" variable.
            path = newPath;


            // stops the Coroutine if it was already playiong.
            StopCoroutine("FollowPath");

            // Starts the Coroutine.
            StartCoroutine("FollowPath");
        }
    }


    // Updates the newly requested path when called.
    public void UpdatePath()
    {
        // // Requests a new path from the PathRequestManager.
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }


    // Makes the Unit look at the target.
    private void LookAtTarget()
    {
        // Calculates the direction to the target
        Vector3 directionToTarget = target.position - transform.position;


        // makes it so the unit doesn't look at the targets Y position.
        directionToTarget.y = 0;


        // Only rotate if the target is not in the same position as the unit.
        if (directionToTarget != Vector3.zero)
        {
            // Calculates the rotation towards the target.
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);


            // Makes it so the unit rotates smoothly towards the target.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aiStatistics.rotationSpeed * Time.deltaTime);
        }
    }


    // Moves the unit along the waypoints in the path.
    private IEnumerator FollowPath()
    {
        // Checks if the targetIndex is within the bounds of the path array.
        if(targetIndex < path.Length) 
        {
            // Gets the first waypoint in the Arrey.
            Vector3 currentWaypoint = path[targetIndex];


            while (true)
            {
                // Makes it so that when  the unit has reached the waypoint it moves on to the next one.
                if (transform.position == currentWaypoint)
                {
                    // Makes the targetIndex go up by one.
                    targetIndex++;


                    // Makes it so that the corutine stops if the targetIndex goes out of bounds of the path array.
                    if (targetIndex >= path.Length)
                    {
                        // Stops the corutine.
                        yield break;
                    }


                    // Updates the current waypoint to the next one.
                    currentWaypoint = path[targetIndex];
                }


                // Moves the unit towards the next waypoint.
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, thisSpeed * Time.deltaTime);


                // Executes the IEnumerator every frame.
                yield return null;
            }
        }
    }


    // createx the visualysation of the waypoints and the path.
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);


                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }

                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
