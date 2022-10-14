using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
public class GridFix : MonoBehaviour
{
    public enum OptionsChoice
    {
        RowsAndColoumns,
        WidthEqualsHeight,
        HeightEqualsWidth
    }

    public OptionsChoice ModeSelect = OptionsChoice.RowsAndColoumns;

    public RectTransform MyTransformComponent;
    public int Coloumns = 5;
    public int Rows = 0;

    void Start()
    {
        MyTransformComponent.offsetMin = Vector2.zero;
        MyTransformComponent.offsetMax = Vector2.zero;

        StartCoroutine("ForceUpdate");
    }

    //This function makes automatic cell sizes for all screen resolutions and splits them up for coloums and rows
    public IEnumerator ForceUpdate()
    {
        if(!MyTransformComponent) { yield break; }
        if (Rows < 1) { Rows = 1; }
        if(Coloumns < 1) { Coloumns = 1; }

        //waits for the UI element to be activated after the game object is activated
        while(MyTransformComponent.rect.width == 0 || MyTransformComponent.rect.height ==0)
        {
            yield return 0;
        }

        //Calculate the pixel width and height of the UI element and split it up for cell dimensions
        float pixelWidth = MyTransformComponent.rect.width;
        //Debug.Log("Pixel width: " + pixelWidth);
        if(pixelWidth <= 0) { pixelWidth = 1; }
        float width = (pixelWidth-(GetComponent<GridLayoutGroup>().spacing.x* (Coloumns-1))) / Coloumns;

        float pixelHeight = MyTransformComponent.rect.height;
        if(pixelHeight <= 0) { pixelHeight = 1; }
        //Debug.Log("Pixel height: " + pixelHeight);
        float height = (pixelHeight - (GetComponent<GridLayoutGroup>().spacing.y * (Rows - 1))) / Rows;

        //Useful for making grids
        if(ModeSelect == OptionsChoice.HeightEqualsWidth)
        {
            height = width;
        }
        if(ModeSelect == OptionsChoice.WidthEqualsHeight)
        {
            width = height;
        }
        
        //Apply the dimensions
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(width, height);
    }

}
