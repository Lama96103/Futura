glslangValidator -V "ImGui/ImGui.vert" -o "ImGui/ImGui.vert.spv"
glslangValidator -V "ImGui/ImGui.frag" -o "ImGui/ImGui.frag.spv"

glslangValidator -g -Od -V "Diffuse/Diffuse.vert.glsl" -o "Diffuse/Diffuse.vert.spv" 
glslangValidator -g -Od -V "Diffuse/Diffuse.frag.glsl" -o "Diffuse/Diffuse.frag.spv"

glslangValidator -g -Od -V "DepthShader/Depth.vert.glsl" -o "DepthShader/Depth.vert.spv" 
glslangValidator -g -Od -V "DepthShader/Depth.frag.glsl" -o "DepthShader/Depth.frag.spv" 