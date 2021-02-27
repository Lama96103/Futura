#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout(set = 0, binding = 1) uniform sampler MainSampler;
layout(set = 1, binding = 1) uniform ModelAttributesBuffer
{
    float IsSelected;
	float p0;
	float p1;
	float p2;
};

layout(set = 2, binding = 0) uniform ColorFromIdentityBuffer
{
    vec4 color;
};


layout(location = 0) out vec4 FragColor;


void main()
{
	FragColor = color;
}