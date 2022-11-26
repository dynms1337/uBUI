using uBUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Example03 : MonoBehaviour
{


    void Start()
    {
        string Title = "Example03 - GridLayout";
        var (window_size, window_leftbottom) = (new Vector2(400, 400), new Vector2(820, 420));

        //isScreenMode
        var container = BContainer.Create(isScreenMode: true, Title, ContainerMode.VARIABLE,
            uiInfo: UIInfo.BCONTAINER_DEFAULT.spacing(5).layoutAlignment(TextAnchor.UpperLeft)
                .rtAnchoredPosition(window_leftbottom).lePreferredSize(window_size.x, 0));

        container.AddText(Title, UIInfo.TEXT_H1.textAlignment(TextAnchor.MiddleCenter));
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

        gp = container.AddGridPanel(uiInfo: UIInfo.PANEL_DARK.rtSizeDelta(200).spacing(5));
        var glg = gp.layoutGroup as GridLayoutGroup;
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 2;

        container.AddSpacer();
    }


}

