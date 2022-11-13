using uGUISimpleWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Example03 : MonoBehaviour
{
    void Start()
    {
        example03();


    }

    private void example03()
    {
        SWindow window = new SWindow();
        window.init_onScreen("Example03", parent: null, leftbottom: new Vector2(900, 100), windowSize: new Vector2(200, 600));
        SPanel content = window.content;

        content.addImage(size: new Vector2(200, 200), color: Color.red);
        content.addImage(size: new Vector2(200, 200), color: Color.green);
        content.addImage(size: new Vector2(200, 200), color: Color.blue);
        content.addImage(size: new Vector2(200, 200), color: Color.yellow);

    }


    public static void DestroyChildObject(Transform parent_trans)
    {
        for (int i = 0; i < parent_trans.childCount; ++i)
        {
            GameObject.Destroy(parent_trans.GetChild(i).gameObject);
        }
    }

}

