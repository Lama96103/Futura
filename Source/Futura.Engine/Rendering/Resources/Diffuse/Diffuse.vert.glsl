#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;
layout(location = 2) in vec2 vertexUV;

layout(location = 0) out vec3 FragPos;
layout(location = 1) out vec3 Normal;
layout(location = 2) out vec2 UV;

out gl_PerVertex 
{
    vec4 gl_Position;
};

layout(set = 0, binding = 0) uniform WorldBuffer
{
    mat4 worldProjection;
    mat4 worldView;
    mat4 worldProjectionView;

    vec3 worldCameraPosition;
	float worldCameraNear;
	float worldCameraFar;
};

layout(set = 1, binding = 0) uniform ModelBuffer
{
    mat4 modelTransform;
    vec4 modelSelectionColor;
	vec4 modelDiffuseColor;
};




void main() 
{
    Normal = mat3(transpose(inverse(modelTransform))) * vertexNormal;
    FragPos = vec3(modelTransform * vec4(vertexPosition, 1.0));
    gl_Position = worldProjectionView * modelTransform * vec4(vertexPosition, 1.0);
    UV = vertexUV;
}