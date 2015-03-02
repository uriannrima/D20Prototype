using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WaypointManager : BaseManager<WaypointManager>
{
    public GameObject Waypoint;

    /// <summary>
    /// Waypoints holder.
    /// </summary>
    private GameObject Waypoints;

    public float YOffSet = 0.15f;

    public Dictionary<GameObject, GameObject> WaypoingPool = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        Waypoints = new GameObject("Waypoints");
        Waypoints.transform.parent = this.transform;
    }

    public void CreateWaypoint(Vector3 waypointPosition, GameObject waypointOwner)
    {
        CreateWaypoint(waypointPosition, waypointOwner);
    }

    public void CreateWaypoint(Vector3 waypointPosition, GameObject waypointOwner, Color waypointColor)
    {
        // Check if already exist waypoint
        if (WaypoingPool.ContainsKey(waypointOwner))
        {
            // If already exists, only move it.
            WaypoingPool[waypointOwner].transform.position = waypointPosition + new Vector3(0, YOffSet, 0);
            WaypoingPool[waypointOwner].SetActive(true);
        }
        else
        {
            // Doesn't exist, create a waypoint for it.
            GameObject waypoint = Instantiate(Waypoint, waypointPosition + new Vector3(0, YOffSet, 0), Quaternion.identity) as GameObject;
            waypoint.transform.parent = Waypoints.transform;
            waypoint.renderer.material.color = waypointColor;
            waypoint.GetComponent<WaypointMark>().Owner = waypointOwner;

            WaypoingPool.Add(waypointOwner, waypoint);
        }
    }
}
