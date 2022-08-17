#version 460 core

layout (location = 0) in vec3  positionVert;
layout (location = 1) in vec4  colorVert;
layout (location = 2) in vec2  texCoordVert;
layout (location = 3) in float texIndexVert;

uniform mat4 projection;

out vec4  colorFrag;
out vec2  texCoordsFrag;
out float texIndexFrag;

void main()
{
    gl_Position = projection * vec4(positionVert, 1.0);

    colorFrag     = colorVert;
    texCoordsFrag = texCoordVert;
    texIndexFrag  = texIndexVert;
}