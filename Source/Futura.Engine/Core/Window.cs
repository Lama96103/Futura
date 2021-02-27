using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Futura.Engine.Core
{
    public enum WindowState
    {
        Normal = 0,
        FullScreen = 1,
        Maximized = 2,
        Minimized = 3,
        BorderlessFullScreen = 4,
        Hidden = 5
    }

    public sealed class Window : Singleton<Window>
    {
        internal Sdl2Window Handle { get; private set; } = null;

        /// <summary>
        /// Gets the Width of the window
        /// </summary>
        public int Width { get => Handle.Width; }
        /// <summary>
        /// Gets the Height of the window
        /// </summary>
        public int Height { get => Handle.Height; }
        /// <summary>
        /// Checks if the window was already created
        /// </summary>
        public bool Exists
        {
            get
            {
                return Handle.Exists;
            }
        }

        public float AspectRatio
        {
            get
            {
                return (float)Handle.Width / (float)Handle.Height;
            }
        }

        public void Init(int x, int y, int width, int height, string title, WindowState state)
        {
            WindowCreateInfo createInfo = new WindowCreateInfo()
            {
                X = x,
                Y = y,
                WindowHeight = height,
                WindowWidth = width,
                WindowTitle = title,
                WindowInitialState = (Veldrid.WindowState)state
            };

            Handle = VeldridStartup.CreateWindow(ref createInfo);
        }

        public InputSnapshot PumpEvents()
        {
            return Handle.PumpEvents();
        }
    }
}
