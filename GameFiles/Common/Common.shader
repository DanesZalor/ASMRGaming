shader_type spatial;

render_mode world_vertex_coords, cull_back, blend_mix, unshaded;

uniform sampler2D tex;
uniform vec2 offset;

void fragment(){
	ALBEDO.rgb = texture(tex, UV + offset).rgb;
}

