﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.ViliWonka.KDTree;
using System.Linq;
public class ParticleController : MonoBehaviour
{
    public GameObject cursor;
    public ParticleSystem particleSys;
    public SettingsMenu settings;
    public enum State
    {
        FreeFly,
        Boid,
    }
    public State state = State.FreeFly;

    private CursorTracking cursorTracking;
    private List<Particle> particles;
    private KDTree tree;
    private ParticleSystem.Particle[] cloud;
    private ParticleSystem.TrailModule trails;
    private bool bPointsUpdated = false;
    private KDQuery query_thread0;

    void Start()
    {
        particles = new List<Particle>();
        for (int x = -4; x <= 4; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                for (int z = -4; z <= 4; z++)
                {
                    particles.Add(new Particle(new Vector3(x, y, z)));
                }
            }
        }
        tree = new KDTree(particles.Select(x => x.position).ToArray(), 64);
        query_thread0 = new KDQuery();
        trails = particleSys.trails;
        cursorTracking = cursor.GetComponent<CursorTracking>();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < particles.Count(); i++)
        {
            Particle particle = particles[i];
            if (cursorTracking.forcePull) particle.acceleration = (cursor.transform.position - particle.position).normalized * settings.attraction.value / 1000f;
            particle.tick(tree, query_thread0, i, particles, cursor.transform, settings, state);
        }
        SetParticles(particles);
        tree.Rebuild();
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

    public void tick(KDTree tree, KDQuery query, int index, List<Particle> particles, Transform cursor, SettingsMenu settings, ParticleController.State state)
    {
        switch (state)
        {
            case ParticleController.State.Boid:
                tree.Points[index] = position;
                List<int> neighbors = new List<int>();
                query.Radius(tree, position, 1f, neighbors);
                if (neighbors.Count() == 0)
                {
                    break;
                }

                Vector3 avgPosition = Vector3.zero, avgHeading = Vector3.zero, awayHeading = Vector3.zero;
                foreach (int idx in neighbors)
                {
                    if (idx == index)
                    {
                        continue;
                    }
                    Particle neighbor = particles[idx];
                    avgPosition += neighbor.position;
                    avgHeading += neighbor.velocity;
                    awayHeading += (position - neighbor.position).normalized / ((position - neighbor.position).magnitude + 0.01f);
                }

                Vector3 desiredVelocity = Vector3.zero;

                // separation: steer to avoid crowding local flockmates
                desiredVelocity += awayHeading * settings.separation.value;//0.01
                // alignment : steer towards the average heading of local flockmates
                desiredVelocity += avgHeading.normalized * settings.alignment.value;//0.05
                // cohesion : steer to move towards the average position(center of mass) of local flockmates
                desiredVelocity += ((avgPosition / (neighbors.Count() - 1)) - position).normalized * settings.cohesion.value;//0.01

                // boundry : steer towards player if outside boundary sphere
                // TODO : make radius a param
                Vector3 boundaryBearing = cursor.position - position;
                int radius = 15;
                if (boundaryBearing.magnitude > radius)
                {
                    desiredVelocity += boundaryBearing.normalized * 0.1f;
                    // position = position.normalized * -radius;
                }

                // acceleration += desiredVelocity - velocity;
                acceleration += desiredVelocity * 0.01f;

                if (acceleration.magnitude > settings.agility.value)
                {
                    acceleration = acceleration.normalized * settings.agility.value;
                }

                if (velocity.magnitude > settings.speed.value)
                {
                    velocity = velocity.normalized * settings.speed.value;
                }

                break;

            default:
            case ParticleController.State.FreeFly:
                velocity -= 0.005f * velocity; //todo make this a param
                break;
        }

        velocity += acceleration;
        position += velocity;
        acceleration = Vector3.zero;

    }
}