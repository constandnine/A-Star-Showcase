using System.Collections;
using UnityEngine;

public class SeekerUnit : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform target; 
    
    
    private Vector3[] path;
    private int targetIndex;


    [Header("Scriptable Objects")]

    [SerializeField] private AIStatistics aiStatistics;


    private float thisSpeed;

    #endregion


    private void Awake()
    {
        target = GameObject.Find("Target").transform;


        thisSpeed = aiStatistics.movementSpeed;
    }


    private void Start()
    {
        UpdatePath();
    }


    void Update()
    {
        LookAtPlayer();


        // Continuously check if the target's position has changed
        if (Vector3.Distance(transform.position, target.position) > 0f)
        {
            // Update the path if the target has moved
            UpdatePath();
        }


        StopCoroutine("FollowPath");
    }
   

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful == true)
        {
            path = newPath;


            // stops the Coroutine if it was already playiong.
            StopCoroutine("FollowPath");

            // Starts the Coroutine.
            StartCoroutine("FollowPath");
        }
    }




    public void UpdatePath()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }


    private void LookAtPlayer()
    {
        Vector3 directionToTarget = target.position - transform.position;


        directionToTarget.y = 0;


        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);


            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aiStatistics.rotationSpeed * Time.deltaTime);
        }
    }


    private IEnumerator FollowPath()
    {
        if(targetIndex < path.Length) 
        {
            Vector3 currentWaypoint = path[targetIndex];

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }


                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, thisSpeed * Time.deltaTime);


                yield return null;
            }
        }
    }


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
