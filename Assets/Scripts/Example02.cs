using uGUISimpleWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Example02 : MonoBehaviour
{
    void Start()
    {
        example02();
    }

    private void example02()
    {
        List<Dictionary<string, string>> itemList = new List<Dictionary<string, string>>();
        for (int i = 0; i < 10; i++)
        {
            var d = new Dictionary<string, string>();
            for (int j = 0; j < 3; j++) { d[$"name{i}-{j}"] = $"value{i}-{j}"; }
            itemList.Add(d);
        }


        SWindow window = new SWindow();
        window.init_onScreen("Example02 - Components", leftbottom: new Vector2(420, 20), windowSize: new Vector2(400, 800), hooterLayout: LayoutType.Vertical);

        //SPanel header = window.header;	//title引数を指定すると、headerには自動的にウィンドウのタイトルが記入されるので操作の必要なし
        //SPanel caption = window.caption;  //windowsで、右上の最小化ボタンなどをまとめてキャプションボタンと呼ぶことから。
        //SPanel content = window.content;
        //SPanel hooter = window.hooter;

        // ************************ caption ************************

        window.caption.addButton(() => { Debug.Log("Close Window Not Implemented"); }, labelStr: "×");

        // ************************ content ************************
        SPanel content = window.content;

        UIInfo uiTitle = UIInfo.TEXT_DEFAULT.textSize(18);
        UIInfo uiDesc = UIInfo.TEXT_DEFAULT.textSize(14);

        {
            content.addText("InputField (Single line)", uiTitle);
            string initialText = "entered string: \r\n";
            Text log = content.addText(initialText);
            content.addTextField(onEndEdit: s => log.text += "\r\n---\r\n\r\n", onValueChanged: s => log.text = $"{initialText}{s}\r\n");
            content.addSpacer();
        }

        {
            content.addText("InputField (Multi lines)", uiTitle);
            string initialText = "entered string: \r\n";
            Text log = content.addText(initialText);
            content.addTextField(onEndEdit: s => log.text += "\r\n---\r\n\r\n", onValueChanged: s => log.text = $"{initialText}{s}\r\n", lineCount: 2);
            content.addSpacer();
        }
        {
            content.addText("Button", uiTitle);
            Text log = content.addText("click count : 0");
            int clickCount = 0;
            content.addButton(() =>
            {
                clickCount++;
                log.text = $"click count : {clickCount}";
            }
            , "click here");
            content.addSpacer();
        }
        {
            content.addText("Toggle", uiTitle);
            SPanel hp = content.addPanel_Horizontal();
            Text textToggleStatus = hp.addText("Toggle Status...");
            hp.addToggle(b => textToggleStatus.text = "Toggle Status :" + (b ? "ON" : "OFF"), "switch here!", isOn: false);
            content.addSpacer();
        }

        {
            content.addText("Radio Buttons (by ToggleGroup)", uiTitle);
            SPanel hp = content.addPanel_Horizontal();
            Text text_radio = hp.addText("click radio button...");
            hp.addRadioButton(s => text_radio.text = "selected :" + s, itemList[0], layoutGroup: LayoutType.Horizontal);
            content.addSpacer();
        }
        {
            content.addText("Slider", uiTitle);
            SPanel hp = content.addPanel_Horizontal();
            Text text_slider = hp.addText("drag slider...");
            hp.addSlider(f => text_slider.text = "value :" + f.ToString());
            content.addSpacer();
        }

        {
            content.addText("Image, Spacer", uiTitle);
            content.addText("By placing a Spacer in the middle of the HorizontalPanel, objects can be placed on both sides.", uiDesc);
            SPanel hp = content.addPanel_Horizontal(UIInfo.PANEL_DEFAULT/*.layoutAlignment(*/);            
            hp.addImage(size: new Vector2(100, 100), color: Color.red);
            hp.addSpacer();
            hp.addImage(size: new Vector2(150, 150), color: Color.green);
            content.addSpacer();
        }


    }


}

