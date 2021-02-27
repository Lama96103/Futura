using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Futura.Engine.Rendering
{
    public class RenderAPI
    {
        public Context Context { get; init; }

        internal GraphicsDevice GraphicAPI { get; init; }

        /// <summary>
        /// Gets a value indicating whether this device's depth values range from 0 to 1. If false, depth values instead range from -1 to 1.
        /// </summary>
        public static bool IsDepthRangeZeroToOne { get; private set; }
        public string CurrentAPI { get { return GraphicAPI.BackendType.ToString(); } }

        internal ResourceFactory Factory { get => GraphicAPI.ResourceFactory; }


        internal RenderAPI(Context context, Window window)
        {
            this.Context = context;


            GraphicsDeviceOptions options = new GraphicsDeviceOptions()
            {
                PreferDepthRangeZeroToOne = true,
                SyncToVerticalBlank = false,
                SwapchainDepthFormat = PixelFormat.R16_UNorm,
#if DEBUG
                Debug = true
#else
                Debug = false
#endif
            };

            if(window == null)
            {
                Log.Error("Window does not exist, RenderAPI will not be created");
                return;
            }

            GraphicAPI = VeldridStartup.CreateGraphicsDevice(window.Handle, options, GraphicsBackend.Direct3D11);

            IsDepthRangeZeroToOne = GraphicAPI.IsDepthRangeZeroToOne;
        }

        /// <summary>
        /// Get CommandList used for Rendering
        /// </summary>
        /// <returns></returns>
        public CommandList GenerateCommandList()
        {
            return GraphicAPI.ResourceFactory.CreateCommandList();
        }

        /// <summary>
        /// Submits CommandList to Graphics Device
        /// </summary>
        /// <param name="commandList"></param>
        public void SubmitCommands(CommandList commandList)
        {
            GraphicAPI.SubmitCommands(commandList);
        }

        /// <summary>
        /// Blocks the current thread until the GraphicsDevice finished rendering
        /// </summary>
        public void WaitForIdle()
        {
            GraphicAPI.WaitForIdle();
        }

        /// <summary>
        /// Swaps Buffers in Graphics Device
        /// </summary>
        public void SwapBuffers()
        {
            GraphicAPI.SwapBuffers();
        }

        /// <summary>
        /// Get new window size to Graphic Device
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResizeWindow(uint width, uint height)
        {
            GraphicAPI.ResizeMainWindow(width, height);
        }

        /// <summary>
        /// Disposes a graphics ressource safly when it not used anymore
        /// </summary>
        /// <param name="disposable"></param>
        public void DisposeWhenIdle(IDisposable disposable)
        {
            GraphicAPI.DisposeWhenIdle(disposable);
        }
    }
}
