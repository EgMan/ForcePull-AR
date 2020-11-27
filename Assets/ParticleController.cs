using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.ViliWonka.KDTree;
using System.Linq;
public class ParticleController : MonoBehaviour
{
    public Transform cursor;
    public ParticleSystem particleSys;
    public float attractSpeed;

    private List<Particle> particles;
    private KDTree tree;
    private ParticleSystem.Particle[] cloud;
    private bool bPointsUpdated = false;
    private KDQuery query_thread0;

    void Start()
{
        particles = new List<Particle>();
        for (int x = -4; x < 4; x++)
        {
            for (int y = -4; y < 4; y++)
            {
                for (int z = -4; z < 4; z++)
                {
                    particles.Add(new Particle(new Vector3(x, y, z)));
                }
            }
        }
        tree = new KDTree(particles.Select(x => x.position).ToArray(), 16);
        query_thread0 = new KDQuery();
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

        if (Input.GetKey(KeyCode.Space))
        {
            attr = true;
        }

        for (int i = 0; i < particles.Count(); i++)
        {
            Particle particle = particles[i];
            if (attr) particle.acceleration = (cursor.transform.position - particle.position).normalized * attractSpeed / 1000f;
            particle.tick(tree, query_thread0, i);
        }
        SetParticles(particles);
        tree.Rebuild();

        // TODO remove
        List<int> results = new List<int>();
        query_thread0.Radius(tree, particles[0].position, .5f, results);
        foreach(var part in particles)
        {
            part.color = Color.red;
        }
        foreach(var idx in results)
        {
            particles[idx].color = Color.green;
        }
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

    public void SetParticles(List<Particle> points)//, Color[] colors)
    {
        cloud = new ParticleSystem.Particle[points.Count];

        for (int i = 0; i < points.Count; ++i)
        {
            cloud[i].position = points[i].position;
            //cloud[i].startColor = colors[i];
            cloud[i].startColor = points[i].color;
            cloud[i].startSize = .1f;
        }
        bPointsUpdated = true;
    }
}

public class Particle
{
    public Vector3 position, acceleration, velocity;
    public Color color;
    public List<Vector3> deleteme = new List<Vector3>();
    public Particle(Vector3 position) : this(position, Vector3.zero, Vector3.zero, Color.white) { }
    public Particle(Vector3 position, Vector3 velocity, Vector3 acceleration, Color color)
    {
        this.position = position;
        this.velocity = velocity;
        this.acceleration = acceleration;
        this.color = color;
    }

    public void tick(KDTree tree, KDQuery query, int index)
    {
        velocity += acceleration;
        velocity -= 0.005f * velocity; //todo make this a param
        position += velocity;
        acceleration = Vector3.zero;

        // TODO only do this if we're in the boid state.  Also, consume results.
        tree.Points[index] = position;
        List<int> results = new List<int>();
        query.Radius(tree, position, .5f, results);

        // foreach (var a in tree.Points)
        // {
        //     if ((a - position).magnitude < 0.5f)
        //     {
        //         deleteme.Add(a);
        //     }
        // }
    }
}