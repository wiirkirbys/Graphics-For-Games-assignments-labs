float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 LightPosition;
float3 CameraPosition;

struct VertexInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct VertexShaderOutput {
	float4 Position : POSITION;
	float4 Normal : TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

VertexShaderOutput PhongVertexShader(VertexInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	return output;
}

float4 PhongPixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 color = float4(N, 1); //normal is the color for quads
	return color;
}

technique Phong
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShader();
		PixelShader = compile ps_4_0 PhongPixelShaderFunction();
	}
}