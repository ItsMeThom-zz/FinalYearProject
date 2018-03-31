using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPathManager : MonoBehaviour {

    public float WalkSpeed = 2.0f;
    public GameObject[] AllNavPoints;
    private Stack<Vector3> currentPath;

    private Vector3 currentNavPointPosition;
    private float moveTimeTotal;
    private float moveTimeCurrent;

    private void Awake()
    {
        AllNavPoints = GameObject.FindGameObjectsWithTag("NavPoint");
    }

    public Vector3[] NaviagateTo(Vector3 destination) {
        AllNavPoints = GameObject.FindGameObjectsWithTag("NavPoint");
        currentPath = new Stack<Vector3>();
        var currentNode = FindClosestToTarget(this.transform.position);
        //print("Closest node to me: " + currentNode.gameObject.name);
        var endNode = FindClosestToTarget(destination);
        //print("Node I want : " + endNode.gameObject.name);
        if (currentNode == null || endNode == null || currentNode == endNode)
        {
            //if(currentNode == null) { print("current node null"); }
            //if(endNode == null) { print("end node null"); }
            //if(currentNode == endNode) { print(AllNavPoints.GetLength(0)+" COUNT"); }
            return null;
        }
        var openList = new SortedList<float, NavPoint>();
        var closedList = new List<NavPoint>();
        openList.Add(0, currentNode);
        currentNode.Previous = null;
        currentNode.Distance = 0f;

        while(openList.Count > 0)
        {
            currentNode = openList.Values[0];
            openList.RemoveAt(0);
            var dist = currentNode.Distance;
            closedList.Add(currentNode);
            if(currentNode == endNode)
            {
                //weve reached destination
                break;
                
            }
            foreach(var neighbour in currentNode.Neighbours)
            {
                if(neighbour == null) { break; }
                if (closedList.Contains(neighbour) || openList.ContainsValue(neighbour))
                    continue;
                neighbour.Previous = currentNode;
                neighbour.Distance = dist + (neighbour.transform.position - currentNode.transform.position).magnitude;
                var distanceToTarget = (neighbour.transform.position - endNode.transform.position).magnitude;
                openList.Add(neighbour.Distance + distanceToTarget, neighbour);
            }
        }
        if(currentNode = endNode)
        {
            while(currentNode.Previous != null)
            {
                currentPath.Push(currentNode.transform.position);
                currentNode = currentNode.Previous;
            }
            currentPath.Push(transform.position);
        }
        return currentPath.ToArray();
    }

    public void StopPath() {
        currentPath = null;
        moveTimeTotal = 0f;
        moveTimeCurrent = 0f;

    }

    /// <summary>
    /// Find closest GameObj to target that is a NavPoint (by tag + component)
    /// returns null if cannot find any
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private NavPoint FindClosestToTarget(Vector3 target)
    {
        GameObject closest = null;
        float closestDist = Mathf.Infinity;
        foreach(var navpoint in GameObject.FindGameObjectsWithTag("NavPoint"))
        {
            var dist = (navpoint.transform.position - target).magnitude;
            if(dist < closestDist)
            {
                closest = navpoint;
                closestDist = dist;
            }
        }
        if(closest != null)
        {
            return closest.GetComponent<NavPoint>();
        }
        return null;
    }

    public GameObject GetRandomNavPointObject()
    {
        AllNavPoints = AllNavPoints = GameObject.FindGameObjectsWithTag("NavPoint");
        int i = UnityEngine.Random.Range(0, AllNavPoints.GetLength(0));
        return AllNavPoints[i];
    }
}
