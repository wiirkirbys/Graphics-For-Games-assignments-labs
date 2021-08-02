float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4 AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float3 LightPosition;
float3 CameraPosition;
float Shininess;
float4 SpecularColor;
float SpecularIntensity;
texture normalMap;
texture environmentMap;
bool mipmap;
float3 UVW;

struct VertexInput {
	float4 Position: POSITION0;
	float4 Normal: NORMAL0;
	float4 Tangent: TANGENT0;
	float4 Binormal: BINORMAL0;
	float2 TexCoord: TEXCOORD0;
};

struct VertexShaderOutput {
	float4 Position: POSITION0;
	float3 Normal: TEXCOORD0;
	float3 Tangent: TEXCOORD1;
	float3 Binormal: TEXCOORD2;
	float2 TexCoord: TEXCOORD3;
	float3 Position3D: POSITION4;

};

sampler tsampler1 = sampler_state
{
	texture = <normalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
};
sampler tsampler2 = sampler_state
{
	texture = <normalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = None;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
};

VertexShaderOutput VertexShaderFunction(VertexInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Normal = mul(input.Normal, WorldInverseTranspose).xyz;
	output.Tangent = mul(input.Tangent, WorldInverseTranspose).xyz;
	output.Binormal = mul(input.Binormal, WorldInverseTranspose).xyz;
	output.Position3D = worldPosition.xyz;
	output.TexCoord = input.TexCoord;
	return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 L = normalize((LightPosition - input.Position3D));
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 N = input.Normal;
	float3 T = input.Tangent;
	float3 B = input.Binormal;
	float3 H = normalize(L + V);
	float2 scaleTexCoord = { input.TexCoord.x * UVW.x, input.TexCoord.y * UVW.y };
	float3 normalTex;
	if (mipmap) normalTex = tex2D(tsampler1, scaleTexCoord).xyz;
	else normalTex = tex2D(tsampler2, scaleTexCoord).xyz;
	normalTex = normalize(normalTex);
	float3 bumpNormal = normalize(N * UVW.z + normalTex.x * T + normalTex.y * B);
	float4 diffuse = DiffuseColor * DiffuseIntensity * max(0, dot(bumpNormal, L));
	float4 specular = SpecularColor * SpecularIntensity * pow(saturate(dot(H, bumpNormal)), Shininess);

	return min(1, max(0, (diffuse + specular)));
}


technique Bump
{
	pass Pass1 {
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}


