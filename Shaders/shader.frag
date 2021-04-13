#version 450 core

in vec2 fragTexCoord;

out vec4 ColorOutput;

uniform sampler2D Textures[6];

void main()
{
    ColorOutput = texture(Textures[5], fragTexCoord);
}