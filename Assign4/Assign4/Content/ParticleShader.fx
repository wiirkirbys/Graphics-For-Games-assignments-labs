
float4x4 World, ViewProj;
float4x4 CamIRot;	// Inverse Camera Matrix
texture2D Texture;

sampler texImage0: register(s0) = sampler_state {
	Texture = <Texture>;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VertexShaderInput
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	float4 ParticlePosition: POSITION1;
	float4 ParticleParameter: POSITION2; // x: Scale x/y: Color
};

struct VertexShaderOutput
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	float4 Color: COLOR0;
};

VertexShaderOutput ParticleVertexShader(VertexShaderInput input) 
{
	VertexShaderOutput output;
	float4 ppos = input.Position;
	ppos = mul(ppos, CamIRot); // Rotate the polygon facing to CAMERA
	ppos.xyz = ppos.xyz * sqrt(input.ParticleParameter.x); 	// Scale the polygon
	ppos += input.ParticlePosition; 	// Move the polygon
	float4 pos = mul(ppos, World);
	output.Position = mul(pos, ViewProj);
	output.TexCoord = input.TexCoord;
	output.Color = 1 - input.ParticleParameter.x / input.ParticleParameter.y; // Change the polygon color
	return output;
}


float4 ParticlePixelShader(VertexShaderOutput input) : COLOR
{
	float4 color = 0;
	color = tex2D(texImage0, input.TexCoord);
	color *= input.Color;
	return color;
}

technique particle {
	pass Pass1 
	{
		VertexShader = compile vs_4_0 ParticleVertexShader();
		PixelShader = compile ps_4_0 ParticlePixelShader();
	}
}
