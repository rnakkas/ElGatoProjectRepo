shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0); // Wipe effect progress
uniform vec4 wipe_color; // Color of the wipe effect

void fragment() {
    vec2 uv = UV;
    float wipe = smoothstep(progress - 0.02, progress + 0.02, uv.x);
    float sceneAlpha = texture(TEXTURE, UV).a; // Get alpha from texture colour

    // Blend wipe color with scene
    vec4 finalColor = mix(sceneAlpha * wipe_color, wipe_color, wipe);
    finalColor.a *= wipe; // Apply wipe effect

    COLOR = finalColor;
}
