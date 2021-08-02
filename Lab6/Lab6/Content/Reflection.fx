float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
texture2D decalMap;
texture environmentMap;
float reflectivity;

sampler tsampler1 = sampler_state {
    texture = <decalMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
samplerCUBE SkyboxSampler = sampler_state
{
    texture = <environmentMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Mirror;
    AddressV = Mirror;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD;
    float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float3 R : TEXCOORD1;
};

VertexShaderOutput ReflectionVertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TextureCoordinate = input.TextureCoordinate;
    float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
    float3 I = normalize(worldPosition.xyz - CameraPosition);
    output.R = reflect(I, normalize(N));
    return output;
}

float4 ReflectPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 reflectedColor = texCUBE(SkyboxSampler, input.R);
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, reflectedColor, 0.5);
}

technique Reflection
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 ReflectionVertexShaderFunction();
        PixelShader = compile ps_4_0 ReflectPixelShaderFunction();
    }
};