# MAS CMS

> KeystoneJS CMS for managing content within MAS

# Running the CMS

Mongo on machine or via Docker. `cd cms` then `npm i` then `npm start`

# Configuring the CMS

Configure the CMS through environment variables.

Running the CMS as part of the Docker stack from the root of this repo will automatically set these for you.

- `MONGO_URL` - The URL for connecting to Mongo. Defaults to *mongodb://localhost:27017*
- `PORT` - The port to run under. Defaults to *3010*.
- `COOKIE_SECRET` 
