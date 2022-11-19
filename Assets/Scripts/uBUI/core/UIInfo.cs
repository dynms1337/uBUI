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


        public enum Fit { Parent, WParentHSelf, WSelfHParent, Self, Fixed, Flexible, WParentHFrexible, UnSpecified }
        public Fit m_fit = Fit.UnSpecified;
        public Vector2 m_margin = Vector2.zero;   //Parentで使用
        public Vector2 m_position = Vector2.zero;       //SpecifiedSizeで使用
        public Vector2 m_uiSize = Vector2.zero;           //SpecifiedSizeで使用、０なら設定しない
        public float m_flexWidth = 0f;        //SpecifiedSizeで使用、０なら設定しない
        public float m_flexHeight = 0f;       //SpecifiedSizeで使用、０なら設定しない
        public int m_textSize = SWHelper.FONT_SIZE;
        public Color m_textColor = SWHelper.COLOR_TEXT;
        public Color m_bgColor = default(Color);      //デフォルト値なら、各UI要素作成時に適切な色を選択
        public TextAnchor m_textAlignment = TextAnchor.MiddleLeft;
        public int m_spacing = SWHelper.LAYOUTGROUP_SPACING;
        public int m_padding_left = -1;
        public int m_padding_right = -1;
        public int m_padding_top = -1;
        public int m_padding_bottom = -1;
        public TextAnchor m_layoutAlignment = TextAnchor.MiddleLeft;


        public static readonly UIInfo TEXT_DEFAULT = new UIInfo().fit_WParentHSelf().bgColor(Color.clear/*SWHelper.COLOR_AREA_BG*/)
            .textSize(SWHelper.FONT_SIZE).textAlignment(TextAnchor.MiddleLeft);
        public static readonly UIInfo BUTTON_DEFAULT = TEXT_DEFAULT.fit_WParentHSelf().bgColor(Color.white)  //ボタンのデフォルト背景色は白。ホバーしたときの色変化を見やすくするため。)
            .textAlignment(TextAnchor.MiddleCenter);
        public static readonly UIInfo INPUTFIELD_DEFAULT = TEXT_DEFAULT.fit_WParentHSelf().bgColor(Color.gray);
        public static readonly UIInfo IMAGE_DEFAULT = new UIInfo().fit_Self();
        public static readonly UIInfo TOGGLE_DEFAULT = new UIInfo().fit_Parent();
        public static readonly UIInfo RADIO_BUTTON_DEFAULT = new UIInfo().fit_WParentHSelf();
        public static readonly UIInfo SCROLLBAR_DEFAULT = new UIInfo().fit_Fixed().position(Vector2.zero).uiSize(SWHelper.UIELEMENT_SIZE);
        public static readonly UIInfo SCROLLVIEW_DEFAULT = new UIInfo().fit_Parent().bgColor(SWHelper.COLOR_AREA_BG);
        public static readonly UIInfo CANVAS_DEFAULT = new UIInfo().fit_Fixed().fit_Fixed();
        public static readonly UIInfo PANEL_DEFAULT = new UIInfo().fit_Parent().fit_Parent().bgColor(SWHelper.COLOR_AREA_BG).layoutAlignment(TextAnchor.MiddleLeft);

        public static readonly UIInfo DEFAULT = new UIInfo().fit_Parent().fit_Parent().bgColor(SWHelper.COLOR_AREA_BG).layoutAlignment(TextAnchor.MiddleLeft);

        public UIInfo() { }

        /* フィールド編集用メソッド
         * メソッドチェインでの利用を想定
         * Usage:
         *  UIInfo uiInfo1 = new UIInfo().fit_Parent().bgColor(Color.red);
         *  UIInfo uiInfo2 = uiInfo1.textSize(14);
         *  
         *  ※　呼び出し元のUIInfoの変更を避けるため、クローンを編集して返却している。
        */
        public UIInfo margin(Vector2 margin) { UIInfo ret = this.Clone(); ret.m_margin = margin; return ret; }
        public UIInfo position(Vector2 position) { UIInfo ret = this.Clone(); ret.m_position = position; return ret; }
        public UIInfo uiSize(Vector2 uiSize)
        {
            UIInfo ret = this.Clone();
            if (ret.is_fit_UnSpecified()) ret = ret.fit_Fixed();
            ret.m_uiSize = uiSize; return ret;
        }
        public UIInfo flexWidth(float flexWidth) { UIInfo ret = this.Clone(); ret.m_flexWidth = flexWidth; return ret; }
        public UIInfo flexHeight(float flexHeight) { UIInfo ret = this.Clone(); ret.m_flexHeight = flexHeight; return ret; }
        public UIInfo textSize(int sizePx) { UIInfo ret = this.Clone(); ret.m_textSize = sizePx; return ret; }
        public UIInfo textColor(Color textColor) { UIInfo ret = this.Clone(); ret.m_textColor = textColor; return ret; }
        public UIInfo bgColor(Color bgColor) { UIInfo ret = this.Clone(); ret.m_bgColor = bgColor; return ret; }
        public UIInfo bgColor(string colorCode) { UIInfo ret = this.Clone(); ret.m_bgColor = SWHelper.parseColor(colorCode); return ret; }
        public UIInfo textAlignment(TextAnchor textAlignment) { UIInfo ret = this.Clone(); ret.m_textAlignment = textAlignment; return ret; }
        public UIInfo layoutAlignment(TextAnchor layoutAlignment) { UIInfo ret = this.Clone(); ret.m_layoutAlignment = layoutAlignment; return ret; }
        public UIInfo spacing(int space) { UIInfo ret = this.Clone(); ret.m_spacing = space; return ret; }
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

        /// <summary>縦横固定サイズ[px]</summary>
        public UIInfo fit_Fixed(Vector2 position = default(Vector2), Vector2 uiSize = default(Vector2))
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Fixed;
            if (position != default(Vector2)) ret.m_position = position;
            if (uiSize != default(Vector2)) ret.m_uiSize = uiSize;
            return ret;
        }
        /// <summary>親GameObjectのサイズに合わせる。</summary>
        public UIInfo fit_Parent(Vector2 margin = default(Vector2))
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Parent;
            if (margin != default(Vector2)) ret.m_margin = margin;
            return ret;
        }
        /// <summary>自身のコンポーネントのサイズに合わせる</summary>
        public UIInfo fit_Self()
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Self;
            return ret;
        }
        /// <summary>親GameObjectに隙間ができないように合わせる</summary>
        public UIInfo fit_Flexible(float flexWidth = 1f, float flexHeight = 1f, Vector2 minSize = default(Vector2))
        {
            if (minSize == default(Vector2)) minSize = Vector2.zero;
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Flexible;
            ret.m_flexWidth = flexWidth; ret.m_flexHeight = flexHeight; ret.m_uiSize = minSize;
            return ret;
        }
        /// <summary>横幅は親、縦幅は自身のサイズに合わせる</summary>
        public UIInfo fit_WParentHSelf(Vector2 margin = default(Vector2))
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.WParentHSelf;
            if (margin != default(Vector2)) ret.m_margin = margin;
            return ret;
        }
        public UIInfo fit_WSelfHParent()
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.WSelfHParent;
            return ret;
        }

        public UIInfo Clone()     // ディープコピー
        {
            string jsonStr = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<UIInfo>(jsonStr);
        }

        public bool is_fit_UnSpecified() { return this.m_fit == Fit.UnSpecified; }
    }
}
