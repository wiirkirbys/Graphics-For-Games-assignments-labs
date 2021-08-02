texture DepthAndNormalTexture;
float offset;
float depthDiff;
float normalDiff;

sampler normalMap = sampler_state 
{
	Texture = <DepthAndNormalTexture>;
	MipFilter = NONE;
	MinFilter = POINT;
	MagFilter = POINT;
};
struct VS_OUTPUT
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	float2 PlusPlus: TEXCOORD1;
	float2 PlusMinus: TEXCOORD2;
	float2 MinusPlus: TEXCOORD3;
	float2 MinusMinus: TEXCOORD4;
};

VS_OUTPUT RenderSceneVS(float4 vPos: POSITION, float2 TexCoord : TEXCOORD0) 
{
	VS_OUTPUT Output;
	Output.Position = vPos;
	vPos.xy = sign(vPos.xy);
	Output.TexCoord.x = (vPos.x + 1.0f) * 0.5f;
	Output.TexCoord.y = 1.0f - (vPos.y + 1.0f) * 0.5f;
	Output.PlusPlus.x = Output.TexCoord.x+offset;
	Output.PlusPlus.y = Output.TexCoord.y+offset;
	Output.PlusMinus.x = Output.TexCoord.x+offset;
	Output.PlusMinus.y = Output.TexCoord.y-offset;
	Output.MinusPlus.x = Output.TexCoord.x-offset;
	Output.MinusPlus.y = Output.TexCoord.y+offset;
	Output.MinusMinus.x = Output.TexCoord.x-offset;
	Output.MinusMinus.y = Output.TexCoord.y-offset;
	return Output;
}
float4 RenderScenePS0(VS_OUTPUT Input) :COLOR0
{
	float4 thisPixel = tex2D(normalMap,Input.TexCoord);
	float4 nextPixel = tex2D(normalMap,Input.PlusPlus);
	float3 thisNormal = thisPixel.xyz * 2.0f - 1.0f;
	float3 nextNormal = nextPixel.xyz * 2.0f - 1.0f;
	if(abs(thisPixel.a - nextPixel.a) >= depthDiff || abs(thisNormal - nextNormal).x >= normalDiff || abs(thisNormal - nextNormal).y >= normalDiff || abs(thisNormal - nextNormal).z >= normalDiff)
	{
		return float4(1,1,1,1);
	}
nextPixel = tex2D(normalMap,Input.PlusMinus);
nextNormal = nextPixel.xyz * 2.0f - 1.0f;
	if(abs(thisPixel.a - nextPixel.a) >= depthDiff|| abs(thisNormal - nextNormal).x >= normalDiff || abs(thisNormal - nextNormal).y >= normalDiff || abs(thisNormal - nextNormal).z >= normalDiff)
	{
		return float4(1,1,1,1);
	}
nextPixel = tex2D(normalMap,Input.MinusPlus);
nextNormal = nextPixel.xyz * 2.0f - 1.0f;
	if(abs(thisPixel.a - nextPixel.a) >= depthDiff||  abs(thisNormal - nextNormal).x >= normalDiff || abs(thisNormal - nextNormal).y >= normalDiff || abs(thisNormal - nextNormal).z >= normalDiff)
	{
		return float4(1,1,1,1);
	}
nextPixel = tex2D(normalMap,Input.MinusMinus);
nextNormal = nextPixel.xyz * 2.0f - 1.0f;
	if(abs(thisPixel.a - nextPixel.a) >= .00005f||  abs(thisNormal - nextNormal).x >= normalDiff || abs(thisNormal - nextNormal).y >= normalDiff || abs(thisNormal - nextNormal).z >= normalDiff)
	{
		return float4(1,1,1,1);
	}
return float4(0,0,0,1);
}
technique RenderScene0
{
	pass P0 
	{
		VertexShader = compile vs_4_0 RenderSceneVS();
		PixelShader = compile ps_4_0 RenderScenePS0();
	}
} 