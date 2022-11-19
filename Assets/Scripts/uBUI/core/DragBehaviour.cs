using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.core
{
    /// <summary>UI要素をドラッグ移動できるようにするコンポーネント </summary>
    class DragBehaviour : MonoBehaviour, IDragHandler
    {
        public RectTransform m_rectTransform = null;
        private void Start() { m_rectTransform = GetComponent<RectTransform>(); }
        public void OnDrag(PointerEventData e) { m_rectTransform.position += new Vector3(e.delta.x, e.delta.y, 0f); }
    }
}

