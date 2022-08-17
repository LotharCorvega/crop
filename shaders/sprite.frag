#version 460 core

in vec4  colorFrag;
in vec2  texCoordsFrag;
in float texIndexFrag;

uniform sampler2D image[32];

out vec4 color;

void main()
{
    int index = int(texIndexFrag);
    color = texture(image[index], texCoordsFrag) * colorFrag;
}  