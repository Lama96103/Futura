using Futura.Engine.Core;
using Futura.Engine.UserInterface.Properties;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Futura.Engine.UserInterface
{
    class SettingView : View
    {
        private SettingsController settingsController;
        private string name;

        private bool didChanges = false;

        public SettingView(SettingsController settingsController, string name)
        {
            this.settingsController = settingsController;
            this.name = name;
        }

        object selected = null;

      
        public override void Tick()
        {
            ImGuiWindowFlags flags = ImGuiWindowFlags.None;
            if (didChanges)
            {
                flags = ImGuiWindowFlags.UnsavedDocument;
            }
            ImGui.Begin(name + " Settings", flags);
            List<object> settingCategories = settingsController.GetAll();

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 200);
            ImGui.BeginChild("selector");
            foreach (var obj in settingCategories)
            {
                if (ImGui.Selectable(obj.GetType().Name, obj == selected))
                {
                    selected = obj;
                }
            }
            if (ImGui.Button("Close##SettingView"))
            {
                UIController.Instance.Unregister(this);
            }
            ImGui.EndChild();

            ImGui.NextColumn();
            ImGui.BeginChild("settingsdetails");

            if (selected != null)
            {
                var fields = selected.GetType().GetFields();
                foreach (var f in fields)
                {
                    var serializer = PropertySerializerHelper.GetSerializer(f.FieldType);
                    if (serializer == null)
                    {
                        ImGui.LabelText(f.Name, "TODO - " + f.FieldType.Name);
                    }
                    else
                    {
                        bool didChange = serializer.Serialize(selected, f);
                        if (didChange) didChanges = true;
                    }
                }

                if (ImGui.Button("Save##SettingView"))
                {
                    settingsController.Save(selected);
                    didChanges = false;
                }
            }
            else
            {
                ImGui.TextDisabled("Please select settings category");
            }


            ImGui.EndChild();
            ImGui.End();
        }
    }
}
