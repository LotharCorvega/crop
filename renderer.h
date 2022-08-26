#pragma once

// from The Cherno: Writing a BATCH RENDERER in ONE HOUR! https://www.youtube.com/watch?v=KyCQBQzaBOM (August 2022)

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

#include "texture.h"
#include "shader.h"
#include "world.h"

class Renderer
{
public:
	static void Init();
	static void Shutdown();

	static void DrawBatch();
	static int GetOverflows();

	static void BatchSprite(const glm::vec3& worldPosition, const Texture2D& texture, const glm::vec2& spriteOffset, const glm::vec2& spriteSize, const glm::vec2& spriteAnchor, const glm::vec4& tint, const bool& animated, const unsigned int& frameCount, const float& frametime);

	static void BatchSprite(const glm::vec3& position, const glm::vec2& size, const glm::vec4& color, const Texture2D& texture);
	static void BatchSquare(const glm::vec3& position, const glm::vec2& size, const glm::vec4& color);

	static void BatchSprite(const glm::vec3& position, const glm::vec2& size, const Texture2D& texture);
	static void BatchTile(const glm::vec2& position, const Texture2D& texture);
};