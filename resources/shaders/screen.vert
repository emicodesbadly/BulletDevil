#version 460
layout(location = 0) in vec2 a_pos;
layout(location = 1) in vec2 a_uv;

out vec2 v_uv;

void main()
{
	gl_Position = vec4(a_pos, 0.0f, 1.0f);
	v_uv = vec2(a_uv.x, a_uv.y);
}
