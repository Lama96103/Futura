using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    public static class FileDialog
    {
        public static bool Open(string label, string filter, ref FileInfo selectedFile, ref DirectoryInfo searchDir)
        {
            string selectedFileName = selectedFile == null ? "" : selectedFile.Name;

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(-1));
            ImGui.Begin("File Dialog - " + label, ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize);

            ImGui.TextDisabled(searchDir.FullName);

            ImGui.BeginChild(1, new System.Numerics.Vector2(300, 250));
            ImGui.TextDisabled("Folder");

            if (searchDir.Parent != null)
            {
                if (ImGui.Selectable(".."))
                {
                    searchDir = searchDir.Parent;
                }
            }

            foreach (DirectoryInfo dir in searchDir.GetDirectories())
            {
                if (ImGui.Selectable(dir.Name))
                {
                    searchDir = dir;
                }
            }
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginChild(2, new System.Numerics.Vector2(300, 250));
            ImGui.TextDisabled("Files");
            foreach (FileInfo dir in searchDir.GetFiles(filter))
            {
                if (ImGui.Selectable(dir.Name))
                {
                    selectedFile = dir;
                    selectedFileName = dir.Name;
                }
            }
            ImGui.EndChild();


            ImGui.InputText("", ref selectedFileName, 200, ImGuiInputTextFlags.ReadOnly);


            if (ImGui.Button("Close"))
            {
                ImGui.End();
                return true;
            }
            ImGui.SameLine(500, 0);
            if (ImGui.Button("Open"))
            {
                ImGui.End();
                return true;
            }

            ImGui.End();
            return false;
        }

        public static bool Save(string label, string filter, ref FileInfo selectedFile, ref DirectoryInfo searchDir)
        {
            string selectedFileName = selectedFile == null ? "" : selectedFile.Name;

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(-1));
            ImGui.Begin("File Dialog - " + label, ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize);

            ImGui.TextDisabled(searchDir.FullName);

            ImGui.BeginChild(1, new System.Numerics.Vector2(300, 250));
            ImGui.TextDisabled("Folder");

            if (searchDir.Parent != null)
            {
                if (ImGui.Selectable(".."))
                {
                    searchDir = searchDir.Parent;
                }
            }

            foreach (DirectoryInfo dir in searchDir.GetDirectories())
            {
                if (ImGui.Selectable(dir.Name))
                {
                    searchDir = dir;
                }
            }
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginChild(2, new System.Numerics.Vector2(300, 250));
            ImGui.TextDisabled("Files");
            foreach (FileInfo dir in searchDir.GetFiles(filter))
            {
                if (ImGui.Selectable(dir.Name))
                {
                    selectedFile = dir;
                    selectedFileName = dir.Name;
                }
            }
            ImGui.EndChild();


            if (ImGui.InputText("", ref selectedFileName, 200))
            {
                string extension = filter.Split('.')[1];
                if (extension != "*")
                {
                    if (!selectedFileName.EndsWith("." + extension))
                    {
                        selectedFileName += "." + extension;
                    }
                }

                selectedFile = new FileInfo(Path.Combine(searchDir.FullName, selectedFileName));
            }


            if (ImGui.Button("Close"))
            {
                ImGui.End();
                selectedFile = null;
                return true;
            }
            ImGui.SameLine(500, 0);
            if (ImGui.Button("Save"))
            {
                ImGui.End();
                return true;
            }

            ImGui.End();
            return false;
        }
    }
}
