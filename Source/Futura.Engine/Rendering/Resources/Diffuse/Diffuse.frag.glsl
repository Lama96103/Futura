#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

#define MAX_POINT_LIGHTS 5

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

struct LightingInfo
{
	vec3 CameraPos;
	float AmountOfPointLights;
};

struct DirectionalLight
{
	vec3 Direction;
	float _padding;
	vec4 Color;
};

struct PointLight
{
	vec3 Position;
	float Intensity;
	vec4 Color;
};



layout(location = 0) in float vertexIndex;
layout(location = 1) in float vertextAmbientOcclusion;
layout(location = 2) in vec3 fragPos;
layout(location = 3) in vec3 normal;

layout(location = 0) out vec4 FragColor;
layout(location = 1) out vec4 SelectionColor;
layout(location = 2) out vec4 DepthColor;


layout(set = 0, binding = 1) uniform sampler MainSampler;


layout(set = 2, binding = 0) uniform texture2D MainTexture; 


layout(set = 3, binding = 0) uniform LightingInfoBuffer
{
	LightingInfo LightingInfoProperties;
};
layout(set = 3, binding = 1) uniform DirectionalLightBuffer
{
	DirectionalLight DirectionalLightProperties;
};
layout(set = 3, binding = 2) uniform PointLightBuffer
{
	PointLight PointLightProperties[MAX_POINT_LIGHTS];
};

vec4 CalculateDirectionalLight(DirectionalLight properties, vec3 normalizedNormal)
{
	vec3 lightDir = normalize(-properties.Direction);
	float diff = max(dot(normalizedNormal, lightDir), 0.0f);

	vec4 diffuseColor = diff * properties.Color;

	return diffuseColor;
}

vec4 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.Position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 1);
    // attenuation
    float distance    = length(light.Position - fragPos);
    float attenuation = 1.0 / (1.0 + light.Intensity * distance + 
  			     0.20 * (distance * distance));    
    // combine results
    vec4 ambient  = light.Color * 0.05f;
    vec4 diffuse  = light.Color  * diff;
    vec4 specular = vec4(1.0f) * spec;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
} 

void main()
{
	vec3 normalizedNormal = normalize(normal);
	vec3 viewDir = normalize(LightingInfoProperties.CameraPos - fragPos);

	// vec4 ambientLight = vec4(0.1f, 0.1f, 0.1f, 1.0f);
	vec4 directionalLight = CalculateDirectionalLight(DirectionalLightProperties, normalizedNormal);
	vec4 pointLight = vec4(0.0f);
	for(int i = 0; i < LightingInfoProperties.AmountOfPointLights; i++)
	{
		pointLight += CalculatePointLight(PointLightProperties[i], normalizedNormal, fragPos, viewDir);
	}
	pointLight.w = 1.0f;

	vec4 ambientOcclusionColor = vec4(vertextAmbientOcclusion, vertextAmbientOcclusion, vertextAmbientOcclusion, 0.0f);
	

	vec4 textureColor = texture(sampler2D(MainTexture, MainSampler), vec2((vertexIndex-1)/256.0f, 0));
	//FragColor = textureColor * (directionalLight + pointLight - ambientOcclusionColor);
	FragColor = (directionalLight + pointLight - ambientOcclusionColor);

	FragColor = vec4(1.0, 1.0, 1.0, 1.0);
	SelectionColor = Color;
	DepthColor = vec4(gl_FragCoord.z);
}

