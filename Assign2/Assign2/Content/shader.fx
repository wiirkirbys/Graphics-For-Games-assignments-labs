float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
texture2D decalMap;
texture environmentMap;
float reflectivity;
float3 etaRatio;
float fresnelPower;
float fresnelBias;
float fresnelScale;

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
    float4 WorldPosition : TEXCOORD1;
    float3 N : NORMAL0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TextureCoordinate = input.TextureCoordinate;
    output.WorldPosition = worldPosition;
    output.N = mul(input.Normal, WorldInverseTranspose).xyz;
    return output;
}

float4 ReflectPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 I = normalize(input.WorldPosition.xyz - CameraPosition);
    float3 R = reflect(I, normalize(input.N));
    float4 reflectedColor = texCUBE(SkyboxSampler, R);
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, reflectedColor, 0.5);
}
float4 RefractPixelShaderFunction(VertexShaderOutput input): COLOR0
{
    float3 I = normalize(input.WorldPosition.xyz - CameraPosition);
    float3 R = refract(I, normalize(input.N), .9f);
    float4 reflectedColor = texCUBE(SkyboxSampler, R);
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, reflectedColor, 0.5);
}
float4 DispersionPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 I = normalize(input.WorldPosition.xyz - CameraPosition);
    float3 red = refract(I, normalize(input.N), etaRatio.x);
    float3 green = refract(I, normalize(input.N), etaRatio.y);
    float3 blue = refract(I, normalize(input.N), etaRatio.z);
    float4 refractedColor;
    refractedColor.r = texCUBE(SkyboxSampler, red).r;
    refractedColor.g = texCUBE(SkyboxSampler, green).g;
    refractedColor.b = texCUBE(SkyboxSampler, blue).b;
    refractedColor.a = 1;
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, refractedColor, 0.5);
}
float4 FresnelPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 I = normalize(input.WorldPosition.xyz - CameraPosition);
    float3 R = reflect(I, normalize(input.N));
    float4 reflectedColor = texCUBE(SkyboxSampler, R);
    float3 red = refract(I, normalize(input.N), etaRatio.x);
    float3 green = refract(I, normalize(input.N), etaRatio.y);
    float3 blue = refract(I, normalize(input.N), etaRatio.z);
    float4 refractedColor;
    refractedColor.r = texCUBE(SkyboxSampler, red).r;
    refractedColor.g = texCUBE(SkyboxSampler, green).g;
    refractedColor.b = texCUBE(SkyboxSampler, blue).b;
    refractedColor.a = 1;
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    reflectedColor = lerp(decalColor, reflectedColor, 0.5);
    refractedColor = lerp(decalColor, refractedColor, 0.5);
    float reflectionFactor = fresnelBias + fresnelScale * pow(1 + dot(I, input.N), fresnelPower);
    return lerp(refractedColor, reflectedColor, reflectionFactor);
}

technique Reflection
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 ReflectPixelShaderFunction();
    }
};
technique Refraction
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 RefractPixelShaderFunction();
    }
};
technique ReflectionDispersion
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 DispersionPixelShaderFunction();
    }
};
technique Fresnel
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 FresnelPixelShaderFunction();
    }
};