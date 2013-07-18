struct VertexShaderInput
{
	float3 pos : POSITION0;
	float2 texCoord : TEXCOORD0;
};

struct PixelShaderInput
{
	float4 pos : SV_POSITION0;
	float fogAmount : COLOR0;
	float2 tex0 : TEXCOORD0;
};

float4x4 worldViewProj;

Texture2D tex : register(t0);
SamplerState texSampler : register(s0);
Texture2D lightmap : register(t1);
SamplerState lightmapSampler : register(s1);

PixelShaderInput VS( VertexShaderInput input )
{
	PixelShaderInput output = (PixelShaderInput)0;
	output.pos = mul(worldViewProj, float4(input.pos, 1));
	output.tex0 = input.texCoord;

	output.fogAmount = saturate(1.0f / pow(2.71828f, 0.0008f * output.pos.z));

	return output;
}

float4 PS( PixelShaderInput input ) : SV_Target
{
	float4 texColor = tex.Sample(texSampler, input.tex0);
	float4 lightmapColor = lightmap.Sample(lightmapSampler, input.tex0);
	float4 fogColor = float4(0.33f, 0.73f, 0.9f, 1.0f);
	texColor = texColor * lightmapColor;

	return lerp( fogColor, texColor, input.fogAmount);
}
