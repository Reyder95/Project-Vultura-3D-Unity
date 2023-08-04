using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldComponent : MonoBehaviour
{
    public GameObject asteroidPrefab;

    public void GenerateField(List<string> oreKeys)
    {
        List<Ore> ores = new List<Ore>();

        foreach (string ore in oreKeys)
        {
            BaseItem newItem = ItemManager.GenerateSpecificBase(ore);

            if (newItem is Ore)
            {
                ores.Add((Ore)newItem);

                GameObject asteroid = Instantiate(asteroidPrefab, new Vector3(gameObject.transform.position.x + Random.Range(-50, 50), gameObject.transform.position.y + Random.Range(-50, 50), gameObject.transform.position.z + Random.Range(-50, 50)), Quaternion.identity);

                asteroid.GetComponent<Asteroid>().SetContent((Ore)newItem);

                //VulturaInstance.AddToSystem(asteroid.GetComponent<Asteroid>().CurrAsteroid); 
                asteroid.GetComponent<Asteroid>().CurrAsteroid.selectableObject = asteroid;
            }
        }
    }
}
