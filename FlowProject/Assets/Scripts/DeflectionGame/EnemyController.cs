using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float acceleration;
    public float speedMax;
    public float draftSensitivity;
    public float draftSensitivityParticles;
    public Vector2 sizeRange;
    public float sizeInitial;
    // max number of children spawned upon splitting
    public int splitDensity;
    // percentage of max size required for splitting
    public float splitThreshold;
    public float splitDeflectionSpeedThreshold;
    // children's average percentage size of parent enemy
    public float splitSize;
    public float splitDelay;

    [SerializeField]
    public Color colorMax;
    public Color colorMin;

    public GameObject pf_particlesSplit;

    private GameObject target;
    private Rigidbody rb;
    private Renderer renderer;
    private float size;
    private Color color;
    private float splitTime;
    private ParticleSystem.Particle[] particlesArray;


    // Use this for initialization
    void Start () {

        splitTime = Time.time + splitDelay;
        
	}
	
    public void Initialize(GameObject target, Vector3 pos, float size)
    {
        this.target = target;
        this.size = Mathf.Clamp(size, 0f, 1f);
        float rgbVals = size;
        this.color = Color.Lerp(colorMin, colorMax, size);
        this.transform.position = pos;
        this.transform.localScale = Vector3.one * (size * (sizeRange.y - sizeRange.x) + sizeRange.x);
        Debug.Log(transform.localScale);
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_EmissionColor", color);

        particlesArray = new ParticleSystem.Particle[100];
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
    }

    // split the enemy if it gets popped OLD
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "hand_deflector")
        {
            HandDeflectorBehavior handDeflector = other.GetComponent<HandDeflectorBehavior>();
            Split(handDeflector);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hand_deflector")
        {
            HandDeflectorBehavior handDeflector = collision.gameObject.GetComponent<HandDeflectorBehavior>();
            HandleDeflectorContact(handDeflector);
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
            return;

        Vector3 targetDirection = target.GetComponent<Transform>().position - transform.position;
        targetDirection = targetDirection.normalized;
        rb.AddForce(targetDirection * Time.deltaTime * acceleration);
        if (rb.velocity.magnitude > speedMax)
        {
            rb.velocity = rb.velocity.normalized * speedMax;
        }
    }

    // handles splitting and deflecting when deflector contacts enemy
    private void HandleDeflectorContact(HandDeflectorBehavior handDeflector)
    {

        if (handDeflector.velocityDeflector.magnitude > splitDeflectionSpeedThreshold)
        {
            float sizeDifference = size - handDeflector.handSize;

            // makes sure the user's deflectors are a valid size to pop the enemy
            if ((size < 0.5f && sizeDifference > 0)
                || (size >= 0.5f && sizeDifference < 0))
            {
                Split(handDeflector);
            }
        }
    }

    // handles the enemy splits
    private void Split(HandDeflectorBehavior handDeflector)
    {
        // prevent from splitting immediately after a previous split
        if (Time.time < splitTime)
            return;

        // only creates children if of sufficient size
        // if the enemy is too small to split, it is destroyed
        Debug.Log("CURRENT SIZE: " + size);
        if (size >= splitThreshold)
        {
            // calculate how many children to spawn based on current size
            int numChildren = (int)(splitDensity * size);

            // adds randomness to spawn position relative to parent enemy
            float noiseVal = size / 2f;

            // hard cap the max amount for now
            if (numChildren > 4)
                numChildren = 4;

            // for each child
            for (int i = 0; i < numChildren; i++)
            {
                // find spawn position
                Vector3 posNoise = new Vector3(
                    Random.Range(-noiseVal, noiseVal),
                    Random.Range(-noiseVal, noiseVal),
                    Random.Range(-noiseVal, noiseVal));
                Vector3 childPos = transform.position + posNoise;

                // new size of the child
                float childSize = size * splitSize;

                // create a copy of this parent enemy
                GameObject childEnemy = Instantiate(this.gameObject);

                // initialize its data
                EnemyController childController = childEnemy.GetComponent<EnemyController>();
                childController.Initialize(target, childPos, childSize);

                // give it movement due to the deflectors
                childController.Draft(handDeflector.velocityDeflector);
            }

            // play particle effect
            GameObject particlesSplit = Instantiate(pf_particlesSplit, transform.position, transform.rotation);
            particlesSplit.transform.Find("Sphere").GetComponent<Rigidbody>().AddForce(handDeflector.velocityDeflector * draftSensitivityParticles);
            particlesSplit.transform.Find("Small Particles").GetComponent<Rigidbody>().AddForce(handDeflector.velocityDeflector * draftSensitivity);
        }

        Destroy(this.gameObject);
    }

    public void Draft(Vector3 force)
    {
        rb.AddForce(force * draftSensitivity);
    }
}
