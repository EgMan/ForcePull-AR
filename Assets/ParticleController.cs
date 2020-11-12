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
        particles.Add(new Particle(Vector3.zero));
        particles.Add(new Particle(cursor.transform.position));
    }

    void FixedUpdate()
    {
        foreach(Particle particle in particles)
        {
        particle.acceleration = (cursor.transform.position - particle.position).normalized*attractSpeed/1000f;
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
            cloud[i].startColor = new Color(255, 0, 0);
            cloud[i].startSize = .5f;
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
        position += velocity;
        acceleration = Vector3.zero;
    }
}