# MAS (Medicines awareness service)

- [MAS (Medicines awareness service)](#mas-medicines-awareness-service)
  - [What is it?](#what-is-it)
    - [Architecture](#architecture)
      - [Diagram](#diagram)
  - [Requirements](#requirements)
    - [IDE](#ide)
  - [Local development](#local-development)
    - [Lambdas](#lambdas)
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
|  UKMi    +--daily---->      CMS                  |                 | CloudWatch   |
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
                          |          |  Views UKMi | User     |      |  | management |  | management |  |
                          |  Static  |  comment    | receives |      |  |            |  |            |  |
                          |  site    +<------------+ email    <------+  -------------+  +------------+  |
                          |          |             |          |      |                                  |
                          +----------+             +----------+      +----------------------------------+
```

> Note: to edit the diagram, copy the source into http://asciiflow.com/ or similar.

## Requirements

- Docker
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
Firstly build the AWS Sam template - this represents the architecture of the application (For more info: https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-template-basics.html). To build the template open command prompt (not bash) within the MAS/lambda directory and the command:
`sam build -t serverless.template`

Next run the lambda execution environmention via SAM CLI with this command ```sam local start-api```

Now get a copy of user-securets.json from a member of the MAS team. Open up [lambdas/MAS.sln](lambdas/MAS.sln) in Visual Studio and right click the MAS project in solution explorer before selecting "Manage user secrets". In the blank file that opens paste in the contents from user-securets.json.

Restore packages and run the *MAS* project. This runs up a mock API gateway on http://localhost:61233 via IIS express.

> Note: you will need to reference the NICE NuGet server(s) to be able to restore packages.

### Rest of the app
Before starting the docker containers the environmental variables creating. These live in the .env file which docker uses for variable replacement. (For more info read: https://docs.docker.com/compose/environment-variables/#the-env-file)
To create the variables get a copy of the .env file from a member of the MAS team and replace the some of the variables to make them relivant to you. Place this it in the root MAS directory.

Install Docker and run `docker-compose up` from the root of the repository. This creates:

- a Mongo dabatase on port 27017
- the CMS on port 3010
- a mock S3 server (MinIO) on port 9000

Each of these ports is exposed to the host machine. This means you can view each application locally, for example visit http://localhost:3010 in your browser to access the CMS or http://localhost:9000 to browse the MinIO server.

> Note: the first time you do this it will **take a while**. This is because it needs to download all the required images. Subsequent runs will use cached images so will be quicker.