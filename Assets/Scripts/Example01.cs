using uBUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Example01 : MonoBehaviour
{


    void Start()
    {
        string Title = "Example01";
        var (window_size, window_leftbottom) = (new Vector2(400, 800), new Vector2(20, 20));

        var container = BContainer.Create(isScreenMode: true, Title,
            uiInfo: UIInfo.BCONTAINER_DEFAULT.rtAnchoredPosition(window_leftbottom).rtSizeDelta(window_size).spacing(5));

        // ************************ header ************************
        var headerRoot = container.AddVerticalPanel();
        var header = headerRoot.AddHorizontalPanel();
        header.AddText(Title, UIInfo.TEXT_H1.leFlexWeight(1, 0));

        // △ Basic Position
        var uiCaption = UIInfo.BUTTON_CAPTION;
        header.AddButton(() => { container.locate_byPosition(left: window_leftbottom.x, bottom: window_leftbottom.y, width: window_size.x, height: window_size.y); },
            labelStr: "△", uiInfo: uiCaption);
        // □ Maximize
        header.AddButton(() => { container.locate_byMarginPx(left: 10, right: 10, top: 10, bottom: 10); }, labelStr: "□"
            , uiInfo: uiCaption);
        // × Hide
        header.AddButton(() => { container.gameObject.SetActive(false); }, labelStr: "×"
            , uiInfo: uiCaption);

        container.AddText("Fixed Container, WindowStyle, Components", UIInfo.TEXT_H2);
        container.AddText(string.Join("\r\n", new string[]
            {
                "Window style container built by Fixed Container Mode.",
                "Fixed Container Mode is `Top Down` UI design.",
                "Container size is fixed, so Components are adjusted to fit.",
                "Suitable for ScrollPanel."
            }), UIInfo.TEXT_DEFAULT.lePreferredSize(window_size.x, 0));

        // ************************ Content ************************
        BPanel content = container.AddScrollPanel(LayoutType.Vertical, UIInfo.SCROLLVIEW_DEFAULT);

        UIInfo uiTitle = UIInfo.TEXT_H2;
        UIInfo uiDesc = UIInfo.TEXT_DEFAULT.textSize(14).lePreferredSize(350, 0);

        content.AddText("------ Content ------", uiTitle.textAlignment(TextAnchor.MiddleCenter));
        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("InputField (Single line)", uiTitle);
            string initialText = "entered string: \r\n";
            Text log = vp.AddText(initialText);
            vp.AddTextField(onEndEdit: s => log.text += "\r\n---\r\n\r\n", onValueChanged: s => log.text = $"{initialText}{s}\r\n");
        }
        content.AddSpacer();

        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("InputField (Multi lines)", uiTitle);
            string initialText = "entered string: \r\n";
            Text log = vp.AddText(initialText);
            vp.AddTextField(onEndEdit: s => log.text += "\r\n---\r\n\r\n", onValueChanged: s => log.text = $"{initialText}{s}\r\n", areaLines: 2);
        }
        content.AddSpacer();
        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("Button", uiTitle);
            Text log = vp.AddText("click count : 0");
            int clickCount = 0;
            vp.AddButton(() =>
            {
                clickCount++;
                log.text = $"click count : {clickCount}";
            }
            , "click here");
        }
        content.AddSpacer();
        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("Toggle", uiTitle);
            BPanel hp = vp.AddHorizontalPanel();
            Text textToggleStatus = hp.AddText("Toggle Status...");
            hp.AddToggle(b => textToggleStatus.text = "Toggle Status :" + (b ? "ON" : "OFF"), "switch here!", isOn: false);
        }
        content.AddSpacer();

        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("Radio Buttons (by ToggleGroup)", uiTitle);
            BPanel hp = vp.AddHorizontalPanel();
            Text text_radio = hp.AddText("click radio button...");

            hp.AddRadioButton(s => text_radio.text = "selected :" + s,
                showValueDict: new Dictionary<string, string>()
                    {
                        {"name0","value0" },
                        {"name1","value1" },
                        {"name2","value2" },
                    },
                layoutGroup: LayoutType.Horizontal);
        }
        content.AddSpacer();
        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("Slider", uiTitle);
            BPanel hp = vp.AddHorizontalPanel();
            Text text_slider = hp.AddText("drag slider...");
            hp.AddSlider(f => text_slider.text = "value :" + f.ToString());
        }
        content.AddSpacer();

        {
            var vp = content.AddVerticalPanel(UIInfo.PANEL_DARK);
            vp.AddText("Image, Spacer", uiTitle);
            vp.AddText("By placing a Spacer in the middle of the HorizontalPanel, objects can be placed on both sides.", uiDesc);
            BPanel hp = vp.AddHorizontalPanel(UIInfo.PANEL_DEFAULT.layoutAlignment(TextAnchor.LowerRight));
            hp.AddImage(uiInfo: UIInfo.IMAGE_DEFAULT.lePreferredSize(100).bgColor(Color.red));
            hp.AddSpacer();
            hp.AddImage(uiInfo: UIInfo.IMAGE_DEFAULT.lePreferredSize(150).bgColor(Color.green));
        }
        content.AddSpacer();

        // ************************ Hooter ************************
        var hooter = container.AddVerticalPanel();
        hooter.AddText("------ Hooter ------", uiTitle.textAlignment(TextAnchor.MiddleCenter));
        {
            Text log = hooter.AddText("click count : 0");
            int clickCount = 0;
            hooter.AddButton(() =>
            {
                clickCount++;
                log.text = $"click count : {clickCount}";
            }
            , "click here");
        }
    }


}

