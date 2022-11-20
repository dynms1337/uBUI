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


        // LayoutElement
        public enum Fit { Fixed, Flexible, UnSpecified }
        //public Fit m_fitWidth = Fit.UnSpecified;
        //public Fit m_fitHeight = Fit.UnSpecified;
        public Vector2 m_lePreferredSize = Vector2.zero;
        public Vector2 m_leMinSize = Vector2.zero;
        public Vector2 m_leFlexWeight = Vector2.zero;


        public Vector2 m_margin = Vector2.zero;   //Parentで使用

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


        public static readonly UIInfo TEXT_DEFAULT = new UIInfo().fitW(Fit.Flexible).bgColor(Color.clear/*SWHelper.COLOR_AREA_BG*/)
            .textSize(SWHelper.FONT_SIZE).textAlignment(TextAnchor.MiddleLeft);
        public static readonly UIInfo BUTTON_DEFAULT = TEXT_DEFAULT.fitW(Fit.Flexible).bgColor(Color.white)  //ボタンのデフォルト背景色は白。ホバーしたときの色変化を見やすくするため。)
            .textAlignment(TextAnchor.MiddleCenter);
        public static readonly UIInfo INPUTFIELD_DEFAULT = TEXT_DEFAULT.fitW(Fit.Flexible).bgColor(Color.gray);
        public static readonly UIInfo IMAGE_DEFAULT = new UIInfo();
        public static readonly UIInfo TOGGLE_DEFAULT = new UIInfo().fitW(Fit.Flexible);
        public static readonly UIInfo RADIO_BUTTON_DEFAULT = new UIInfo().fitW(Fit.Flexible);
        public static readonly UIInfo SCROLLBAR_DEFAULT = new UIInfo().fitWH(Fit.Fixed, 20); //  .fit_Fixed().position(Vector2.zero).uiSize(SWHelper.UIELEMENT_SIZE);
        public static readonly UIInfo SCROLLVIEW_DEFAULT = new UIInfo().fitWH(Fit.UnSpecified).bgColor(SWHelper.COLOR_AREA_BG);
        public static readonly UIInfo CANVAS_DEFAULT = new UIInfo().fitWH(Fit.UnSpecified); //.fit_Fixed();
        public static readonly UIInfo PANEL_DEFAULT = new UIInfo().fitWH(Fit.Flexible).bgColor(SWHelper.COLOR_AREA_BG).layoutAlignment(TextAnchor.MiddleLeft);

        //public static readonly UIInfo DEFAULT = new UIInfo().fit_Parent().fit_Parent().bgColor(SWHelper.COLOR_AREA_BG).layoutAlignment(TextAnchor.MiddleLeft);

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

        public UIInfo lePreferredSize(float width, float height)
        {
            UIInfo ret = this.Clone();
            if (width != 0) ret.m_lePreferredSize.x = width;
            if (height != 0) ret.m_lePreferredSize.y = height;
            return ret;
        }

        public UIInfo leMinSize(float width, float height)
        {
            UIInfo ret = this.Clone();
            if (width != 0) ret.m_leMinSize.x = width;
            if (height != 0) ret.m_leMinSize.y = height;
            return ret;
        }
        public UIInfo leFlexWeight(float wWeight, float hWeight)
        {
            UIInfo ret = this.Clone();
            if (wWeight != 0) ret.m_leFlexWeight.x = wWeight;
            if (hWeight != 0) ret.m_leFlexWeight.y = hWeight;
            return ret;
        }

        // ************************ Syntax sugar for LayoutElement settings ************************
        public UIInfo fitW(Fit fit, float? sizeOrWeight = 1f)
        {
            UIInfo ret = this.Clone();
            //ret.m_fitWidth = fit;
            if (sizeOrWeight != null) switch (fit)
                {
                    case Fit.Fixed:
                        ret.lePreferredSize(sizeOrWeight.Value, 0); 
                        break;
                    case Fit.Flexible:
                        ret.leFlexWeight(sizeOrWeight.Value, 0); 
                        break;
                    case Fit.UnSpecified: break;
                    default: break;
                }
            return ret;
        }
        public UIInfo fitH(Fit fit, float? sizeOrWeight = 1f)   // 1f : default for Flexible
        {
            UIInfo ret = this.Clone();
            ret.m_fitHeight = fit;
            if (sizeOrWeight != null) switch (fit)
                {
                    case Fit.Fixed: ret.lePreferredSize(0, sizeOrWeight.Value); break;
                    case Fit.Flexible: ret.leFlexWeight(0, sizeOrWeight.Value); break;
                    case Fit.UnSpecified: break;
                    default: break;

                }
            return ret;
        }

        public UIInfo fitWH(Fit fit) { return fitW(fit).fitH(fit); }
        public UIInfo fitWH(Fit fit, float? sizeOrWeight = null) { return fitW(fit, sizeOrWeight).fitH(fit, sizeOrWeight); }
        public UIInfo fitWH(Fit fit, Vector2? sizeOrWeight = null) { return fitW(fit, sizeOrWeight.Value.x).fitH(fit, sizeOrWeight.Value.y); }


        // ********************** Other Methods **********************************

        public UIInfo Clone()     // ディープコピー
        {
            string jsonStr = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<UIInfo>(jsonStr);
        }

        //public bool is_fit_UnSpecified() { return this.m_fit == Fit.UnSpecified; }
    }
}
