using uBUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Example02 : MonoBehaviour
{


    void Start()
    {
        string Title = "Example02";
        var (window_size, window_leftbottom) = (new Vector2(400, 400), new Vector2(420, 420));

        var container = BContainer.Create(isScreenMode: true, Title, ContainerMode.VARIABLE,
            uiInfo: UIInfo.BCONTAINER_DEFAULT.spacing(5).layoutAlignment(TextAnchor.UpperLeft)
                .rtAnchoredPosition(window_leftbottom).lePreferredSize(window_size.x, 0));

        container.AddText(Title, UIInfo.TEXT_H1.textAlignment(TextAnchor.MiddleCenter));
        container.AddText("Variable Container, GridLayout", UIInfo.TEXT_H2);
        container.AddText(string.Join("\r\n", new string[] {
            "Variable Container Mode is `Bottom UP` UI design.",
            "Container grows to fit component, so that all components are large enough.",
            "(i.e. Problems such as cut off text do not occur)",
            " Not suitable for ScrollPanel.",
        }), UIInfo.TEXT_DEFAULT.lePreferredSize(window_size.x, 0));
        container.AddSpacer();

        BPanel gp = null;
        int clickCount = 0;
        UIInfo uiGridCell = UIInfo.TEXT_DEFAULT.rtSizeDelta(100).bgColor(new Color(0f, 0f, 0f, 0.3f)).textAlignment(TextAnchor.MiddleCenter);
        container.AddButton(() =>
        {
            clickCount++;
            gp.AddText($"Click count : {clickCount}", uiGridCell);
            gp.AddText("Click log", uiGridCell);
        }
        , "Add Text Component (then UI grows)");

        gp = container.AddGridPanel(uiInfo: UIInfo.PANEL_DARK.spacing(5).constraintCount(2));

        container.AddSpacer();
    }


}

