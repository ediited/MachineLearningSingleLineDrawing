using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaLogic : MonoBehaviour {

    public bool respawn;
    public BananaArea myArea;
    public GameObject badBanana;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    void CreateBanana(int numBana, GameObject bananaType)
    {
        for (int i = 0; i < numBana; i++)
        {
            GameObject bana = Instantiate(bananaType, new Vector3(this.transform.localPosition.x, 4f,
                                                              this.transform.localPosition.z) + transform.position,
                                          Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            bana.GetComponent<BananaLogic>().respawn = false ;
            bana.GetComponent<BananaLogic>().myArea = myArea;
            bana.transform.position = this.gameObject.transform.position;
            bana.transform.position = new Vector3(bana.transform.position.x, 4, bana.transform.position.z);
         
        }
    }


    public void OnEaten() {     
        if (respawn) 
        {
            {

                myArea.bananaCounter++;
                badBanana = myArea.badBanana;
                CreateBanana(1, badBanana);

                Destroy(gameObject);
            }
            /*
            transform.position = new Vector3(Random.Range(-myArea.range, myArea.range), 
                                             transform.position.y + 3f, 
                                             Random.Range(-myArea.range, myArea.range));
            */    
    }
        else 
        {
            
        }
    }
    void replaceWithBadBanana() {
       
    }
}
