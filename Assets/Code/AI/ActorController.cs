using Assets.Code.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

public enum ActorState { Roam, Chase, Attack, Search} //search not yet implemented

[RequireComponent(typeof(NavPathManager))]
public class ActorController : MonoBehaviour, IDamageable {

    public static int BOREDOM_MAX = 10;
    public GameController GameController;
    private NavPathManager PathManager;
    public Animator Animator;

    public Transform Target;
    private Vector3[] TargetPath;


    public bool PlayerInRange = false;
    private Vector3 PlayerFaceHeight = new Vector3(0,0.5f, 0); //adjust for seeing the player in the face
    public GameObject Eyes;

    private int currentNavPoint;
    private float WalkSpeed = 1.0f;
    private float RunSpeed = 2.6f;
    private float SightRange = 15.0f;
    private int StateBordedomCounter = BOREDOM_MAX;
    private Timer BoredomTimer;
    public ActorState State;

    int HP = 30;

    private void Awake()
    {
        GameController = GameController.GetSharedInstance();
        PathManager = GetComponent<NavPathManager>();
        Animator = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        //start that roamin'
        State = ActorState.Roam;
        Animator.SetBool("Roaming", true);
	}
	// Update is called once per frame
	void Update () {
        var canAttackPlayer = PlayerInAttackRange();
        if (canAttackPlayer)
        {
            print("Can attack now!");

            State = ActorState.Attack;
        }
        else if (PlayerInViewRange() && PlayerInDirectView())
        {
          State = ActorState.Chase;
            
        }
        else
        {
            State = ActorState.Roam;
        }
        
        //check current state + set animation state
        switch (State)
        {
            case ActorState.Roam:
                //print("ROAM STATE");
                
                    print("I AM BORED NOW");
                    Animator.SetBool("Roaming", true);
                    Animator.SetBool("Chasing", false);
                    Animator.SetBool("Attacking", false);
                    //StateBordedomCounter = BOREDOM_MAX;
                    Roam();
                
                break;
            case ActorState.Chase:
                //print("CHASE STATE");
                Animator.SetBool("Roaming", false);
                Animator.SetBool("Chasing", true);
                Animator.SetBool("Attacking", false);
                
                Chase(GameController.PlayerController.transform);
                break;
            case ActorState.Attack:
                Animator.SetBool("Roaming", false);
                Animator.SetBool("Chasing", false);
                Animator.SetBool("Attacking", true);
                Attack();
                break;
            default:
                Roam();
                break;
        }
        
	}

    #region Actor States
    //keep moving towards the player
    public void Chase(Transform target)
    {
        Vector3 direction = target.transform.position - this.transform.position;
        direction.y = 0;
        float step = RunSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
    }

    /// <summary>
    /// Select a random navpoint and path to it. Then follow that path
    /// </summary>
    public void Roam()
    {
        StartBoredomTimer();
        if(TargetPath == null)
        {
            while(TargetPath == null)
            {
                Target = PathManager.GetRandomNavPointObject().transform;
                TargetPath = PathManager.NaviagateTo(Target.position);
            }
        }
        if (currentNavPoint < TargetPath.GetLength(0))
        {
            if ((TargetPath[currentNavPoint] - transform.position).magnitude > 1.1f)
            {
                Vector3 direction = TargetPath[currentNavPoint] - transform.position;
                direction.y = 0;
                float step = WalkSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, TargetPath[currentNavPoint], step);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);

            }
            else
            {
                currentNavPoint++;
            }
        }
        else
        {
            TargetPath = null;
            currentNavPoint = 0;
        }
    }

    public void Attack()
    {
        
    }
    #endregion

    public bool PlayerInViewRange()
    {
        var Player = GameController.PlayerController;
        if(Player == null) { return false; }
        var playerTransform = Player.transform;
        if(Vector3.Distance(this.Eyes.transform.position, playerTransform.position) < SightRange)
        {
            Debug.Log("I am in Range of the Player");
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check player is within Actors forward view arc
    /// </summary>
    /// <returns></returns>
    public bool PlayerInDirectView()
    {
        var Player = GameController.PlayerController;
        if(Player == null) { return false; }
        var playerTransform = Player.transform;
        var direction = (playerTransform.position + PlayerFaceHeight)- this.Eyes.transform.position;
        var angle = Vector3.Angle(direction, this.Eyes.transform.forward);
        if(angle < 45 && CanSeePlayerDirectly(direction))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Cast ray to play to ensure he can be seen directly
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CanSeePlayerDirectly(Vector3 direction)
    {
        var PlayerController = GameController.PlayerController;//.gameObject;
        if(PlayerController == null){ return false; }
        var Player = PlayerController.gameObject;
        bool clearSight = false;
        RaycastHit hit;
        if (Physics.Raycast(Eyes.transform.position, direction, out hit, this.SightRange))
        {
            if (hit.collider.gameObject == Player)
            {
                Debug.DrawLine(this.Eyes.transform.position, hit.point);
                clearSight = true;
            }
        }
        return clearSight;
    }

    public bool PlayerInAttackRange()
    {
        var player = GameController.PlayerController;//.transform;
        if(player == null) { return false; }
        var playerTransform = player.transform;
        var distance = (this.transform.position - playerTransform.position).magnitude;
        if(distance <= 1.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if(HP <= 0)
        {
            print("Killed Goblin!");
            Destroy(this.gameObject);
        }
    }

    public void StartBoredomTimer()
    {
       
    }
}
