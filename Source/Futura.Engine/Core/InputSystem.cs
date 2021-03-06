using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Core
{
    internal class InputSystem : SubSystem
    {
        internal InputSnapshot Snapshot { get; private set; }

        private Window window = Window.Instance;
        private float windowWidth = 0;
        private float windowHeight = 0;

        internal Vector2 mousePosition = Vector2.Zero;
        private Vector2 lastMousePosition = Vector2.Zero;
        internal Vector2 mouseOffset = Vector2.Zero;

        private bool[] lastMouseState = new bool[13];
        private bool[] currentMouseState = new bool[13];

        private HashSet<Key> pressedKeys = new HashSet<Key>();
        private HashSet<Key> newPressedKeys = new HashSet<Key>();
        private HashSet<Key> newReleasedKeys = new HashSet<Key>();

        internal override void Init()
        {
            Input.Init(this);
            window.WindowResized += Window_WindowResized;
            windowWidth = window.Width;
            windowHeight = window.Height;
        }

        private void Window_WindowResized(object sender, WindowResizedEventArgs e)
        {
            windowWidth = e.Width;
            windowHeight = e.Height;
        }

        internal override void Tick(double deltaTime)
        {
            Snapshot =  window.PumpEvents();

            mousePosition = Snapshot.MousePosition;
            mouseOffset = mousePosition - lastMousePosition;
            lastMousePosition = mousePosition;

      
            Array.Copy(currentMouseState, 0, lastMouseState, 0, 13);
            for (int i = 0; i < 13; i++)
            {
                currentMouseState[i] = Snapshot.IsMouseDown((MouseButton)i);
            }


            newPressedKeys.Clear();
            newReleasedKeys.Clear();
            for (int i = 0; i < Snapshot.KeyEvents.Count; i++)
            {
                KeyEvent ke = Snapshot.KeyEvents[i];
                if (ke.Down)
                {
                    if (pressedKeys.Add(ke.Key))
                    {
                        newPressedKeys.Add(ke.Key);
                    }
                }
                else
                {
                    pressedKeys.Remove(ke.Key);
                    newPressedKeys.Remove(ke.Key);
                    newReleasedKeys.Add(ke.Key);
                }
            }
        }


        /// <summary>
        /// Checks if the mousestate has changed to down
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        internal bool IsMouseDown(MouseButton mouseButton)
        {
            bool lastState = lastMouseState[(int)mouseButton];
            bool currentState = currentMouseState[(int)mouseButton];

            if (currentState == true && lastState == false) return true;
            else return false;
        }

        /// <summary>
        /// Checks if the mousestate has changed to up
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        internal bool IsMouseUp(MouseButton mouseButton)
        {
            bool lastState = lastMouseState[(int)mouseButton];
            bool currentState = currentMouseState[(int)mouseButton];

            if (currentState == false && lastState == true) return true;
            else return false;
        }


        internal bool IsKeyDown(Key key)
        {
            return newPressedKeys.Contains(key);
        }

        internal bool IsKeyUp(Key key)
        {
            return newReleasedKeys.Contains(key);
        }

        internal bool IsKey(Key key)
        {
            return pressedKeys.Contains(key);
        }
    }
}

namespace Futura
{
    public static class Input
    {
        private static InputSystem input;

        internal static void Init(InputSystem inputSystem)
        {
            if(input != null)
            {
                Log.Error("Input was alread initiliazed");
            }
            Input.input = inputSystem;
        }

        public static Vector2 MousePosition { get => input.mousePosition; }
        public static Vector2 MouseOffset { get => input.mouseOffset; }

        /// <summary>
        /// Return true if the mouse key was pressed
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool IsMouseDown(MouseButton mouseButton)
        {
            return input.IsMouseDown(mouseButton);
        }

        /// <summary>
        /// Returns true if the mouse key was released
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool IsMouseUp(MouseButton mouseButton)
        {
            return input.IsMouseUp(mouseButton);
        }

        /// <summary>
        /// Return true if the mouse key is down
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool IsMouse(MouseButton mouseButton)
        {
            return input.Snapshot.IsMouseDown(mouseButton);
        }

        /// <summary>
        ///  Return true if the key is down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKey(Key key)
        {
            return input.IsKey(key);
        }

        /// <summary>
        /// Return true if the key was pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyDown(Key key)
        {
            return input.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if the key was released
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyUp(Key key)
        {
            return input.IsKeyUp(key);
        }
    }
}
