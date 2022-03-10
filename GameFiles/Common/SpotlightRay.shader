shader_type spatial;

render_mode  unshaded, cull_back, blend_add;

uniform vec4 albedo : hint_color;
uniform sampler2D gridtex;
//uniform vec2 offset;

varying float intensity;

void vertex(){
	intensity = VERTEX.z/50f * 5f;
	if(intensity<0.65f) intensity = 0.65f;
	if(intensity>1f) intensity = 1f;
	intensity = 1f - intensity;
}

void fragment(){
	vec3 GRIDTEX = texture(
		gridtex, 
		UV*vec2(9f,0.5f) + 
		vec2(TIME*0.25f, TIME*0.1f) 
	).rgb * (sin(TIME*5f)*0.5f + 1f);
	vec3 TEX = albedo.rgb + GRIDTEX;
	TEX *= intensity;
	//ALPHA = intensity * intensity * 4f;
	//vec3 SCREENTEX =  texture(SCREEN_TEXTURE, SCREEN_UV).rgb ;
	ALBEDO.rgb = TEX ;
	//ALPHA = 0.5f;
}

