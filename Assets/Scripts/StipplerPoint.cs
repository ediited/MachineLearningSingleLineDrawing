using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StipplerPoint
{
    // Start is called before the first frame update

    private float x;
    private float y;
    private float toX;
    private float toY;
    private float fromX;
    private float fromY;
    private float pointID;
    private GameObject pointGO;
    private Vector3 CentroidPosition;
    float progress;
    Vector3 from;
    Vector3 to;
    public bool paired = false;
    
    public StipplerPoint(float xPos, float yPos, float pointID, GameObject Handler)
    {
        this.PointID = pointID;
        this.X = xPos;
        this.Y = yPos;
        this.PointGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        PointGO.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        PointGO.transform.SetParent(Handler.transform,false);
        PointGO.gameObject.name = pointID.ToString();
        //toX = x;
        //fromX = x;
        //toY = y;
       // fromY = y;

    }

    public float X { get => x; set => x = value; }
    public float Y { get => y; set => y = value; }
    public float PointID { get => pointID; set => pointID = value; }
    public GameObject PointGO { get => pointGO; set => pointGO = value; }
    public Vector3 CentroidPosition1 { get => CentroidPosition; set => CentroidPosition = value; }


 
    public void draw()
    {
       
        if ((toX - x != 0) || (toY - y != 0))
        {
            pointGO.transform.localPosition = Vector3.Lerp(from, to, progress);
            progress += 0.05f;
            //x = pointGO.transform.localPosition.x;
           // y = pointGO.transform.localPosition.y;
        }
        else
        {
            PointGO.transform.localPosition = new Vector3(this.X, 0, this.Y);
        }

    }
    public void moveTo(float targetX, float targetY)
    {
        fromX = x;
        fromY = y;
        from = new Vector3(fromX,0, fromY);
        toX = targetX;
        toY = targetY;
        to = new Vector3(toX, 0,toY);
        progress = 0;
    }

    public float CompareTo(Vector2f comparePart)
    {
        // A null value means that this object is greater.
        if (comparePart == null)
            return 1;

        else
            return Vector2f.DistanceSquare(comparePart, new Vector2f(x, y));
    }
}