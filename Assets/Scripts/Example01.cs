using uGUISimpleWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Example01 : MonoBehaviour
{
    void Start()
    {
        example01();
    }

    private void example01()
    {
        Debug.Log("start 01");

        int window_width = 300;
        int window_height = 800;

        // windowを作成、各部のパネルを変数に格納
        //SWindow window = SWindow.Create(title: "Example01", position: new Vector2(20, 20), size: new Vector2(window_width, window_height));
        SWindow window = new SWindow();
        window.init_onScreen(title:"Example01");
        window.locate_byPosition(left: 20, bottom: 20, width: window_width, height: window_height);

        //SPanel header = window.header;	//title引数を指定すると、headerには自動的にウィンドウのタイトルが記入されるので操作の必要なし
        SPanel caption = window.caption;
        SPanel content = window.content;
        SPanel hooter = window.hooter;

        // captionにボタンを追加
        caption.addButton(() => { window.locate_byPosition(left: 20, bottom: 20, width: window_width, height: window_height); }, labelStr: "△");  //基本位置
        caption.addButton(() => { window.locate_byMarginPx(left: 10, right: 10, top: 10, bottom: 10); }, labelStr: "□");  // 最大化
        caption.addButton(() => { window.SetActive(false); }, labelStr: "×");  // 非表示

        // contentにUI要素を追加
        Text logger = content.addText("input text in text field...");

        // hooterにUI要素を追加
        hooter.addButton(() => { logger.text += "added from button.\r\n text1 text1 text1 \r\n"; }, labelStr: "+ text1");
        hooter.addButton(() => { logger.text += "added from button.\r\n text2 text2 text2 \r\n"; }, labelStr: "+ text2");
        hooter.addTextField(onEndEdit: s => logger.text += "added from textfield.\r\n " + s);
    }



}

