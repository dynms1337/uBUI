using uBUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Example02 : MonoBehaviour
{


    void Start()
    {
        string Title = "Example02 - Variable Container";
        var (window_size, window_leftbottom) = (new Vector2(400, 800), new Vector2(420, 420));

        //isScreenMode
        var container = BContainer.Create(RenderMode.ScreenSpaceOverlay, Title, ContainerMode.VARIABLE,
            uiInfo: UIInfo.BCONTAINER_DEFAULT.spacing(5).rtAnchoredPosition(window_leftbottom).rtSizeDelta(window_size));

        container.AddText(Title, UIInfo.TEXT_H1.textAlignment(TextAnchor.MiddleCenter));
        container.AddText(string.Join("\r\n", new string[] {
            "Variable Container Mode is `Bottom UP` UI design.",
            "Container grows to fit component, so that all components are large enough.",
            "(i.e. Problems such as cut off text do not occur)",
            " Not suitable for ScrollPanel.",
        }), UIInfo.TEXT_DEFAULT.lePreferredSize(window_size.x, 0));
        container.AddSpacer();
        container.AddSpacer();

        int clickCount = 0;
        container.AddButton(() =>
        {
            clickCount++;
            container.AddText($"Click count : {clickCount}");
        }
        , "Add Text Component (so that UI expands.)");
        container.AddSpacer();
        Text log = container.AddText("Click log", UIInfo.TEXT_DEFAULT);
    }


}

