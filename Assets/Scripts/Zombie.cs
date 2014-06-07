using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int AttackDamage = 25;
	public int WallAttackDamage = 5;
    public float AttackRange = 3.0f;
    public AudioClip AttackSound;
    public float AttackSpeed = 2.0f;
    public float ChaseDistance = 10.0f;
    public int Health;
    public float MovementSpeed = 2.5f;
    public float SoundAggroDistance = 20.0f;
    public int WaypointDuration = 10;
    private NavMeshAgent mAgent;
    private bool mChasing;
    private Vector3 mCurrentWaypoint;
    private bool mIsAlive;
	public bool IsStaticZombie;
	private bool mIsChasingPlayer;
    private float mNextAttack;
    private float mNextWaypointPathing;
    private GameObject mPlayer;
	public DestructibleController mDestructibleController;

    private PlayerController mPlayerController;
    private VitalsController mVitalsController;

    public bool IsAlive
    {
        get { return mIsAlive; }
    }


    private void Start()
    {
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mPlayerController = mPlayer.GetComponent<PlayerController>();
        mVitalsController = mPlayer.GetComponent<VitalsController>();
        mIsAlive = true;
        mAgent = GetComponent<NavMeshAgent>();

		if (!IsStaticZombie && mDestructibleController != null) {

			mAgent.SetDestination(mDestructibleController.AttackPosition.position);

				}
    }

	public void SetDestination(Transform dest){

		mAgent.SetDestination (dest.position);

		}

    private void BroadcastZombieHeadshot(PlayerController playerController)
    {
        Health = 0;
        mIsAlive = false;
        Destroy(gameObject);
        print("Zombie has been headshot! Insta-death!");

    }

    private void BroadcastZombieHit(PlayerController playerController)
    {
        WeaponBase weapon = mPlayerController.CurrentWeapon;
        if (weapon != null)
        {
            Health -= weapon.Damage;
            print(string.Format("Zombie shot by {0} for {1} damage! Current health is {2}",
                weapon.name,
                weapon.Damage,
                Health));
        }
    }

    private void BroadcastSoundWaveReceiver(PlayerController playerController)
    {
        if (playerController != null && IsStaticZombie)
        {
            float distance = Vector3.Distance(transform.position, playerController.transform.position);
            if (distance <= SoundAggroDistance && !mChasing)
            {
                AggroToSound(playerController.transform.position);
            }
        }
    }

    private void AggroToSound(Vector3 destination)
    {
        if (mIsAlive) {
						mAgent.SetDestination (destination);
				}
    }

    private void Update()
    {
		float distanceFromPlayer = Vector3.Distance(transform.position, mPlayer.transform.position);
        // If the zombie is alive...
        if (mIsAlive && IsStaticZombie)
        {
            if (((distanceFromPlayer <= ChaseDistance && distanceFromPlayer > AttackRange)) && !mVitalsController.IsDead)
            {
                mChasing = true;
                mAgent.SetDestination(mPlayer.transform.position);   
			} else if (distanceFromPlayer <= AttackRange)
			{
				AttackPlayer ();
			}
            else // Not within range of a player, wander aimlessly.
            {
                mChasing = false;
                if (Time.time > mNextWaypointPathing)
                {
                    mNextWaypointPathing = Time.time + WaypointDuration;

                    mCurrentWaypoint = Random.insideUnitSphere*20 + transform.position;
                    mCurrentWaypoint.y = transform.position.y;
                    transform.LookAt(mCurrentWaypoint);
                }

                // Half the movement speed when not chasing.
                transform.position += transform.forward*(MovementSpeed/2)*Time.deltaTime;
            }
        } else if (mDestructibleController != null){

			if (!mDestructibleController.IsActive){
				mIsChasingPlayer = true;
			} else if (Vector3.Distance (gameObject.transform.position, mDestructibleController.transform.position) < AttackRange){
				AttackWall ();
			}
		
			if (mIsChasingPlayer) {

				if (distanceFromPlayer > AttackRange - 1){
					mAgent.SetDestination(mPlayer.transform.position);
				} else if (Time.time > mNextAttack){
					AttackPlayer ();
				}
			}
		}

		if (Health <= 0)
		{
			Health = 0;
			mIsAlive = false;
			Destroy(gameObject);
		}
	}
	
	void AttackPlayer(){
		
		if (Time.time > mNextAttack)
		{
			mNextAttack = Time.time + AttackSpeed;
			audio.PlayOneShot(AttackSound);
			mVitalsController.CoreVitals.Blood -= AttackDamage;
			print(string.Format("Zombie hit player for {0} damage", AttackDamage));
		}
	}

	void AttackWall(){

		if (Time.time > mNextAttack) {
						mNextAttack = Time.time + AttackSpeed;
						audio.PlayOneShot (AttackSound);
						mDestructibleController.CurrentHealth -= WallAttackDamage;
						print (string.Format ("Zombie hit Wall for {0} damage", AttackDamage));
				}
		}
	
}