using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{
    public class ContainerMode
    {
        public readonly static ContainerMode FIXED = new ContainerMode("Fixed");
        public readonly static ContainerMode VARIABLE = new ContainerMode("Variable");
        private string mode;

        protected ContainerMode(string mode) { this.mode = mode; }
        public override string ToString() { return mode; }
    }
    /// <summary>uGUI Builder Container</summary>
    public class BContainer : BPanel
    {

        ContainerMode mode;
        Canvas canvas;
        RectTransform rtContainer;

        protected BContainer(ContainerMode containerMode, LayoutGroup lg) : base(lg)
        {
            rtContainer = this.gameObject.GetComponent<RectTransform>();
            canvas = this.gameObject.getParent().GetComponent<Canvas>();
            mode = containerMode;
        }

        /// <param name="renderMode">RenderMode.WorldSpace or RenderMode.ScreenSpaceOverlay</param>
        /// <param name="uiInfo">rtAnchoredPosition:container position, rtSizeDelta: container size</param>
        public static BContainer Create(RenderMode renderMode, string goCanvasName = "Canvas", ContainerMode containerMode = null,
             UIInfo uiInfo = null, bool draggable4screen = true,  // Canvas parameters
             LayoutType Layout = LayoutType.Vertical, // Panel parameters
             float canvasScale = 1f, GameObject parent = null)
        {
            if (containerMode == null) containerMode = ContainerMode.FIXED;
            if (uiInfo == null) uiInfo = UIInfo.CANVAS_DEFAULT;

            LayoutGroup lg = BHelper.CreateCanvas(renderMode,
                uiInfo: uiInfo, layoutGroup: LayoutType.Vertical,
                draggable4screen: draggable4screen, canvasScale: canvasScale, parent: parent, goName: goCanvasName);
            if (containerMode == ContainerMode.VARIABLE)
            {
                var csf = lg.gameObject.GetOrAddComponent<ContentSizeFitter>();
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

               var rt = lg.gameObject.GetOrAddComponent<RectTransform>();
                rt.pivot = new Vector2(0, 1);  // Expand to the bottom/right.
            }
            else { } // FIXED : pass
            var ret = new BContainer(containerMode, lg);
            return ret;
        }


        /// <summary>ScreenSpace only.</summary>
        public void locate_byPosition(float? left, float? bottom, float? width, float? height)
        {
            rtContainer.anchorMin = Vector2.zero;
            rtContainer.anchorMax = Vector2.zero;
            rtContainer.sizeDelta = new Vector2(width ?? rtContainer.sizeDelta.x, height ?? rtContainer.sizeDelta.y);
            rtContainer.position = new Vector2(left ?? rtContainer.position.x, bottom ?? rtContainer.position.y);
        }

        /// <summary>ScreenSpace only. Locate window by "percentage from screen circumference". </summary>
        public void locate_byMarginPct(float? left, float? right, float? top, float? bottom)
        {
            rtContainer.anchorMin = new Vector2(left ?? rtContainer.anchorMin.x, bottom ?? rtContainer.anchorMin.y);
            rtContainer.anchorMax = new Vector2((1 - right) ?? rtContainer.anchorMax.x, (1 - top) ?? rtContainer.anchorMax.y);
            rtContainer.offsetMin = Vector2.zero;
            rtContainer.offsetMax = Vector2.zero;
        }

        /// <summary>ScreenSpace only. Locate window by "px from screen circumference". </summary>
        public void locate_byMarginPx(float? left, float? right, float? top, float? bottom)
        {
            rtContainer.anchorMin = Vector2.zero;
            rtContainer.anchorMax = Vector2.one;
            rtContainer.offsetMin = new Vector2(left ?? rtContainer.offsetMin.x, bottom ?? rtContainer.offsetMin.y);
            rtContainer.offsetMax = new Vector2((-right) ?? rtContainer.offsetMax.x, (-top) ?? rtContainer.offsetMin.y);
        }

    }
}
