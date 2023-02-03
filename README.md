# MAS (Medicines awareness service)

- [MAS (Medicines awareness service)](#mas-medicines-awareness-service)
  - [What is it?](#what-is-it)
    - [Architecture](#architecture)
      - [Diagram](#diagram)
  - [Requirements](#requirements)
    - [IDE](#ide)
  - [Local development](#local-development)
    - [Lambdas](#lambdas)
      - [Faking the lamda schedule locally](#faking-the-lamda-schedule-locally)
        - [SAM requirements](#sam-requirements)
        - [Disable TeamCity Nuget](#disable-teamcity-nuget)
        - [Build SAM project](#build-sam-project)
        - [Invoke the lambda](#invoke-the-lambda)
    - [Rest of the app](#rest-of-the-app)

## What is it?

Medicines awareness service is:

> A daily or weekly email service that provides links to current awareness and evidence-based information relating to medicines and prescribing. Provides a quick overview of the latest evidence-based information, to help busy health professionals stay up to date.

### Architecture

The overall Medicines awareness service system consists of the following applications:

- Headless CMS (KeystoneJS) backed by a Mongo database
- S3 bucket to serve a static website
- Several lambdas to:
  - Translate content from the CMS into:
    - HTML on the static site
    - Feeds on the static site
  - Send the MAS daily email
  - Send the MAS weekly email
- MailChimp to manage subscribers, templates and send emails.

#### Diagram

```
+----------+           +---------------------------+                 +--------------+
|          |  Manages  |                           |                 |              |
|  SPS     +--daily---->      CMS                  |                 | CloudWatch   |
|          |  content  |                           |                 | events       |
+----------+           |  +----------+  +-------+  |                 |              |
                       |  |          |  |       |  |                 +------+-------+
+----------+           |  | Webhooks |  |  API  <-------------+             |
|          |  Curates  |  |          |  |       |  |          |             |Scheduled trigger
|  NICE    +--weekly--->  +----+-----+  +-------+  |       Gets             |
|          |  content  |       |                   |       content   +------v-------+
+----------+           +---------------------------+          |      |              |
                               |                              +------+ Daily/weekly |
                               |On content                           | lambdas      |
                               |change                               |              |
                               |                                     +------+-------+
                          +----v-----+                                      |
                          |          |                                      |Creates and sends email
                          |  Lambda  |                                      |
                          |          |                               +------v---------------------------+
                          +----+-----+                               |                                  |
                               |                                     |            MailChimp             |
                               |Creates and                          |                                  |
                               |pushes HTML                          |  +------------+  +------------+  |
                               |                   +----------+      |  |            |  |            |  |
                          +----v-----+             |          |      |  | Subscriber |  | Template   |  |
                          |          |  Views SPS  | User     |      |  | management |  | management |  |
                          |  Static  |  comment    | receives |      |  |            |  |            |  |
                          |  site    +<------------+ email    <------+  -------------+  +------------+  |
                          |          |             |          |      |                                  |
                          +----------+             +----------+      +----------------------------------+
```

> Note: to edit the diagram, copy the source into http://asciiflow.com/ or similar.

## Requirements

- Docker (a licence is required for Docker Desktop)
- AWS CLI
- SAM CLI
- Amazon.lambda.tools 
  - Install this with the following command: 
  - `dotnet tool install --global Amazon.Lambda.Tools --version 3.3.1`
  - > Note: If you encounter an error try disabling all non-standard nuget sources

### IDE

Visual Studio 2017+
VS Code, extension(s) - TODO add extensions.json

## Local development

You can run each part of the application (CMS, lambdas, S3 etc) independently. See each of the sections below for detailed steps.

However, the easiest way to run the [lambdas](#lambdas) via Visual Studio and the [rest of the app](#rest-of-the-app) via Docker:

### Lambdas

We use the [Amazon.Lambda.AspNetCoreServer](https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.AspNetCoreServer) package which makes running a lambda via IIS Express (as a fake API Gateway) locally really easy.

First, you'll need user secrets with all the application settings locally. This avoids checking application config into git. Get a copy of user-secrets.json from a member of the MAS team. Open up [lambdas/MAS.sln](lambdas/MAS.sln) in Visual Studio and right click the MAS project in solution explorer before selecting "Manage user secrets". In the blank file that opens paste in the contents from user-secrets.json.

Restore packages and run the *MAS* project. This runs up a mock API gateway on http://localhost:64418 via IIS express (it will just serve a 404 as there's nothing set up serve from the root).

> Note: you will need to reference the NICE NuGet server(s) to be able to restore packages.

#### Faking the lamda schedule locally

We have an [AWS Serverless Application Model (SAM) template](lambda/MAS/serverless.template) that describes the lambda. This can be used to invoke the lambda locally using the SAM CLI. If you've never used SAM, then read the [AWS SAM docs](https://docs.aws.amazon.com/serverless-application-model/index.html) before getting started.

> Note: we don't use this SAM template for our deployments to production, it's only used for local testing.

In production, we use CloudWatch schedule triggers to send the daily and weekly emails. Locally, we normally run the lambda via IIS Express to mimick API Gateway. Use SAM CLI instead to trigger the lambda locally to fake the schedule event.

##### SAM requirements

Make sure you have the latest:

- [Microsoft .NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) (>=2.1.803) (check your version by running `dotnet --version`)
- [NuGet](https://www.nuget.org/downloads) (on Windows run `nuget update -self`)
- [AWS CLI](https://aws.amazon.com/cli/)
- [SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install-windows.html) (run `sam --version` to check your version)

##### Disable TeamCity Nuget

You might need to temporarily disable the TeamCity NuGet feed first. Open *%appdata%\NuGet\NuGet.Config* and add the following:

```xml
<disabledPackageSources>
    <add key="TeamCity" value="true" />
</disabledPackageSources>
```

> This stops the SAM CLI commands from getting a 401 from our internal TeamCity NuGet feed.

##### Build SAM project

When you've installed the dependencies above:

1. Open CMD on Windows (the `sam` command doesn't seem to work in Bash on Windows)
2. `cd` into the *lambda/MAS* project directory
3. Build the SAM project: `sam build -t serverless.template`

This builds the project and creates a *.aws-sam* folder which contains the template (yaml) and the compiled application.

> Note: when you've done this once, next time you can speed it up by running `sam build -t serverless.template --skip-pull-image`

Open *MAS/.aws-sam/build/AspNetCoreFunction/appsettings.json* and you'll notice all the settings are empty. To run the lambda locally, copy the contents of your secrets.json into this appsettings.json.

The *.aws-sam* folder is excluded from git, so don't worry about these secrets.

##### Invoke the lambda

Once you've built the SAM project as above, execute the daily schedule trigger by running:

```sh
echo {"resource": "daily" } | sam local invoke -e -
```

Or the weekly with 

```sh
echo {"resource": "weekly" } | sam local invoke -e -
```

> Note: You can also run lambda and fake API Gateway via SAM CLI with this command `sam local start-api`, but we normally don't do this as it's A LOT easier to just use IIS Express.

### Rest of the app
Before starting the docker containers the environmental variables creating. These live in the .env file which docker uses for variable replacement. (For more info read: https://docs.docker.com/compose/environment-variables/#the-env-file)
To create the variables get a copy of the .env file from a member of the MAS team and replace the some of the variables to make them relivant to you. Place this it in the root MAS directory.

Install Docker and run `docker-compose up` from the root of the repository. This creates:

- a Mongo dabatase on port 27017
- the CMS on port 3010
- a mock S3 backend server (MinIO) on port 9000
- a mock S3 frontend on port 8000.

Each of these ports is exposed to the host machine. This means you can view each application locally, for example visit:

- http://localhost:3010 in your browser to access the CMS
- http://localhost:9000 to browse the MinIO server
- http://localhost:8000 to view the static website.

> Note: the first time you do this it will **take a while**. This is because it needs to download all the required images. Subsequent runs will use cached images so will be quicker.
