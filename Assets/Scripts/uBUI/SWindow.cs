using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{
    public class SWindow
    {
        public SPanel container { get; protected internal set; }
        public SPanel header { get; protected internal set; }
        public SPanel title { get; protected internal set; }
        public SPanel caption { get; protected internal set; }
        public SPanel content { get; protected internal set; }
        public SPanel hooter { get; protected internal set; }
        public Text uiTitle { get; protected internal set; }

        UIInfo uiContainerPanel = new UIInfo().bgColor(new Color(1f, 1f, 1f, 0.1f));
        UIInfo uiContentPanel = new UIInfo().bgColor(new Color(0f, 0f, 0f, 0.3f));
        ActionRunner actionRunner;

        public SWindow() { }

        protected internal void _SWindow(LayoutGroup lgContainer, string title = "", LayoutType hooterLayout = LayoutType.Horizontal,
            UIInfo uiInfo = null, GameObject parent = null)
        {
            if (uiInfo == null) uiInfo = uiContentPanel.Clone();
            this.container = SPanel.CreateFromPanel(lgContainer);

            this.header = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().leFlexWeight(1,0).bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Vertical, parent: container.gameObject, goName: title + "-header"));
            SPanel _title_caption_container = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().leFlexWeight(1,0).bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Horizontal, parent: header.gameObject, goName: title + "-title-caption-container"));
            this.title = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().leFlexWeight(1,0).bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Horizontal, parent: _title_caption_container.gameObject, "left"));  //子要素のサイズに合わせる
            this.caption = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Horizontal, parent: _title_caption_container.gameObject, "right")); //子要素のサイズに合わせる
            uiTitle = this.title.AddText(title);

            this.content = SPanel.CreateFromPanel(
                SWHelper.CreateScrollView(container.gameObject, uiInfo: uiInfo.leFlexWeight(1,1), goName: title + "-content"));
            this.hooter = SPanel.CreateFromPanel(
                SWHelper.CreatePanel(uiInfo: uiInfo, layoutGroup: hooterLayout, parent: container.gameObject, goName: title + "-hooter"));

            actionRunner = container.gameObject.AddComponent<ActionRunner>();
        }

        class ActionRunner : MonoBehaviour
        {
            List<UnityAction> actionList = new List<UnityAction>();
            public void addEvent(UnityAction action) { actionList.Add(action); }
            public void Update()
            {
                foreach (var action in actionList) action();
            }
        }

        internal void updateScrollContentWidth()
        {
            content.gameObject.GetComponent<LayoutElement>().preferredWidth = calcScrollViewWidth();
        }

        public float calcScrollViewWidth()
        { return SWHelper.calcScrollViewWidth(container.gameObject); }



        /// <summary>
        /// ウィンドウを作成して、スクリーン上にUIを配置する。（CanvasのRenderModeがScreenSpaceOverlayになる）
        /// </summary>
        /// <param name="title">ウィンドウのタイトル</param>
        /// <param name="leftbottom">ウィンドウの左下角の座標（原点：スクリーンの左下、右・上方向が正）</param>
        /// <param name="windowSize">ウィンドウのサイズ</param>
        /// <param name="headerLayout">ヘッダーパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="hooterLayout">フッターパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="uiInfo">UI要素のプロパティの指定（背景色とか）</param>
        /// <param name="draggable">ウィンドウをドラッグ可能にするか</param>
        /// <param name="canvasScale">UI要素のサイズを微調整。文字サイズのバランスやUIの配置を保ったままウィンドウを大きくしたいときに使う。</param>
        /// <param name="parent">親GameObject</param>
        /// <returns></returns>
        public void init_onScreen(string title = "", Vector2 leftbottom = default(Vector2), Vector2 windowSize = default(Vector2),
             LayoutType hooterLayout = LayoutType.Horizontal, UIInfo uiInfo = null, bool draggable = true,
             float canvasScale = 1f, GameObject parent = null)
        {
            LayoutGroup lg = SWHelper.CreateCanvas(RenderMode.ScreenSpaceOverlay, leftbottom, windowSize,
                uiInfo: uiContainerPanel, layoutGroup: LayoutType.Vertical,
                draggable4screen: draggable, canvasScale: canvasScale, parent: parent, goName: title + "-window");
            _SWindow(lg, title, hooterLayout, uiInfo, parent);
            onStartInit();
            onEndInit();
        }

        /// <summary>
        /// ウィンドウを作成して、ゲーム空間にUIを配置する。（CanvasのRenderModeがWorldSpaceになる）
        /// </summary>
        /// <param name="title">ウィンドウのタイトル</param>
        /// <param name="leftbottom">ウィンドウの左下角の座標（原点：スクリーンの左下、右・上方向が正）</param>
        /// <param name="windowSize">ウィンドウのサイズ</param>
        /// <param name="headerLayout">ヘッダーパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="hooterLayout">フッターパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="uiInfo">UI要素のプロパティの指定（背景色とか）</param>
        /// <param name="camera">どのカメラを通してUIを操作するかを指定</param>
        /// <param name="meterPerPx">１ピクセルをゲーム空間の何m（or 距離単位）に割り当てるか</param>
        /// <param name="canvasScale">ウィンドウ全体のサイズを微調整。文字サイズのバランスやUIの配置を保ったままウィンドウを大きくしたいときに使う。</param>
        /// <param name="parent">親GameObject</param>
        /// <returns></returns>
        public void init_onWorld(string title = "", Vector2 leftbottom = default(Vector2), Vector2 windowSize = default(Vector2),
            LayoutType hooterLayout = LayoutType.Horizontal, UIInfo uiInfo = null,
             Camera camera = null, float meterPerPx = 0.001f, float canvasScale = 1f, GameObject parent = null)
        {
            LayoutGroup lg = SWHelper.CreateCanvas(RenderMode.WorldSpace, leftbottom, windowSize,
                uiInfo: uiContainerPanel, layoutGroup: LayoutType.Vertical,
                  parent: parent, camera4world: camera, meterPerPx4world: meterPerPx, canvasScale: canvasScale, goName: title + "-window");
            _SWindow(lg, title, hooterLayout, uiInfo, parent: parent);
            onStartInit();
            onEndInit();
        }
        protected internal virtual void onStartInit() { }
        protected internal virtual void onEndInit() { }

        /// <summary>
        /// キャプションボタン（ウィンドウ右上のボタン）を追加
        /// </summary>
        /// <param name="TaskOnClick"></param>
        /// <param name="labelStr"></param>
        /// <param name="texPath"></param>
        /// <param name="tex"></param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public Button addCaptionButton(UnityAction TaskOnClick, string labelStr = "", string texPath = "",
            Texture2D tex = null, Sprite sprite = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null)
        {
            Button ret = caption.AddButton(TaskOnClick, labelStr, texPath, tex, sprite, imgSize, imgColor, uiInfo);
            _disable_flexWidth(ret.gameObject);  //すべてのGameObjectのflexWidthを0に設定
            return ret;
        }
        protected internal void _disable_flexWidth(GameObject go)
        {
            LayoutElement le = go.GetComponent<LayoutElement>();
            if (le != null) le.flexibleWidth = 0f;
            SWHelper.Foreach(go, _disable_flexWidth);
        }

        /// <summary>
        /// ウィンドウの位置・サイズを、座標で指定
        /// （ウィンドウをスクリーン上に配置した場合のみ、正常に実行できる）
        /// </summary>
        /// <param name="left">ウィンドウ左下角の点のX座標</param>
        /// <param name="bottom">ウィンドウ左下角の点のY座標</param>
        /// <param name="width">ウィンドウの幅</param>
        /// <param name="height">ウィンドウの高さ</param>
        public void locate_byPosition(float? left, float? bottom, float? width, float? height)
        {
            RectTransform rt = container.gameObject.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.zero;
            rt.sizeDelta = new Vector2(width ?? rt.sizeDelta.x, height ?? rt.sizeDelta.y);
            //rt.localPosition = new Vector2(left, bottom);
            rt.position = new Vector2(left ?? rt.position.x, bottom ?? rt.position.y);
            updateScrollContentWidth();
        }
        /// <summary>
        /// ウィンドウの位置・サイズを、スクリーン外周からの割合（PCT:PerCenTage）で指定
        /// （ウィンドウをスクリーン上に配置した場合のみ、正常に実行できる）
        /// </summary>
        /// <param name="left">スクリーン左端からの距離の割合</param>
        /// <param name="right">スクリーン右端からの距離の割合</param>
        /// <param name="top">スクリーン上端からの距離の割合</param>
        /// <param name="bottom">スクリーン下端からの距離の割合</param>
        public void locate_byMarginPct(float? left, float? right, float? top, float? bottom)
        {
            RectTransform rt = container.gameObject.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(left ?? rt.anchorMin.x, bottom ?? rt.anchorMin.y);
            rt.anchorMax = new Vector2((1 - right) ?? rt.anchorMax.x, (1 - top) ?? rt.anchorMax.y);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            updateScrollContentWidth();
        }
        /// <summary>
        /// ウィンドウの位置・サイズを、スクリーン外周からのピクセル数で指定
        /// （ウィンドウをスクリーン上に配置した場合のみ、正常に実行できる）
        /// </summary>
        /// <param name="left">スクリーン左端からのピクセル数</param>
        /// <param name="right">スクリーン右端からのピクセル数</param>
        /// <param name="top">スクリーン上端からのピクセル数</param>
        /// <param name="bottom">スクリーン下端からのピクセル数</param>
        public void locate_byMarginPx(float? left, float? right, float? top, float? bottom)
        {
            RectTransform rt = container.gameObject.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left ?? rt.offsetMin.x, bottom ?? rt.offsetMin.y);
            rt.offsetMax = new Vector2((-right) ?? rt.offsetMax.x, (-top) ?? rt.offsetMin.y);
            updateScrollContentWidth();
        }

        public GameObject getCanvasGameObject_orNull()
        {
            if (container == null) return null;
            if (container.gameObject == null) return null;
            return container.gameObject.gameObject.getParent();
        }

        /// <summary>
        /// SWindowオブジェクトを削除。
        /// 実際には、WindowのGameObjectの１階層上の、Canvasを保持するGameObjectを削除する。
        /// </summary>
        public void Dispose()
        {
            var go = getCanvasGameObject_orNull();
            if (go != null) UnityEngine.Object.Destroy(go);
        }
        public void SetActive(bool isActive)
        { getCanvasGameObject_orNull()?.SetActive(isActive); }

        public bool GetActive()
        {
            var go = getCanvasGameObject_orNull();
            if (go != null) return go.activeSelf;
            return false;
        }

        public void setWindowScale(float scale)
        {
            container.gameObject.gameObject.getParent().GetComponent<Canvas>().scaleFactor = scale;
        }

        public void setWindowTitle(string title)
        {
            container.gameObject.gameObject.getParent().name = title;
            uiTitle.text = title;

        }

        /// <summary>毎フレーム実行される処理を登録。containerパネルに対応するGameObjectにアタッチされるので、ウィンドウを破棄したときに無効になる。</summary>
        public void addUpdateAction(UnityAction action)
        {
            actionRunner.addEvent(action);
        }

        protected virtual void onRepaint() { }
        public virtual void repaint()
        {
            this.updateScrollContentWidth();
            onRepaint();
        }

    }
}
