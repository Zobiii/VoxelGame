#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProj;

out vec3 vNormal;

void main()
{
    vec4 worldPos = uModel * vec4(aPos, 1.0);
    vNormal = mat3(uModel) * aNormal;
    gl_Position = uProj * uView * worldPos;
}
