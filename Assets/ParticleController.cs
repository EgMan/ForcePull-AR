using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParticleController : MonoBehaviour
{
    public Transform cursor;
    public ParticleSystem particleSys;
    public float attractSpeed;

    private List<Particle> particles;
    private ParticleSystem.Particle[] cloud;
    private bool bPointsUpdated = false;

    void Start()
    {
        particles = new List<Particle>();
        for (int x = -5; x < 5; x++)
        {
            for (int y = -5; y < 5; y++)
            {
                for (int z = -5; z < 5; z++)
                {
                    particles.Add(new Particle(new Vector3(x, y, z)));
                }
            }
        }
    }

    void FixedUpdate()
    {
        bool attr = false;
        foreach (Touch touch in Input.touches)
        {

            if (touch.fingerId == 0)
            {
                attr = true;
            }

            if (touch.fingerId == 1)
            {
            }
        }
        foreach (Particle particle in particles)
        {
            if (attr) particle.acceleration = (cursor.transform.position - particle.position).normalized * attractSpeed / 1000f;
            particle.tick();
        }
        SetParticles(particles);
    }
    void Update()
    {
        if (bPointsUpdated)
         {
            particleSys.SetParticles(cloud, cloud.Length);
            bPointsUpdated = false;
        }
        return;
    }

    public void SetParticles(List<Particle> positions)//, Color[] colors)
    {
        cloud = new ParticleSystem.Particle[positions.Count];

        for (int i = 0; i < positions.Count; ++i)
        {
            cloud[i].position = positions[i].position;
            //cloud[i].startColor = colors[i];
            cloud[i].startColor = new Color(255, 255, 255);
            cloud[i].startSize = .1f;
        }
        bPointsUpdated = true;
    }
}

public class Particle
{
    public Vector3 position, acceleration, velocity;
    public Particle(Vector3 position) : this(position, Vector3.zero, Vector3.zero) { }
    public Particle(Vector3 position, Vector3 velocity, Vector3 acceleration)
    {
        this.position = position;
        this.velocity = velocity;
        this.acceleration = acceleration;
    }

    public void tick()
    {
        velocity += acceleration;
        velocity -= 0.005f * velocity; //todo make this a param
        position += velocity;
        acceleration = Vector3.zero;
    }
}