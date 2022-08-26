#include "world.h"

#include <math.h>
#include <iostream>


World::World()
{

}

World::~World()
{
    for (int i = 0; i < activeChunks.size(); i++)
        activeChunks[i].Unload();

    std::cout << "chunks saved";
}

void World::Load(const char* worldName)
{   
    activeChunks.push_back(Chunk(0, 0));
}

void World::Save()
{
    for (int i = 0; i < activeChunks.size(); i++)
        activeChunks[i].Unload();
}

void World::PlayerMoved(glm::vec2 position)
{
    glm::vec2 chunkPos = getChunk(screenToWorld(position));

    if ((int)chunkPos.x != playerChunkX || (int)chunkPos.y != playerChunkY) //maybe move chunk update checking to game class
    {
        playerChunkX = (int)chunkPos.x;
        playerChunkY = (int)chunkPos.y;       
    }
}

void World::Draw()
{
    activeChunks.clear();   //update later

    for (int x = -renderDistance; x <= renderDistance; x++)
    {
        for (int y = -renderDistance + std::abs(x); y <= renderDistance - std::abs(x); y++)
        {
            activeChunks.push_back(Chunk(x + playerChunkX, y + playerChunkY));
        }
    }
    
    for (int i = 0; i < activeChunks.size(); i++)
        activeChunks[i].Draw();
}

glm::vec2 World::screenToWorld(glm::vec2 v)
{
    return glm::mat2x2(1.0f / 44.0f, -1.0f / 44.0f, 1.0f / 22.0f, 1.0f / 22.0f) * v;
}

glm::vec2 World::worldToScreen(glm::vec2 v)
{
    return glm::mat2x2(22.0f, 11.0f, -22.0f, 11.0f) * v;

    //  Matrix:

    //  |  22  11 |
    //  | -22  11 |


    // det = 1 / (22 * 11 + 11 * 22) = 1 / (2 * 11 * 22)
    //

    //  | 11 -11 |
    //  | 22  22 |  * 1 / (2 * 11 * 22)
}

glm::vec2 World::getChunk(glm::vec2 position)
{
    glm::vec2 chunkPos;
    
    chunkPos.x = std::floor(position.x / Chunk::CHUNK_SIZE);
    chunkPos.y = std::floor(position.y / Chunk::CHUNK_SIZE);

    return chunkPos;
}