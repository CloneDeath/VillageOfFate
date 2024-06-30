# Village of Fate

This program is inspired by the Manga / Light Novel "This Village Sim NPC Could Only Be Human".

The goal is to make a realistic village sim, using GPT to make it as realistic as possible.

## Vision

The goal is to create a web server, which simulates several villages, in real-time.

Once per day, players can send a message to their village, giving them instructions, hope, courage, or direction.

Otherwise, players can cast miracles, observe the world, and view past conversations.

Players may also occassionally get the opportunity to view Villager's dreams, read their diary, etc.

### Miracles

You primarily gain miracle points by paying money (to fund the GPT costs). Otherwise, you gain a slow trickle based on
how many villagers you have, and how much they believe in you and worship you. You can also gain a small boost via daily offerings from your villagers.

These miracle points can be spent to cast miracles such as:

[ ] Weather Control (Rain, Clear Skies, Thunderstorm, etc)
[ ] Chance Encounters (Wandering Herbalist, Traveling Merchant, etc) 
[ ] Familiars (Raven, Stone Golem, etc)

## Authentication

To use your own Authentication:

1) Make a new Project in [Google Cloud Console](https://console.cloud.google.com/cloud-resource-manager)
2) Navigate to the "Credentials" section.
3) Configure the OAuth consent screen.
4) Click "Create Credentials" and choose "OAuth 2.0 Client IDs".
5) Choose "Web application" and set the authorized redirect URIs.
It will be something like https://yourapp.com/authentication/login-callback.
6) Update the `GoogleClientId` in `VillageOfFate.Client/wwwroot/appsettings.Development.json` with your new Client ID.
