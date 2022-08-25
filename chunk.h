#pragma once

class Chunk
{
private:
	int chunkX;
	int chunkY;

public:
	static const int CHUNK_SIZE = 8;

	char tileData[CHUNK_SIZE * CHUNK_SIZE];

	Chunk(int x, int y);
	~Chunk();

	void Load();
	void Unload();

	void Generate();

	void Draw();
};

