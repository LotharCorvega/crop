#pragma once

#include <string>
#include <fstream>
#include <vector>
#include <glad/glad.h>
#include <glm/glm.hpp>

#include "texture.h"
#include "chunk.h"
#include "renderer.h"
#include "resource_manager.h"

class World
{
public:
    static const int TILE_WIDTH = 42;
    static const int TILE_HEIGHT = 22;

    int playerChunkX;
    int playerChunkY;

    static const int renderDistance = 2;
    std::vector<Chunk> activeChunks;

    World();
    ~World();

    void Load(const char* worldName);
    void Save();

    void PlayerMoved(glm::vec2 position);
    void Draw();

    static glm::vec2 screenToWorld(glm::vec2 v);
    static glm::vec2 worldToScreen(glm::vec2 v);

    static glm::vec2 getChunk(glm::vec2 v);

private:
};