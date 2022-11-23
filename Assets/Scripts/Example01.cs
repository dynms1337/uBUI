using uBUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Example01 : MonoBehaviour
{


    void Start() { example01(); }

    private void example01()
    {
        string Title = "Example01 - Components";
        var (window_size, window_leftbottom) = (new Vector2(400, 800), new Vector2(420, 20));

        //isScreenMode
        var container = BContainer.Create(RenderMode.ScreenSpaceOverlay, Title, window_leftbottom, window_size);

        // ************************ header ************************
        var header = container.addPanel_Horizontal();
        header.addText(Title, UIInfo.TEXT_H1);
        header.addSpacer();

        // △ Basic Position
        var uiCaption = UIInfo.BUTTON_CAPTION;
        header.addButton(() => { container.locate_byPosition(left: window_leftbottom.x, bottom: window_leftbottom.y, width: window_size.x, height: window_size.y); },
            labelStr: "△", uiInfo: uiCaption);
        // □ Maximize
        header.addButton(() => { container.locate_byMarginPx(left: 10, right: 10, top: 10, bottom: 10); }, labelStr: "□"
            , uiInfo: uiCaption);
        // × Hide
        header.addButton(() => { container.goPanel.SetActive(false); }, labelStr: "×"
            , uiInfo: uiCaption);


        // ************************ Content ************************
        SPanel content = container.addPanel_Scroll(LayoutType.Vertical, UIInfo.SCROLLVIEW_DEFAULT);

        UIInfo uiTitle = UIInfo.TEXT_H2;
        UIInfo uiDesc = UIInfo.TEXT_DEFAULT.textSize(14).lePreferredSize(350, 0);

        content.addText("------ Content ------", uiTitle.textAlignment(TextAnchor.MiddleCenter));
        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("InputField (Single line)", uiTitle);
            string initialText = "entered string: \r\n";
            Text log = vp.addText(initialText);
            vp.addTextField(onEndEdit: s => log.text += "\r\n---\r\n\r\n", onValueChanged: s => log.text = $"{initialText}{s}\r\n");
        }
        content.addSpacer();

        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("InputField (Multi lines)", uiTitle);
            string initialText = "entered string: \r\n";
            Text log = vp.addText(initialText);
            vp.addTextField(onEndEdit: s => log.text += "\r\n---\r\n\r\n", onValueChanged: s => log.text = $"{initialText}{s}\r\n", lineCount: 2);
        }
        content.addSpacer();
        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("Button", uiTitle);
            Text log = vp.addText("click count : 0");
            int clickCount = 0;
            vp.addButton(() =>
            {
                clickCount++;
                log.text = $"click count : {clickCount}";
            }
            , "click here");
        }
        content.addSpacer();
        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("Toggle", uiTitle);
            SPanel hp = vp.addPanel_Horizontal();
            Text textToggleStatus = hp.addText("Toggle Status...");
            hp.addToggle(b => textToggleStatus.text = "Toggle Status :" + (b ? "ON" : "OFF"), "switch here!", isOn: false);
        }
        content.addSpacer();

        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("Radio Buttons (by ToggleGroup)", uiTitle);
            SPanel hp = vp.addPanel_Horizontal();
            Text text_radio = hp.addText("click radio button...");

            hp.addRadioButton(s => text_radio.text = "selected :" + s,
                showValueDict: new Dictionary<string, string>()
                    {
                        {"name0","value0" },
                        {"name1","value1" },
                        {"name2","value2" },
                    },
                layoutGroup: LayoutType.Horizontal);
        }
        content.addSpacer();
        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("Slider", uiTitle);
            SPanel hp = vp.addPanel_Horizontal();
            Text text_slider = hp.addText("drag slider...");
            hp.addSlider(f => text_slider.text = "value :" + f.ToString());
        }
        content.addSpacer();

        {
            var vp = content.addPanel_Vertical(UIInfo.PANEL_DARK);
            vp.addText("Image, Spacer", uiTitle);
            vp.addText("By placing a Spacer in the middle of the HorizontalPanel, objects can be placed on both sides.", uiDesc);
            SPanel hp = vp.addPanel_Horizontal(UIInfo.PANEL_DEFAULT.layoutAlignment(TextAnchor.LowerRight));
            hp.addImage(size: new Vector2(100, 100), color: Color.red);
            hp.addSpacer();
            hp.addImage(size: new Vector2(150, 150), color: Color.green);
        }
        content.addSpacer();

        // ************************ Hooter ************************
        var hooter = container.addPanel_Vertical();
        hooter.addText("------ Hooter ------", uiTitle.textAlignment(TextAnchor.MiddleCenter));
        {
            Text log = hooter.addText("click count : 0");
            int clickCount = 0;
            hooter.addButton(() =>
            {
                clickCount++;
                log.text = $"click count : {clickCount}";
            }
            , "click here");
        }
    }


}

