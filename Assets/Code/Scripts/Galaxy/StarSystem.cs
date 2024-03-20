using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem
{
    public string system_name;
    public string star; // Needs a Star class
    public List<SystemEntity> systemEntities = new List<SystemEntity>();
    public List<SystemGate> systemGates = new List<SystemGate>();

    public StarSystem(int size, string system_name, string star, SystemPlanet[] systemPlanets, SystemAsteroid[] systemAsteroids)
    {
        this.system_name = system_name;
        this.star = star;

        foreach (SystemPlanet planet in systemPlanets)
        {
            float radius = ((float)planet.radius / 100.0f) * size;
            float x = radius * Mathf.Sin(Mathf.PI * 2 * planet.angle / 360);
            float y = radius * Mathf.Cos(Mathf.PI * 2 * planet.angle / 360);
            float z = (planet.height / 100) * size;

            Vector3 planetPosition = new Vector3(x, y, z);
            
            this.systemEntities.Add(new SystemEntity(planetPosition, null, planet.name, false, planet.type[0].ToString().ToUpper() + planet.type.Substring(1) + " Planet"));

            foreach (SystemBody body in planet.bodies)
            {
                float radiusInner = 10000;
                float xInner = planetPosition.x + (radiusInner * Mathf.Sin(Mathf.PI * 2 * body.angle / 360));
                float yInner = planetPosition.y + (radiusInner * Mathf.Cos(Mathf.PI * 2 * body.angle / 360));

                this.systemEntities.Add(new SystemEntity(new Vector3(xInner, yInner, z), null, body.name, false, body.sub_type));
            }
        }

        foreach(SystemAsteroid asteroid in systemAsteroids)
        {
            float radius = ((float)asteroid.radius / 100.0f) * size;
            float x = radius * Mathf.Sin(Mathf.PI * 2 * asteroid.angle / 360);
            float y = radius * Mathf.Cos(Mathf.PI * 2 * asteroid.angle / 360);
            float z = (asteroid.height / 100) * size;

            List<SystemEntity> subEntities = new List<SystemEntity>();

            SystemEntity asteroidFieldEntity = new SystemEntity(new Vector3(x,y,z), null, asteroid.name, false, "Asteroid Field");

            foreach (SystemSubBody subBody in asteroid.sub_bodies)
            {
                SystemEntity newEntity = new SystemEntity(new Vector3(0, 0, 0), null, subBody.type + " Asteroid", false, "Asteroid");
                newEntity.mainEntity = asteroidFieldEntity;
                subEntities.Add(newEntity);
                
            }

            
            asteroidFieldEntity.subEntities = subEntities;

            this.systemEntities.Add(asteroidFieldEntity);
        }
    }

    public override int GetHashCode()
    {
        return system_name.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is StarSystem other)
        {
            return system_name.Equals(other.system_name);
        }

        return false;
    }

    public void AddGate(SystemGate gate)
    {
        systemGates.Add(gate);
    }
}