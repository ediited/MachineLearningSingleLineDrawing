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
    private Vector3 CentroidPosition;
    
    public StipplerPoint(float xPos, float yPos, float pointID, GameObject Handler)
    {
        this.PointID = pointID;
        this.X = xPos;
        this.Y = yPos;
        this.PointGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        PointGO.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        PointGO.transform.SetParent(Handler.transform,false);
        PointGO.gameObject.name = pointID.ToString();

    }

    public float X { get => x; set => x = value; }
    public float Y { get => y; set => y = value; }
    public float PointID { get => pointID; set => pointID = value; }
    public GameObject PointGO { get => pointGO; set => pointGO = value; }
    public Vector3 CentroidPosition1 { get => CentroidPosition; set => CentroidPosition = value; }

    public void draw()
    {
        PointGO.transform.localPosition = new Vector3(this.X, 0, this.Y);
    }
}