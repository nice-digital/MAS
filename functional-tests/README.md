# Front end testing base

> Start point for browser based testing

- [Front end testing base](#front-end-testing-base)
  - [What is it?](#what-is-it)
    - [Features](#features)
  - [Required software](#required-software)
    - [Optional software](#optional-software)
      - [Firefox](#firefox)
      - [Cucumber (Gherkin) Full Support](#cucumber-gherkin-full-support)
  - [Getting started](#getting-started)
    - [Excluding tests](#excluding-tests)
    - [Running single features](#running-single-features)
  - [Docker](#docker)

## What is it?

A starter boilerplate project for writing browser-based functional tests.

We've used a package called [WebdriverIO](http://webdriver.io/) which is a helper library for selenium written in Node.

### Features

- Single command `npm test` to run tests, using [wdio-selenium-standalone-service](https://github.com/webdriverio/wdio-selenium-standalone-service)
- [Cucumber framework](https://cucumber.io/) for writing BDD [features](features)
- Uses [@nice-digital/wdio-cucumber-steps](https://github.com/nice-digital/wdio-cucumber-steps) shared step definitions
- [TeamCity wdio config](wdio.teamcity.conf.js) with [TeamCity reporter](https://github.com/sullenor/wdio-teamcity-reporter)
- [docker-compose script](run.sh) for running tests against headless Chrome and Firefox in Docker
- [Babel](https://babeljs.io/) for writing custom step definitions in [ES6](https://github.com/lukehoban/es6features#readme) and [Flow](https://flow.org)
- [Cucumber (Gherkin) Full Support VSCode](https://marketplace.visualstudio.com/items?itemName=alexkrechik.cucumberautocomplete#overview) extension support

## Required software

- Node >= 6.3
- Chrome

#### Cucumber (Gherkin) Full Support

The [Cucumber (Gherkin) Full Support VSCode extension](https://marketplace.visualstudio.com/items?itemName=alexkrechik.cucumberautocomplete#overview) is great for intellisense for step definitions.

Install the extension and configure by adding the following to *.vscode/settings.json*:

```diff
{
+    "cucumberautocomplete.steps": [
+        "node_modules/@nice-digital/wdio-cucumber-steps/src/given/definitions.js",
+        "node_modules/@nice-digital/wdio-cucumber-steps/src/when/definitions.js",
+        "node_modules/@nice-digital/wdio-cucumber-steps/src/then/definitions.js",
+    ]
}
```

## Getting started

- ```git clone https://github.com/nice-digital/frontend-testing-base.git```
- ```cd frontend-testing-base```
- ```npm i```

If you have issues with regard to:

```gyp ERR! configure error
gyp ERR! stack Error: Can't find Python executable "python", you can set the PYTHON env variable.
```

Please follow these instructions: Windows + OS X instructions here: https://github.com/nodejs/node-gyp

> Also note it seems you need to be on node 10 in order to get past the fibers@2.0.2 install script error.

After the install has finished run the tests by running the following command. This starts a selenium server and opens Chrome to run the tests:

```sh
npm test
```

> Note: On Windows run in *cmd* and not *GitBash* otherwise the window just hangs.

### Excluding tests

Exclude tests by using the `@pending` [cucumber tag](https://github.com/cucumber/cucumber/wiki/Tags).

### Running single features

To run a single feature file, use the following command:

```sh
npm test -- --spec ./features/homepage.feature
```

Note: you can pass in multiple files, separated by a comma.

Or you can use a keyword to filter e.g.:

```sh
npm test -- --spec homepage
```

Finally, if you've grouped your specs into suites you can run and individual suite with:

```sh
npm test -- --suite homepage
```

See [organizing test suites](http://webdriver.io/guide/testrunner/organizesuite.html) in the WebdriverIO docs for more info.

if you want to run tests against all browsers locally you can do so via this command:

```sh
npm run test:allbrowsers
```

The baseUrl in wdio.conf.js determines which instance of guidance web that tests will run against, you can change this to suit your needs.

## Docker

Running tests on Docker is a good option as it means you don't need browsers installed on the host machine, and the Selenium grid is automatically created for you. This is useful on a TeamCity build agent where you can't rely on Chrome and Firefox being installed.

In bash:

```sh
./run.sh
```

Or in CMD on Windows:

```sh
run
```

Or in PowerShell:

```sh
cmd /c "run"
```
