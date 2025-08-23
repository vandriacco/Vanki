## Descriptions
This is a personal project to practice my asp.net programming. It is a clone of the functionality of the popular spaced repetiton software Anki.
With this software, one creates an account and can make and edit flashcard decks. This implements the supermemo-2 srs algorithm as described here: https://super-memory.com/english/ol/sm2.htm

## Methods
The user verification system uses json web tokens to register and login and ensure a user can only access their own flashcard decks. I used these templates as a reference for setting up my jwt verification and handling:
  - https://github.com/cornflourblue/blazor-webassembly-jwt-authentication-example/blob/master/Services/HttpService.cs
  - https://code-maze.com/add-bearertoken-httpclient-request/
And I used these articles for the generation:
  - https://dotnetfullstackdev.medium.com/jwt-token-authentication-in-c-a-beginners-guide-with-code-snippets-7545f4c7c597

The front end is done in Blazor WASM. I chose this as a compromise between a quick turnaround since I already know how to program in c# and the ability to call my api without directly querying the database like in BLazor Server. This way, if I ever want to redo the front end in the future using a javascript framework I can do so easily.

## Demo
https://github.com/user-attachments/assets/ec6e9d0d-0334-497d-b25a-5a9f8e1eb08b
