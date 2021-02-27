#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 normalVector;
layout(location = 2) in float vertexIndex;
layout(location = 3) in float vertexAmbientOcclusion;

layout(set = 0, binding = 0) uniform WorldBuffer
{
    mat4 Projection;
    mat4 View;
    mat4 ProjectionView;
    vec3 CameraPosition;
};

layout(set = 1, binding = 0) uniform ModelBuffer
{
    mat4 Transform;
    vec4 Color;
};


layout(location = 0) out float VertexIndex;
layout(location = 1) out float VertextAmbientOcclusion;
layout(location = 2) out vec3 FragPos;
layout(location = 3) out vec3 Normal;

out gl_PerVertex 
{
    vec4 gl_Position;
};


void main() 
{
    Normal = mat3(transpose(inverse(Transform))) * normalVector;
    FragPos = vec3(Transform * vec4(vertexPosition, 1.0));
    VertexIndex = vertexIndex;
    VertextAmbientOcclusion = vertexAmbientOcclusion;
    gl_Position = ProjectionView * Transform * vec4(vertexPosition, 1.0);

}