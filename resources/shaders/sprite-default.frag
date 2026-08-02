#version 460
out vec4 FragColor;

in vec2 v_uv;

uniform sampler2D texture0;

void main()
{
	FragColor = texture(texture0, v_uv);
}
