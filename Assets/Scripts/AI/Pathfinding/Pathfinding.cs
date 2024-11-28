using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private PathRequestManager pathRequestManager;
    private Grid grid;


    // Is caled when the script is first loaded.
    void Awake()
    {
        // Gets the "PathRequestManager".
        pathRequestManager = GetComponent<PathRequestManager>();

        // Gets the "PathRequestManager".
        grid = GetComponent<Grid>();
    }


    //starts coroutine when called.
    public void StartFindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        // starts coroutine.
        StartCoroutine(FindPath(startPosition, targetPosition));
    }


    // Calculates the shorthest path from the startNode to the targetNode.
    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        // adds a brief deley so other systems can be executed before it.
        yield return new WaitForSeconds(0f);


        // makes a Empty array to store waypoints
        Vector3[] waypoints = new Vector3[0];


        // Flags the pathSucces to true.
        bool pathSuccess = false;


        // converts the worldposition to the position of the startNode
        Node startNode = grid.NodeFromWorldPoint(startPosition);
        // converts the worldposition to the position of the targetNode
        Node targetNode = grid.NodeFromWorldPoint(targetPosition);


        // Checks if both the start and target node are walkable.
        if (startNode.isWalkable && targetNode.isWalkable)
        {
            // Creates a open set with nodes that stiil need to evaluated.
            Heap<Node> openSet = new Heap<Node>(grid.MaximalSize);
            // Creates a closed set with nodes that sare already evaluated.
            HashSet<Node> closedSet = new HashSet<Node>();


            // Adds the start node to the openset.
            openSet.AddItem(startNode);


            // Loops til the openset is empty or the target node is found/.
            while (openSet.Count > 0)
            {
                // gets the node with the lowest fCost from the openset.
                Node currentNode = openSet.RemoveFirstItem();


                // Adds the current node to the closet set
                closedSet.Add(currentNode);


                // Makes it so that when the target node is reached the pathfinding is succesfull.
                if (currentNode == targetNode)
                {
                    // Flags the Pathsucces to true.
                    pathSuccess = true;


                    // Ends the Coroutine.
                    break;
                }


                // Checks the Nodes Next to the current node.
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    // Skips the node if its Unwalkable or already evaluated.
                    if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                    {
                        // Skips to the next node.
                        continue;
                    }


                    // Calculates the cot to get to the neighbouring node.
                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);


                    // If the new cost is cheaper or the neighbor isn't in the open set it gets updated.
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        // Updates the neighbouring node cost and parent.
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;


                        // adds the neighbour node to the openset if it wasn't in there already
                        if (!openSet.Contains(neighbour))
                        {
                            // adds the node to the openset.
                            openSet.AddItem(neighbour);
                        }

                        // If the neighbor is in the open set, update its cost.
                        else
                        {
                            // updates the cost.
                            openSet.UpdateItems(neighbour);
                        }
                    }
                }
            }
        }


        // retraces the path and adds waypoints to it if a succesfull path is found
        if (pathSuccess)
        {
            // Retrace the path from the target node back to the start node to generate the waypoints.
            waypoints = RetracePath(startNode, targetNode);
        }


        // executes the FinishedProccecingPath Void.
        pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }


    // Retraces the path back from the target node to the start node.
    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        // list of nodes to store the path in.
        List<Node> path = new List<Node>();


        // Start retracing the path from the target node back to the start node.
        Node currentNode = endNode;


        // Loop until the current node reaches the start node.
        while (currentNode != startNode)
        {
            // dd the current node to the path list.
            path.Add(currentNode);


            // Move to the parent node.
            currentNode = currentNode.parent;
        }


        // simplefys thw path by removing unnecessary waypoints.
        Vector3[] waypoints = SimplifyPath(path);


        // reverses the path so it wil go from the start to the target node.
        Array.Reverse(waypoints);


        // 
        return waypoints;
    }


    // Sisimplefys thw path by removing unnecessary waypoints.
    private Vector3[] SimplifyPath(List<Node> path)
    {
        //list that stores the new and simpeler waypoints.
        List<Vector3> waypoints = new List<Vector3>();


        // tracks the old Direction.
        Vector2 oldDirection = Vector2.zero;


        // loops trought the nodes in the path list starting at the second node.
        for (int i = 1; i < path.Count; i++)
        {
            // Calculates the direction between the previus node and the current node.
            Vector2 newDirection = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);


            //if the direction has changed add the current position to the node
            if (newDirection != oldDirection)
            {
                // Add's the current node's world position to the list of waypoints.
                waypoints.Add(path[i].worldPosition);
            }


            // Sets the old directio n to the new direction.
            oldDirection = newDirection;
        }

        // Returns the siplefyed waipoints to an array.
        return waypoints.ToArray();
    }


    // Calculates the cost to travel between 2 nodes.
    private int GetDistance(Node nodeA, Node nodeB)
    {
        //Calculates the distnace on the X axis between the two nodes.
        int distanceFromXAxis = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        //Calculates the distnace on the Y axis between the two nodes.
        int distanceFromYAxis = Mathf.Abs(nodeA.gridY - nodeB.gridY);


        // Checks if the horisontal distance is greater than the vertical distance.
        if (distanceFromXAxis > distanceFromYAxis)
        {
            // Return's the combined cost based on the horizontal distance and vertical positions.
            return 14 * distanceFromYAxis + 10 * (distanceFromXAxis - distanceFromYAxis);
        }


        // returns the combined cost.
        return 14 * distanceFromXAxis + 10 * (distanceFromYAxis - distanceFromXAxis);
    }
}
