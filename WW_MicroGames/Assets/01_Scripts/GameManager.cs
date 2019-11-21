using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(32, 16);
    public Vector2 gridSpacing = new Vector2(1, 1);
    public Transform gridParentObj;
    public GameObject nod;
    public Transform lineTest;
    public float ratioDistanceScale = 3;
    public int safeZoneWidth = 1;

    public int nbDesiredLines;
    public int nbCutLines;
    public int nbNotCutLines;
    public int minLinesLength = 4;
    public int maxLinesLength = 6;

    private Vector2[,] dotsMatrix;
    private GameObject nodClone;

    private Vector2 extremity0;
    private Vector2 extremity1;

    private void Start()
    {
        GridGeneration();
    }

    void GridGeneration()
    {
        dotsMatrix = new Vector2[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                dotsMatrix[x, y] = new Vector2(gridParentObj.position.x + gridSpacing.x * x, gridParentObj.position.y + gridSpacing.y * y);
                nodClone = Instantiate(nod, gridParentObj);
                nodClone.transform.position = dotsMatrix[x, y];

                if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
                {
                    nodClone.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LineGeneration();
        }
    }

    void LineGeneration()
    {
        extremity0 = ChoosingExtremity0();
        extremity1 = ChoosingExtremity1();

        //Position
        lineTest.position = new Vector2(extremity1.x + ((extremity0.x - extremity1.x) / 2), extremity1.y + ((extremity0.y - extremity1.y) / 2));

        //Rotation
        Vector2 newDir = extremity0 - (Vector2)lineTest.transform.position;
        Quaternion newRot = Quaternion.LookRotation(newDir);
        newRot.x = lineTest.transform.rotation.x;
        newRot.y = lineTest.transform.rotation.y;
        lineTest.transform.rotation = newRot;

        //Extention
        lineTest.localScale = new Vector3(Vector2.Distance(extremity0, extremity1) * ratioDistanceScale, 1, 0);
    }

    Vector2Int randomStartPoint;
    Vector2 ChoosingExtremity0()
    {
        do
        {
            randomStartPoint = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        } while (randomStartPoint.x < safeZoneWidth || randomStartPoint.x > gridSize.x - (1 + safeZoneWidth) || randomStartPoint.y < safeZoneWidth || randomStartPoint.y > gridSize.y - (1 + safeZoneWidth));

        return dotsMatrix[randomStartPoint.x, randomStartPoint.y];
    }

    Vector2 polarisation;
    Vector2 coordinates;
    Vector2Int coordAlt;
    Vector2 ChoosingExtremity1()
    {
        do
        {
            polarisation = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            coordAlt = new Vector2Int(Random.Range(randomStartPoint.x + minLinesLength * Polarisation(polarisation.x), randomStartPoint.x + maxLinesLength * Polarisation(polarisation.x)),
                Random.Range(randomStartPoint.y + minLinesLength * Polarisation(polarisation.y), randomStartPoint.y + maxLinesLength * Polarisation(polarisation.y)));
        } while (coordAlt.x < safeZoneWidth || coordAlt.x > gridSize.x - (1 + safeZoneWidth) || coordAlt.y < safeZoneWidth || coordAlt.y > gridSize.y - (1 + safeZoneWidth));

        coordinates = dotsMatrix[coordAlt.x, coordAlt.y];

        return coordinates;
    }

    int Polarisation(float valueToPolarise)
    {
        if (valueToPolarise < 0)            
            valueToPolarise = -1;
        else
            valueToPolarise = 1;

        return (int)valueToPolarise;
    }
}