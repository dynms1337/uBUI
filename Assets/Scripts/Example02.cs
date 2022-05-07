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
        example01();
        //example02();
        //example03();


    }

    private void example01()
    {
        int window_width = 300;
        int window_height = 800;

        // windowを作成、各部のパネルを変数に格納
        //SWindow window = SWindow.Create(title: "Example01", position: new Vector2(20, 20), size: new Vector2(window_width, window_height));
        SWindow window = new SWindow();
        window.locate_byPosition(left: 20, bottom: 20, width: window_width, height: window_height);

        //SPanel header = window.header;	//title引数を指定すると、headerには自動的にウィンドウのタイトルが記入されるので操作の必要なし
        SPanel caption = window.caption;  //windowsで、右上の最小化ボタンなどをまとめてキャプションボタンと呼ぶことから。
        SPanel content = window.content;
        SPanel hooter = window.hooter;

        // captionにボタンを追加
        caption.addButton(() => { Debug.Log("Close Window Not Implemented"); }, labelStr: "×");
        caption.addButton(() => { Debug.Log("Maximize Not Implemented"); }, labelStr: "□");

        // contentにUI要素を追加
        Text logger = content.addText("input text in text field...");

        // hooterにUI要素を追加
        hooter.addButton(() => { logger.text += "added from button.\r\n text1 text1 text1 \r\n"; }, labelStr: "+ text1");
        hooter.addButton(() => { logger.text += "added from button.\r\n text2 text2 text2 \r\n"; }, labelStr: "+ text2");
        hooter.addTextField(onEndEdit: s => logger.text += "added from textfield.\r\n " + s);
    }


    //private void example02()
    //{
    //    List<Dictionary<string, string>> itemList = new List<Dictionary<string, string>>();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        var d = new Dictionary<string, string>();
    //        for (int j = 0; j < 3; j++) { d[$"name{i}-{j}"] = $"value{i}-{j}"; }
    //        itemList.Add(d);
    //    }

    //    SWindow window = SWindow.Create("Example02", position: new Vector2(420, 20), size: new Vector2(400, 800), hooterLayout: LayoutType.Vertical);
    //    //SPanel header = window.header;	//title引数を指定すると、headerには自動的にウィンドウのタイトルが記入されるので操作の必要なし
    //    SPanel caption = window.caption;  //windowsで、右上の最小化ボタンなどをまとめてキャプションボタンと呼ぶことから。
    //    SPanel content = window.content;
    //    SPanel hooter = window.hooter;

    //    caption.addButton(() => { Debug.Log("Close Window Not Implemented"); }, labelStr: "×");

    //    foreach (var d in itemList)
    //    {
    //        SPanel vp = content.addVerticalPanel(bgColor: new Color(1f, 1f, 1f, 0.1f));
    //        foreach (var kvp in d)
    //        {
    //            SPanel hp = vp.addHorizontalPanel();
    //            hp.addText(kvp.Key);
    //            hp.addText(kvp.Value);
    //        }
    //    }

    //    hooter.addText("１行のテキストボックスの例");
    //    SPanel hooter_textfield = hooter.addHorizontalPanel();
    //    Text text1 = hooter_textfield.addText("input text in text field...");
    //    hooter_textfield.addTextField(onEndEdit: s => text1.text += "entered", onValueChanged: s => text1.text = "text field :" + s);

    //    hooter.addText("N行のテキストボックスの変更・確定検知の例");
    //    SPanel hooter_textarea = hooter.addHorizontalPanel();
    //    Text text2 = hooter_textarea.addText("input text...");
    //    hooter_textarea.addTextArea(onEndEdit:s=>text2.text+="entered", onValueChanged: s => text2.text = "text area :" + s);

    //    hooter.addText("トグルボタンのbool取得の例");
    //    SPanel hooter_toggle = hooter.addHorizontalPanel();
    //    Text text3 = hooter_toggle.addText("click toggle...");
    //    hooter_toggle.addToggle(b => text3.text = "Toggle Status :" + (b ? "ON" : "OFF"), "switching", isOn: false);

    //    hooter.addText("ラジオボタンから選択アイテム取得の例");
    //    SPanel hooter_radio = hooter.addHorizontalPanel();
    //    Text text_radio = hooter_radio.addText("click radio button...");
    //    hooter_radio.addRadioButton(s => text_radio.text = "selected :" + s, itemList[0], layoutGroup: LayoutType.Horizontal);

    //    hooter.addText("スライダからfloat取得の例");
    //    SPanel hooter_slider = hooter.addHorizontalPanel();
    //    Text text_slider = hooter_slider.addText("drag slider...");
    //    hooter_slider.addSlider(f => text_slider.text = "value :" + f.ToString());
    //}

    //private void example03()
    //{
    //    SWindow window = SWindow.Create("Example03", parent: null, position: new Vector2(440, 100), size: new Vector2(200, 600));
    //    SPanel content = window.content;

    //    content.addImage(size: new Vector2(200, 200), color: Color.red);
    //    content.addImage(size: new Vector2(200, 200), color: Color.green);
    //    content.addImage(size: new Vector2(200, 200), color: Color.blue);
    //    content.addImage(size: new Vector2(200, 200), color: Color.yellow);

    //}


    public static void DestroyChildObject(Transform parent_trans)
    {
        for (int i = 0; i < parent_trans.childCount; ++i)
        {
            GameObject.Destroy(parent_trans.GetChild(i).gameObject);
        }
    }

}

