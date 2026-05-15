using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3.0f;
    private Vector3 targetPosition;

    [Header("Detection Settings")]
    public float detectionRange = 10.0f;
    public float chaseSpeed = 5.0f;

    [Header("Audio Settings")]
    public AudioSource chaseAudioSource;
    public AudioClip chaseMusic;
    [Range(0f, 1f)] public float chaseAudioVolume = 0.5f;
    public AudioSource[] otherAudioSources;
    private bool wasPlayingChaseMusicLastFrame = false;

    private Transform playerTransform;
    private Rigidbody monsterRigidbody;
    private bool isChasing = false;
    private bool isChasingPreviousFrame = false;

    void Start()
    {
        if (pointB != null) targetPosition = pointB.position;

        monsterRigidbody = GetComponent<Rigidbody>();

        // Setup AudioSource for chase music
        if (chaseAudioSource == null)
        {
            chaseAudioSource = gameObject.AddComponent<AudioSource>();
        }
        chaseAudioSource.loop = true;
        chaseAudioSource.playOnAwake = false;

        // Configure Rigidbody for monster movement
        monsterRigidbody.useGravity = true;
        monsterRigidbody.isKinematic = false;
        monsterRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        monsterRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Lock rotation to prevent flipping
        monsterRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange) isChasing = true;
        else if (distanceToPlayer > detectionRange + 2f) isChasing = false;

        // Manage audio when chase state changes
        if (isChasing != isChasingPreviousFrame)
        {
            if (isChasing)
            {
                StartChaseMusic();
            }
            else
            {
                StopChaseMusic();
            }
            isChasingPreviousFrame = isChasing;
        }

        if (isChasing) ChasePlayer();
        else Patrol();
    }

    void Patrol()
    {
        if (pointA == null || pointB == null) return;

        MoveTowardsTarget(targetPosition, patrolSpeed);

        // Compare positions on X and Z axis only (ignore Y)
        Vector3 flatPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatTarget = new Vector3(targetPosition.x, 0, targetPosition.z);

        if (Vector3.Distance(flatPos, flatTarget) < 0.5f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void ChasePlayer()
    {
        MoveTowardsTarget(playerTransform.position, chaseSpeed);
    }

    void MoveTowardsTarget(Vector3 target, float speed)
    {
        // Calculate direction toward target
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; // Keep movement on horizontal plane

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        // Move toward target
        Vector3 movePos = Vector3.MoveTowards(transform.position,
                          new Vector3(target.x, transform.position.y, target.z),
                          speed * Time.deltaTime);

        transform.position = movePos;
    }

    /// <summary>
    /// Starts playing chase music and stops all other audio
    /// </summary>
    void StartChaseMusic()
    {
        if (chaseMusic == null)
        {
            Debug.LogWarning("Chase music not assigned to " + gameObject.name);
            return;
        }

        // Stop all other audio sources
        if (otherAudioSources != null)
        {
            foreach (AudioSource audioSource in otherAudioSources)
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        // Stop Horror Sound if found in scene
        GameObject horrorSoundObj = GameObject.Find("Horror Sound");
        if (horrorSoundObj != null)
        {
            AudioSource horrorAudio = horrorSoundObj.GetComponent<AudioSource>();
            if (horrorAudio != null && horrorAudio.isPlaying)
            {
                horrorAudio.Stop();
            }
        }

        // Play chase music
        if (chaseAudioSource != null)
        {
            chaseAudioSource.clip = chaseMusic;
            chaseAudioSource.volume = chaseAudioVolume;
            if (!chaseAudioSource.isPlaying)
            {
                chaseAudioSource.Play();
            }
        }

        wasPlayingChaseMusicLastFrame = true;
    }

    /// <summary>
    /// Stops chase music and resumes main music
    /// </summary>
    void StopChaseMusic()
    {
        if (chaseAudioSource != null && chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Stop();
        }

        // Resume Horror Sound
        GameObject horrorSoundObj = GameObject.Find("Horror Sound");
        if (horrorSoundObj != null)
        {
            AudioSource horrorAudio = horrorSoundObj.GetComponent<AudioSource>();
            if (horrorAudio != null && !horrorAudio.isPlaying)
            {
                horrorAudio.Play();
            }
        }

        wasPlayingChaseMusicLastFrame = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Call Respawn from PlayerController
            var pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null) pc.Respawn();

            // Stop chasing and reset position
            isChasing = false;
            isChasingPreviousFrame = false;
            StopChaseMusic();
            
            float distToA = Vector3.Distance(transform.position, pointA.position);
            transform.position = (distToA < Vector3.Distance(transform.position, pointB.position)) ? pointA.position : pointB.position;
        }
    }
}
