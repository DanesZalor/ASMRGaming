shader_type spatial;

render_mode blend_add, unshaded;

uniform vec4 albedo : hint_color;

varying float intensity;

void vertex(){
	intensity = VERTEX.z/60f * 5f;
	if(intensity<0.65f) intensity = 0.65f;
	if(intensity>1f) intensity = 1f;
	intensity = 1f - intensity;
}

void fragment(){
	
	vec3 albs = albedo.rgb*albedo.a;
	//if( (int( (UV.x+TIME*0.05f) * 80f) % 5 == 0) &&
	//	(int( (UV.y+TIME*0.05f) * 80f) % 5 == 0)  ) albs*=0.5f;	
	if( (int(FRAGCOORD.y + TIME*10f) % 5 != 0) ||
		(int(FRAGCOORD.x + TIME*10f) % 5 == 0) ) albs *= 1.5f;
	
	ALBEDO.rgb = albs * intensity;
}