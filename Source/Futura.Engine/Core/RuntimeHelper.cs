using Futura.ECS;
using Futura.Engine.ECS;
using Futura.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    class RuntimeHelper : Singleton<RuntimeHelper>
    {
        private Asset selectedAsset;
        public Asset SelectedAsset
        {
            get { return selectedAsset; }
            set
            {
                if (SelectedEntity != null) SelectedEntity = null;
                selectedAsset = value;
                AssetSelectionChanged?.Invoke(this, new AssetSelectionChangedEventArgs(selectedAsset));
            }
        }
        public event EventHandler<AssetSelectionChangedEventArgs> AssetSelectionChanged;

        private Entity selectedEntity;
        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (SelectedAsset != null) SelectedAsset = null;
                if (selectedEntity != null) selectedEntity.GetComponent<RuntimeComponent>().IsSelected = false;
                selectedEntity = value;
                if (selectedEntity != null) selectedEntity.GetComponent<RuntimeComponent>().IsSelected = true;
                EntitySelectionChanged?.Invoke(this, new EntitySelectionChangedEventArgs(selectedEntity));
            }
        }
        public event EventHandler<EntitySelectionChangedEventArgs> EntitySelectionChanged;
    }

    class EntitySelectionChangedEventArgs
    {
        public Entity Entity { get; private set; }

        public EntitySelectionChangedEventArgs(Entity entity)
        {
            Entity = entity;
        }
    }

    class AssetSelectionChangedEventArgs : EventArgs
    {
        public Asset Asset { get; private set; }

        public AssetSelectionChangedEventArgs(Asset asset)
        {
            this.Asset = asset;
        }
    }
}
