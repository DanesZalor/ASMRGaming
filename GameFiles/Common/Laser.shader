shader_type spatial;

render_mode cull_back, unshaded;
uniform vec4 albedo : hint_color;
uniform float SCREENTEX_INTENSITY: hint_range(0f,1f);
uniform float brightness = 1f;

uniform sampler2D tex;


void fragment(){

        vec3 TEX = texture(tex,UV).rgb * (1f-albedo.a) + albedo.rgb*albedo.a;
        vec4 SCREENTEX = texture(SCREEN_TEXTURE, SCREEN_UV);

        vec4 col = vec4(1f);
        col.rgb = SCREENTEX.rgb * (SCREENTEX_INTENSITY) + TEX.rgb * (1f-SCREENTEX_INTENSITY);
        col.rgb = mix(vec3(0.0), col.rgb, brightness);
        ALBEDO = col.rgb;

}
