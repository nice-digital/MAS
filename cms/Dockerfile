# Use LTS version of Node
FROM node:8.11.1

RUN npm -g install npm@latest

RUN mkdir -p /app

WORKDIR /app

COPY package.json /app
COPY package-lock.json /app

RUN npm ci

COPY . /app

EXPOSE 3010

CMD ["node", "keystone"]
