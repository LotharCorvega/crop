/*******************************************************************
** This code is part of Breakout.
**
** Breakout is free software: you can redistribute it and/or modify
** it under the terms of the CC BY 4.0 license as published by
** Creative Commons, either version 4 of the License, or (at your
** option) any later version.
******************************************************************/
#include "game.h"

#include "world.h"
#include "renderer.h"
#include "resource_manager.h"

#include <iostream>
#include <glm/glm.hpp>

gameState Game::State;
bool Game::Keys[1024];

unsigned int Game::Width;
unsigned int Game::Height;

glm::vec2 camPosition = { 0,0 };

float scale = 1.0f;
float maxspeed = 50;

World world1;

void Game::Init(unsigned int width, unsigned int height)
{
	State = GAME_ACTIVE;
	
	Width = width;
	Height = height;

	Renderer::Init();

	world1.Load("saves/world1/world1.chunk");	

	ResourceManager::LoadTexture("textures/empty.png", true, "empty");
	ResourceManager::LoadTexture("textures/grass.png", true, "grass");
	ResourceManager::LoadTexture("textures/test.png", true, "test");
	ResourceManager::LoadTexture("textures/character.png", true, "character");

	ResourceManager::LoadShader("shaders/sprite.vert", "shaders/sprite.frag", nullptr, "sprite");

	// Set up Shader, move to renderer later
	{
		ResourceManager::GetShader("sprite").Use();

		auto loc = glGetUniformLocation(ResourceManager::GetShader("sprite").ID, "image");
		int samplers[32];

		for (int i = 0; i < 32; i++)
			samplers[i] = i;

		glUniform1iv(loc, 32, samplers);

		glm::mat4 projection = glm::ortho(-160.0f, 160.0f, -90.0f, 90.0f);
		ResourceManager::GetShader("sprite").SetMatrix4("projection", projection);
	}
}

void Game::Shutdown()
{
	Renderer::Shutdown();
}

void Game::ProcessInput(float dt)
{
	if (Keys[GLFW_KEY_D] || Keys[GLFW_KEY_A])
		camPosition.x += (Keys[GLFW_KEY_D] * maxspeed - Keys[GLFW_KEY_A] * maxspeed) * dt;

	if (Keys[GLFW_KEY_W] || Keys[GLFW_KEY_S])
		camPosition.y += (Keys[GLFW_KEY_W] * maxspeed - Keys[GLFW_KEY_S] * maxspeed) * dt;
}

void Game::Update(float dt)
{

}

void Game::Render()
{	
	glm::mat4 projection = glm::ortho(-160.0f * scale + camPosition.x, 160.0f * scale + camPosition.x, -90.0f * scale + camPosition.y, 90.0f * scale + camPosition.y);
	ResourceManager::GetShader("sprite").SetMatrix4("projection", projection);
	
	world1.Draw();	
	
	Renderer::BatchSprite({ 0, 0, 0 }, { 23, 43 }, ResourceManager::GetTexture("character"));

	// check Batch Overflows
	{
		int overflows = Renderer::GetOverflows();
		if (overflows)
			std::cout << overflows << " BATCH OVERFLOWS!" << std::endl;
	}

	Renderer::DrawBatch();
}

void Game::ScrollInput(int scroll) //temporary
{
	scale -= scroll / 5.0f;

	if (scale < 1)
		scale = 1;
	else if (scale > 5)
		scale = 5;
}