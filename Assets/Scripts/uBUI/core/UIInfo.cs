using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{

    [Serializable]
    public class UIInfo
    {
        /// <summary>    /// Reference version of Vector2. b/c Vector2? (that is, nullable structure) cannot serialize by JsonUtility.    /// </summary>
        [Serializable]
        public class Vector2R
        {
            public bool notNull = false;
            public Vector2 Value;
            public Vector2R() { }  // default constructor for JSON serialize/deserialize
            public static Vector2R make(Vector2 vec)
            {
                var ret = new Vector2R();
                ret.Value = vec;
                ret.notNull = true;  // instantiate without constructor to avoid set notNull true when deserialize.
                return ret;
            }
        }

        public static readonly Color COLOR_LIGHT = new Color(0.9f, 0.9f, 0.9f, 0.1f);
        public static readonly Color COLOR_DARK = new Color(0.1f, 0.1f, 0.1f, 0.1f);

        // ********************************* fields *********************************
        // LayoutElement
        public Vector2R m_lePreferredSize = new Vector2R();
        public Vector2R m_leMinSize = new Vector2R();
        public Vector2R m_leFlexWeight = new Vector2R();
        // RectTransform
        public Vector2R m_rtPosition = new Vector2R();
        public Vector2R m_rtSizeDelta = new Vector2R();
        public Vector2R m_rtAnchoredPosition = new Vector2R();
        public Vector2R m_rtAnchorMin = new Vector2R();
        public Vector2R m_rtAnchorMax = new Vector2R();
        public Vector2R m_rtPivot = new Vector2R();
        public Vector2R m_rtOffsetMin = new Vector2R();
        public Vector2R m_rtOffsetMax = new Vector2R();

        public int m_textSize = BHelper.FONT_SIZE;
        public Color m_textColor = BHelper.COLOR_TEXT;
        public Color m_bgColor = default(Color);      //デフォルト値なら、各UI要素作成時に適切な色を選択
        public TextAnchor m_textAlignment = TextAnchor.MiddleLeft;

        // LayoutGroup
        public Vector2R m_spacing = Vector2R.make(new Vector2(BHelper.LAYOUTGROUP_SPACING, BHelper.LAYOUTGROUP_SPACING));
        public int m_padding_left = -1;
        public int m_padding_right = -1;
        public int m_padding_top = -1;
        public int m_padding_bottom = -1;
        public TextAnchor m_layoutAlignment = TextAnchor.MiddleLeft;
        public bool m_childForceExpandWidth = false;
        public bool m_childForceExpandHeight = false;
        public int m_constraintCount = -1;



        // ********************************* UIInfo default settings *********************************
        public static readonly UIInfo TEXT_DEFAULT = new UIInfo().leFlexWeight(1, 0).bgColor(Color.clear).childForceExpand(true, true)
            .textSize(BHelper.FONT_SIZE).textAlignment(TextAnchor.MiddleLeft);
        public static readonly UIInfo TEXT_H1 = TEXT_DEFAULT.textSize(BHelper.FONT_SIZE + 4);
        public static readonly UIInfo TEXT_H2 = TEXT_DEFAULT.textSize(BHelper.FONT_SIZE + 2);
        public static readonly UIInfo TEXT_H3 = TEXT_DEFAULT.textSize(BHelper.FONT_SIZE + 1);

        public static readonly UIInfo BUTTON_DEFAULT = TEXT_DEFAULT.leFlexWeight(1, 0).bgColor(Color.white)  //ボタンのデフォルト背景色は白。ホバーしたときの色変化を見やすくするため。)
            .textAlignment(TextAnchor.MiddleCenter);
        public static readonly UIInfo BUTTON_CAPTION = BUTTON_DEFAULT.lePreferredSize(20).leFlexWeight(0, 0);

        public static readonly UIInfo INPUTFIELD_DEFAULT = TEXT_DEFAULT.leFlexWeight(1, 0).bgColor(Color.gray);

        public static readonly UIInfo IMAGE_DEFAULT = new UIInfo();

        public static readonly UIInfo TOGGLE_DEFAULT = new UIInfo().leFlexWeight(1, 0);
        public static readonly UIInfo RADIO_BUTTON_DEFAULT = new UIInfo().leFlexWeight(1, 0);

        public static readonly UIInfo SCROLLBAR_DEFAULT = new UIInfo().rtSizeDelta(new Vector2(20, 20)); // .lePreferredSize(20, 20); //  .fit_Fixed().position(Vector2.zero).uiSize(SWHelper.UIELEMENT_SIZE);
        public static readonly UIInfo SCROLLVIEW_DEFAULT = new UIInfo().bgColor(BHelper.COLOR_AREA_BG).leFlexWeight(1);

        public static readonly UIInfo CANVAS_DEFAULT = new UIInfo().bgColor(COLOR_DARK).layoutAlignment(TextAnchor.MiddleLeft);

        public static readonly UIInfo PANEL_DEFAULT = new UIInfo().bgColor(Color.clear).layoutAlignment(TextAnchor.MiddleLeft);
        public static readonly UIInfo PANEL_DARK = PANEL_DEFAULT.bgColor(COLOR_DARK);
        public static readonly UIInfo PANEL_LIGHT = PANEL_DEFAULT.bgColor(COLOR_LIGHT);

        public static readonly UIInfo BCONTAINER_DEFAULT = CANVAS_DEFAULT.rtAnchoredPosition(10, 10).rtSizeDelta(200, 600);



        public UIInfo() { }

        // ********************************* Field editing method (Immutable like) *********************************
        /* Usage:
         *  UIInfo uiInfo1 = new UIInfo().fit_Parent().bgColor(Color.red);
         *  UIInfo uiInfo2 = uiInfo1.textSize(14);
        */
        public UIInfo textSize(int sizePx) { UIInfo ret = this.Clone(); ret.m_textSize = sizePx; return ret; }
        public UIInfo textColor(Color textColor) { UIInfo ret = this.Clone(); ret.m_textColor = textColor; return ret; }
        public UIInfo bgColor(Color bgColor) { UIInfo ret = this.Clone(); ret.m_bgColor = bgColor; return ret; }
        public UIInfo bgColor(string colorCode) { UIInfo ret = this.Clone(); ret.m_bgColor = BHelper.parseColor(colorCode); return ret; }
        public UIInfo textAlignment(TextAnchor textAlignment) { UIInfo ret = this.Clone(); ret.m_textAlignment = textAlignment; return ret; }

        // LayoutGroup
        public UIInfo layoutAlignment(TextAnchor layoutAlignment) { UIInfo ret = this.Clone(); ret.m_layoutAlignment = layoutAlignment; return ret; }
        public UIInfo padding(int pad) { return this.padding(pad, pad, pad, pad); }
        public UIInfo padding(int left, int right, int top, int bottom)
        {
            UIInfo ret = this.Clone();
            ret.m_padding_left = left;
            ret.m_padding_right = right;
            ret.m_padding_top = top;
            ret.m_padding_bottom = bottom;
            return ret;
        }
        public UIInfo spacing(float spacing) { return this.spacing(new Vector2(spacing, spacing)); }
        public UIInfo spacing(float x, float y) { return spacing(new Vector2(x, y)); }
        public UIInfo spacing(Vector2 xy)
        {
            UIInfo ret = this.Clone();
            ret.m_spacing = Vector2R.make(xy);
            return ret;
        }
        public UIInfo childForceExpand(bool? width, bool? height)
        {
            UIInfo ret = this.Clone();
            if (width != null) ret.m_childForceExpandWidth = width.Value;
            if (height != null) ret.m_childForceExpandHeight = height.Value;
            return ret;
        }
        public UIInfo constraintCount(int count)
        {
            UIInfo ret = this.Clone();
            ret.m_constraintCount = count;
            return ret;
        }

        // ----- RectTransform -----
        public UIInfo rtSizeDelta(float size) { return rtSizeDelta(new Vector2(size, size)); }
        public UIInfo rtSizeDelta(float width, float height) { return rtSizeDelta(new Vector2(width, height)); }
        public UIInfo rtSizeDelta(Vector2 sizeDelta)
        {
            UIInfo ret = this.Clone();
            ret.m_rtSizeDelta = Vector2R.make(sizeDelta);
            return ret;
        }

        public UIInfo rtPosition(float x, float y) { return rtPosition(new Vector2(x, y)); }
        public UIInfo rtPosition(Vector2 position)
        {
            UIInfo ret = this.Clone();
            ret.m_rtPosition = Vector2R.make(position);
            return ret;
        }

        public UIInfo rtAnchoredPosition(float x, float y) { return rtAnchoredPosition(new Vector2(x, y)); }
        public UIInfo rtAnchoredPosition(Vector2 position)
        {
            UIInfo ret = this.Clone();
            ret.m_rtAnchoredPosition = Vector2R.make(position);
            return ret;
        }

        public UIInfo rtAnchorMin(float value) { return rtAnchorMin(new Vector2(value, value)); }
        public UIInfo rtAnchorMin(float x, float y) { return rtAnchorMin(new Vector2(x, y)); }
        public UIInfo rtAnchorMin(Vector2 anchor)
        {
            UIInfo ret = this.Clone();
            ret.m_rtAnchorMin = Vector2R.make(anchor);
            return ret;
        }

        public UIInfo rtAnchorMax(float value) { return rtAnchorMax(new Vector2(value, value)); }
        public UIInfo rtAnchorMax(float x, float y) { return rtAnchorMax(new Vector2(x, y)); }
        public UIInfo rtAnchorMax(Vector2 anchor)
        {
            UIInfo ret = this.Clone();
            ret.m_rtAnchorMax = Vector2R.make(anchor);
            return ret;
        }

        public UIInfo rtOffsetMin(float value) { return rtOffsetMin(new Vector2(value, value)); }
        public UIInfo rtOffsetMin(float x, float y) { return rtOffsetMin(new Vector2(x, y)); }
        public UIInfo rtOffsetMin(Vector2 offset)
        {
            UIInfo ret = this.Clone();
            ret.m_rtOffsetMin = Vector2R.make(offset);
            return ret;
        }

        public UIInfo rtOffsetMax(float value) { return rtOffsetMax(new Vector2(value, value)); }
        public UIInfo rtOffsetMax(float x, float y) { return rtOffsetMax(new Vector2(x, y)); }
        public UIInfo rtOffsetMax(Vector2 offset)
        {
            UIInfo ret = this.Clone();
            ret.m_rtOffsetMax = Vector2R.make(offset);
            return ret;
        }


        public UIInfo rtPivot(float x, float y) { return rtPivot(new Vector2(x, y)); }
        public UIInfo rtPivot(Vector2 pivot)
        {
            UIInfo ret = this.Clone();
            ret.m_rtPivot = Vector2R.make(pivot);
            return ret;
        }


        // ----- LayoutElement -----
        public UIInfo lePreferredSize(float size) { return lePreferredSize(new Vector2(size, size)); }
        public UIInfo lePreferredSize(float width, float height) { return lePreferredSize(new Vector2(width, height)); }
        public UIInfo lePreferredSize(Vector2 size)
        {
            UIInfo ret = this.Clone();
            //ret.m_lePreferredSize = size;
            ret.m_lePreferredSize = Vector2R.make(size);
            return ret;
        }

        public UIInfo leMinSize(float size) { return leMinSize(new Vector2(size, size)); }
        public UIInfo leMinSize(float width, float height) { return leMinSize(new Vector2(width, height)); }
        public UIInfo leMinSize(Vector2 size)
        {
            UIInfo ret = this.Clone();
            ret.m_leMinSize = Vector2R.make(size);
            return ret;
        }

        public UIInfo leFlexWeight(float weight) { return leFlexWeight(new Vector2(weight, weight)); }
        public UIInfo leFlexWeight(float wWeight, float hWeight) { return leFlexWeight(new Vector2(wWeight, hWeight)); }
        public UIInfo leFlexWeight(Vector2 weight)
        {
            UIInfo ret = this.Clone();
            ret.m_leFlexWeight = Vector2R.make(weight);
            return ret;
        }

        // ********************** Syntax Sugars **********************************
        public UIInfo rtAnchorParent() { return rtAnchorMin(0).rtAnchorMax(1).rtAnchoredPosition(0, 0); }

        // ********************** Other Methods **********************************

        public UIInfo Clone()     // ディープコピー
        {
            string jsonStr = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<UIInfo>(jsonStr);
        }

        //public bool is_fit_UnSpecified() { return this.m_fit == Fit.UnSpecified; }
    }
}
