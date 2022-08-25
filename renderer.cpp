#include "renderer.h"

static const unsigned int maxQuadCount = 1024;
static const unsigned int maxVertexCount = maxQuadCount * 4;
static const unsigned int maxIndexCount = maxQuadCount * 6;
static const unsigned int maxTextureSlots = 32;

static unsigned int VAO = 0;
static unsigned int VBO = 0;
static unsigned int EBO = 0;

static unsigned int whiteTexture = 0;

struct Vertex
{
	glm::vec3 position;
	glm::vec4 color;
	glm::vec2 texCoord;
	float texIndex;
};

static Vertex* vertexBuffer;
static unsigned int quadCount;

static unsigned int* textureSlots;
static unsigned int  textureCount;

void Renderer::Init()
{
	vertexBuffer = new Vertex[maxVertexCount];
	quadCount = 0;

	textureSlots = new unsigned int[maxTextureSlots];
	textureCount = 1;

	glCreateVertexArrays(1, &VAO);
	glBindVertexArray(VAO);

	glGenBuffers(1, &VBO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, maxVertexCount * sizeof(Vertex), vertexBuffer, GL_DYNAMIC_DRAW);

	glEnableVertexArrayAttrib(VAO, 0);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (const void*)offsetof(Vertex, position));

	glEnableVertexArrayAttrib(VAO, 1);
	glVertexAttribPointer(1, 4, GL_FLOAT, GL_FALSE, sizeof(Vertex), (const void*)offsetof(Vertex, color));

	glEnableVertexArrayAttrib(VAO, 2);
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (const void*)offsetof(Vertex, texCoord));

	glEnableVertexArrayAttrib(VAO, 3);
	glVertexAttribPointer(3, 1, GL_FLOAT, GL_FALSE, sizeof(Vertex), (const void*)offsetof(Vertex, texIndex));

	unsigned int indices[maxIndexCount];
	unsigned int offset = 0;

	for (int i = 0; i < maxIndexCount; i += 6)
	{
		indices[i + 0] = 0 + offset;
		indices[i + 1] = 1 + offset;
		indices[i + 2] = 2 + offset;

		indices[i + 3] = 2 + offset;
		indices[i + 4] = 3 + offset;
		indices[i + 5] = 0 + offset;

		offset += 4;
	}

	glCreateBuffers(1, &EBO);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

	glCreateTextures(GL_TEXTURE_2D, 1, &whiteTexture);
	glBindTexture(GL_TEXTURE_2D, whiteTexture);

	uint32_t color = 0xffffffff;
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, 1, 1, 0, GL_RGBA, GL_UNSIGNED_BYTE, &color);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_CLAMP_TO_EDGE);

	textureSlots[0] = whiteTexture;
}

void Renderer::Shutdown()
{
	glDeleteVertexArrays(1, &VAO);
	glDeleteBuffers(1, &VBO);
	glDeleteBuffers(1, &EBO);

	delete[] vertexBuffer;
	delete[] textureSlots;
}

void Renderer::DrawBatch()
{
	for (unsigned int i = 0; i < textureCount; i++)
		glBindTextureUnit(i, textureSlots[i]);

	int vertices = 4 * quadCount;
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferSubData(GL_ARRAY_BUFFER, 0, vertices * sizeof(Vertex), vertexBuffer);

	int indices = 6 * quadCount;
	glBindVertexArray(VAO);
	glDrawElements(GL_TRIANGLES, indices, GL_UNSIGNED_INT, nullptr);

	quadCount = 0;
	textureCount = 1;
}

int overflows = 0;

void Renderer::BatchSprite(const glm::vec3& position, const glm::vec2& size, const glm::vec4& color, const Texture2D& texture) // position.z useless sice depth buffer does not work
{
	if (quadCount >= maxQuadCount || textureCount >= maxTextureSlots)
	{
		DrawBatch();
		overflows++;
	}


	float textureIndex = 0.0f;

	for (unsigned int i = 1; i < textureCount; i++)
		if (textureSlots[i] == texture.ID)
		{
			textureIndex = (float)i;
			break;
		}

	if (textureIndex == 0.0f)
	{
		textureIndex = (float)textureCount;
		textureSlots[textureCount] = texture.ID;
		textureCount++;
	}

	vertexBuffer[4 * quadCount + 0].position = { position.x , position.y , position.z };
	vertexBuffer[4 * quadCount + 0].color = color;
	vertexBuffer[4 * quadCount + 0].texCoord = { 0 , 1 };
	vertexBuffer[4 * quadCount + 0].texIndex = textureIndex;

	vertexBuffer[4 * quadCount + 1].position = { position.x + size.x, position.y , position.z };
	vertexBuffer[4 * quadCount + 1].color = color;
	vertexBuffer[4 * quadCount + 1].texCoord = { 1, 1 };
	vertexBuffer[4 * quadCount + 1].texIndex = textureIndex;

	vertexBuffer[4 * quadCount + 2].position = { position.x + size.x, position.y + size.y , position.z };
	vertexBuffer[4 * quadCount + 2].color = color;
	vertexBuffer[4 * quadCount + 2].texCoord = { 1, 0 };
	vertexBuffer[4 * quadCount + 2].texIndex = textureIndex;

	vertexBuffer[4 * quadCount + 3].position = { position.x , position.y + size.y , position.z };
	vertexBuffer[4 * quadCount + 3].color = color;
	vertexBuffer[4 * quadCount + 3].texCoord = { 0 , 0 };
	vertexBuffer[4 * quadCount + 3].texIndex = textureIndex;

	quadCount++;
}

void Renderer::BatchSquare(const glm::vec3& position, const glm::vec2& size, const glm::vec4& color)
{
	if (quadCount >= maxQuadCount || textureCount >= maxTextureSlots)
	{
		DrawBatch();
		overflows++;
	}

	float textureIndex = 0.0f;

	vertexBuffer[4 * quadCount + 0].position = { position.x , position.y , position.z };
	vertexBuffer[4 * quadCount + 0].color = color;
	vertexBuffer[4 * quadCount + 0].texCoord = { 0 , 0 };
	vertexBuffer[4 * quadCount + 0].texIndex = textureIndex;

	vertexBuffer[4 * quadCount + 1].position = { position.x + size.x, position.y , position.z };
	vertexBuffer[4 * quadCount + 1].color = color;
	vertexBuffer[4 * quadCount + 1].texCoord = { 1, 0 };
	vertexBuffer[4 * quadCount + 1].texIndex = textureIndex;

	vertexBuffer[4 * quadCount + 2].position = { position.x + size.x, position.y + size.y , position.z };
	vertexBuffer[4 * quadCount + 2].color = color;
	vertexBuffer[4 * quadCount + 2].texCoord = { 1, 1 };
	vertexBuffer[4 * quadCount + 2].texIndex = textureIndex;

	vertexBuffer[4 * quadCount + 3].position = { position.x , position.y + size.y , position.z };
	vertexBuffer[4 * quadCount + 3].color = color;
	vertexBuffer[4 * quadCount + 3].texCoord = { 0 , 1 };
	vertexBuffer[4 * quadCount + 3].texIndex = textureIndex;

	quadCount++;
}

void Renderer::BatchSprite(const glm::vec3& position, const glm::vec2& size, const Texture2D& texture)
{
	BatchSprite(position, size, { 1.0f, 1.0f, 1.0f, 1.0f }, texture);
}

void Renderer::BatchTile(const glm::vec2& position, const Texture2D& texture)
{
	glm::vec2 pos = World::worldToScreen(position);

	BatchSprite({ pos.x - World::TILE_WIDTH / 2, pos.y, 0 }, { World::TILE_WIDTH, World::TILE_HEIGHT }, texture);
}

int Renderer::GetOverflows()
{
	int FrameOverflows = overflows;
	overflows = 0;
	
	return FrameOverflows;

}