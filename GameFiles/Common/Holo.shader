shader_type spatial;

render_mode blend_add, unshaded;

uniform vec4 albedo : hint_color;

/*
void fragment(){
	ALBEDO.rgb = albedo.rgb;
	if( (int(FRAGCOORD.y+TIME*200f) % 5 != 0) || 
		(int(FRAGCOORD.x+TIME*10f) % 5 == 0) ) discard;
}*/

void fragment(){
	
	vec3 albs = albedo.rgb*albedo.a;
	//if( (int( (UV.x+TIME*0.05f) * 80f) % 5 == 0) &&
	//	(int( (UV.y+TIME*0.05f) * 80f) % 5 == 0)  ) albs*=0.5f;	
	if( (int(FRAGCOORD.y + TIME*10f) % 5 != 0) ||
		(int(FRAGCOORD.x + TIME*10f) % 4 == 0) ) albs *= 0.5f;
	
	ALBEDO.rgb = albs;
}