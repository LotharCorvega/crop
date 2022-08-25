#include "chunk.h"

#include "resource_manager.h"
#include "renderer.h"

#include <fstream>
#include <string>
#include <iostream>

#include <GLFW/glfw3.h>

Chunk::Chunk(int X, int Y) 
{
	chunkX = X;
	chunkY = Y;

    Load();
}

Chunk::~Chunk()
{
    //idk
}


void Chunk::Load()
{
    std::string path;
    path = "saves/world1/" + std::to_string(chunkX) + "_" + std::to_string(chunkY) + ".chunk";

    std::string line;
    std::ifstream file(path, std::ios::binary);

    if (file)
    {
        file.read(tileData, CHUNK_SIZE * CHUNK_SIZE);
    } else 
    {
        //std::cout << "COULDNT LOAD " << path << "!" << std::endl;

        Generate();
    }

    file.close();

}

void Chunk::Unload()
{
    std::string path;
    path = "saves/world1/" + std::to_string(chunkX) + "_" + std::to_string(chunkY) + ".chunk";

    /*std::ofstream file(path, std::ios::binary);

    if (file) 
    {
        file.write(tileData, CHUNK_SIZE * CHUNK_SIZE);
    } else 
    {
        std::cout << "COULD NOT SAVE" << path << std::endl;
    }    

    file.close();*/
}

#include "perlin.h"
float scale = 0.1f;

void Chunk::Generate()
{
    for (int x = 0; x < CHUNK_SIZE; x++)
    {
        for (int y = 0; y < CHUNK_SIZE; y++)
        {            
            float value = perlin((x + chunkX * CHUNK_SIZE) * scale, (y + chunkY * CHUNK_SIZE) * scale);

            if (value < -0.3)
            {
                tileData[x + y * CHUNK_SIZE] = 'r';
            }
            else if (value < 0.0f)
            {
                tileData[x + y * CHUNK_SIZE] = 'd';
            }
            else if (value < 0.3f)
            {
                tileData[x + y * CHUNK_SIZE] = 'g';
            }
            else
            {
                tileData[x + y * CHUNK_SIZE] = 's';
            }
        }
    }
}

void Chunk::Draw()
{
    for (int x = 0; x < CHUNK_SIZE; x++)
    {
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            glm::vec2 pos = { x + CHUNK_SIZE * chunkX, y + CHUNK_SIZE * chunkY };

            switch (tileData[x + y * CHUNK_SIZE])
            {
            case 't':
                Renderer::BatchTile(pos, ResourceManager::GetTexture("test"));
                break;
            case 'g':
                Renderer::BatchTile(pos, ResourceManager::GetTexture("grass"));
                break;
            case 'd':
                Renderer::BatchTile(pos, ResourceManager::GetTexture("dirt"));
                break;
            case 's':
                Renderer::BatchTile(pos, ResourceManager::GetTexture("sand"));
                break;
            case 'r':
                Renderer::BatchTile(pos, ResourceManager::GetTexture("stone"));
                break;
            default:
                Renderer::BatchTile(pos, ResourceManager::GetTexture("empty"));
                break;
            }
        }
    }
}