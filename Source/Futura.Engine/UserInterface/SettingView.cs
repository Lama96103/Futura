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
        private string txt_WindowName;
        private string txt_ChildSelector;
        private string txt_ChildSettings;
        private string txt_CloseButton;
        private string txt_SaveButton;

        private SettingsController settingsController;
        private string name;

        private bool didChanges = false;
        object selected = null;

        public SettingView(SettingsController settingsController, string name)
        {
            this.settingsController = settingsController;
            this.name = name;
        }

        public override void Init()
        {
            txt_WindowName = $"{name} Settings##{ID}";
            txt_ChildSelector = $"Selector##{ID}";
            txt_ChildSettings = $"Setting##{ID}";
            txt_CloseButton = $"Close##{ID}";
            txt_SaveButton = $"Save##{ID}";
        }

        public override void Tick()
        {
            ImGuiWindowFlags flags = ImGuiWindowFlags.None;
            if (didChanges)
                flags = ImGuiWindowFlags.UnsavedDocument;

            SetInitalWindowSize(350, 200);
            ImGui.Begin(txt_WindowName, ref isOpen, flags);
            List<object> settingCategories = settingsController.GetAll();

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 200);
            ImGui.BeginChild(txt_ChildSelector);
            foreach (var obj in settingCategories)
            {
                string name = Helper.UpperCaseSpace(obj.GetType().Name.Replace("Settings", ""));
                if (ImGui.Selectable(name, obj == selected))
                {
                    selected = obj;
                }
            }
            if (ImGui.Button(txt_CloseButton))
            {
                UIController.Instance.Unregister(this);
            }
            ImGui.EndChild();

            ImGui.NextColumn();
            ImGui.BeginChild(txt_ChildSettings);

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

                if (ImGui.Button(txt_SaveButton))
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
