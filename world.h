#pragma once

#include <string>
#include <fstream>
#include <glad/glad.h>
#include <glm/glm.hpp>

#include "texture.h"
#include "renderer.h"
#include "resource_manager.h"

class World
{
public:
    static const int TILE_WIDTH = 42;
    static const int TILE_HEIGHT = 22;

    static const int CHUNK_SIZE = 8;

    World();
    ~World();

    void Load(const char* worldName);
    void Save();

    void LoadChunk();
    void SaveChunk();
    void GenerateChunk();

    void Draw();

    static glm::vec2 screenToWorld(glm::vec2 v);
    static glm::vec2 worldToScreen(glm::vec2 v);

private:
    std::string data;
};