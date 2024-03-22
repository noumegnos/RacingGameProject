using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

//tooltip displays a text box when the UI object is moused over

public class Tooltip : MonoBehaviour
{
    public string tip;

    public Transform tooltipObject; //the object is called TooltipScreenSpaceUI

    private void Start()
    {
        
    }

    public void DoTooltip()
    {
        tooltipObject.GetComponent<TooltipScreenSpaceUI>().EnableTooltip(tip);


        //System.Func<string> getTooltipTextFunc = () =>
        //{
        //    return tip;
        //};

        //tooltipObject.GetComponent<TooltipScreenSpaceUI>().ShowTooltip_Static(getTooltipTextFunc);
    }

    public void UndoTooltip()
    {
        tooltipObject.GetComponent<TooltipScreenSpaceUI>().DisableTooltip();
            //.HideTooltip_Static();

    }
}
