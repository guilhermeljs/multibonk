using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace Multibonk.UserInterface
{

    public abstract class WindowBase
    {
        protected Rect windowRect;
        private bool dragging = false;
        private Vector2 dragOffset;

        protected WindowBase(Rect initialRect)
        {
            windowRect = initialRect;
        }

        private void InitStyles()
        {
            GUI.backgroundColor = Color.black;
        }

        public void Handle()
        {
            Color prevColor = GUI.backgroundColor;
            InitStyles();

            Utils.HandleWindowDrag(ref windowRect, ref dragging, ref dragOffset);

            InitStyles();

            RenderWindow(windowRect);

            GUI.backgroundColor = prevColor;

            GUI.color = Color.white;
        }

        protected abstract void RenderWindow(Rect rect);
    }
}
