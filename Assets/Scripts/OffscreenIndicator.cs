using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private MultiplayerManager manager;
    //[SerializeField] private RectTransform canvas;

    [Header("Screen Units")]
    float screenHeight;
    float screenWidth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        screenHeight = Screen.height;//canvas.rect.height * canvas.localScale.y;
        screenWidth = Screen.width;//canvas.rect.width * canvas.localScale.x;

        foreach(GameObject player in manager.GetPlayerList()) {
            setIndicatorPosition(player);
        }
    }


    // Sets the indicator position for a given target
    void setIndicatorPosition(GameObject target) {
        //
        // Ref -> https://gamedevelopment.tutsplus.com/tutorials/positioning-on-screen-indicators-to-point-to-off-screen-targets--gamedev-6644
        //     -> https://stackoverflow.com/questions/32005754/how-to-create-ofscreen-enemy-indicator-in-unity-3d/32151202
        //
        
        // Generates targets position on screen, relative to the camera's view
        Vector3 screenTargetPosition = gameCamera.WorldToScreenPoint(target.transform.position);
        if(screenTargetPosition.z < 0)
            screenTargetPosition *= -1;

        if (isOnScreen(screenTargetPosition)) {
            // Target's indicator is now invisible
            target.GetComponent<PlayerController>().indicator.SetActive(false);
            
        } else {
            // Center of the screen
            Vector3 center = new Vector3 (screenWidth / 2f, screenHeight / 2f, 0);
            // Angle between the center of the screen and the target's screen position
            float angle = Mathf.Atan2(screenTargetPosition.y-center.y, screenTargetPosition.x-center.x) * Mathf.Rad2Deg;

            // Determines where the target is compared to the screen
            float coef;
            if (screenWidth > screenHeight)
                coef = screenWidth / screenHeight;
            else
                coef = screenHeight / screenWidth;

            float degreeRange = 360f / (coef + 1);

            float angle2 = Mathf.Atan2(screenHeight - center.y, screenWidth - center.x) * Mathf.Rad2Deg;	

            if(angle < 0) 
                angle = angle + 360;

            // Determines which side of the screen the target is present
            int edgeLine;
            //if(angle < degreeRange / 4f)
            if (angle < angle2)
                edgeLine = 0;
            else if (angle < 180 - angle2)//degreeRange / 4f)
                edgeLine = 1;
            else if (angle < 180 + angle2)//degreeRange /  4f)
                edgeLine = 2;
            else if (angle < 360 - angle2)//degreeRange / 4f)
                edgeLine = 3;
            else
                edgeLine = 0;

            // Activates target's indicator
            target.GetComponent<PlayerController>().indicator.SetActive(true);
            // Sets target's indicator into the correct position
            target.GetComponent<PlayerController>().indicator.GetComponent<RectTransform>().position = pointOfIntersection(edgeLine, center, screenTargetPosition);
            target.GetComponent<PlayerController>().indicator.GetComponent<RectTransform>().eulerAngles = new Vector3 (0, 0, angle+90);
        }


        Debug.Log(screenTargetPosition.ToString());
    }


    // Returns true if point is inside the screen boundaries 
    bool isOnScreen(Vector3 point){
        return !(point.x > screenWidth || point.x < 0 || point.y > screenHeight || point.y < 0);
    }

    // Returns the point at which the line intersects with the screen edge
    Vector3 pointOfIntersection(int edgeLine, Vector3 line2point1, Vector3 line2point2){
        float[] A1 = {-screenHeight, 0, screenHeight, 0};
        float[] B1 = {0, -screenWidth, 0, screenWidth};
        float[] C1 = {-screenWidth * screenHeight,-screenWidth * screenHeight,0, 0};

        float A2 = line2point2.y - line2point1.y;
        float B2 = line2point1.x - line2point2.x;
        float C2 = A2 * line2point1.x + B2 * line2point1.y;

        float det = A1[edgeLine] * B2 - A2 * B1[edgeLine];

        return new Vector3 ((B2 * C1[edgeLine] - B1[edgeLine] * C2) / det, (A1[edgeLine] * C2 - A2 * C1[edgeLine]) / det, 0);
    }
}
