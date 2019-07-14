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

    public float imageResolutionWidth;

    public float imageResolutionHeight;

    public Texture2D toDrawTexture;

    public bool drawingModePricise = true;

    private float movementFactorX;
    private float movementFactorY;

    public List<StipplerPoint> allPoints = new List<StipplerPoint>();

    private Color[] MonitoredPixels;

    private float baseImageWidth;
    private float baseImageHeight;



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
        baseImageWidth = toDrawTexture.width;
        baseImageHeight = toDrawTexture.height;


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

            itterateTheColor();

            drawTheVoronoi();

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
        
        //call the Llyod Relaxiation Function
        voronoi.LloydRelaxation(1);
        sites = voronoi.SitesIndexedByLocation;
        Debug.Log("created Voronoi"); 



        //and turn it back into Points
        int counterForEach = 0;
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            points[counterForEach] = new Vector2f(kv.Key.x, kv.Key.y);
            counterForEach++;
        }
    }

    void itterateTheColor()
    {
        for(var i = 0; i < points.Count; i++)
        {

            float currentX = points[i].x;
            float currentY = points[i].y;
            float darkestPixel = 1;
            float darkestPixelX = currentX;
            float darkestPixelY = currentY;
            for(float j = 0; j < stipplingRange; j+= 0.1f)
            {
                for(float k = 0; k < stipplingRange; k+= 0.1f)
                {
                    try
                    {
                        float tempDarkness = getBrightnessAtObjectPoint(points[i].x-stipplingRange+k, points[i].y-stipplingRange+j);
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
            //Debug.Log(darkestPixel);
            points[i] = new Vector2f(darkestPixelX, darkestPixelY);
        }
    }
    void drawTheVoronoi()
    {

        if (!drawingModePricise)
        {
            for (var i = 0; i < points.Count; i++)
            {
                allPoints[i].moveTo(points[i].x, points[i].y);
            }
        }



    }


    public float[] pixelToCoord(float pixelX, float PixelY)
    {
        float[] writerArray = new float[2];

   

        return writerArray;
    }


    public List<PixelWithColor> getSurroundingPixels()
    {
        List<PixelWithColor> writerList = new List<PixelWithColor>();


        return writerList;
    }

    public float getBrightnessAtObjectPoint(float xPoint, float yPoint)
    {

        if(xPoint<0||xPoint > StipplingAreaBounds[0] || yPoint< 0 || yPoint > StipplingAreaBounds[1])
        {
            return 2;
        }
        //relative Position to Drawing Area Size
        float pcPosX = xPoint / StipplingAreaBounds[0];
        float pcPosY = yPoint / StipplingAreaBounds[0];
        //relative Position in Image Context;
        float relPosX = pcPosX * (baseImageWidth / imageResolutionWidth);
        float relPosY = pcPosY * (baseImageHeight / imageResolutionHeight);
        return ColorToBrightness(toDrawTexture.GetPixel(Mathf.RoundToInt(relPosX), Mathf.RoundToInt(relPosY)));
    }

    


    public Color getColorFromFullImage(float atx, float aty)
    {
        return toDrawTexture.GetPixel((int)Mathf.Round((atx / imageResolutionWidth) * toDrawTexture.width), (int)Mathf.Round((float)(aty / imageResolutionHeight) * toDrawTexture.height));
    }

    float ColorToBrightness(Color input)
    {
        return (0.2126f * input.r + 0.7152f * input.g + 0.0722f * input.b);
    }


    public class PixelWithColor
    {
        Color color;
        Vector2f coordinates;
        public PixelWithColor(Color setColor, Vector2f setCoord)
        {
            color = setColor;
            coordinates = setCoord;
        }
    }
}