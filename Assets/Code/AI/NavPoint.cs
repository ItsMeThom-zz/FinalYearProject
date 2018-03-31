using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NavPoint : MonoBehaviour {

    //own collider, used to ignore when checking overlaps
    public SphereCollider SelfCollider;

    public List<NavPoint> Neighbours;

    public bool Baked = false;

    public bool Colliding = false;
    public bool DestroyOnCollision = false;

    public static float MAX_DISTANCE = 15f;

    private void Awake()
    {
        Neighbours = new List<NavPoint>();
        this.gameObject.layer = 8; //nav layer of collision
        FindNeighbours();
        UpdateNeighbours();
        Baked = true;
    }

    public NavPoint Previous
    {
        get;
        set;
    }

    public float Distance
    {
        get;
        set;
    }

    public Vector3 Point
    {
        get
        {
            return this.transform.position;
        }
    }

    private void Update()
    {
        if (!Baked)
        {
            FindNeighbours();
            UpdateNeighbours();
            //CheckCollision();
        }
    }

    private void Start()
    {
        
    }

    public void CheckCollision()
    {
        //move object to seperate collision layer temporarily
        gameObject.layer++;
        var overlappedObj = Physics.OverlapSphere(this.Point, 1.0f, (gameObject.layer-1));
        if(overlappedObj != null &&overlappedObj.GetLength(0) > 0)
        {
            foreach(var coll in overlappedObj)
            {
                //dont collide with self
                if(coll != this.GetComponent<SphereCollider>())
                {
                    Colliding = true;
                    break;
                }
            }
            if (DestroyOnCollision)
            {
                RemoveSelf();
            }
        }
        else
        {
            Colliding = false;
        }
        gameObject.layer--;
    }

    public void AddNeighbour(NavPoint neighbour)
    {
        if (!Neighbours.Contains(neighbour))
        {
            Neighbours.Add(neighbour);
        }
    }
    public void RemoveNeighbour(NavPoint neighbour)
    {
        if (Neighbours.Contains(neighbour))
        {
            Neighbours.Remove(neighbour);
        }
    }

    private void RemoveSelf()
    {
        if(Neighbours != null && Neighbours.Count > 0)
        {
            for(int i = 0; i < Neighbours.Count; i++)
            {
                Neighbours[i].RemoveNeighbour(this);
            }
                
        }
        this.enabled = false;
        //Destroy(this.gameObject);
    }

    public void UpdateNeighbours()
    {
        foreach(NavPoint n in Neighbours)
        {
            if(Vector3.Distance(n.Point, this.Point) > MAX_DISTANCE)
            {
                n.RemoveNeighbour(this);
                this.RemoveNeighbour(n);
                break;
            }
        }
    }

    public void FindNeighbours()
    {
        var nearby = Physics.OverlapSphere(this.transform.position, MAX_DISTANCE);
        if(nearby != null)
        {
            foreach (var obj in nearby)
            {
                if(obj != SelfCollider)
                {
                    NavPoint potentialNeighbour = obj.GetComponent<NavPoint>();
                    if (potentialNeighbour)
                    {
                        if (!Neighbours.Contains(potentialNeighbour) && ClearRaycastToNeighbour(potentialNeighbour.transform))
                        {
                            AddNeighbour(potentialNeighbour);
                            potentialNeighbour.AddNeighbour(this);
                        }
                    }
                }
            }
        }
    }

    private bool ClearRaycastToNeighbour(Transform neighbour)
    {
        RaycastHit hit;
        bool clear = true;
        var neighbourPostion = neighbour.position;
        var direction = neighbourPostion - this.transform.position;
        if(Physics.Raycast(this.transform.position, direction, out hit))
        {
            //Debug.DrawRay(this.transform.position, direction, Color.red, 1);
            if(hit.collider.gameObject != neighbour.gameObject)
            {
                //print("I hit " + hit.collider.gameObject.name);
                clear = false;
            }
            
        }
        return clear;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
        if (Neighbours == null || Neighbours.Count == 0)
        {
            return;
        }
        foreach(var n in Neighbours)
        {
            if(n != null)
            {
                Gizmos.DrawLine(transform.position, n.transform.position);
            }
        }
        
    }


}
