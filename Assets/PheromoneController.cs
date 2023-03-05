using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone {
    public Vector2 position;
    public float strength;
}

public class AntPosition {
    public Vector2 position;
    public float sensorStrength;
}


public class PheromoneController : MonoBehaviour
{
    public ComputeShader computeShader;

    public Dictionary<Vector2, List<Pheromone>> allBluePheromones = new Dictionary<Vector2, List<Pheromone>>();
    public Dictionary<Vector2, List<Pheromone>> allRedPheromones = new Dictionary<Vector2, List<Pheromone>>();

    static float radius = 0.45f;
    private Pheromone[] pheromoneData;

    void Start()
    {
        //useShader(new Vector2(10, 10));


        for (int x= -100; x < 100; x++) {
            for (int y = -100; y < 100; y++)
            {
                allBluePheromones.Add(new Vector2(x, y), new List<Pheromone>());
                allRedPheromones.Add(new Vector2(x, y), new List<Pheromone>());
            }
        }

    }
/*
    public float useShader(Vector2 antPosition, Pheromone[] pheromoneData) {
        if (pheromoneData.Length == 0) {
            return 0;
        }

        int vectorSize = sizeof(float)*3;
        int totalSize = vectorSize;
        ComputeBuffer pheromoneBuffer = new ComputeBuffer(pheromoneData.Length, totalSize);
        pheromoneBuffer.SetData(pheromoneData);
        computeShader.SetBuffer(0, "pheromones", pheromoneBuffer);

        ComputeBuffer antBuffer = new ComputeBuffer(1, sizeof(float)*3);
        AntPosition[] antData = new AntPosition[1];
        antData[0].position = antPosition;
        antData[0].sensorStrength = 37;
        antBuffer.SetData(antData);
        computeShader.SetBuffer(0, "antPosition", antBuffer);


        computeShader.Dispatch(0, pheromoneData.Length % 8, pheromoneData.Length % 8, pheromoneData.Length % 8);

        pheromoneBuffer.GetData(pheromoneData);
        pheromoneBuffer.Dispose();
        antBuffer.Dispose();
        float res = 0;

        foreach (Pheromone p in pheromoneData) {
    
            res += p.strength;
        }

        return res;
        /*Debug.Log("Shader says: " + antData[0].position.ToString());
        Debug.Log("Shader says: " + pheromoneData[5].position.ToString());
        Debug.Log("Shader says: " + pheromoneData[1].position.ToString());
        Debug.Log("Shader says: " + pheromoneData[6600].position.ToString());*/
       // buffer.Dispose();
//    }

    public void addPheromone(ref Pheromone pheromone, PheromoneType type) {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++)
            {
                List<Pheromone> pheromones = getPheromonesByType(type)[new Vector2((int)(pheromone.position.x / radius) - x, (int)(pheromone.position.y / radius) - y)];
                pheromones.Add(pheromone);
                if (pheromones.Count > 200) {
                    //Debug.Log("set phero to 0");
                    Pheromone pheromone1 = pheromones[Random.Range(0, pheromones.Count)];
                    pheromone1.strength = 0;
                    //pheromones.Remove(pheromone1);
                }
                //Debug.Log("wasadded:  x: " + ((int)(pheromone.position.x / radius) - x) + "  y: " + ((int)(pheromone.position.y / radius) - y));
            }
        }
    }

    public void removePheromone(ref Pheromone pheromone, PheromoneType type) {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
               // Debug.Log("before remove map: " + getPheromonesByType(type)[new Vector2((int)(pheromone.position.x / radius) - x, (int)(pheromone.position.y / radius) - y)].Count);
                bool removed = getPheromonesByType(type)[new Vector2((int)(pheromone.position.x / radius) - x, (int)(pheromone.position.y / radius) - y)].Remove(pheromone);
                
                
                //Debug.Log("wasremoved: " + removed + "  x: " +((int)(pheromone.position.x / radius) - x) + "  y: " + ((int)(pheromone.position.y / radius) - y));
                //Debug.Log(removed + "after remove map: " + getPheromonesByType(type)[new Vector2((int)(pheromone.position.x / radius) - x, (int)(pheromone.position.y / radius) - y)].Count);
            }
        }
    }

    public float getPheromoneStrength(Vector2 position, PheromoneType type) {
        float pheromoneStrength = 0;
        
        foreach (Pheromone pheromone in getPheromonesByType(type)[new Vector2((int)(position.x / radius), (int)(position.y / radius))]) {
            if (Vector2.Distance(pheromone.position, position) < radius) {
                pheromoneStrength += pheromone.strength;


            }
        }
        

        //pheromoneStrength = useShader(position, getPheromonesByType(type)[new Vector2((int)(position.x / radius), (int)(position.y / radius))].ToArray());
        
        return pheromoneStrength;
    }

    private Dictionary<Vector2, List<Pheromone>> getPheromonesByType(PheromoneType type) {
        if (type.Equals(PheromoneType.PLAYERCOLOR))
        {
            return allBluePheromones;
        }
        else
        {
            return allRedPheromones;
        }
    }
}
