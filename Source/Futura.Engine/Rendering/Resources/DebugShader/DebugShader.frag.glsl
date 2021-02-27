#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable


layout(location = 0) in float vertexIndex;
layout(location = 0) out vec4 FragColor;

layout(set = 0, binding = 1) uniform sampler MainSampler;
layout(set = 1, binding = 1) uniform ModelAttributesBuffer
{
    float IsSelected;
	float p0;
	float p1;
	float p2;
};


void main()
{
	if(IsSelected == 1)
	{
		FragColor = vec4(0.8f, 0.8f, 0.2f, 1.0f);
	}
	else
	{
		FragColor = vec4(0.5f, 0.5f, 1.0f, 1.0f);
	}
}