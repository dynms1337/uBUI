using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{
    public class LayoutType
    {
        public static readonly LayoutType Grid = new LayoutType();
        public static readonly LayoutType Horizontal = new LayoutType();
        public static readonly LayoutType Vertical = new LayoutType();
        public static readonly LayoutType None = new LayoutType();
    }


}
