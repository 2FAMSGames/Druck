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

    public GameObject planoGO;
    private Plane plano;
    private MeshCollider PlaneCollider;

    public GameObject RenderCam;
    private RectTransform rTransf;

    private void Start()
    {
        plano = new Plane(planoGO.transform.up, planoGO.transform.position);
        PlaneCollider = planoGO.GetComponent<MeshCollider>();

        Bounds canvasBounds = PlaneCollider.bounds;

        rTransf = RenderCam.GetComponent<RectTransform>();
        Vector3 bottomLeftCameraWorld = canvasBounds.center - canvasBounds.extents; //transform a coord de la cam
        Vector3 bottomLeftCameraScreen = Camera.main.WorldToScreenPoint(bottomLeftCameraWorld); //x&Y meter  RectTransform
        Vector3 canvasSize = canvasBounds.size;
        rTransf.position.Set(bottomLeftCameraScreen.x, bottomLeftCameraScreen.y, bottomLeftCameraScreen.z);
        rTransf.sizeDelta = canvasSize;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //si colisiona con el plano
            if (PlaneCollider.bounds.Contains(GetClickPosOnPlane().GetValueOrDefault()))
            {
                GameObject drawing = Instantiate(brush);
                lineRenderer = drawing.GetComponent<LineRenderer>();
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);

                lineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(mousePos));
                lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(mousePos));
            }
        }

        else if (Input.GetMouseButton(0))
        {
            if (PlaneCollider.bounds.Contains(GetClickPosOnPlane().GetValueOrDefault()))
            {
                FreeDraw();
            }
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

    public void Save()
    {
        StartCoroutine(CoSave());
    }

    private IEnumerator CoSave()
    {
        //wait for rendering
        yield return new WaitForEndOfFrame();
        //Debug.Log(Application.dataPath + "/savedImage.png");
        //Debug.Log(Application.dataPath + "/savedImage" + Random.Range(0, 4) + ".png");

        //set active texture
        RenderTexture.active = RTexture;

        //convert rendering texture to texture2D
        var texture2D = new Texture2D(RTexture.width, RTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
        texture2D.Apply();

        //write data to file
        var Imagedata = texture2D.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/savedImage.png", Imagedata);
        //File.WriteAllBytes(Application.dataPath + "/savedImage" + Random.Range(0, 4) + ".png", data);

        // Store the byte array as a string in PlayerPrefs
        string imageString = System.Convert.ToBase64String(Imagedata);
        PlayerPrefs.SetString("TransferredImage", imageString);
        
        // TODO: método de enviar la textura.
    }



    //plano, devuelva el punto inter
    private Vector3? GetClickPosOnPlane()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        float RayDistance;
        if (plano.Raycast(camRay, out RayDistance))
        {
            return camRay.GetPoint(RayDistance);
        }
        return null;
    }

}