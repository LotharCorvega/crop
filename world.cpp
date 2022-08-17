#include "world.h"

char chunk[World::CHUNK_SIZE][World::CHUNK_SIZE];

World::World()
{

}

World::~World()
{

}

void World::Load(const char* worldName)
{   
    std::string line;
    std::ifstream fstream(worldName);    

    if (fstream)
    {
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            std::getline(fstream, line);

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                chunk[x][y] = line[x];
            }
        }
    }
}

void World::Draw()
{
    for (int x = 0; x < CHUNK_SIZE; x++)
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            switch (chunk[x][y])
            {
                case 'g':
                    Renderer::BatchTile({ x, y }, ResourceManager::GetTexture("grass"));
                    break;
                case 't':
                    Renderer::BatchTile({ x, y }, ResourceManager::GetTexture("test"));
                    break;
                case 'e':
                    Renderer::BatchTile({ x, y }, ResourceManager::GetTexture("empty"));
                    break;
                default:
                    break;
            }
        }
}

glm::vec2 World::screenToWorld(glm::vec2 v)
{
    return glm::mat2x2(1.0f, 0.5f, -1.0f, 0.5f) * v;    //Wrong
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