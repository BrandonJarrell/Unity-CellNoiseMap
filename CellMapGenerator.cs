using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class CellMapGenerator : MonoBehaviour
{
   // scale of the array and image
   public int cellMapWidth;

   // Remembers the last time the update script was ran
   private float timeToUpdate;

   // Array of floats, how far they are from the closest point
   private float[,] imageArray;

   // the RawImage's gameObject
   public GameObject cellImage;


   public int cellFrequency;

   // Used for viewing
   public bool updateNow;
   public bool fillNow;

   // Adjustment variables
   public int cellSpread;
   public float cellBrightness;
   public bool whiteDots;

   //////////////////////////////////////////////////////////////////////////

   // START
   void Start()
   {
      timeToUpdate = Time.time;
      updateNow = false;
      fillNow = false;
      //cellFrequency = 30;

      //randomThing = 0.5f;
      //cellMapWidth = 300; // try dynamic

      // Things we just flat out need
      cellImage = GameObject.Find("RawImage");
      
      imageArray = new float[cellMapWidth+1, cellMapWidth+1]; // Plus one because of some stupid reason, when I use a loop and do i > cellMapWidth, it doesn't throw an error -_-

   }

   // UPDATE
   void Update()
   {
      if (updateNow)
      {
         populateArray();
         populateImage();
         updateNow = false;
      }

      if (fillNow)
      {
         fillArray();
         populateImage();
         fillNow = false;
      }
   }

   /*************************
    *  Builds the array
   *************************/
   void populateArray()
   {

      // POPULATE THE ARRAY WITH POINTS
      for (int z = 0; z < cellMapWidth; z++)
      {
         for (int x = 0; x < cellMapWidth; x++)
         {
            // not effecient at all, it calls random per pixel
            //if(x == cellMapWidth /2 && z == cellMapWidth /2)
            if (Random.Range(0, (1000 - cellFrequency)) == 1)
               imageArray[x, z] = 1;
            else imageArray[x, z] = 0;
         }
      }

      //fillArray();
   }

   void fillArray()
	{
      // START FILLING IN THE GAPS IN ARRAY
      float closest;
      float closestOfficial;
      for (int z = 0; z < cellMapWidth; z++)
      {
         for (int x = 0; x < cellMapWidth; x++)
         {
            ////////////////////////////////////
            closest = 0;
            closestOfficial = 0;
            for (int zt = -30; zt <= 30; zt++)
            {
					// if less than 0, skip
/*					if (zt + z < 0)
						continue;
					if (zt + z > cellMapWidth)
						break;*/

					for (int xt = -30; xt <= 30; xt++)
               {
                  int randomx = xt + x;
                  int randomz = zt + z;
                  // if less than 0, skip
                  /*                  if (xt + x < 0)
                                       continue;
                                    if (xt + x > cellMapWidth)
                                       break;*/
                  //print((randomx) + "," + (randomz) + " IS TESTING");
                  if (randomz > cellMapWidth || randomz < 0 || randomx > cellMapWidth || randomx < 0)
                  {
                     //print((randomx) + "," + (randomz) + " Failed");
                     continue;
                  }
                  if (imageArray[randomx, randomz] == 1)
                  {
                     closest = (Mathf.Sqrt(Mathf.Pow(xt, 2) + Mathf.Pow(zt, 2)) / cellSpread);
                     closest = (1 - Mathf.Pow(closest,cellBrightness));
                     if (closest > closestOfficial)
                     {
                        closestOfficial = closest;
                     }
                  
                  }
               } //XT
            } //ZT
            //if(imageArray[x,z] != 1 && closestOfficial > (minimum / 10))
            if (imageArray[x, z] != 1)
               imageArray[x, z] = closestOfficial;
            ////////////////////////////////////  
         } //X
      } //Z

   }

   /*******************************
   * Populates the image 
   ********************************/
   void populateImage()
   {
      Texture2D cellMapTexture = new Texture2D(cellMapWidth, cellMapWidth);

      for (int z = 0; z < cellMapWidth; z++)
      {
         for (int x = 0; x < cellMapWidth; x++)
         {
            // Controls if the cell centers are white or black
            if(whiteDots)  // Use the float array to assign each pixel a value between black and white
               cellMapTexture.SetPixel(x, z, Color.Lerp(Color.black, Color.white, imageArray[x, z]));
            else
               cellMapTexture.SetPixel(x, z, Color.Lerp(Color.white, Color.black, imageArray[x, z]));
         }
      }
      cellMapTexture.filterMode = FilterMode.Point;
      cellMapTexture.Apply();

      cellImage.GetComponent<RawImage>().texture = cellMapTexture;


      /////////////////////////////////////////////////////////////////////////////


      return;
   }
}
