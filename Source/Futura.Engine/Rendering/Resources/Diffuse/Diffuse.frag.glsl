#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

// -------------------------------- Structs
struct PointLightInfo
{
	vec3 Position;
	float Intensity;
	vec3 Color;
	float Range;
};
struct PointLightsInfo
{
	PointLightInfo PointLights[4];
	int NumActiveLights;
	float _padding0;
	float _padding1;
	float _padding2;
};

// -------------------------------- Structs END



// -------------------------------- Layouts
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;
layout(location = 2) in vec2 vertexUV;

layout(location = 0) out vec4 FragmentColor;
layout(location = 1) out vec4 SelectionColor;
layout(location = 2) out vec4 DepthColor;

// -------------------------------- Layouts END


// -------------------------------- Uniforms
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

	float modelLightingEnabled;
};

layout(set = 2, binding = 0) uniform LightingBuffer
{
	vec3 directionalLightColor;
	float directionalLightIntensity;

	vec3 directionalLightDirection;
	float ambientLightIntensity;
};

layout(set = 2, binding = 1) uniform PointLightBuffer
{
	PointLightsInfo pointLightsInfo;
};
// -------------------------------- Uniforms END







vec3 CalculateDirectionalLight(vec3 normal, vec3 lightDir)
{
	float diff = max(dot(normal, lightDir), 0.0);
	return (directionalLightColor * diff) * directionalLightIntensity;
}

vec3 CalculateAmbientLight()
{
	return directionalLightColor * ambientLightIntensity;
}


void main()
{
	if(modelLightingEnabled == 1.0)
	{
		vec3 normalizedNormal = normalize(vertexNormal);
		// vec3 lightDir = normalize(-directionalLightDirection);
		vec3 lightDir = normalize(directionalLightDirection);

		vec3 directional = CalculateDirectionalLight(normalizedNormal, lightDir);
		vec3 ambient = CalculateAmbientLight();

		vec4 pointDiffuse = vec4(0, 0, 0, 1);
		for (int i = 0; i < pointLightsInfo.NumActiveLights; i++)
		{
			PointLightInfo pli = pointLightsInfo.PointLights[i];
			vec3 ptLightDir = normalize(pli.Position - vertexPosition);
			float intensity = clamp(dot(normalizedNormal, ptLightDir), 0, 1);
			float lightDistance = distance(pli.Position, vertexPosition);
			intensity = clamp(intensity * (1 - (lightDistance / pli.Range)), 0, 1);
			pointDiffuse += (intensity * vec4(pli.Color, 1)) * pli.Intensity;
		}

		FragmentColor = (vec4((ambient + directional), 1.0) + pointDiffuse) * modelDiffuseColor;
	}
	else
	{
		FragmentColor = modelDiffuseColor;
	}


	// Different render targets
	SelectionColor = modelSelectionColor;
	DepthColor = vec4(vec3(gl_FragCoord.z), 1.0);
}

