using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.SPIRV;

namespace Futura.Engine.Rendering
{
    public enum ShaderStatus
    {
        Idle, Compiling, Succeeded, Failed
    }

    public class Shader
    {
        internal Veldrid.Shader[] Handles { get => shaders; }
        private Veldrid.Shader[] shaders = new Veldrid.Shader[2];

        public ShaderStatus Status { get; private set; } = ShaderStatus.Idle;

        public void Compile(ResourceFactory factory, byte[] vertexData, byte[] fragmentData, bool debug)
        {
            Status = ShaderStatus.Compiling;

            ShaderDescription vertexShader = new ShaderDescription(ShaderStages.Vertex, vertexData, "main", debug);
            ShaderDescription fragmentShader = new ShaderDescription(ShaderStages.Fragment, fragmentData, "main", debug);

            shaders[0] = factory.CreateFromSpirv(vertexShader);
            shaders[1] = factory.CreateFromSpirv(fragmentShader);

            if(shaders[0] == null || shaders[1] == null)
            {
                Log.Error("Could not complile shader");
                Status = ShaderStatus.Failed;
                return;
            }
            Status = ShaderStatus.Succeeded;
        }

    }
}
