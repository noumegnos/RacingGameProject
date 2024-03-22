using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//started by following Code Monkey tutorial https://www.youtube.com/watch?v=YUIohCXt_pc
//adapted and simplified
//couldn't really use instances here, so I'm removing them, as well as the delegate system
//I'd have to redo a bunch of the game scene's ui canvas object hierarchy systems, easier to scale back Code Monkey's advanced example

public class TooltipScreenSpaceUI : MonoBehaviour
{
    //public static TooltipScreenSpaceUI tipInstance {  get; private set; }

    [SerializeField] private RectTransform canvasRectTransform;
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;

    //private System.Func<string> getTooltipTextFunc;

    private void Awake()
    {
        //tipInstance = this;

        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();

        DisableTooltip();
        //HideTooltip();
    }

    private void SetText(string toolTipText)
    {
        toolTipText = toolTipText.Replace("\\n", "\n");

        textMeshPro.SetText(toolTipText);
        textMeshPro.ForceMeshUpdate();


        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(180, 24);

        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }

    private void Update()
    {
        //SetText(getTooltipTextFunc());
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        if(anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }

        rectTransform.anchoredPosition = anchoredPosition; 
    }

    public void EnableTooltip(string tip)
    {
        gameObject.SetActive(true);
        SetText(tip);
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
    }

    //private void ShowTooltip(string tooltipText)
    //{
    //    ShowTooltip(() => tooltipText);
    //}

    //public void ShowTooltip(System.Func<string> getTooltipTextFunc)
    //{
    //    this.getTooltipTextFunc = getTooltipTextFunc;
    //    gameObject.SetActive(true);
    //    SetText(getTooltipTextFunc());
    //}

    //private void HideTooltip()
    //{
    //    gameObject.SetActive(false);
    //}

    //public static void ShowTooltip_Static(string tooltipText)
    //{
    //    tipInstance.ShowTooltip(tooltipText);
    //}

    //public static void ShowTooltip_Static(System.Func<string> getTooltipTextFunc)
    //{
    //    tipInstance.ShowTooltip(getTooltipTextFunc);
    //}

    //public static void HideTooltip_Static()
    //{
    //    tipInstance.HideTooltip();
    //}

}
