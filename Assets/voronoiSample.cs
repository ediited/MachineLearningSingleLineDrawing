using System.Collections.Generic;
using UnityEngine;

using csDelaunay;

public class voronoiSample : MonoBehaviour
{

    // The number of polygons/sites we want
    public int polygonNumber = 200;

    List<Vector2f> points;

    Voronoi voronoi;

    Rectf bounds;

    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;



    //base Image
    public Texture2D fromImage;


    //Pixel Checking Range
    public float cR;

    

    //width and height
    private float bW;
    private float bH;

    void Start()
    {
        bW = fromImage.width;
        bH = fromImage.height;

        // Create your sites (lets call that the center of your polygons)
        points = CreateRandomPoint();

        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        bounds = new Rectf(0, 0, bW, bH);

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        voronoi = new Voronoi(points, bounds);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;

        // DisplayVoronoiDiagram();


        InvokeRepeating("GetCloserWrapper", 2.0f, 5f);
        //GetCloser(true);
    }

    public float MaxIterations;
    private float currIt = 0;
    void GetCloserWrapper()
    {
        if (currIt < MaxIterations)
        {
            currIt++;
            GetCloser();
        }
    }

   
    void GetCloser()
    {
        
        Debug.Log("starting next Itteration");
        //set new sites as Points
        int iC = 0;
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            points[iC] = new Vector2f(kv.Key.x, kv.Key.y);
            iC++;
        }
        Debug.Log("Points from Sites");


        //move those Points towards a random darker Pixel
        for (int i = 0; i < points.Count; i++)
        {
            float currentBrigthness = ColorToBrightness(fromImage.GetPixel(Mathf.RoundToInt(points[i].x), Mathf.RoundToInt(points[i].y)));
            float frX = points[i].x;
            float frY = points[i].y;
            List < Vector2f > darkerPixels = new List<Vector2f>();

            for (int xPix = 0; xPix < cR*2; xPix++)
            {
                for (int yPix = 0; yPix < cR*2; yPix++)
                {

                    if(frX-cR+xPix>0 && frX - cR + xPix < bW && frY-cR+yPix > 0 && frY - cR + yPix < bH)
                       
                    {
                        float toCom = ColorToBrightness(fromImage.GetPixel(Mathf.RoundToInt(frX - cR + xPix), Mathf.RoundToInt(frY - cR + yPix)));
                        if (toCom < currentBrigthness)
                        {
                            darkerPixels.Add(new Vector2f(frX - cR + xPix, frY - cR + yPix));
                        }
                    }
                   
                }
            }

            if (darkerPixels.Count > 0) {
                int nPi = Mathf.FloorToInt(Random.Range(0, darkerPixels.Count));


                points[i] = new Vector2f(darkerPixels[nPi].x, darkerPixels[nPi].y);

            }
        }
        Debug.Log("moved to darker pixels");

        voronoi = new Voronoi(points, bounds);
        voronoi.LloydRelaxation(1);
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;


        Debug.Log("new Diagram and Relaxed");

        DisplayVoronoiDiagram();
        Debug.Log("Draw Diagram");

        
    }

    private float ColorToBrightness(Color input)
    { 
        return (0.2126f * input.r + 0.7152f * input.g + 0.0722f * input.b);
    }
    private List<Vector2f> CreateRandomPoint()
    {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++)
        {
            points.Add(new Vector2f(Random.Range(0, 512), Random.Range(0, 512)));
        }

        return points;
    }

    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D(512, 512);
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            //tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
            tx.DrawCircle(Color.black, (int)kv.Key.x, (int)kv.Key.y, 2);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

           // DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        this.GetComponent<MeshRenderer>().material.mainTexture = tx;
    }





    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}

