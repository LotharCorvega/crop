#version 450 core

out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D Textures[6];

void main()
{
    outputColor = texture(Textures[0], texCoord);
}