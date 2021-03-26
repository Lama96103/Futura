using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    partial class Runtime
    {
        public interface IRuntimeCommand
        {
            public void Execute(Runtime runtime, Context context);
        }

        public class ChangeRuntimeStateCommand : Runtime.IRuntimeCommand
        {
            public Runtime.RuntimeState State { get; set; }

            public ChangeRuntimeStateCommand(RuntimeState state)
            {
                State = state;
            }

            public void Execute(Runtime runtime, Context context)
            {
                WorldSystem worldSystem = context.GetSubSystem<WorldSystem>();

                if(runtime.State == RuntimeState.Editor)
                {
                    worldSystem.Save();
                }
                else
                {
                    if (State == RuntimeState.Editor)
                    {
                        worldSystem.Load(worldSystem.CurrentSceneFile);
                        RuntimeHelper.Instance.SelectedEntity = null;
                        RuntimeHelper.Instance.SelectedAsset = null;
                    }
                }

                context.GetSubSystem<RenderSystem>().UseEditorCamera = State != RuntimeState.Playing;

                runtime.State = State;
            }
        }

        public class LoadSceneCommand : Runtime.IRuntimeCommand
        {
            public FileInfo SceneFile { get; set; }

            public LoadSceneCommand(FileInfo sceneFile)
            {
                SceneFile = sceneFile;
            }

            public void Execute(Runtime runtime, Context context)
            {
                WorldSystem world = context.GetSubSystem<WorldSystem>();
                RuntimeHelper.Instance.SelectedAsset = null;
                RuntimeHelper.Instance.SelectedEntity = null;
                world.Load(SceneFile);
            }
        }

        public class SaveSceneCommand : Runtime.IRuntimeCommand
        {
            public void Execute(Runtime runtime, Context context)
            {
                WorldSystem world = context.GetSubSystem<WorldSystem>();
                world.Save();
            }
        }
    }
}
