
texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};
struct VertexPositionTexture {
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};
VertexPositionTexture MyVertexShader2(VertexPositionTexture input) {
	return input;
}
float4 MyPixelShader2(VertexPositionTexture input) : COLOR
{
	return tex2D(mySampler, input.TextureCoordinate);
}

struct VertexPositionColor {
	float4 Position: POSITION;
	float4 Color: COLOR;
};

VertexPositionColor MyVertexShader(VertexPositionColor input) {
	return input;
}

float4 MyPixelShader(VertexPositionColor input) : COLOR
{
	float4 color = input.Color;
	if (color.r % 0.1 < 0.05f) return float4(1, 1, 1, 1);
	else return color;
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 MyVertexShader2();
		PixelShader = compile ps_4_0 MyPixelShader2();
	}
}