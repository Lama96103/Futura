using Futura.ECS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Components
{
    public class Transform : IComponent
    {
        [SerializeField] [Name("Position")]
        private Vector3 localPosition = Vector3.Zero;
        [SerializeField] [Name("Rotation")]
        private Quaternion localRotation = Quaternion.Identity;
        [SerializeField] [Name("Scale")]
        private Vector3 localScale = Vector3.One;

        [JsonIgnore] public Matrix4x4 LocalMatrix { get; private set; } = Matrix4x4.Identity;


        public Vector3 Position
        {
            get
            {
                return localPosition;
            }
            set
            {
                if(localPosition != value)
                {
                    localPosition = value;
                    UpdateTransform();
                }
            }
        }

        public Quaternion Rotation
        {
            get => localRotation;
            set
            {
                if(localRotation != value)
                {
                    localRotation = value;
                    UpdateTransform();
                }
            }
        }

        public Vector3 Scale
        {
            get => localScale;
            set
            {
                if (localScale != value)
                {
                    localScale = value;
                    UpdateTransform();
                }
            }
        }


        public Vector3 Forward()
        {
            return Vector3.Transform(Vector3.UnitZ, localRotation);
        }

        public Vector3 Right()
        {
            return Vector3.Transform(-Vector3.UnitX, localRotation);
        }

        public Vector3 Up()
        {
            return Vector3.Transform(Vector3.UnitY, localRotation);
        }

             
        internal void UpdateTransform()
        {
            LocalMatrix = CalculateModelMatrix(localPosition, localRotation, localScale);
        }


        public void Translate(in Vector3 delta)
        {
            Position = localPosition + delta;
        }

        public void Rotate(in Quaternion delta)
        {
            Rotation = Quaternion.Normalize(Quaternion.Multiply(localRotation, delta));
        }



        #region Static Functions
        public static Matrix4x4 CalculateModelMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateTranslation(position);
        }
        #endregion
    }
}
