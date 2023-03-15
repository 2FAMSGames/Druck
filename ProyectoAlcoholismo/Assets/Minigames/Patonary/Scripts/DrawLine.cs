using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DrawLine : MonoBehaviour
{

    private LineRenderer lineRenderer;
    public GameObject brush;
    public float BrushSize = 5f;
    public RenderTexture RTexture;

    //private IEnumerator coroutine;

    //private void Start()
    //{
    //    // Start function WaitAndPrint as a coroutine.

    //    coroutine = WaitAndPrint(2.0f);
    //    StartCoroutine(coroutine);

    //    print("Before WaitAndPrint Finishes " + Time.time);
    //}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject drawing = Instantiate(brush);
            lineRenderer = drawing.GetComponent<LineRenderer>();
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);

            lineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(mousePos));
            lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(mousePos));
        }

        else if (Input.GetMouseButton(0))
        {
            FreeDraw();
        }
    }

    void FreeDraw()
    {
        lineRenderer.startWidth = BrushSize / 100f;
        lineRenderer.endWidth = BrushSize / 100f;
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, Camera.main.ScreenToWorldPoint(mousePos));

    }






    //private IEnumerator WaitAndPrint(float waitTime)
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(waitTime);
    //        print("WaitAndPrint " + Time.time);
    //    }
    //}




    public void Save()
    {
        StartCoroutine(CoSave());
    }

    private IEnumerator CoSave()
    {
        //wait for rendering
        yield return new WaitForEndOfFrame();
        Debug.Log(Application.dataPath + "/savedImage" + Random.Range(0, 4) + ".png");

        //set active texture
        RenderTexture.active = RTexture;

        //convert rendering texture to texture2D
        var texture2D = new Texture2D(RTexture.width, RTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
        texture2D.Apply();

        //write data to file
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/savedImage" + Random.Range(0, 4) + ".png", data);

    }
}