# FinalYearProject
Procedural Generation of a first person adventure in Unity3d.

Code for my final year project, titled "Procedural Content Generation in Game Design & Development"

Unity 2018.2 based first person action-rpg.

Some of the features:
* Procedural generation of landmasses
  * Threaded _chunk-based rendering_ around the player to load/unload the world as they move.
  * Biomes using _K-Nearest-Neighbour_ sampling to determine biome types from temperature/moiusture/heightmaps
  * Chainable builder pattern for noise generation to allow combiniations of noise layers and types to produce different kinds of features
  * _Poisson disc-sampling_ implementation for distribution of trees, and vegetation.
  * Higher-order Terrain features (Shapes the terrain into a single large island while retaining landmase)

* Procedural music generation and dynamic-situation music
  * **Hand-rolled sampler and sequencer implementation**
  * _Markov-chain_ based song structures (Chorus-> bridge-> chorus2 etc)
  * Melody and Chord progression generation based on musical theory engine.
  * Dynamic audioscape adjustment: Calm music in overworld seamlessly transitions into combat music and back without discord.

* Procedural Dungeon generation
  * _Generative-Grammar_ based dungeon generation, handmade prefab engine to define dungeon rooms with grammar rules and have them resolve into unqiue but coherent layouts at runtime, ensuring all areas are reachable.
* Procedrual Weapon/Item generation

* _Behavior Tree_ based AI and a custom _pathfinding graph_ implementation

TODOs:
Some areas are in need of refactor and rewriting, as they were added in time for deadlines.
Code needs to be updated to latest unity version
More tests need to be written

