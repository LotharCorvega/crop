#version 450 core

in vec2 TextureCoordinates;

out vec4 outputColor;

uniform sampler2D Textures[6];

void main()
{
    outputColor = texture(Textures[0], TextureCoordinates);
}