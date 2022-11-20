using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace uBUI
{

    /// <summary>
    /// 無限スクロールするパネル
    /// </summary>
    public class InfiniteScroll : MonoBehaviour
    {
        int MAX_ITEM_COUNT = 200;

        List<Item> itemList = new List<Item>();
        private SPanel container;
        private Func<GameObject, int, Item> itemBuildFunc;
        RectTransform rt;
        int currentIdx = 0;
        float itemHeight = 30f;
        private int itemCount;
        private bool _forceUpdate = true;

        public void initFields(SPanel spanel, Func<GameObject, int, Item> itemBuildFunc, float itemHeight, int initialItemCount)
        {
            this.container = spanel;
            this.itemBuildFunc = itemBuildFunc;
            this.itemHeight = itemHeight;
            this.itemCount = initialItemCount;
        }

        private void _init()
        {
            container.goPanel.GetComponent<LayoutElement>().preferredHeight = itemHeight * (itemCount + 1.5f);
            this.rt = container.goPanel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            float panelHeight = container.goPanel.getParent().GetComponent<RectTransform>().rect.height;
            float hSum = 0;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                var item = itemBuildFunc(container.goPanel, i);
                itemList.Add(item);
                hSum += itemHeight;
                if (hSum > panelHeight) break;
            }
        }

        public void Update()
        {
            if (this.rt == null) _init();
            int newIdx = Mathf.FloorToInt(rt.localPosition.y / (itemHeight));
            //描画範囲のインデックスが変わっていたら、描画範囲をすべて書き直し
            if (_forceUpdate | currentIdx != newIdx)
            {
                _forceUpdate = false;
                for (int i = 0; i < itemList.Count; i++)
                {
                    var item = itemList[i];
                    item.rt.anchoredPosition = new Vector2(0 /*item.rt.sizeDelta.x/2*/, -itemHeight * (0.5f + newIdx + i));
                    item.repaint(newIdx + i);
                }
            }
            currentIdx = newIdx;
        }

        public void forceUpdate() { _forceUpdate = true; }


        public abstract class Item
        {
            protected GameObject parent;
            internal SPanel spanel;
            internal LayoutGroup lg;
            internal RectTransform rt;

            internal Item(GameObject parent, int itemWidth)
            {
                this.lg = SWHelper.CreatePanel(uiInfo: UIInfo.PANEL_DEFAULT.fitW(  //.fit_Fixed(uiSize: new Vector2(itemWidth, 30))
                    , layoutGroup: LayoutType.Horizontal, parent: parent);
                this.spanel = SPanel.CreateFromPanel(lg);
                this.rt = spanel.goPanel.GetComponent<RectTransform>();  //GetComponentは遅いのであらかじめインスタンス取得しておく
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                this.parent = parent;
            }
            public abstract void repaint(int idx);
        }

    }
}
