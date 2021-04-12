#version 450 core

layout(location = 0) in vec2 vertPosition;
layout(location = 1) in vec2 vertSize;
layout(location = 2) in vec2 vertTexCoord;
layout(location = 3) in vec2 vertTexSize;

out vec2 geomSize;
out vec2 geomTexCoord;
out vec2 geomTexSize;

void main(void)
{
    gl_Position = vec4(vertPosition, 0.0, 1.0);
    geomSize = vertSize;
    geomTexCoord = vertTexCoord;
    geomTexSize = vertTexSize;
}