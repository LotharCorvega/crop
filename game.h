/*******************************************************************
** This code is part of Breakout.
**
** Breakout is free software: you can redistribute it and/or modify
** it under the terms of the CC BY 4.0 license as published by
** Creative Commons, either version 4 of the License, or (at your
** option) any later version.
******************************************************************/
#pragma once

#include <glad/glad.h>
#include <GLFW/glfw3.h>

enum gameState 
{
    GAME_ACTIVE,
    GAME_MENU,
    GAME_WIN
};

class Game
{
public:
    static gameState State;
    static bool Keys[1024];

    static unsigned int Width;
    static unsigned int Height;

    static void Init(unsigned int width, unsigned int height);
    static void Shutdown();

    static void ProcessInput(float dt);
    static void Update(float dt);
    static void Render();

    static void ScrollInput(int scroll);
};