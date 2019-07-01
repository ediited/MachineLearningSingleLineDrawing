using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class StipplerMain : MonoBehaviour
{

    //unity 3D Variables
    public float amountOfPoints;

    public float stipplingRange;

    public GameObject stipplerContainer;

    public float[] StipplingAreaBounds;

    public float StipplingStepSize;

    public float imageResolutionWidth;

    public float imageResolutionHeight;

    public Texture2D toDrawTexture;

    public bool drawingModePricise = true;

    private float movementFactorX;
    private float movementFactorY;

    public List<StipplerPoint> allPoints = new List<StipplerPoint>();

    private Color[] MonitoredPixels;


    //2D Voronoi Variables 
    // The number of polygons/sites we want
    public int polygonNumber;

    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    private List<Vector2f> points = new List<Vector2f>();
    Rectf bounds;



    void Start()
    {



        movementFactorX = StipplingAreaBounds[0] / imageResolutionWidth;
        movementFactorY = StipplingAreaBounds[1] / imageResolutionHeight;


        bounds = new Rectf(0, 0, StipplingAreaBounds[0], StipplingAreaBounds[1]);

        //instantiate all the Stippler Points;
        //allPoints = new StipplerPoint[(int)amountOfPoints];
        for (var i = 0; i < amountOfPoints; i++)
        {

            allPoints.Add( new StipplerPoint(Random.Range(0f, StipplingAreaBounds[0]), Random.Range(0f, StipplingAreaBounds[1]), i, stipplerContainer));
            allPoints[i].draw();

        }
        Debug.Log("created all Stippling Points");

        //draw them once!

        for (var i = 0; i < allPoints.Count; i++)
        {
            allPoints[i].draw();
        }

        //now copy those Points into the 2D Voronoi
        for (var i = 0; i < allPoints.Count; i++)
        {
            points.Add(new Vector2f(allPoints[i].X, allPoints[i].Y));
        }



        //get the desired Pixels from the texture
        //and save them into an array 
        MonitoredPixels = new Color[(int)(imageResolutionHeight * imageResolutionWidth)];

        for (var i = 0; i < imageResolutionWidth; i++)
        {
            for (var k = 0; k < imageResolutionHeight; k++)
            {
                MonitoredPixels[(int)((k * imageResolutionWidth) + i)] = getColorFromFullImage(i, k);
            }
        }
        Debug.Log("saved all the relevant Pixels");
    }

    // Update is called once per frame

    int onlyEvery5Sec = 0;
    void Update()
    {
        onlyEvery5Sec++;
        if(onlyEvery5Sec == 300)
        {   
            itterateTheVoronoi();

            drawTheVoronoi();

            itterateTheColor();
            onlyEvery5Sec = 0;
            
        }
      for(var i = 0; i < allPoints.Count; i++)
        {
            allPoints[i].draw();
        }
       
    }
    void itterateTheVoronoi()
    {


       
        
        for (var i = 0; i < allPoints.Count; i++)
        {
            points[i] = new Vector2f(allPoints[i].X, allPoints[i].Y);
          
        }
        

        //now Create a Voronoi from it
        Voronoi voronoi = new Voronoi(points, bounds);
        voronoi.LloydRelaxation(1);
        sites = voronoi.SitesIndexedByLocation;
        Debug.Log("created Voronoi"); 

        //Debug.Log("1: old: " + points[0].x + " " + points[0].y);
        //Debug.Log("2: old: " + points[1].x + " " + points[1].y);
        int counterForEach = 0;


        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            points[counterForEach] = new Vector2f(kv.Key.x, kv.Key.y);
            counterForEach++;
        }






    }

    void itterateTheColor()
    {
        for(var i = 0; i < allPoints.Count; i++)
        {
            float currentBrightness = getBrightnessOfImageAt(allPoints[i].X, allPoints[i].Y);
            float currentX = allPoints[i].X;
            float currentY = allPoints[i].Y;
            float darkestPixel = 1;
            float darkestPixelX = currentX;
            float darkestPixelY = currentY;
            for(var j = 0; j < stipplingRange; j++)
            {
                for(var k = 0; k < stipplingRange; k++)
                {
                    try
                    {
                        float tempDarkness = getBrightnessOfImageAt((currentX - stipplingRange + k), currentY - stipplingRange + j);
                        if (tempDarkness < darkestPixel)
                        {
                            darkestPixel = tempDarkness;
                            darkestPixelX = currentX - stipplingRange + k;
                            darkestPixelY = currentY - stipplingRange + j;
                        }
                    }
                    catch
                    {
                    
                    }
                }
            }
            allPoints[i].moveTo(darkestPixelX,darkestPixelY);
        }
    }
    void drawTheVoronoi()
    {

        if (!drawingModePricise)
        {
            for (var i = 0; i < points.Count; i++)
            {
                //7 allPoints[i].X = points[i].x;
                //allPoints[i].Y = points[i].x;
                allPoints[i].moveTo(points[i].x, points[i].y);
            }
        }


        if (drawingModePricise)
        {
            //List<StipplerPoint> tempList = allPoints;

            foreach (KeyValuePair<Vector2f, Site> kv in sites)
            {
                int closesPointId = 0;
                float closesPointDist = 10000;
                for (var i = 0; i < allPoints.Count; i++)
                {
                    if (!allPoints[i].paired)
                    {
                        float distance = Vector2.Distance(new Vector2(allPoints[i].X, allPoints[i].Y), new Vector2(kv.Key.x, kv.Key.y));
                        if (distance < closesPointDist)
                        {
                            closesPointDist = distance;
                            closesPointId = i;
                            allPoints[i].paired = true;
                        }
                    }
                }
                allPoints[closesPointId].moveTo(kv.Key.x, kv.Key.y);
            }
            for(var i = 0; i < allPoints.Count; i++)
            {
                allPoints[i].paired = false;
            }
        }

    }

    
    public Color getColorFromFullImage(float atx, float aty)
    {
        return toDrawTexture.GetPixel((int)Mathf.Round((atx / imageResolutionWidth) * toDrawTexture.width), (int)Mathf.Round((float)(aty / imageResolutionHeight) * toDrawTexture.height));
    }

    float getBrightnessOfImageAt(float x, float y)
    {
        return ColorToBrightness(MonitoredPixels[(int)((y * imageResolutionWidth) + x)]);
    }

    float ColorToBrightness(Color input)
    {
        return (0.2126f * input.r + 0.7152f * input.g + 0.0722f * input.b);
    }
}
