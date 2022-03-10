shader_type spatial;

render_mode world_vertex_coords, cull_back, blend_mix, unshaded;

uniform sampler2D tex;
uniform vec4 albedo : hint_color;
uniform vec2 offset;
uniform vec2 uvmul = vec2(1);

void fragment(){
	vec3 col = (texture(tex, UV * uvmul + offset).rgb * (1f-albedo.a)) + (albedo.rgb * albedo.a) ;
	ALBEDO.rgb = col;
	//EMISSION.rgb = col * 0.4f;
}

