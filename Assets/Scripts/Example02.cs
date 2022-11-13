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
        window.init_onScreen("Example02", leftbottom: new Vector2(420, 20), windowSize: new Vector2(400, 800), hooterLayout: LayoutType.Vertical);
        
        //SPanel header = window.header;	//title引数を指定すると、headerには自動的にウィンドウのタイトルが記入されるので操作の必要なし
        SPanel caption = window.caption;  //windowsで、右上の最小化ボタンなどをまとめてキャプションボタンと呼ぶことから。
        SPanel content = window.content;
        SPanel hooter = window.hooter;

        caption.addButton(() => { Debug.Log("Close Window Not Implemented"); }, labelStr: "×");

        foreach (var d in itemList)
        {
            SPanel vp = content.addPanel_Vertical(uiInfo:UIInfo.PANEL_DEFAULT.bgColor(new Color(1f, 1f, 1f, 0.1f)));
            foreach (var kvp in d)
            {
                SPanel hp = vp.addPanel_Horizontal();
                hp.addText(kvp.Key);
                hp.addText(kvp.Value);
            }
        }

        hooter.addText("１行のテキストボックスの例");
        SPanel hooter_textfield = hooter.addPanel_Horizontal();
        Text text1 = hooter_textfield.addText("input text in text field...");
        hooter_textfield.addTextField(onEndEdit: s => text1.text += "entered", onValueChanged: s => text1.text = "text field :" + s);

        hooter.addText("N行のテキストボックスの変更・確定検知の例");
        SPanel hooter_textarea = hooter.addPanel_Horizontal();
        Text text2 = hooter_textarea.addText("input text...");
        hooter_textarea.addTextField(onEndEdit: s => text2.text += "entered", onValueChanged: s => text2.text = "text area :" + s);

        hooter.addText("トグルボタンのbool取得の例");
        SPanel hooter_toggle = hooter.addPanel_Horizontal();
        Text text3 = hooter_toggle.addText("click toggle...");
        hooter_toggle.addToggle(b => text3.text = "Toggle Status :" + (b ? "ON" : "OFF"), "switching", isOn: false);

        hooter.addText("ラジオボタンから選択アイテム取得の例");
        SPanel hooter_radio = hooter.addPanel_Horizontal();
        Text text_radio = hooter_radio.addText("click radio button...");
        hooter_radio.addRadioButton(s => text_radio.text = "selected :" + s, itemList[0], layoutGroup: LayoutType.Horizontal);

        hooter.addText("スライダからfloat取得の例");
        SPanel hooter_slider = hooter.addPanel_Horizontal();
        Text text_slider = hooter_slider.addText("drag slider...");
        hooter_slider.addSlider(f => text_slider.text = "value :" + f.ToString());
    }


}

