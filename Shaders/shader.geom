#version 450 core

layout(points) in;
layout(triangle_strip, max_vertices = 4) out;

in vec2 geomSize[];
in vec2 geomTexCoord[];
in vec2 geomTexSize[];

out vec2 fragTexCoord;
out float fragTexIndex;

uniform mat4 ViewMatrix;

void main()
{
    gl_Position = gl_in[0].gl_Position + vec4(0.0, 0.0, 0.0, 0.0);
    gl_Position *= ViewMatrix;
    fragTexCoord = vec2(geomTexCoord[0].x, geomTexCoord[0].y);
    EmitVertex();

    gl_Position = gl_in[0].gl_Position + vec4(0.0, geomSize[0].y, 0.0, 0.0);
    gl_Position *= ViewMatrix;
    fragTexCoord = vec2(geomTexCoord[0].x, geomTexCoord[0].y - geomTexSize[0].y);
    EmitVertex();

    gl_Position = gl_in[0].gl_Position + vec4(geomSize[0].x, 0.0, 0.0, 0.0);
    gl_Position *= ViewMatrix;
    fragTexCoord = vec2(geomTexCoord[0].x + geomTexSize[0].x, geomTexCoord[0].y);
    EmitVertex();

    gl_Position = gl_in[0].gl_Position + vec4(geomSize[0].x, geomSize[0].y, 0.0, 0.0);
    gl_Position *= ViewMatrix;
    fragTexCoord = vec2(geomTexCoord[0].x + geomTexSize[0].x, geomTexCoord[0].y - geomTexSize[0].y);
    EmitVertex();

    EndPrimitive();
}