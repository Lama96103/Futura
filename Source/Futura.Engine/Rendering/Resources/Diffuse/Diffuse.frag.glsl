#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable


layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;
layout(location = 2) in vec2 vertexUV;

layout(location = 0) out vec4 FragmentColor;
layout(location = 1) out vec4 SelectionColor;
layout(location = 2) out vec4 DepthColor;



layout(set = 0, binding = 0) uniform WorldBuffer
{
    mat4 worldProjection;
    mat4 worldView;
    mat4 worldProjectionView;
    vec3 worldCameraPosition;
	float worldCameraNear;
	float worldCameraFar;
};
layout(set = 0, binding = 1) uniform sampler MainSampler;

layout(set = 1, binding = 0) uniform ModelBuffer
{
    mat4 modelTransform;
    vec4 modelSelectionColor;
	vec4 modelDiffuseColor;
};





void main()
{
	FragmentColor = modelDiffuseColor;
	SelectionColor = modelSelectionColor;
	DepthColor = vec4(vec3(gl_FragCoord.z), 1.0);
}

