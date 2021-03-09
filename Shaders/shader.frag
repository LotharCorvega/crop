#version 330

out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D texturee;

void main()
{
    outputColor = texture(texturee, texCoord);
}