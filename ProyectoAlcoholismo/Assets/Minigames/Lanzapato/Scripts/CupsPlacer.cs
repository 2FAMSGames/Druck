using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupsPlacer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Must have pivot on top and in the nearest side to camera")]
    GameObject table;
    [SerializeField]
    [Tooltip("Must have pivot on bottom and centered")]
    GameObject cupPrefab;

    [SerializeField]
    float xOffset = 2;
    [SerializeField]
    float zOffset = 2;

    [SerializeField]
    float zMargin = 1;

    List<GameObject> listInstatiatedCups;

    [HideInInspector]
    public bool isInitialized = false;

    private void Start()
    {
        listInstatiatedCups = new List<GameObject>();
        isInitialized = true;
    }

    public void PlaceCups (int totalRows)
    {
        //Calculates desk bounds to get the back center position of the table
        Bounds tableBounds = GetMaxBounds(table);
        Vector3 backCenterTable = tableBounds.center + new Vector3(0, tableBounds.extents.y, tableBounds.extents.z);
        backCenterTable -= new Vector3(0, 0, zMargin);
        float topTable = tableBounds.center.y + tableBounds.extents.y;

        float zPos = backCenterTable.z;
        //Iterates over each row
        for (int i = totalRows; i > 0; i--)
        {
            float xLeft = 0;
            float xRight = 0;

            if (i % 2 == 0)
            {
                xLeft += xOffset / 2;
                xRight -= xOffset / 2;
                InstantiateCup(new Vector3(xLeft, topTable, zPos));
                InstantiateCup(new Vector3(xRight, topTable, zPos));

                for (int j = 1; j < i-1; j += 2)
                {
                    xLeft += xOffset;
                    xRight -= xOffset;
                    InstantiateCup(new Vector3(xLeft, topTable, zPos));
                    InstantiateCup(new Vector3(xRight, topTable, zPos));
                }
            }
            else
            {
                InstantiateCup(new Vector3(0, topTable, zPos));
                for (int j = 1; j < i; j += 2)
                {
                    xLeft += xOffset;
                    xRight -= xOffset;
                    InstantiateCup(new Vector3(xLeft, topTable, zPos));
                    InstantiateCup(new Vector3(xRight, topTable, zPos));
                }
            }
            zPos -= zOffset;
        }
    }

    public void DestroyCups()
    {
        foreach(GameObject cup in listInstatiatedCups)
        {
            Destroy(cup);
        }
    }

    void InstantiateCup(Vector3 pos)
    {
        GameObject cupInstantiated = Instantiate(cupPrefab, pos, Quaternion.identity, gameObject.transform);
        listInstatiatedCups.Add(cupInstantiated);
    }

    public void SetTableLength(float length)
    {
        Vector3 tableScale = table.transform.localScale;
        tableScale.z = length;
        table.transform.localScale = tableScale;
    }


    //TODO: Move to utils after merge with origin/feature/networking
    Bounds GetMaxBounds(GameObject g)
    {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
}
