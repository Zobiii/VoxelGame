#version 330 core
in vec3 vNormal;

out vec4 FragColor;

uniform vec3 uColor;     // cube color (white)
uniform vec3 uLightDir;  // directional light direction (world space)

void main()
{
    vec3 N = normalize(vNormal);
    vec3 L = normalize(-uLightDir);

    float ambient = 0.35;
    float diff = max(dot(N, L), 0.0);
    float lighting = ambient + diff * 0.65;

    FragColor = vec4(uColor * lighting, 1.0);
}