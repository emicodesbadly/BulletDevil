#version 460
out vec4 FragColor;

in vec2 v_uv;

uniform sampler2D texture0;

void main()
{
	FragColor = texture(texture0, v_uv);
    //FragColor = vec4(v_uv, 0.0f, 1.0f);
    //FragColor = vec4(0.0f, 1.0f, 0.0f, 0.0f);
}
