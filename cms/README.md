# MAS CMS

> KeystoneJS CMS for managing content within MAS

# Running the CMS

Mongo on machine or via Docker. `cd cms` then `npm i` then `npm start`

# Configuring the CMS

Configure the CMS through environment variables.

Running the CMS as part of the Docker stack from the root of this repo will automatically set these for you.

- `MONGO_URL` - The URL for connecting to Mongo. Defaults to _mongodb://localhost:27017_
- `PORT` - The port to run under. Defaults to _3010_.
- `COOKIE_SECRET`

# Tests

There are unit tests for custom logic and validation within the CMS. These use Jest under the hood.

Run the tests (and linting) on the command line with:

```
npm test
```

Alternatively, run `npm run test:unit` to just run the unit tests without linting.

## Watch mode

Run the tests in watch mode when developing to automatically re-run changed tests:

```
npm run test:unit:watch
```

## Coverage

Collect coverage by running the following command:

```
npm run test:unit:coverage
```

This reports coverage on the command line, and as HTML within the _coverage_ folder. It also reports coverage to TeamCity automatically when run on a build agent (or when the `TEAMCITY_VERSION` environment variable is set).
