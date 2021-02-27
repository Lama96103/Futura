#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 normalVector;
layout(location = 2) in float vertexIndex;
layout(location = 3) in float vertexAmbientOcclusion;

layout(set = 0, binding = 0) uniform ProjectionViewMatrixBuffer
{
    mat4 projection_view_matrix;
};

layout(set = 1, binding = 0) uniform ModelMatrixBuffer
{
    mat4 model_matrix;
};


layout(location = 0) out float VertexIndex;

out gl_PerVertex 
{
    vec4 gl_Position;
};


void main() 
{
    VertexIndex = vertexIndex;
    gl_Position = projection_view_matrix * model_matrix * vec4(vertexPosition, 1.0);

}