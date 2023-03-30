using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float repathDist=2;
    [SerializeField] float attackRange=7.5f;
    Vector3 posCache;
    private NavMeshAgent navMeshAgent;
    public enum moveType { APPROACH, ASLEEP, RANDOM };
    [SerializeField]
    public moveType move = moveType.ASLEEP;
    moveType moveCache = moveType.ASLEEP;
    [SerializeField] bool autoSleep = true;
    [SerializeField] float randomRadius;
    [SerializeField] Gun1 gun;
    [SerializeField] float lookSpeed;
    [SerializeField] LayerMask randomLayerMask = -1;
    [SerializeField] float wakeTime = 1f;
    [SerializeField] bool lookAtPlayer = true;
    [SerializeField] bool drawPath = true;
    [SerializeField] Transform turningPart;
    NavMeshPath navMeshPath;
    NavMeshPath navMeshPath2;
    [SerializeField] LineRenderer lr;
    bool waking;
    bool wokeOnce=false;
    [SerializeField]
    private DungeonRoom attachedRoom;
    // Start is called before the first frame update
    void Start()
    {
        if(turningPart==null)turningPart = this.transform;
        navMeshPath = new NavMeshPath();
        navMeshPath2 = new NavMeshPath();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("MainCharacter").GetComponent<Transform>();
        if (autoSleep)
        {
            moveCache = move;
            move = moveType.ASLEEP;
        }
        if (move != moveType.ASLEEP) RandomTarget();

        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame

    public void AttachRoom(DungeonRoom room)
    {
        attachedRoom = room;
    }

    public DungeonRoom GetAttachedRoom()
    {
        return attachedRoom;
    }

    private void FixedUpdate()
    {
        if (navMeshAgent.path.corners.Length > 0 && drawPath)
        {
            lr.positionCount = navMeshAgent.path.corners.Length;
            for (int i = 0; i < navMeshAgent.path.corners.Length; i++)
            {
                lr.SetPosition(i, navMeshAgent.path.corners[i]);
            }
        }// draws the path

        //code below determines if player is reachable, awakens if they are, sleeps if they arent
        Vector3 playerpt = RandomNavSphere(player.position, randomRadius, randomLayerMask);
        navMeshAgent.CalculatePath(player.position, navMeshPath2);
        if (navMeshPath2.status == NavMeshPathStatus.PathComplete && move == moveType.ASLEEP && autoSleep && !waking)
        {
            StartCoroutine(awaken());
        }
        else if (navMeshPath.status != NavMeshPathStatus.PathComplete && move != moveType.ASLEEP && autoSleep)
        {
            moveCache = move;
            move = moveType.ASLEEP;
        }
        if (move != moveType.ASLEEP)
        {
            if ((navMeshAgent.remainingDistance < repathDist && move == moveType.RANDOM) || (move == moveType.APPROACH && Vector3.Distance(navMeshAgent.destination, player.position) > randomRadius + 0.5))
            {
                RandomTarget();//if we're close to the target position or the player has significantly changed position, recalculate target pos.
            }

            if (Vector3.Distance(transform.position, player.position) < attackRange)//attacks player when in range
            {
                if (lookAtPlayer)//might not necessarily want to shoot at player
                {
                    FaceTarget(player.position);

                }
                if (gun != null)
                {
                    gun.Shoot();
                    //Debug.Log("shot");
                }
            }
        }
    }

    void RandomTarget()
    {
        Vector3 ori = (move == moveType.RANDOM) ? this.transform.position : player.position;
        Vector3 randompt = RandomNavSphere(ori, randomRadius, randomLayerMask);
        //Debug.Log("pathing");
        do//checking if the point is actually reachable(only works in random, theoretically if the player radius is too high it may mess up and try to go off screen but I'm stupid and this works okay)
        {
            randompt = RandomNavSphere(ori, randomRadius, randomLayerMask);
            navMeshAgent.CalculatePath(randompt, navMeshPath);
        
        
        } while (navMeshPath.status != NavMeshPathStatus.PathComplete && move == moveType.RANDOM);
        
        navMeshAgent.destination = randompt;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)//gets random point in radius
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        if(NavMesh.SamplePosition(randomDirection, out navHit, 4, layermask))
            return navHit.position;
        else return origin;//failsafe
    }
    private void FaceTarget(Vector3 destination)//looks at target instead of move direction, clunky as hell but it'll do the trick
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        turningPart.rotation = Quaternion.Slerp(turningPart.rotation, rotation, Time.deltaTime * lookSpeed);
    }
    IEnumerator awaken()
    {
        waking = true;
        if (wokeOnce == false)
            yield return new WaitForSeconds(wakeTime);
        else
            yield return new WaitForSeconds(0.1f);
        wokeOnce = true;
        move = moveCache;
        RandomTarget();
        waking = false;
    }
}
