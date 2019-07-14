using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BananaArea : Area
{
    public GameObject Agent;
    public GameObject banana;
    public GameObject badBanana;
    public int numBananas;
    public int numBadBananas;
    public bool respawnBananas;
    public float range;

    public float bananaCounter = 0;
    private float bananaAmount;
    // Use this for initialization
    void Start()
    {
        bananaAmount = GameObject.Find("Ground").GetComponent<voronoiSample>().polygonNumber;
        Agent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (bananaCounter >= bananaAmount)
        {
            Agent.SetActive(false);
        }
    }

    void CreateBanana(int numBana, GameObject bananaType)
    {
        for (int i = 0; i < numBana; i++)
        {
            GameObject bana = Instantiate(bananaType, new Vector3(Random.Range(-range, range), 1f,
                                                              Random.Range(-range, range)) + transform.position,
                                          Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            bana.GetComponent<BananaLogic>().respawn = respawnBananas;
            bana.GetComponent<BananaLogic>().myArea = this;
            
        }
    }


    public void CreateBananaAt(int numBana, GameObject bananaType,Vector3 pos)
    {
        for (int i = 0; i < numBana; i++)
        {
            Vector3 mappedToRange = new Vector3(
                map(pos.x, 0, 512, -range, range),
                3,
                map(pos.z, 0, 512, -range, range)
                );
            

            GameObject bana = Instantiate(bananaType, mappedToRange + transform.position,
                                          Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            bana.GetComponent<BananaLogic>().respawn = respawnBananas;
            bana.GetComponent<BananaLogic>().myArea = this;

        }
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
    public void ResetBananaArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                                                       Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        CreateBanana(numBananas, banana);
        CreateBanana(numBadBananas, badBanana);
    }

    public override void ResetArea()
    {
    }
}
