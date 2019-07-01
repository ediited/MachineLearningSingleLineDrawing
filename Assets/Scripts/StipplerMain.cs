using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StipplerMain : MonoBehaviour
{
 
    public float amountOfPoints;

    public GameObject stipplerContainer;

    public float[] StipplingAreaBounds;

    public float StipplingStepSize;

    public float imageResolutionWidth;
    
    public float imageResolutionHeight;

    public Texture2D toDrawTexture;

    private float movementFactorX;
    private float movementFactorY;



    public StipplerPoint[] allPoints;

    private Color[] MonitoredPixels;

    void Start()
    {
        movementFactorX =   StipplingAreaBounds[0]/ imageResolutionWidth;
        movementFactorY = StipplingAreaBounds[1] / imageResolutionHeight;

        //instantiate all the Stippler Points;
        allPoints = new StipplerPoint[(int)amountOfPoints];
        for (var i = 0; i < allPoints.Length; i++)
        {

            allPoints[i] = new StipplerPoint(Random.Range(0f, StipplingAreaBounds[0]), Random.Range(0f, StipplingAreaBounds[1]), i, stipplerContainer);
            allPoints[i].draw();

        }
        Debug.Log("created all Stippling Points");

        //get the desired Pixels from the texture
        //and save them into an array 
        MonitoredPixels = new Color[(int)(imageResolutionHeight * imageResolutionWidth)];
         
        for (var i = 0; i < imageResolutionWidth; i++)
        {
            for(var k = 0; k < imageResolutionHeight;k++)
            {
                MonitoredPixels[(int)((k * imageResolutionWidth) + i)] = getColorFromFullImage(i, k);
            }
        }
        Debug.Log("saved all the relevant Pixels");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color getColorFromFullImage(float atx,float aty)
    {
        return toDrawTexture.GetPixel((int)Mathf.Round((atx / imageResolutionWidth) * toDrawTexture.width),(int) Mathf.Round((float)(aty / imageResolutionHeight) * toDrawTexture.height));
    }

    float getBrightnessOfImageAt(float x, float y)
    {
        return ColorToBrightness(MonitoredPixels[(int)((y*imageResolutionWidth)+x)]);
    }
   
    float ColorToBrightness(Color input)
    {
        return (0.2126f * input.r + 0.7152f * input.g + 0.0722f * input.b);
    }
}
