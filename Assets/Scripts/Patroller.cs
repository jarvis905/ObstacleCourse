using System;
using System.Collections.Generic;
using UnityEngine;

// Patroller.cs
public class Patroller : MonoBehaviour
{
    // Consts:
    private const float rotationSlerpAmount = 0.68f;

    [Header("References")]
    public Transform trans;
    public Transform modelHolder;

    [Header("Stats")]
    public float movespeed = 30;

    // Private variables:
    private int currentPointIndex;
    private Transform currentPoint;
    private Transform[] patrolPoints;

    // Returns a List containing the Transform of each child with a name that starts with "Patrol Point (".
    private List<Transform> GetUnsortedPatrolPoints()
    {
        // Get the transform of each child in the Patroller:
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        
        // Declare a local List storing Transforms:
        var points = new List<Transform>();

        // Loop through child Transforms
        for (int i = 0; i < children.Length; i++)
        {
            // Check if the child name starts with "Patrol Point (".
            if (children[i].gameObject.name.StartsWith("Patrol Point ("))
            {
                // If true, add it to the 'points' List
                points.Add(children[i]);
            }
        }
        // Return the points List
        return points;
    }

    // Method to set current Patrol Point
    private void SetCurrentPatrolPoint(int index)
    {
        currentPointIndex = index;
        currentPoint = patrolPoints[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get an unsorted List of Patrol Points
        List<Transform> points = GetUnsortedPatrolPoints();

        // Only continue if we find at least 1 Patrol Point
        if (points.Count > 0)
        {
            // Prepare our array of Patrol Points
            patrolPoints = new Transform[points.Count];

            // Loop through all Patrol Points
            for (int i = 0; i < points.Count; i++)
            {
                // Quick reference to the current point
                Transform point = points[i];

                // Isolate just the Patrol Point number within the name
                int closingParenthesisIndex = point.gameObject.name.IndexOf(')');

                string indexSubstring = point.gameObject.name.Substring(14, closingParenthesisIndex - 14);
                
                // Convert the number from a string to an integer
                int index = Convert.ToInt32(indexSubstring);

                // Set a reference in the script - patrolPoints array
                patrolPoints[index] = point;

                // Unparent each Patrol Point so it doesn't move with us
                point.SetParent(null);

                // Hide Patrol Points in the Hierarchy
                point.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            // Start patrolling at the first point in the array
            SetCurrentPatrolPoint(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only operate if we have a currentPoint
        if (currentPoint != null)
        {
            // Move root GameObject towards the currentPoint
            trans.position = Vector3.MoveTowards(trans.position, currentPoint.position, movespeed * Time.deltaTime);

            // If we're on top of the point already, then change the currentPoint
            if (trans.position == currentPoint.position)
            {
                // If we're at the last Patrol Point, we'll set to the first Patrol Point (double back)
                if (currentPointIndex >= patrolPoints.Length - 1)
                {
                    SetCurrentPatrolPoint(0);
                }
                // Else if we're not at the last Patrol Point, then go to the next index after the currentPoint
                else
                {
                    SetCurrentPatrolPoint(currentPointIndex + 1);
                }
            }
            // Else if we're not on the point yet, then rotate the model towards it
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation((currentPoint.position - trans.position).normalized);
                modelHolder.rotation = Quaternion.Slerp(modelHolder.rotation, lookRotation, rotationSlerpAmount);
            }
        }
    }
}
