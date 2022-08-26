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

#include "dependencies/entt/entt.hpp"
#include <iostream>
#include <glm/glm.hpp>
#include <random>

gameState Game::State;
bool Game::Keys[1024];

unsigned int Game::Width;
unsigned int Game::Height;

const unsigned int MAX_PIXEL_SCALE = 5;
const unsigned int MIN_PIXEL_SCALE = 1;

float pixelScale = 5.0f;

glm::vec2 camPosition = { 0,0 };
float camMaxSpeed = 50;

World world1;

entt::registry myRegistry;

struct PositionComponent
{
	glm::vec2 Position;
};

struct VelocityComponent
{
	glm::vec2 Velocity;
};

struct SpriteComponent
{
	glm::vec2 size;

	Texture2D texture;
};


void Game::Init(unsigned int width, unsigned int height)
{
	State = GAME_ACTIVE;

	Width = width;
	Height = height;

	Renderer::Init();

	world1.Load("saves/world1/world1.chunk");

	ResourceManager::LoadTexture("textures/empty.png", true, "empty");
	ResourceManager::LoadTexture("textures/test.png", true, "test");

	ResourceManager::LoadTexture("textures/sand.png", true, "sand");
	ResourceManager::LoadTexture("textures/grass.png", true, "grass");
	ResourceManager::LoadTexture("textures/dirt.png", true, "dirt");
	ResourceManager::LoadTexture("textures/stone.png", true, "stone");

	ResourceManager::LoadTexture("textures/character.png", true, "character");
	ResourceManager::LoadTexture("textures/cube.png", true, "cube");
	ResourceManager::LoadTexture("textures/bouncy.png", true, "bouncy");

	ResourceManager::LoadShader("shaders/sprite.vert", "shaders/sprite.frag", nullptr, "sprite");

	std::default_random_engine generator;

	std::uniform_real_distribution<float> randPosition(-80.0f, 80.0f);
	std::uniform_real_distribution<float> randVelocity(-10.0f, 10.0f);

	for (int i = 0; i < 1; i++)
	{
		PositionComponent pos;
		pos.Position = glm::vec2(randPosition(generator), randPosition(generator));

		VelocityComponent vel;
		vel.Velocity = glm::vec2(randVelocity(generator), randVelocity(generator));

		SpriteComponent sprite;
		sprite.size = glm::vec2(42, 44);
		sprite.texture = ResourceManager::GetTexture("cube");

		entt::entity cube = myRegistry.create();

		myRegistry.emplace<PositionComponent>(cube, pos);
		myRegistry.emplace<VelocityComponent>(cube, vel);
		myRegistry.emplace<SpriteComponent>(cube, sprite);
	}

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
		camPosition.x += (Keys[GLFW_KEY_D] * camMaxSpeed - Keys[GLFW_KEY_A] * camMaxSpeed) * dt;

	if (Keys[GLFW_KEY_W] || Keys[GLFW_KEY_S])
		camPosition.y += (Keys[GLFW_KEY_W] * camMaxSpeed - Keys[GLFW_KEY_S] * camMaxSpeed) * dt;
}

void Game::Update(float dt)
{
	auto view = myRegistry.view<VelocityComponent>();
	for (auto entity : view)
	{
		PositionComponent& pos = myRegistry.get<PositionComponent>(entity);
		VelocityComponent& vel = myRegistry.get<VelocityComponent>(entity);

		pos.Position += vel.Velocity * dt;
	}

	world1.PlayerMoved(camPosition);
}

void Game::Render()
{
	float scale = MAX_PIXEL_SCALE / pixelScale;
	glm::mat4 projection = glm::ortho(-160.0f * scale + camPosition.x, 160.0f * scale + camPosition.x, -90.0f * scale + camPosition.y, 90.0f * scale + camPosition.y);
	ResourceManager::GetShader("sprite").SetMatrix4("projection", projection);

	world1.Draw();

	auto view = myRegistry.view<PositionComponent, SpriteComponent>();
	// use a callback
	view.each([](auto& pos, auto& sprite)
		{
			Renderer::BatchSprite({ pos.Position, 0 }, sprite.size, sprite.texture);
		});

	Renderer::BatchSprite({ camPosition.x - 11, camPosition.y, 0 }, { 23, 43 }, ResourceManager::GetTexture("character"));
	Renderer::BatchSquare({ camPosition, 0 }, { 1, 1 }, { 1,0,0,1 });

	Renderer::BatchSprite({ 7.5f, 7.5f, 0 }, ResourceManager::GetTexture("bouncy"), { 0,0 }, { 14, 18 }, { 7, 18 }, { 1.0f, 1.0f, 1.0f, 1.0f }, true, 8, 0.05f);

	// check Batch Overflows
	{
		int overflows = Renderer::GetOverflows();
		//if (overflows)
			//std::cout << overflows << " BATCH OVERFLOWS!" << std::endl;
	}

	Renderer::DrawBatch();
}

void Game::ScrollInput(int scroll) //temporary
{
	bool smoothZoom = false;

	if (smoothZoom)
	{
		pixelScale += scroll / 5.0f;

		if (pixelScale < MIN_PIXEL_SCALE)
			pixelScale = MIN_PIXEL_SCALE;
		else if (pixelScale > MAX_PIXEL_SCALE)
			pixelScale = MAX_PIXEL_SCALE;
	}
	else {
		pixelScale += scroll;

		if (pixelScale < MIN_PIXEL_SCALE)
			pixelScale = MIN_PIXEL_SCALE;
		else if (pixelScale > MAX_PIXEL_SCALE)
			pixelScale = MAX_PIXEL_SCALE;

		pixelScale = std::roundf(pixelScale);
	}
}