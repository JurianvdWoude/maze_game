# Maze Game

This game is part of the assessment for DTT. It is a development assessment made in the [Unity Game Engine](https://unity.com/).
The hours spent on the project are provided in the Excel document attached to the main folder of the project.

The goal is to make a maze generating game. Inspiration can be found on the [Wikipedia page on maze generation algorithms](https://en.wikipedia.org/wiki/Maze_generation_algorithm).

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)

## Installation

The project can be used directly by cloning it with git.
Afterwards you can download Unity Hub, and click on 'Open' to select the folder that has the project.

## Usage

The project contains two scenes. The Start scene contains the start menu, and it is the first scene that you'll see when playing the game.
The Maze scene is where the maze is generated. The game can be built and played, or run directly in Unity from either scene.

Certain gameobjects serve a special purpose:
- <b>GameManager</b>: Contains events, special flags, and logic for restarting or stopping the game
- <b>Maze</b>: Has algorithms for generating the maze, has static batching logic, removes walls, sets pillars, etc.
- <b>Canvas</b>: Contains UI elements and a UI script for the Input fields, OnClick events, hiding UI elements
- <b>Player</b>: Controls the first-person view when pressing 'Play', and has code for moving and looking around.
- <b>Static Batch</b>: Is a game object to which instantiated maze objects can be attached for static batching.

