shader_type spatial;

render_mode world_vertex_coords, cull_back, blend_mix, unshaded;

uniform sampler2D tex;
uniform vec4 albedo : hint_color;
uniform vec2 offset;

void fragment(){
	
	ALBEDO.rgb = (texture(tex, UV + offset).rgb * (1f-albedo.a)) + (albedo.rgb * albedo.a) ;
}

