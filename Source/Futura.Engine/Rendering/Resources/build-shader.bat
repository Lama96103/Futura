glslangValidator -V "ImGui/ImGui.vert" -o "ImGui/ImGui.vert.spv"
glslangValidator -V "ImGui/ImGui.frag" -o "ImGui/ImGui.frag.spv"

glslangValidator -V "Diffuse/Diffuse.vert.glsl" -o "Diffuse/Diffuse.vert.spv"
glslangValidator -V "Diffuse/Diffuse.frag.glsl" -o "Diffuse/Diffuse.frag.spv"

glslangValidator -V "Selection/Selection.vert" -o "Selection/Selection.vert.spv"
glslangValidator -V "Selection/Selection.frag" -o "Selection/Selection.frag.spv"

glslangValidator -V "DebugShader/DebugShader.vert.glsl" -o "DebugShader/DebugShader.vert.spv"
glslangValidator -V "DebugShader/DebugShader.frag.glsl" -o "DebugShader/DebugShader.frag.spv"