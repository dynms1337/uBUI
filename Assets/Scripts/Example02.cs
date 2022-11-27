using uBUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example02 : MonoBehaviour
{
    void Start()
    {
        string Title = "Example02 - Variable Container";
        var window_size = new Vector2(400, 0); var window_leftbottom = new Vector2(420, 420);

        var container = BContainer.Create(isScreenMode: true, Title, ContainerMode.VARIABLE,
            uiInfo: UIInfo.BCONTAINER_DEFAULT.spacing(5).layoutAlignment(TextAnchor.UpperLeft)
                .rtAnchoredPosition(window_leftbottom).lePreferredSize(window_size));

        container.AddText(Title, UIInfo.TEXT_H1.textAlignment(TextAnchor.MiddleCenter));
        container.AddSpacer();
        { // Scroll Panel
            container.AddText("ScrollPanel(in VariableContainer)", UIInfo.TEXT_H2);
            BPanel content = container.AddScrollPanel(LayoutType.Vertical, UIInfo.SCROLLVIEW_DEFAULT.lePreferredSize(new Vector2(0, 100)));

            int clickCount = 0;
            content.AddButton(() =>
            {
                clickCount++;
                add();
            }
            , "Add Text (UI not grows since the ScrollPanel height is fixed.");
            add();
            container.AddSpacer();
            void add() { content.AddText($"Click count : {clickCount}"); }
        }

        { // Grid Layout
            container.AddText("GridPanel", UIInfo.TEXT_H2);
            BPanel gp = null;
            int clickCount = 0;
            UIInfo uiGridCell = UIInfo.TEXT_DEFAULT.rtSizeDelta(100).bgColor(new Color(0f, 0f, 0f, 0.3f)).textAlignment(TextAnchor.MiddleCenter);
            container.AddButton(() =>
            {
                clickCount++;
                add();
            }
            , "Add Text Component (then UI grows)");
            gp = container.AddGridPanel(uiInfo: UIInfo.PANEL_DARK.spacing(5).constraintCount(2));
            add();
            container.AddSpacer();

            void add()
            {
                gp.AddText($"Click count : {clickCount}", uiGridCell);
                gp.AddText("Click log", uiGridCell);
            }
        }
    }
}

