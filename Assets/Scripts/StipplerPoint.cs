using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StipplerPoint
{
    // Start is called before the first frame update

    private float x;
    private float y;
    private float pointID;
    private GameObject pointGO;
    
    public StipplerPoint(float xPos, float yPos, float pointID, GameObject Handler)
    {
        this.pointID = pointID;
        this.x = xPos;
        this.y = yPos;
        this.pointGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointGO.transform.SetParent(Handler.transform,false);
        pointGO.gameObject.name = pointID.ToString();

    }

    public void draw()
    {
        pointGO.transform.position = new Vector3(this.x, 0, this.y);
    }
}